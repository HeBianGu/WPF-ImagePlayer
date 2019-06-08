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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary>
    /// MediaPartControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPartControl : UserControl
    {
        public MediaPartControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭局部放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {

            this.Visibility = Visibility.Collapsed;

            this.OnClosed();
        }

        /// <summary>
        /// 动态显示的形状
        /// </summary>
        public DynamicShape DynamicShape
        {
            get { return (DynamicShape)GetValue(DynamicShapeProperty); }
            set { SetValue(DynamicShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DynamicShapeProperty =
            DependencyProperty.Register("DynamicShape", typeof(DynamicShape), typeof(MediaPartControl), new PropertyMetadata(default(DynamicShape), (d, e) =>
            {
                MediaPartControl control = d as MediaPartControl;

                if (control == null) return;

                DynamicShape config = e.NewValue as DynamicShape;

                var geo = Geometry.Combine(control.rectangle_clip.RenderedGeometry, new RectangleGeometry(config.Rect), GeometryCombineMode.Exclude, null);

                Rect rect = new Rect(config.Rect.X - 5, config.Rect.Y - 5, config.Rect.Width + 5, config.Rect.Height + 5);
                control.visualbrush_part.Viewbox = rect;
                //control.visualbrush_part.Viewbox = config.Rect;
                control.rectangle_clip.Clip = geo;

            }));

        /// <summary>
        /// 动态显示区域在全图的位置视图
        /// </summary>
        public Visual ImageVisual
        {
            get { return (Visual)GetValue(ImageVisualProperty); }
            set { SetValue(ImageVisualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageVisualProperty =
            DependencyProperty.Register("ImageVisual", typeof(Visual), typeof(MediaPartControl), new PropertyMetadata(default(Visual), (d, e) =>
            {
                MediaPartControl control = d as MediaPartControl;

                if (control == null) return;

                Visual config = e.NewValue as Visual;

                control.visualbrush_image.Visual = config;

            }));


        //声明和注册路由事件
        public static readonly RoutedEvent ClosedRoutedEvent =
            EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(DynamicShape));

        /// <summary>
        /// 关闭局部放大事件
        /// </summary>
        public event RoutedEventHandler Closed
        {
            add { this.AddHandler(ClosedRoutedEvent, value); }
            remove { this.RemoveHandler(ClosedRoutedEvent, value); }
        }

        /// <summary>
        /// 触发关闭路由事件
        /// </summary>
        protected void OnClosed()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClosedRoutedEvent, this);
            this.RaiseEvent(args);
        }
    }
}
