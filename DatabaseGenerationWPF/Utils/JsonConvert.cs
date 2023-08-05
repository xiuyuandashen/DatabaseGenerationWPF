using DatabaseGenerationWPF.ViewModels;
using HandyControl.Controls;
using MaterialDesignColors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using System.Text;

using System.Threading.Tasks;
using static NPOI.HSSF.UserModel.HeaderFooter;

namespace DatabaseGenerationWPF.Utils
{
    public static class JsonConvert
    {

        public static ObservableCollection<FieldVM> JSON2Fields(this string json)
        {
            try
            {
                ObservableCollection<FieldVM> fields = new ObservableCollection<FieldVM>();
                JObject jr = JObject.Parse(json);

                foreach (var field in jr)
                {

                    FieldVM obj = new FieldVM();
                    obj.FieldName = field.Key;
                    obj.FieldType = field.Value.JToken2SqlTypeString();
                    fields.Add(obj);
                }

                return fields;
            }
            catch (Exception)
            {
                MessageBox.Show("JSON字符串不合法", "提示");
                return new ObservableCollection<FieldVM>(); ;
            }


        }

        /// <summary>
        /// 传入JToken解析成SQL类型
        /// </summary>
        /// <param name="jToken">JObject每项的值</param>
        /// <returns></returns>
        public static string JToken2SqlTypeString(this JToken jToken)
        {
            string type = string.Empty;

            JTokenType tokenType = jToken.Type;

            switch (tokenType)
            {
                case JTokenType.String: type = "NVARCHAR"; break;
                case JTokenType.Boolean: type = "bit"; break;
                case JTokenType.Integer: type = "int"; break;
                case JTokenType.Date: type = "datetime2"; break;
                case JTokenType.Float: type = "decimal"; break;
            }

            return type;
        }


        public static string Field2Sql(this ObservableCollection<FieldVM> fields, string tableName = "", string tableDesc = "")
        {
            string sql = string.Empty;
            if (string.IsNullOrWhiteSpace(tableName)) tableName = "表名";
            if (string.IsNullOrWhiteSpace(tableDesc)) tableDesc = "表描述";
            StringBuilder sb = new StringBuilder();
            StringBuilder sbDesc = new StringBuilder();
            sb.Append($"CREATE TABLE {tableName} (\r\n");
            foreach (var field in fields)
            {
                string fieldName = field.FieldName;
                string dbType = field.FieldType;
                string size = field.FieldSize;
                string isEmpty = field.FieldIsEmpty;
                string isKey = field.FieldIsKey;
                string desc = field.FieldDesc;
                string sqlItem = string.Empty;

                if (string.IsNullOrEmpty(size.Trim()) && !(dbType.ToUpper().Equals("NVARCHAR") || dbType.ToUpper().Equals("VARCHAR")))
                {
                    sqlItem = $"[{fieldName}] {dbType} ";
                }
                else
                {
                    // 如果是INT类型不需要是大小
                    if (dbType.ToUpper().Equals("INT"))
                    {
                        sqlItem = $"[{fieldName}] {dbType} ";
                    }
                    // 如果 NVARCHAR VARCHAR 的size为0
                    else if ((dbType.ToUpper().Equals("NVARCHAR") || dbType.ToUpper().Equals("VARCHAR")) && (size.Equals("0") || size.Trim().Equals(string.Empty)))
                    {
                        sqlItem = $"[{fieldName}] {dbType}(255) ";
                    }
                    // 时间类型
                    else if (dbType.ToUpper().Equals("DATETIME"))
                    {
                        sqlItem = $"[{fieldName}] {dbType} ";
                    }
                    else
                    {
                        sqlItem = $"[{fieldName}] {dbType}({size}) ";
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
                sb.Append(sqlItem + "," + "\r\n");
                sbDesc.Append($"execute sp_addextendedproperty 'MS_Description','{desc}','user','dbo','table','{tableName}','column','{fieldName}'; \r\n ");

            }
            sb.Append(")");
            string tableDescSQl = $"execute sp_addextendedproperty 'MS_Description','{tableDesc}','user','dbo','table','{tableName}',null,null;";

            sql = sb.ToString() + "\r\n" + sbDesc.ToString() + "\r\n" + tableDescSQl;

            return sql;
        }
    }
}
