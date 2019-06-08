using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary>
    /// 全屏显示窗口
    /// </summary>
    public partial class ImageFullScreenWindow : Window
    {
        public ImageFullScreenWindow()
        {
            InitializeComponent();


            //double screeHeight = SystemParameters.FullPrimaryScreenHeight;

            //double screeWidth = SystemParameters.FullPrimaryScreenWidth;

            double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度

            string id = Guid.NewGuid().ToString();

            this.Height = y;
            this.Width = x;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        /// <summary>
        /// 全屏中放的控件
        /// </summary>
        public UIElement CenterContent
        {
            set
            {
                this.grid_all.Children.Add(value);
            }
        }

        /// <summary>
        /// 退出关闭命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ClearClose();
        }

        public void ClearClose()
        {
            this.grid_all.Children.Clear();

            this.Close();
        }

        /// <summary>
        /// 是否可以退出关闭验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Image) && !(e.OriginalSource is Grid)) return;


            this.ClearClose();
        }

    }
}
