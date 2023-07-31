using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToDB
{
    internal class ExcelHelper
    {

        public static DataTable ExcelToTable(string fileName, Stream fs, bool isFirstRowColumn, string sheetName, int headerIndex, int colNum)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            IWorkbook workbook = null;
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {

                    IRow firstRow = sheet.GetRow(headerIndex);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                    if (colNum != -1)
                        cellCount = colNum;

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    if (data.Columns.Contains(cellValue))
                                    {
                                        cellValue += i;
                                        column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                    }
                                    else
                                    {
                                        data.Columns.Add(column);
                                    }

                                }
                            }
                        }
                        startRow = headerIndex + 1;
                    }
                    else
                    {
                        //startRow = sheet.FirstRowNum;
                        startRow = sheet.FirstRowNum + 1;
                    }
                    // 计算公式
                    IFormulaEvaluator evaluator = null;
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        evaluator = new XSSFFormulaEvaluator(workbook);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        evaluator = new HSSFFormulaEvaluator(workbook);


                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null
                        // 过滤空行
                        //if (row.GetCell(0) == null || row.GetCell(0).ToString().Equals(""))
                        //{
                        //    break;
                        //}
                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null)
                            {
                                string value = null;
                                ICell cell = row.GetCell(j);
                                //判断数据的类型
                                switch (cell.CellType)
                                {
                                    case CellType.Numeric: //数字
                                        cell.SetCellType(CellType.Numeric);
                                        value = (cell.NumericCellValue).ToString();
                                        break;
                                    case CellType.String: //字符串
                                        cell.SetCellType(CellType.String);
                                        value = cell.StringCellValue;
                                        break;
                                    case CellType.Boolean: //Boolean
                                        cell.SetCellType(CellType.Boolean);
                                        value = (cell.BooleanCellValue).ToString();
                                        break;
                                    //case CellType.Formula: //公式
                                    //    evaluator.EvaluateInCell(cell);
                                    //    cell.SetCellType(CellType.String);
                                    //    value = cell.StringCellValue;
                                    //    break;
                                    case CellType.Unknown: //空值
                                        value = "";
                                        break;
                                    case CellType.Error: //故障
                                        value = "非法字符";
                                        break;
                                    default:
                                        row.GetCell(j).SetCellType(CellType.String);
                                        value = row.GetCell(j).StringCellValue;
                                        break;
                                }
                                // 科学计数法转正常数值
                                if (value.IndexOf("E") != -1)
                                {
                                    double number;
                                    try
                                    {
                                        bool flag = Double.TryParse(value, System.Globalization.NumberStyles.Float, null, out number);
                                        if (flag)
                                        {
                                            value = number.ToString();
                                        }
                                        else
                                        {
                                            row.GetCell(j).SetCellType(CellType.String);
                                            value = row.GetCell(j).StringCellValue;
                                        }

                                    }
                                    catch (Exception e)
                                    {

                                    }


                                }
                                if (value.Equals("#N/A"))
                                {
                                    value = "";
                                }
                                dataRow[j] = value;
                            }
                            else
                            {
                                dataRow[j] = null;
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

       
    }
}
