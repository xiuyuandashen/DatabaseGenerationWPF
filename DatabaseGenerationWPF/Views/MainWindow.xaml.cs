using HandyControl.Interactivity;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Services.Dialogs;
using System.Diagnostics;
using System;
using System.IO;
using System.Windows;
using DatabaseGenerationWPF.ViewModels;
using HandyControl.Tools;

namespace DatabaseGenerationWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭弹窗后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawerLeft_Closed(object sender, RoutedEventArgs e)
        {
            //关闭抽屉后

        }

        /// <summary>
        /// 关闭弹窗 Click事件会在Command命令前执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 关闭弹窗
            this.DrawerLeft.IsOpen = false;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建 OpenFileDialog 对象
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置对话框的标题和默认文件类型过滤器
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "所有文件 (*.*)|*.*";

            // 打开对话框并获取用户选择的文件
            bool? result = openFileDialog.ShowDialog();

            // 检查用户是否选择了文件
            if (result == true)
            {
                // 获取用户选择的文件的完整路径
                string filePath = openFileDialog.FileName;

                // 在这里可以使用 filePath 来处理用户上传的文件
                // 例如，你可以将文件复制到特定位置，或者读取文件内容等操作
                // 你可以根据需求进行相应的文件处理逻辑

                var ViewModel = this.DataContext as MainWindowViewModel;
                ViewModel.File.FilePath = filePath;
                ViewModel.File.FileName = Path.GetFileName(filePath);
            }
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplateDownloadClick(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                // 设置对话框的标题和描述信息
                folderDialog.Title = "选择保存文件路径";
                folderDialog.IsFolderPicker = true;

                // 打开对话框并获取用户选择的文件夹
                CommonFileDialogResult result = folderDialog.ShowDialog();

                // 检查用户是否选择了文件夹
                if (result == CommonFileDialogResult.Ok)
                {
                    // 获取用户选择的文件夹路径
                    string selectedFolderPath = folderDialog.FileName;

                    // 在这里可以使用 selectedFolderPath 来处理用户选择的文件夹
                    // 例如，你可以在界面上显示选定的文件夹路径，或者根据该路径执行相应的操作
                    // 你可以根据需求进行相应的处理逻辑
                    selectedFolderPath = Path.Combine(selectedFolderPath, "模板.xlsx");
                    // System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase 获取和设置包括该应用程序的目录的名称
                    string templeFile = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"assist\模板.xlsx";
                    using FileStream target = File.Create(selectedFolderPath);
                    using FileStream source = File.OpenRead(templeFile);
                    source.CopyTo(target);
                    OpenFolderAndSelectedFile(selectedFolderPath);
                }
            }
        }

        /// <summary>
        /// 打开目录且选中文件
        /// </summary>
        /// <param name="filePathAndName">文件的路径和名称（比如：C:\Users\Administrator\test.txt）</param>
        private static void OpenFolderAndSelectedFile(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + filePathAndName;
            process.StartInfo = psi;

            //process.StartInfo.UseShellExecute = true;
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                process?.Close();

            }
        }


        // 单词计数方法
        private int CountWords(string text)
        {
            // 去除字符串前后的空格
            text = text.Trim();

            // 将字符串按空格分割成单词数组
            string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return words.Length;
        }

        private void SqlCodeBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int lineCount = SqlCodeBox.LineCount;
            int wordCount = CountWords(SqlCodeBox.Text);
            var ViewModel = this.DataContext as MainWindowViewModel;
            ViewModel.Record.Row = lineCount;
            ViewModel.Record.Num = wordCount;
        }
    }

    public class User
    {
        public string Name { get; set; }

        public string Password { get; set; }
    }
}
