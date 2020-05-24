using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace HeBianGu.General.ImageView
{
    [TemplatePart(Name = "PART_CenterCanvas", Type = typeof(InkCanvas))]
    [TemplatePart(Name = "PART_ImageCenter", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Grid_Mouse_Drag", Type = typeof(ContentControl))]
    [TemplatePart(Name = "svImg", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "grid_mark", Type = typeof(Grid))]
    [TemplatePart(Name = "vb", Type = typeof(Viewbox))]
    [TemplatePart(Name = "rootGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "controlmask", Type = typeof(ContentControl))]
    [TemplatePart(Name = "mask", Type = typeof(MaskCanvas))]
    [TemplatePart(Name = "popup", Type = typeof(Grid))]

    //  Do ：放大镜效果应用
    [TemplatePart(Name = "BigBox", Type = typeof(Canvas))]
    [TemplatePart(Name = "bigImg", Type = typeof(Image))]
    [TemplatePart(Name = "bigrect", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "MoveRect", Type = typeof(Ellipse))]
    [TemplatePart(Name = "grid_all", Type = typeof(Grid))]

    //  Do ：局部放大效果应用
    [TemplatePart(Name = "_dynamic", Type = typeof(DynamicShape))]

    public partial class ImageBase : ContentControl
    {
        static ImageBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageBase), new FrameworkPropertyMetadata(typeof(ImageBase)));
        }

        #region - 内部成员 -

        internal InkCanvas _centerCanvas = null;

        internal Image _imageCenter = null;

        /// <summary> 用于鼠标按下拖动图片移动效果 </summary>
        internal ContentControl grid_Mouse_drag = null;

        internal ScrollViewer svImg = null;

        internal Grid grid_mark = null;

        internal Viewbox vb = null;

        internal Grid rootGrid = null;

        internal ContentControl controlmask = null;

        internal MaskCanvas mask = null;

        TransformGroup tfGroup;

        internal Grid popup = null;

        internal Canvas BigBox = null;

        internal Image bigImg = null;

        internal RectangleGeometry bigrect = null;

        internal Ellipse MoveRect = null;

        internal Grid grid_all = null;

        internal DynamicShape _dynamic = null;


        double hOffSetRate = 0;//滚动条横向位置横向百分比

        double vOffSetRate = 0;//滚动条位置纵向百分比

        /// <summary> 图片的宽度 </summary>
        internal double imgWidth;

        /// <summary> 图片的高度 </summary>
        internal double imgHeight;

        List<IBehavior> behaviors = new List<IBehavior>();

        #endregion

        #region - 初始化 -

        public ImageBase()
        {
            //  Do：改变窗口自适应大小
            this.SizeChanged += (l, k) =>
            {
                this.SetFullImage();

            };

            //  Do ：绑定菜单命令
            CommandBinding command = new CommandBinding(ImageBaseCommands.DefaultCommand, (l, k) =>
               {
                   string commond = k.Parameter?.ToString();

                   if (commond == "删除")
                   {
                       this.DeleteShape();
                   }
                   else if (commond == "放大")
                   {
                       this.ShowShape();
                   }

               });

            this.CommandBindings.Add(command);

            //  Do ：绑定自定义行为

            this.behaviors.Add(new ImageBaseMouseWheelBehavior(this));
            this.behaviors.Add(new ImageBaseMouseEnlargeBehavior(this));
            this.behaviors.Add(new ImageBaseMouseDragBehavior(this));
            this.behaviors.Add(new ImageBaseMouseSignBehavior(this));
            this.behaviors.Add(new ImageBaseMouseBubbleBehavior(this));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._centerCanvas = Template.FindName("PART_CenterCanvas", this) as InkCanvas;

            this._imageCenter = Template.FindName("PART_ImageCenter", this) as Image;

            grid_Mouse_drag = Template.FindName("PART_Grid_Mouse_Drag", this) as ContentControl;

            svImg = Template.FindName("svImg", this) as ScrollViewer;

            grid_mark = Template.FindName("grid_mark", this) as Grid;

            vb = Template.FindName("vb", this) as Viewbox;

            rootGrid = Template.FindName("rootGrid", this) as Grid;

            mask = Template.FindName("mask", this) as MaskCanvas;

            controlmask = Template.FindName("controlmask", this) as ContentControl;

            popup = Template.FindName("popup", this) as Grid;

            BigBox = Template.FindName("BigBox", this) as Canvas;

            bigImg = Template.FindName("bigImg", this) as Image;

            bigrect = Template.FindName("bigrect", this) as RectangleGeometry;

            grid_all = Template.FindName("grid_all", this) as Grid;

            MoveRect = Template.FindName("MoveRect", this) as Ellipse;

            _dynamic = Template.FindName("_dynamic", this) as DynamicShape;

            //  Do ：初始化宽度高度
            this.InitWidthHeight();

            //  Do ：初始化设置平铺
            this.SetFullImage();

            this.NoticeMessaged += (m, n) => Debug.WriteLine(this.Message);

            this.behaviors.ForEach(l=>l.RegisterBehavior());
        }

        #endregion

        #region - 属性 -
        /// <summary> 滚轮放大倍数 </summary>
        public double WheelScale { get; set; } = 0.5;

        /// <summary> 设置最大放大倍数 </summary>
        public int MaxScale { get; set; } = 25;

        #endregion

        [Browsable(false)]
        [Category("Appearance")]
        [ReadOnly(true)]
        public OperateType OperateType
        {
            get { return (OperateType)GetValue(OperateTypeProperty); }
            set { SetValue(OperateTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OperateTypeProperty =
            DependencyProperty.Register("OperateType", typeof(OperateType), typeof(ImageBase), new PropertyMetadata(default(OperateType), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 OperateType config = (OperateType)e.NewValue;

             }));




        /// <summary> 当前图片资源 </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageBase), new PropertyMetadata(default(ImageSource), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 ImageSource config = e.NewValue as ImageSource;

                 if (control.grid_Mouse_drag == null) return;

                 control.SetFullImage();


             }));


        /// <summary> 设置放大倍数 </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ImageBase), new PropertyMetadata(1.0, (d, e) =>
            {
                ImageBase control = d as ImageBase;

                if (control == null) return;

                double config = (double)e.NewValue;

                control.Scale = config < 0 ? 0 : config;

                control.RefreshImageByScale();

            }));

        /// <summary> 是否放到最大 </summary>
        public bool IsMaxScaled
        {
            get { return (bool)GetValue(IsMaxScaledProperty); }
            set { SetValue(IsMaxScaledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaxScaledProperty =
            DependencyProperty.Register("IsMaxScaled", typeof(bool), typeof(ImageBase), new PropertyMetadata(default(bool), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 //bool config = e.NewValue as bool;

             }));

        /// <summary> 是否放到最小 </summary>
        public bool IsMinScaled
        {
            get { return (bool)GetValue(IsMinScaledProperty); }
            set { SetValue(IsMinScaledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinScaledProperty =
            DependencyProperty.Register("IsMinScaled", typeof(bool), typeof(ImageBase), new PropertyMetadata(default(bool), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 //bool config = e.NewValue as bool;

             }));


        /// <summary> 交互消息 </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ImageBase), new PropertyMetadata(default(string), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 string config = e.NewValue as string;

             }));



        //声明和注册路由事件
        public static readonly RoutedEvent NoticeMessagedRoutedEvent =
            EventManager.RegisterRoutedEvent("NoticeMessaged", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageBase));
        //CLR事件包装
        public event RoutedEventHandler NoticeMessaged
        {
            add { this.AddHandler(NoticeMessagedRoutedEvent, value); }
            remove { this.RemoveHandler(NoticeMessagedRoutedEvent, value); }
        }

        //激发路由事件,借用Click事件的激发方法

        internal void OnNoticeMessaged(string message)
        {
            this.Message = message;

            RoutedEventArgs args = new RoutedEventArgs(NoticeMessagedRoutedEvent, this);

            this.RaiseEvent(args);
        }
        #region - 通用方法 -

        /// <summary> 当初始化时初始化图片的宽和高 </summary>
        void InitWidthHeight()
        {
            this.UpdateLayout();

            imgWidth = this.grid_Mouse_drag.ActualWidth;
            imgHeight = this.grid_Mouse_drag.ActualHeight;

            //  Message：修改旋转后的图片宽高显示方式
            if (tfGroup != null)
            {
                var rotate = tfGroup.Children[2] as RotateTransform;

                if (rotate.Angle % 180 == 0)
                {
                    imgWidth = this.grid_Mouse_drag.ActualWidth;
                    imgHeight = this.grid_Mouse_drag.ActualHeight;
                }
                else
                {
                    imgHeight = this.grid_Mouse_drag.ActualWidth;
                    imgWidth = this.grid_Mouse_drag.ActualWidth;
                }
            }
        }

        /// <summary> 设置初始图片为平铺整个控件 </summary>
        internal void SetFullImage()
        {
            this.InitWidthHeight();

            if (imgWidth == 0 || imgHeight == 0)
                return;

            //SetbtnActualsizeEnable();

            //btnNarrow.IsEnabled = false;

            this.IsMinScaled = true;

            RefreshImageByScale();

            Scale = this.GetFullScale();

            //this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";

            this.OnNoticeMessaged(((int)(Scale * 100)).ToString() + "%");
        }

        /// <summary> 当Scale改变时刷新图片大小 </summary>
        internal void RefreshImageByScale()
        {
            GetOffSetRate();

            if (imgWidth < 0 || imgHeight < 0) return;

            vb.Width = Scale * imgWidth;
            vb.Height = Scale * imgHeight;

            SetOffSetByRate();

            this.RefreshMarkVisible();
        }


        /// <summary> 当Scale变化时设置更新后水平和垂直位移 </summary>
        internal void SetOffSetByRate()
        {
            this.UpdateLayout();

            if (svImg.ScrollableWidth > 0)
            {
                double hOffSet = hOffSetRate * svImg.ScrollableWidth;
                svImg.ScrollToHorizontalOffset(hOffSet);
            }
            if (svImg.ScrollableHeight > 0)
            {
                double vOffSet = vOffSetRate * svImg.ScrollableHeight;
                svImg.ScrollToVerticalOffset(vOffSet);
            }
        }

        /// <summary> 获取适应屏幕大小的范围 </summary>
        public double GetFullScale()
        {
            double result = svImg.ActualWidth / imgWidth;

            result = Math.Min(result, svImg.ActualHeight / imgHeight);

            return result;

        }

        /// <summary> 当Scale变化时获取更新前水平和垂直位移 </summary>
        internal void GetOffSetRate()
        {
            if (svImg.ScrollableWidth > 0)
            {
                if (svImg.HorizontalOffset != 0)
                    hOffSetRate = svImg.HorizontalOffset / svImg.ScrollableWidth;
            }
            if (svImg.ScrollableHeight > 0)
            {
                if (svImg.VerticalOffset != 0)
                    vOffSetRate = svImg.VerticalOffset / svImg.ScrollableHeight;
            }
        }

        /// <summary> 根据Scale放大倍数设置鸟撖图是否可见 </summary>
        internal void RefreshMarkVisible()
        {
            if (imgWidth == 0 || imgHeight == 0) return;

            if (Scale > Math.Min(svImg.ActualWidth / imgWidth, svImg.ActualHeight / imgHeight))
            {
                this.grid_mark.Visibility = Visibility.Visible;
            }
            else
            {
                this.grid_mark.Visibility = Visibility.Collapsed;
            }
        }

        #endregion



        public ObservableCollection<RectangleShape> RectangleShapes
        {
            get { return (ObservableCollection<RectangleShape>)GetValue(RectangleShapesProperty); }
            set { SetValue(RectangleShapesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectangleShapesProperty =
            DependencyProperty.Register("RectangleShapes", typeof(ObservableCollection<RectangleShape>), typeof(ImageBase), new PropertyMetadata(new ObservableCollection<RectangleShape>(), (d, e) =>
             {
                 ImageBase control = d as ImageBase;

                 if (control == null) return;

                 ObservableCollection<RectangleShape> config = e.NewValue as ObservableCollection<RectangleShape>;

             }));



        internal void AddShape(Rect rect)
        {
            SampleShape resultStroke = new SampleShape(this._dynamic);
            resultStroke.Name = DateTime.Now.ToString();
            resultStroke.Code = DateTime.Now.ToString();
            resultStroke.Draw(this._centerCanvas);
            this.RectangleShapes.Add(resultStroke);

            ////  Do：清除动态框
            //_dynamic.BegionMatch(false);
        }

        void DeleteShape()
        {
            var find = this.RectangleShapes.FirstOrDefault(l => l.IsSelected);

            if (find == null) return;

            this._centerCanvas.Children.Remove(find);

            this.RectangleShapes.Remove(find);
        }

        void ShowShape()
        {
            var find = this.RectangleShapes.FirstOrDefault(l => l.IsSelected);

            if (find == null) return;

            this.ShowShape(find.Rect);
        }

        /// <summary> 按矩形框放大 </summary>
        internal void ShowShape(Rect rect)
        {
            if (this.imgWidth == 0 || this.imgHeight == 0)
                return;

            double percentX = rect.X / this._centerCanvas.ActualWidth;

            double percentY = rect.Y / this._centerCanvas.ActualHeight;

            double timeW = rect.Width / this._centerCanvas.ActualWidth;
            double timeH = rect.Height / this._centerCanvas.ActualHeight;

            double w = this.mask.ActualWidth * timeW;
            double h = this.mask.ActualHeight * timeH;


            //  Message：设置缩放比例
            this.Scale = Math.Min(this.svImg.ActualWidth / this.imgWidth, this.svImg.ActualHeight / this.imgHeight);

            this.Scale = this.Scale / Math.Max(timeW, timeH);

            //this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";

            //if (sb_Tip != null) sb_Tip.Begin();

      

            double indicatorWidth = mask.Indicator.ActualWidth / mask.ActualWidth;
            double indicatorHeight = mask.Indicator.ActualHeight / mask.ActualHeight;

            double transWidth = indicatorWidth - timeW;
            double transHeight = indicatorHeight - timeH;

            percentX = Math.Abs(percentX - transWidth / 2);
            percentY = Math.Abs(percentY - transHeight / 2);

            w = indicatorWidth * mask.ActualWidth;
            h = mask.ActualHeight * indicatorHeight;

            SetImageByScale();

            //  Message：更改区域位置
            Rect rectMark = new Rect(percentX * this.mask.ActualWidth, percentY * this.mask.ActualHeight, w, h);

            this.mask.UpdateSelectionRegion(rectMark, true);

        }

        private void SetImageByScale()
        {
            this.GetOffSetRate();

            if (this.imgWidth < 0 || this.imgHeight < 0) return;

            this.vb.Width = this.Scale * this.imgWidth;
            this.vb.Height = this.Scale * this.imgHeight;

            this.SetOffSetByRate();

            this.RefreshMarkVisible();
        }
    }


    //将所有命令封装在一个类里面

    public class ImageBaseCommands
    {
        public static RoutedUICommand DefaultCommand = new RoutedUICommand();
    }


}
