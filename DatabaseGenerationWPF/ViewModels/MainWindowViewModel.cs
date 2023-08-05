using DatabaseGenerationWPF.Event;
using DatabaseGenerationWPF.Views;
using ExcelToDB;
using HandyControl.Controls;
using MathNet.Numerics;
using NPOI.HPSF;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerationWPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "数据库表生成器";
        private SqlFile sqlFile = new SqlFile();
        private Record record = new Record { Row = 0, Num = 0 };

        private HandyControl.Controls.Dialog dialog;

        public SqlFile File
        {
            get => sqlFile; set
            {
                SetProperty(ref sqlFile, value);
            }
        }

        public Record Record
        {
            get => record; set
            {
                SetProperty(ref record, value);

            }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        public DelegateCommand generateCommand { get; set; }

        public DelegateCommand ClearCommand { get; set; }


        public DelegateCommand JsonOpenDialogCommand { get; set; }
        public IEventAggregator EventAggregator { get; }

        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            generateCommand = new DelegateCommand(GenerateCode);
            ClearCommand = new DelegateCommand(ClearData);
            JsonOpenDialogCommand = new DelegateCommand(JsonOpenDialog);
            EventAggregator = eventAggregator;

            // 订阅事件
            EventAggregator.GetEvent<SqlEvent>().Subscribe((sql) =>
            {
                File.Sql = sql;
                dialog?.Close();
            });
        }

        private async void JsonOpenDialog()
        {
            dialog = HandyControl.Controls.Dialog.Show<JsonGenerateDialog>();
            //await Task.Delay(5 * 1000);
            //d.Close();
        }

        private void ClearData()
        {
            File = new SqlFile();
        }

        private void GenerateCode()
        {
            if (string.IsNullOrEmpty(File.TableName))
            {
                MessageBox.Show("请填写表名!","提示");
                return;
            }

            if (string.IsNullOrEmpty(File.TableDesc))
            {
                MessageBox.Show("请填写表描述!","提示");
                return;
            }

            if (string.IsNullOrEmpty(File.FileName))
            {
                MessageBox.Show("请上传文件!", "提示");
                return;
            }
            StringBuilder sb = new StringBuilder();
            if (File.IsField == true)
            {

                using FileStream stream = System.IO.File.OpenRead(File.FilePath);
                DataTable DT = ExcelHelper.ExcelToTable(File.FilePath, stream, true, null, 0, 8);

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    DataRow dr = DT.Rows[i];
                    string fielName = dr[1].ToString();
                    string dbType = dr[2].ToString();
                    string size = dr[3].ToString();
                    string isEmpty = dr[4].ToString();
                    string isKey = dr[5].ToString();
                    string desc = dr[6].ToString();
                    //ALTER TABLE employee  ADD  spbh varchar(20) NOT NULL Default 0
                    string sql = string.Empty;
                    sql += $"ALTER TABLE {File.TableName} ADD ";

                    // 如果是INT类型不需要是大小
                    if (dbType.ToUpper().Equals("INT"))
                    {
                        sql += $"[{fielName}] {dbType} ";
                    }
                    // 如果 NVARCHAR VARCHAR 的size为0
                    else if ((dbType.ToUpper().Equals("NVARCHAR") || dbType.ToUpper().Equals("VARCHAR")) && (size.Equals("0") || size.Trim().Equals(string.Empty)))
                    {
                        sql += $"[{fielName}] {dbType}(255) ";
                    }
                    // 时间类型
                    else if (dbType.ToUpper().Equals("DATETIME"))
                    {
                        sql += $"[{fielName}] {dbType} ";
                    }
                    else
                    {
                        sql += $"[{fielName}] {dbType}({size}) ";
                    }

                    if ("否".Equals(isEmpty))
                    {
                        sql += $"NOT NULL ";
                    }
                    sb.Append(sql + ";\r\n");
                }

                File.Sql = sb.ToString();

                return;
            }


            using FileStream fs = System.IO.File.OpenRead(File.FilePath);
            DataTable dt = ExcelHelper.ExcelToTable(File.FilePath, fs, true, null, 0, 8);

            StringBuilder sbDesc = new StringBuilder();
            sb.Append($"CREATE TABLE {File.TableName} (\r\n");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                string fielName = dr[1].ToString();
                string dbType = dr[2].ToString();
                string size = dr[3].ToString();
                string isEmpty = dr[4].ToString();
                string isKey = dr[5].ToString();
                string desc = dr[6].ToString();
                string sqlItem = string.Empty;
                if (string.IsNullOrEmpty(size.Trim()))
                {
                    sqlItem = $"[{fielName}] {dbType} ";
                }
                else
                {
                    // 如果是INT类型不需要是大小
                    if (dbType.ToUpper().Equals("INT"))
                    {
                        sqlItem = $"[{fielName}] {dbType} ";
                    }
                    // 如果 NVARCHAR VARCHAR 的size为0
                    else if ((dbType.ToUpper().Equals("NVARCHAR") || dbType.ToUpper().Equals("VARCHAR")) && (size.Equals("0") || size.Trim().Equals(string.Empty)))
                    {
                        sqlItem = $"[{fielName}] {dbType}(255) ";
                    }
                    // 时间类型
                    else if (dbType.ToUpper().Equals("DATETIME"))
                    {
                        sqlItem = $"[{fielName}] {dbType} ";
                    }
                    else
                    {
                        sqlItem = $"[{fielName}] {dbType}({size}) ";
                    }

                }

                if ("是".Equals(isKey))
                {
                    sqlItem += $"primary key ";
                }
                if ("否".Equals(isEmpty))
                {
                    sqlItem += $"not null ";
                }
                //if(!String.IsNullOrEmpty(desc))
                //{
                //    sqlItem += $"comment '{desc}' ";
                //}
                if (i < dt.Rows.Count - 1)
                {
                    sqlItem += ",";
                }
                sb.Append(sqlItem + "\r\n");
                #region 添加表字段注释
                sbDesc.Append($"execute sp_addextendedproperty 'MS_Description','{desc}','user','dbo','table','{File.TableName}','column','{fielName}'; \r\n ");
                #endregion

            }
            sb.Append(")");
            #region 生成表注释
            string tableDescSQl = $"execute sp_addextendedproperty 'MS_Description','{File.TableDesc}','user','dbo','table','{File.TableName}',null,null;";
            #endregion
            File.Sql = sb.ToString() + "\r\n" + sbDesc.ToString() + "\r\n" + tableDescSQl;


        }
    }

    /// <summary>
    /// 每个对象属性也要进行双向绑定通知
    /// </summary>
    public class SqlFile : BindableBase
    {
        private string tableName;
        private string fileName;
        private string sql;
        private bool? isField = false;
        private string tableDesc;
        private string filePath;

        public string TableName
        {
            get => tableName; set
            {
                SetProperty(ref tableName, value);
            }
        }
        public string FileName
        {
            get => fileName; set
            {
                SetProperty(ref fileName, value);
            }
        }
        public string Sql
        {
            get => sql; set
            {
                SetProperty(ref sql, value);
            }
        }

        // 是否添加字段
        public bool? IsField
        {
            get => isField; set
            {
                SetProperty(ref isField, value);
            }
        }
        // 表描述
        public string TableDesc
        {
            get => tableDesc; set
            {
                SetProperty(ref tableDesc, value);
            }
        }

        public string FilePath
        {
            get => filePath; set
            {

                SetProperty(ref filePath, value);
            }
        }
    }


    public class Record : BindableBase
    {
        private int? row;
        private int? num;

        public int? Row
        {
            get => row; set
            {

                SetProperty(ref row, value);
            }
        }
        public int? Num
        {
            get => num; set
            {

                SetProperty(ref num, value);
            }
        }
    }
}
