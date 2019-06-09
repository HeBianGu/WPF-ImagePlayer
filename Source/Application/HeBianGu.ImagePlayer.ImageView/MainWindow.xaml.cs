using HeBianGu.Base.WpfBase.Color;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeBianGu.ImagePlayer.ImageView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();

            ThemeService.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
         
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                DirectoryInfo directory = new DirectoryInfo(dialog.SelectedPath);

                var images = this.GetAllFile(directory, l => l.Extension.EndsWith("jpg") || l.Extension.EndsWith("png"));

                imageview.LoadImg(images.ToList()); 
            }

        }

        /// <summary> 获取当前文件夹下所有匹配的文件 </summary>

        IEnumerable<string> GetAllFile(DirectoryInfo dir, Predicate<FileInfo> match = null)
        {
            foreach (var d in dir.GetFileSystemInfos())
            {
                if (d is DirectoryInfo)
                {
                    DirectoryInfo dd = d as DirectoryInfo;

                    var result = GetAllFile(dd, match);

                    foreach (var item in result)
                    {
                        yield return item;
                    }
                }

                else if (d is FileInfo)
                {
                    FileInfo dd = d as FileInfo;
                    if (match == null || match(dd))
                    {
                        yield return d.FullName;
                    }
                }
            }
        }


        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
