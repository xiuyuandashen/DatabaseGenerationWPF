using DatabaseGenerationWPF.Event;
using DatabaseGenerationWPF.Utils;
using HandyControl.Interactivity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.UserModel.HeaderFooter;

namespace DatabaseGenerationWPF.ViewModels
{
    public class JsonGenerateDialogViewModel : BindableBase
    {
        private string jsonContent;
        private ObservableCollection<FieldVM> fields = new ObservableCollection<FieldVM>();
        private string tableName;
        private string tableDesc;

        public DelegateCommand JSON2FieldCommand { get; set; }

        public DelegateCommand Field2SqlCommand { get; set; }

        public string JsonContent
        {

            get => jsonContent; set
            {

                SetProperty(ref jsonContent, value);
            }

        }


        public string TableName
        {
            get => tableName; set
            {

                SetProperty(ref tableName, value);
            }
        }
        public string TableDesc
        {
            get => tableDesc; set
            {

                SetProperty(ref tableDesc, value);
            }
        }

        public ObservableCollection<FieldVM> Fields
        {

            get => fields; set
            {

                SetProperty(ref fields, value);
            }
        }

        public IEventAggregator EventAggregator { get; }

        public JsonGenerateDialogViewModel(IEventAggregator eventAggregator)
        {
            JSON2FieldCommand = new DelegateCommand(JSON2Field);
            Field2SqlCommand = new DelegateCommand(Field2Sql);
            EventAggregator = eventAggregator;
        }

        /// <summary>
        /// Field 转换 SQL语句并发布事件
        /// </summary>
        private void Field2Sql()
        {
            string sql = Fields.Field2Sql(TableName, TableDesc);
            // 发布事件
            EventAggregator.GetEvent<SqlEvent>().Publish(sql);
        }

        /// <summary>
        /// JSON 转换 Field实体
        /// </summary>
        private void JSON2Field()
        {
            Fields = JsonContent.JSON2Fields();
        }
    }


    public class FieldVM : BindableBase
    {
        private string fieldName = string.Empty;
        private string fieldType = string.Empty;
        private string fieldSize = string.Empty;
        private string fieldIsEmpty = string.Empty;
        private string fieldIsKey = string.Empty;
        private string fieldDesc = string.Empty;

        public string FieldName
        {
            get => fieldName; set
            {
                SetProperty(ref fieldName, value);
            }
        }

        public string FieldType
        {
            get => fieldType; set
            {

                SetProperty(ref fieldType, value);
            }
        }

        public string FieldSize
        {
            get => fieldSize; set
            {
                SetProperty(ref fieldSize, value);
            }
        }

        public string FieldIsEmpty
        {
            get => fieldIsEmpty; set
            {

                SetProperty(ref fieldIsEmpty, value);
            }
        }

        public string FieldIsKey
        {
            get => fieldIsKey; set
            {

                SetProperty(ref fieldIsKey, value);
            }
        }

        public string FieldDesc
        {
            get => fieldDesc; set
            {

                SetProperty(ref fieldDesc, value);
            }
        }


    }
}
