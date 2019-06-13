using HeBianGu.ImagePlayer.ImageControl;
using HeBianGu.ImagePlayer.ImageControl.Hook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeBianGu.ImagePlayer.ImageControl
{
    public partial class ImageCore : UserControl
    {
        #region - 成员变量 -

        int thumbWidth;
        int thumbHeight;
        double scale = 1;
        double imgWidth;
        double imgHeight;
        double hOffSetRate = 0;//滚动条横向位置横向百分比
        double vOffSetRate = 0;//滚动条位置纵向百分比

        Storyboard sb_ShowTools;
        Storyboard sb_HideTools;
        Storyboard sb_Tip;
        TransformGroup tfGroup;
        #endregion

        #region - 初始化 -

        void InitSource()
        {
            sb_ShowTools = this.FindResource("Sb_ShowTools") as Storyboard;

            sb_HideTools = this.FindResource("Sb_HideTools") as Storyboard;

            sb_Tip = this.FindResource("sb_Tips") as Storyboard;

            tfGroup = this.FindResource("TfGroup") as TransformGroup;
        }

        void InitHandle()
        {
            this.Loaded += MainWindow_Loaded;

            //  ToEdit ：设置工具栏显示的控件
            this.grid_all.MouseEnter += ImageViews_MouseEnter;
            this.grid_all.MouseLeave += ImageViews_MouseLeave;

            //  Do：设置标定模式时滚动条效果
            this.rootGrid.MouseWheel += svImg_MouseWheel;

            svImg.ScrollChanged += svImg_ScrollChanged;


            gridMouse.MouseWheel += svImg_MouseWheel;
            gridMouse.PreviewMouseDown += control_MouseLeftButtonDown;
            gridMouse.PreviewMouseUp += control_MouseLeftButtonUp;
            gridMouse.PreviewMouseMove += control_MouseMove;


            btnActualsize.Click += btnActualsize_Click;
            btnEnlarge.Click += btnEnlarge_Click;
            btnNarrow.Click += btnNarrow_Click;
            btnRotate.Click += btnRotate_Click;

            this.mask.LoationChanged += (l, k) =>
            {
                var result = GetScrollWidthAndHeight();

                double xleft = (this.rootGrid.ActualWidth - this.vb.ActualWidth) / 2;
                double ytop = (this.rootGrid.ActualHeight - this.vb.ActualHeight) / 2;

                svImg.ScrollToHorizontalOffset(k.Left * svImg.ExtentWidth * result.Item1 + xleft);
                svImg.ScrollToVerticalOffset(k.Top * svImg.ExtentHeight * result.Item2 + ytop);
            };

            SetbtnActualsizeEnable();

            //  Message：注册鼠标悬停事件，注意删除和新增的时候
            mouseEventHandler = (l, k) =>
            {
                RectangleShape rectangleShape = l as RectangleShape;

                ShowPartWithShape(rectangleShape);
            };


            this.RegisterDefaltApi();

            //  Message：初始化时这两个方法很关键，否则图片没有平铺
            this.SetMarkType(MarkType.None);

            this.SetBubbleScale(200);

            this.ConvertSpeedFunction = l =>
            {
                switch (l)
                {
                    case -1:
                        return 3;
                    case -2:
                        return 4;
                    case -4:
                        return 5;
                    case -8:
                        return 6;
                    case 1:
                        return 2;
                    case 2:
                        return 1;
                    case 4:
                        return 0.5;
                    case 8:
                        return 0.1;
                    default:
                        return 2;
                }
            };

            this.LoadedAction = l =>
            {
                //  Message：注：此处要加载两遍SetAdaptiveSize，否则会出现缩放百分比不正确的问题
                l.SetAdaptiveSize();

                l.Scale = this.GetFullScale();

                l.SetAdaptiveSize();
            };

            //  Do：改变窗口自适应大小
            this.SizeChanged += (l, k) =>
            {
                this.SetAdaptiveSize();

                this.Scale = this.GetFullScale();

                this.SetAdaptiveSize();

            };
        }

        public ImageCore()
        {
            InitializeComponent();

            this.InitSource();


            this.InitHandle();
        }

        #endregion

        #region - 内部方法 -


        void svImg_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if (imgWidth == 0 || imgHeight == 0)
                return;

            thumbWidth = (int)controlmask.ActualWidth;
            thumbHeight = (int)controlmask.ActualHeight;

            double xleft = (this.rootGrid.ActualWidth - this.vb.ActualWidth) / 2;
            double ytop = (this.rootGrid.ActualHeight - this.vb.ActualHeight) / 2;


            var result = this.GetScrollWidthAndHeight();

            double scroll_width = svImg.ViewportWidth + svImg.ScrollableWidth;

            double timeH = svImg.ViewportHeight / (svImg.ViewportHeight + svImg.ScrollableHeight);
            double timeW = svImg.ViewportWidth / (scroll_width - 2 * xleft);

            double w = thumbWidth * timeW;
            double h = thumbHeight * timeH;

            double offsetx = 0;
            double offsety = 0;

            if (svImg.ScrollableWidth == 0)
            {
                offsetx = 0;
            }
            else
            {
                offsetx = (w - thumbWidth) / svImg.ScrollableWidth * (svImg.HorizontalOffset);
            }

            offsetx = -(svImg.HorizontalOffset - xleft) / (scroll_width - xleft * 2) * thumbWidth;

            if (svImg.ScrollableHeight == 0)
            {
                offsety = 0;
            }
            else
            {
                offsety = (h - thumbHeight) / svImg.ScrollableHeight * svImg.VerticalOffset;
            }

            Rect rect = new Rect(-offsetx, -offsety, w, h);

            mask.UpdateSelectionRegion(rect);
        }

        /// <summary> 获取适应屏幕大小的范围 </summary>
        public double GetFullScale()
        {
            double result = svImg.ActualWidth / imgWidth;

            result = Math.Min(result, svImg.ActualHeight / imgHeight);

            return result;

        }

        private void SetImageByScale()
        {
            GetOffSetRate();

            if (imgWidth < 0 || imgHeight < 0) return;

            vb.Width = Scale * imgWidth;
            vb.Height = Scale * imgHeight;

            SetOffSetByRate();

            this.RefreshMarkVisible();
        }

        /// <summary> 设置1:1按钮可用 </summary>
        private void SetbtnActualsizeEnable()
        {
            //btnActualsize.IsEnabled = (int)(Scale * 100) != 100;
        }

        void CloseFullScreen()
        {

            if (window == null) return;

            window.ClearClose();
        }

        ImageFullScreenWindow window;

        /// <summary> 显示全屏 </summary>
        void ShowFullScreen()
        {
            //  Do：将数据初始化
            start = new System.Windows.Point(-1, -1);

            window = new ImageFullScreenWindow();

            window.DataContext = this.ViewModel;
            this.Content = null;
            window.CenterContent = this.grid_all;

            //  Do：触发全屏状态事件
            this.FullScreenChangedEvent?.Invoke(true, this);

            window.Loaded += (l, k) =>
            {
                //  Message：设置全屏自适应
                this.SetFullImage();
            };

            window.ShowDialog();

            //  Do：将数据初始化
            start = new System.Windows.Point(-1, -1);

            this.Content = this.grid_all;

            //  Message：设置全屏自适应
            this.SetFullImage();

            //  Do：触发取消全屏状态事件
            this.FullScreenChangedEvent?.Invoke(false, this);
        }

        private void SetOffSetByRate()
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

        private void GetOffSetRate()
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

        /// <summary> 设置初始图片为平铺整个控件 </summary>
        void RefreshMarkVisible()
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            if (Scale > Math.Min(svImg.ActualWidth / imgWidth, svImg.ActualHeight / imgHeight))
            {
                this.grid_mark.Visibility = Visibility.Visible;
            }
            else
            {
                this.grid_mark.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary> 设置初始图片为平铺整个控件 </summary>
        internal void SetFullImage()
        {
            this.GetImageWidthHeight();

            if (imgWidth == 0 || imgHeight == 0)
                return;

            SetbtnActualsizeEnable();

            btnNarrow.IsEnabled = false;

            SetImageByScale();

            Scale = this.GetFullScale();

            this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";
        }

        Tuple<double, double> GetScrollWidthAndHeight()
        {
            double xleft = 1 - (this.rootGrid.ActualWidth - this.vb.ActualWidth) / this.rootGrid.ActualWidth;
            double ytop = 1 - (this.rootGrid.ActualHeight - this.vb.ActualHeight) / this.rootGrid.ActualHeight;

            return Tuple.Create(xleft, ytop);
        }

        public void GetImageWidthHeight()
        {
            this.UpdateLayout();

            imgWidth = this.gridMouse.ActualWidth;
            imgHeight = this.gridMouse.ActualHeight;

            //  Message：修改旋转后的图片宽高显示方式
            if (tfGroup != null)
            {
                var rotate = tfGroup.Children[2] as RotateTransform;

                if (rotate.Angle % 180 == 0)
                {
                    imgWidth = this.gridMouse.ActualWidth;
                    imgHeight = this.gridMouse.ActualHeight;
                }
                else
                {
                    //imgHeight = this.gridMouse.ActualWidth- this.vb.ActualWidth;
                    //imgWidth = this.gridMouse.ActualWidth + this.vb.ActualWidth

                    imgHeight = this.gridMouse.ActualWidth;
                    imgWidth = this.gridMouse.ActualWidth;
                }
            }
        }

        void Rotate(double angle)
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            if (false)
            {
                //  Message：设置1:1并旋转
                Scale = 1;
                this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";
            }

            this.SetAdaptiveSize();

            if (sb_Tip != null)
                sb_Tip.Begin();
            SetbtnActualsizeEnable();
            btnNarrow.IsEnabled = true;
            btnEnlarge.IsEnabled = true;
            hOffSetRate = 0;
            vOffSetRate = 0;

            if (tfGroup != null)
            {
                var rotate = tfGroup.Children[2] as RotateTransform;
                rotate.Angle += angle;
            }

            this.SetAdaptiveSize();

            this.grid_mark.Visibility = Visibility.Visible;
        }

        #endregion

        #region - 注册事件 - 
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetImageWidthHeight();
        }

        void btnRotate_Click(object sender, RoutedEventArgs e)
        {

            this.SetRotateRight();
        }

        void btnNarrow_Click(object sender, RoutedEventArgs e)
        {
            this.SetNarrow();
        }

        void btnActualsize_Click(object sender, RoutedEventArgs e)
        {
            this.SetOriginalSize();
        }

        void ImageViews_MouseLeave(object sender, MouseEventArgs e)
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            if (sb_ShowTools != null)
                sb_ShowTools.Pause();
            if (sb_HideTools != null)
                sb_HideTools.Begin();

        }

        void ImageViews_MouseEnter(object sender, MouseEventArgs e)
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            if (sb_HideTools != null)
                sb_HideTools.Pause();
            if (sb_ShowTools != null)
                sb_ShowTools.Begin();
        }

        void btnEnlarge_Click(object sender, RoutedEventArgs e)
        {
            this.SetEnlarge();
        }

        #endregion  

        #region - 鸟撖图操作 -

        private bool mouseDown;

        private System.Windows.Point mouseXY;

        void control_MouseMove(object sender, MouseEventArgs e)
        {

            if ((this._markType != MarkType.None) && e.MiddleButton == MouseButtonState.Released)
            {
                return;
            }

            if (imgWidth == 0 || imgHeight == 0)
                return;

            var img = sender as UIElement;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }

        private void Domousemove(UIElement img, MouseEventArgs e)
        {
            if ((this._markType != MarkType.None) && e.MiddleButton == MouseButtonState.Released)
            {
                return;
            }


            var position = e.GetPosition(img);
            double X = mouseXY.X - position.X;
            double Y = mouseXY.Y - position.Y;
            mouseXY = position;
            if (X != 0)
                svImg.ScrollToHorizontalOffset(svImg.HorizontalOffset + X);
            if (Y != 0)
                svImg.ScrollToVerticalOffset(svImg.VerticalOffset + Y);

            GetOffSetRate();
        }

        void control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((this._markType != MarkType.None && e.ChangedButton != MouseButton.Middle))
            {
                return;
            }


            var img = sender as UIElement;


            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;


            this.RefreshCursor();
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                if ((this._markType != MarkType.None))
                {
                    return;
                }
            }

            var img = sender as UIElement;

            if (img == null)
            {
                return;
            }

            img.CaptureMouse();

            mouseDown = true;

            mouseXY = e.GetPosition(img);

        }

        #endregion 

        #region - 绘制矩形框和相关操作 -

        System.Windows.Point start;
        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewModel == null) return;

            if ((this._markType == MarkType.None))
            {
                return;
            }

            _dynamic.BegionMatch(true);

            start = e.GetPosition(sender as InkCanvas);


        }
        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ViewModel == null) return;

            if (this._markType == MarkType.None)
            {
                return;
            }

            if (this._markType == MarkType.Bubble)
            {
                FrameworkElement element = sender as FrameworkElement;

                // 计算鼠标在X轴的移动距离
                double deltaV = e.GetPosition(element).Y;
                //计算鼠标在Y轴的移动距离
                double deltaH = e.GetPosition(element).X;

                double newTop = deltaV - this.MoveRect.ActualHeight / 2 <= 0 ? 0 : deltaV - this.MoveRect.ActualHeight / 2;
                double newLeft = deltaH - this.MoveRect.ActualWidth / 2 <= 0 ? 0 : deltaH - this.MoveRect.ActualWidth / 2;

                newTop = deltaV + this.MoveRect.ActualHeight / 2 > this.canvas.ActualHeight ? this.canvas.ActualHeight - this.MoveRect.ActualHeight : newTop;
                newLeft = deltaH + this.MoveRect.ActualWidth / 2 > this.canvas.ActualWidth ? this.canvas.ActualWidth - this.MoveRect.ActualWidth : newLeft;

                this.MoveRect.SetValue(InkCanvas.TopProperty, newTop);
                this.MoveRect.SetValue(InkCanvas.LeftProperty, newLeft);

                AdjustBigImage();


                var position2 = Mouse.GetPosition(this.grid_all);

                this.popup.Margin = new Thickness(position2.X - this.popup.Width / 2, position2.Y - this.popup.Height / 2, 0, 0);

                if (this.start.X <= 0 || this.start.Y <= 0) { return; }

                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (this.start.X <= 0) return;

            System.Windows.Point end = e.GetPosition(this.canvas);

            //this._isMatch = Math.Abs(start.X - end.X) > 50 && Math.Abs(start.Y - end.Y) > 50;

            _dynamic.Visibility = Visibility.Visible;

            _dynamic.Refresh(start, end);

        }
        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((this._markType == MarkType.None))
            {
                return;
            }

            //  Do：检查选择区域是否可用
            if (!_dynamic.IsMatch())
            {
                _dynamic.Visibility = Visibility.Collapsed;
                return;
            };

            if (this.start.X <= 0) return;

            //  Do：如果是选择局部放大
            //if (this.r_screen.IsChecked.HasValue && this.r_screen.IsChecked.Value)
            if (this._markType == MarkType.Enlarge)
            {

                this.ShowScaleWithRect(this._dynamic.Rect);

                _dynamic.Visibility = Visibility.Collapsed;

                //  Message：设置只允许放大一次
                this.SetMarkType(MarkType.None);
            }
            else
            {
                ImageMarkEntity imgMarkEntity = new ImageMarkEntity();

                imgMarkEntity.MarkType = this._markType == MarkType.Defect ? 0 : 1;

                // 切割图片
                ImageSource imageSource = this.Source;
                System.Drawing.Bitmap bitmap = SystemUtils.ImageSourceToBitmap(imageSource);
                BitmapSource bitmapSource = SystemUtils.BitmapToBitmapImage(bitmap);

                Int32Rect rect = new Int32Rect();
                rect.X = (int)_dynamic.Rect.X;
                rect.Y = (int)_dynamic.Rect.Y;
                rect.Width = (int)_dynamic.Rect.Width;
                rect.Height = (int)_dynamic.Rect.Height;

                BitmapSource newBitmapSource = SystemUtils.CutImage(bitmapSource, new Int32Rect((int)_dynamic.Rect.X, (int)_dynamic.Rect.Y, (int)_dynamic.Rect.Width, (int)_dynamic.Rect.Height));

                //// 使用切割后的图源
                //img1.Source = newBitmapSource;

                imgMarkEntity.PicData = SystemUtils.ToBytes(newBitmapSource);

                imgMarkEntity.Width = rect.Width;

                imgMarkEntity.Height = rect.Height;

                imgMarkEntity.X = rect.X;

                imgMarkEntity.Y = rect.Y;


                this.DrawMarkedMouseUp?.Invoke(imgMarkEntity, this._markType, this);
            }

            //  Do：将数据初始化
            start = new System.Windows.Point(-1, -1);


        }
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.start.X <= 0) return;

            if (e.LeftButton == MouseButtonState.Released) return;

            InkCanvas_MouseUp(sender, null);


            Debug.WriteLine("Canvas_MouseLeave");

        }

        /// <summary> 按矩形框放大 </summary>
        void ShowScaleWithRect(Rect rect)
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            double percentX = rect.X / this.canvas.ActualWidth;

            double percentY = rect.Y / this.canvas.ActualHeight;

            double timeW = rect.Width / this.canvas.ActualWidth;
            double timeH = rect.Height / this.canvas.ActualHeight;

            double w = mask.ActualWidth * timeW;
            double h = mask.ActualHeight * timeH;


            //  Message：设置缩放比例
            Scale = Math.Min(svImg.ActualWidth / imgWidth, svImg.ActualHeight / imgHeight);

            Scale = Scale / Math.Max(timeW, timeH);

            this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";

            if (sb_Tip != null) sb_Tip.Begin();

            SetImageByScale();

            //  Message：更改区域位置
            Rect rectMark = new Rect(percentX * mask.ActualWidth, percentY * mask.ActualHeight, w, h);

            mask.UpdateSelectionRegion(rectMark, true);

        }

        void ShowPartWithShape(RectangleShape rectangle)
        {
            RectangleGeometry rect = new RectangleGeometry(new Rect(0, 0, this.canvas.ActualWidth, this.canvas.ActualHeight));

            //  Do：设置覆盖的蒙版
            var geo = Geometry.Combine(rect, new RectangleGeometry(rectangle.Rect), GeometryCombineMode.Exclude, null);

            DynamicShape shape = new DynamicShape(rectangle);

            this.ShowScaleWithRect(rectangle.Rect);

            _dynamic.Visibility = Visibility.Collapsed;
        }

        RectangleShape _currentShap;

        MouseEventHandler mouseEventHandler;

        public void ShowDefaultDefectPart(bool flag)
        {
            if (this.ViewModel == null) return;

            if (this.ViewModel.SampleCollection.Count == 0) return;

            _currentShap = this.ViewModel.SampleCollection.First().RectangleLayer.First() as RectangleShape;


            foreach (var sample in this.ViewModel.SampleCollection)
            {
                foreach (var shape in sample.RectangleLayer)
                {
                    RectangleShape rectangleShape = shape as RectangleShape;

                    if (flag)
                    {
                        rectangleShape.MouseEnter += mouseEventHandler;
                    }
                    else
                    {
                        rectangleShape.MouseEnter -= mouseEventHandler;

                        //  Message：恢复到平铺样式
                        this.SetFullImage();
                    }
                }
            }
        }

        void ShowCurrentShape()
        {
            if (_currentShap == null)
            {
                _currentShap = this.ViewModel.SampleCollection.First().RectangleLayer.First() as RectangleShape;
            }

            this.ShowPartWithShape(_currentShap);
        }

        public void ShowNextShape()
        {
            if (this.ViewModel == null) return;

            if (this.ViewModel.SampleCollection.Count == 0) return;

            var sample = this.ViewModel.SampleCollection.Where(l => l.RectangleLayer.Contains(this._currentShap));

            if (sample == null || sample.Count() == 0) return;

            int index = this.ViewModel.SampleCollection.IndexOf(sample.First());

            //  Message：如果是最后一项则跳转到第一项
            index = this.ViewModel.SampleCollection.Count - 1 == index ? 0 : index + 1;

            RectangleShape shape = this.ViewModel.SampleCollection[index].RectangleLayer.First() as RectangleShape;

            _currentShap = shape;

            this.ShowCurrentShape();
        }

        public void ShowPreShape()
        {
            if (this.ViewModel == null) return;

            if (this.ViewModel.SampleCollection.Count == 0) return;

            var sample = this.ViewModel.SampleCollection.Where(l => l.RectangleLayer.Contains(this._currentShap));

            if (sample == null || sample.Count() == 0) return;

            int index = this.ViewModel.SampleCollection.IndexOf(sample.First());

            //  Message：如果是最后一项则跳转到第一项
            index = 0 == index ? this.ViewModel.SampleCollection.Count - 1 : index - 1;

            RectangleShape shape = this.ViewModel.SampleCollection[index].RectangleLayer.First() as RectangleShape;

            _currentShap = shape;

            this.ShowCurrentShape();
        }
        #endregion

        #region - 气泡放大 -

        bool trackingMouseMove = false;

        System.Windows.Point mousePosition;

        private void MoveRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(element);
            trackingMouseMove = true;
            if (null != element)
            {
                //强制获取此元素
                element.CaptureMouse();
                element.Cursor = Cursors.Hand;
            }
        }
        private void MoveRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();
            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;

        }

        private void MoveRect_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (true)
            {
                ////计算鼠标在X轴的移动距离
                //double deltaV = e.GetPosition(element).Y - mousePosition.Y;
                ////计算鼠标在Y轴的移动距离
                //double deltaH = e.GetPosition(element).X - mousePosition.X;

                ////得到图片Top新位置
                //double newTop = deltaV + (double)element.GetValue(InkCanvas.TopProperty);
                ////得到图片Left新位置
                //double newLeft = deltaH + (double)element.GetValue(InkCanvas.LeftProperty);


                //计算鼠标在X轴的移动距离
                double deltaV = e.GetPosition(this.canvas).Y;
                //计算鼠标在Y轴的移动距离
                double deltaH = e.GetPosition(this.canvas).X;

                //得到图片Top新位置
                double newTop = deltaV;
                //得到图片Left新位置
                double newLeft = deltaH;

                ////边界的判断
                //if (newLeft <= 0)
                //{
                //    newLeft = 0;
                //}

                ////左侧图片框宽度 - 半透明矩形框宽度
                //if (newLeft >= (this.canvas.Width - this.MoveRect.Width))
                //{
                //    newLeft = this.canvas.Width - this.MoveRect.Width;
                //}

                //if (newTop <= 0)
                //{
                //    newTop = 0;
                //}

                ////左侧图片框高度度 - 半透明矩形框高度度
                //if (newTop >= this.canvas.Height - this.MoveRect.Height)
                //{
                //    newTop = this.canvas.Height - this.MoveRect.Height;
                //}
                element.SetValue(InkCanvas.TopProperty, newTop);
                element.SetValue(InkCanvas.LeftProperty, newLeft);
                AdjustBigImage();

                ////  Message：设置跟随鼠标显示
                //popup.IsOpen = false;
                //popup.IsOpen = true;


                if (mousePosition.X <= 0 || mousePosition.Y <= 0) { return; }
            }
        }

        /// <summary>
        /// 调整右侧大图的位置
        /// </summary>
        void AdjustBigImage()
        {
            //获取右侧大图框与透明矩形框的尺寸比率
            double n = this.BigBox.Width / this.MoveRect.Width;

            //获取半透明矩形框在左侧小图中的位置
            double left = (double)this.MoveRect.GetValue(InkCanvas.LeftProperty);
            double top = (double)this.MoveRect.GetValue(InkCanvas.TopProperty);

            //计算和设置原图在右侧大图框中的Canvas.Left 和 Canvas.Top
            double newLeft = -left * n;
            double newTop = -top * n;
            bigImg.SetValue(Canvas.LeftProperty, newLeft);
            bigImg.SetValue(Canvas.TopProperty, newTop);
        }

        #endregion 

        #region - 成员属性 -

        //  Do：所有图片路径集合
        LinkedList<string> _collection = new LinkedList<string>();


        public LinkedListNode<string> Current
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                 {
                     return (LinkedListNode<string>)GetValue(CurrentProperty);
                 });
            }
            set
            {
                this.Dispatcher.Invoke(() =>
               {
                   SetValue(CurrentProperty, value);
               });
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("Current", typeof(LinkedListNode<string>), typeof(ImageCore), new PropertyMetadata(default(LinkedListNode<string>), (d, e) =>
             {
                 //ImageViews control = d as ImageViews;

                 //if (control == null) return;

                 //LinkedListNode<string> config = e.NewValue as LinkedListNode<string>;

             }));


        #endregion

        #region - 路由事件 -

        //声明和注册路由事件
        public static readonly RoutedEvent LastClickedRoutedEvent =
            EventManager.RegisterRoutedEvent("LastClicked", RoutingStrategy.Bubble, typeof(EventHandler<ObjectRoutedEventArgs<ImgSliderMode>>), typeof(ImageCore));

        /// <summary>
        /// 上一页路由事件
        /// </summary>
        public event EventHandler<ObjectRoutedEventArgs<ImgSliderMode>> LastClicked
        {
            add { this.AddHandler(LastClickedRoutedEvent, value); }
            remove { this.RemoveHandler(LastClickedRoutedEvent, value); }
        }

        /// <summary>
        /// 激发上一页
        /// </summary>
        public void OnLastClicked(ImgSliderMode imgSliderMode = ImgSliderMode.User)
        {
            if (Current != null)
            {
                Current = Current.Previous;

                if (Current == null)
                {
                    Current = Collection.Last;
                }

                this.LoadImage(Current.Value);
            }

            this.PreviousImgEvent?.Invoke();

            Application.Current.Dispatcher.Invoke(() =>
            {
                ObjectRoutedEventArgs<ImgSliderMode> args = new ObjectRoutedEventArgs<ImgSliderMode>(LastClickedRoutedEvent, this, imgSliderMode);
                this.RaiseEvent(args);
            });

        }

        void RefreshCurrentText()
        {
            if (this.Collection == null) return;

            if (this.Current == null) return;

            var index = this.Collection.ToList().FindIndex(l => l == this.Current.Value);

            this.Dispatcher.Invoke(() =>
            {
                this.btn_play_current.Content = $"第{(index + 1).ToString()}/{this.Collection?.Count}张";
            });

        }

        //声明和注册路由事件
        public static readonly RoutedEvent NextClickRoutedEvent =
            EventManager.RegisterRoutedEvent("NextClick", RoutingStrategy.Bubble, typeof(EventHandler<ObjectRoutedEventArgs<ImgSliderMode>>), typeof(ImageCore));

        /// <summary>
        /// 下一页路由事件
        /// </summary>
        public event EventHandler<ObjectRoutedEventArgs<ImgSliderMode>> NextClick
        {
            add { this.AddHandler(NextClickRoutedEvent, value); }
            remove { this.RemoveHandler(NextClickRoutedEvent, value); }
        }

        /// <summary>
        /// 激发下一页
        /// </summary>
        public void OnNextClick(ImgSliderMode imgSliderMode = ImgSliderMode.User)
        {
            if (Current != null)
            {
                Current = Current.Next;

                if (Current == null)
                {
                    Current = Collection.First;
                }

                this.LoadImage(Current.Value);
            }

            //  Do：触发下一页
            this.NextImgEvent?.Invoke();

            this.Dispatcher.Invoke(() =>
            {
                ObjectRoutedEventArgs<ImgSliderMode> args = new ObjectRoutedEventArgs<ImgSliderMode>(NextClickRoutedEvent, this, imgSliderMode);

                this.RaiseEvent(args);
            });

        }

        #endregion

        #region - 工具框操作 -

        private void Btn_next_Click(object sender, RoutedEventArgs e)
        {
            this.OnNextClick();
        }

        private void Btn_preview_Click(object sender, RoutedEventArgs e)
        {
            this.OnLastClicked();
        }

        private void BtnMacthsize_Click(object sender, RoutedEventArgs e)
        {

            this.SetAdaptiveSize();
        }

        private void BtnRotate_left_Click(object sender, RoutedEventArgs e)
        {
            this.SetRotateLeft();
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.Current == null) return;

            this.DeleteImgEvent?.Invoke(this.Current.Value, this);
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is Image) && !(e.OriginalSource is Grid)) return;

            this.SetFullScreen(true);
        }

        /// <summary> 设置双击是否触发全屏 </summary>
        public bool DoubleClickSetFullScreen { get; set; } = false;

        /// <summary> 幻灯片播放 </summary>
        private void Btn_play_Click(object sender, RoutedEventArgs e)
        {
            this.StartSlidePlay();
        }

        private void Btn_play_addspeed_Click(object sender, RoutedEventArgs e)
        {
            this.ImgPlaySpeedUp();
        }

        private void Btn_play_mulspeed_Click(object sender, RoutedEventArgs e)
        {
            this.ImgPlaySpeedDown();
        }

        private void Btn_play_start_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshPlay();
        }

        /// <summary> 刷新播放状态 </summary>
        void RefreshPlay()
        {
            if (this.btn_play_start.ToolTip.ToString() == "播放")
            {
                this.PlayStart();
            }
            else
            {
                this.PlayStop();
            }
        }

        /// <summary> 开始播放 </summary>
        void PlayStart()
        {
            this.SetImgPlay(ImgPlayMode.正序);
            this.btn_play_start.ToolTip = "暂停";
            this.path_stat.Visibility = Visibility.Collapsed;
            this.path_stop.Visibility = Visibility.Visible;
        }

        /// <summary> 停止播放 </summary>
        void PlayStop()
        {
            this.SetImgPlay(ImgPlayMode.停止播放);
            this.btn_play_start.ToolTip = "播放";
            this.path_stat.Visibility = Visibility.Visible;
            this.path_stop.Visibility = Visibility.Collapsed;
        }

        /// <summary> 退出播放 </summary>
        private void Btn_play_close_Click(object sender, RoutedEventArgs e)
        {
            this.StopSlidePlay();
        }

        private void menu_closefullscreen_Click(object sender, RoutedEventArgs e)
        {
            this.CloseFullScreen();
        }

        private void CommandBinding_FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetFullScreen(true);
        }

        private void CommandBinding_FullScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute=! this._isFullScreen;

            e.CanExecute = true;
        }

        private void CommandBinding_LastImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnLastClicked();
        }

        private void CommandBinding_LastImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        private void CommandBinding_NextImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnNextClick();
        }

        private void CommandBinding_NextImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ViewModel != null;
        }

        private void CommandBinding_CloseFullScreen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_CloseFullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SetFullScreen(false);
        }

        private void MoveRect_MouseLeave(object sender, MouseEventArgs e)
        {
            this.popup.Visibility = Visibility.Collapsed;
        }

        private void MoveRect_MouseEnter(object sender, MouseEventArgs e)
        {
            this.popup.Visibility = Visibility.Visible;
        }

        public void LoadFolder(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            var images = this.GetAllFile(directory, l => l.Extension.EndsWith("jpg") || l.Extension.EndsWith("png"));

            this.LoadImg(images.ToList());
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
        #endregion 

        #region - 播放相关 -  

        /// <summary> 上一页、下一页时清理局部放大还有蒙版等页面 </summary>
        public void Clear()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ViewModel == null) return;

                //  Do：清理动态形状
                this._dynamic.Visibility = Visibility.Collapsed;

                //  Do：清理所有样本形状
                foreach (var sample in this.ViewModel.SampleCollection)
                {
                    foreach (var item in sample.RectangleLayer)
                    {
                        item.Clear(this.canvas);
                    }

                    sample.RectangleLayer.Clear();
                }

                this.ViewModel.SampleCollection.Clear();
            });

            ////  Do：隐藏蒙版
            //this.HideRectangleClip();

        }

        Task task;

        public double GetCacheCount()
        {
            switch (this.Speed)
            {
                case -1:
                    return 1.0 / 3.0;
                case -2:
                    return 1.0 / 2.0;
                case -4:
                    return 1.0 / 1.0;
                case -8:
                    return 1.0 / 0.5;
                case 1:
                    return 1.0 / 5.0;
                case 2:
                    return 1.0 / 10.0;
                case 4:
                    return 1.0 / 20.0;
                case 8:
                    return 1.0 / 40.0;
                default:
                    return 1.0 / 5.0;
            }

        }

        //  Message：取消播放任务
        CancellationTokenSource tokenSource;

        Semaphore _playSemaphore = new Semaphore(1, 1);

        /// <summary> 開始播放 </summary>
        void Start()
        {
            Action action = null;

            action = () =>
            {
                if (tokenSource.IsCancellationRequested) return;

                ImgPlayMode playMode = ImgPlayMode.正序;

                double speedTime = 0;

                bool isBuzy = false;

                this.Dispatcher.Invoke(() =>
                {
                    playMode = this.ImgPlayMode;
                    speedTime = this.ConvertSpeedFunction(this.Speed);
                    isBuzy = this.ViewModel == null ? false : this.ViewModel.IsBuzy;
                });

                if (playMode == ImgPlayMode.正序)
                {
                    if (!isBuzy)
                    {
                        this.OnNextClick(ImgSliderMode.System);
                    }
                }
                else if (playMode == ImgPlayMode.倒叙)
                {
                    if (!isBuzy)
                    {
                        this.OnLastClicked(ImgSliderMode.System);
                    }
                }

                Task nextTask = Task.Delay(TimeSpan.FromMilliseconds((1000 * speedTime)), tokenSource.Token);

                nextTask.ContinueWith(l => action());

            };

            tokenSource = new CancellationTokenSource();

            task = new Task(action, tokenSource.Token);

            task.Start();
        }

        /// <summary> 停止播放 </summary>
        void Stop()
        {
            tokenSource.Cancel();
        }

        /// <summary> 重新刷新绘制所有样本数据 </summary>
        public void RefreshAll()
        {
            foreach (var items in this.ViewModel.SampleCollection)
            {
                foreach (var item in items.RectangleLayer)
                {
                    item.Clear(this.canvas);

                    item.Draw(this.canvas);
                }
            }
        }

        #endregion

     
    }

    /// <summary> 接口实现 </summary>
    partial class ImageCore : IImageCore
    {
        public event Action<ImageMarkEntity, IImageCore> ImgMarkOperateEvent;

        public event Action PreviousImgEvent;

        public event Action NextImgEvent;

        public event Action<ImageMarkEntity, MarkType, IImageCore> DrawMarkedMouseUp;

        public event Action<string, IImageCore> DeleteImgEvent;

        public event Action<bool, IImageCore> FullScreenChangedEvent;

        public event Action<ImageMarkEntity, IImageCore> MarkEntitySelectChanged;
        public Func<int, double> ConvertSpeedFunction { get; set; }


        double _wheelScale = 0.5;

        public double WheelScale
        {
            get
            {
                return _wheelScale;
            }
            set
            {
                _wheelScale = value;
            }
        }

        public void OnImgMarkOperateEvent(ImageMarkEntity entity)
        {
            this.ImgMarkOperateEvent?.Invoke(entity, this);
        }

        public void AddImgFigure(Dictionary<string, string> imgFigures)
        {
            if (this.ViewModel == null)
            {
                Debug.WriteLine("请先加载图片数据，在添加标定信息");
                return;
            }

            this.ViewModel.FigureCollection = imgFigures;
        }

        public void AddMark(ImageMarkEntity imgMarkEntity)
        {
            MarkEntityViewModel sample = new MarkEntityViewModel();

            //sample.Name = imgMarkEntity.Name;

            //sample.Code = imgMarkEntity.PHMCodes;

            sample.Model = imgMarkEntity;

            //  Do：根据选择的样本类型来生成缺陷/样本
            if (imgMarkEntity.MarkType == 0)
            {
                DefectShape resultStroke = new DefectShape(this._dynamic);
                sample.Flag = "\xe688";
                sample.Type = imgMarkEntity.MarkType.ToString();
                resultStroke.Name = sample.Name;
                resultStroke.Code = sample.Code;
                resultStroke.Draw(this.canvas);
                sample.Add(resultStroke);
            }
            else if (imgMarkEntity.MarkType == 1)
            {
                SampleShape resultStroke = new SampleShape(this._dynamic);
                sample.Flag = "\xeaf3";
                sample.Type = imgMarkEntity.MarkType.ToString();
                resultStroke.Name = sample.Name;
                resultStroke.Code = sample.Code;
                resultStroke.Draw(this.canvas);
                sample.Add(resultStroke);
            }

            //  Message：注册选中事件
            foreach (var item in sample.RectangleLayer)
            {
                item.Selected += Item_Selected;
            }

            this.ViewModel.Add(sample);

            //  Do：触发新增事件
            this.ImgMarkOperateEvent?.Invoke(sample.Model, this);

            //  Do：清除动态框
            _dynamic.BegionMatch(false);
        }

        public Control BuildEntity()
        {
            return this;
        }

        public void CancelAddMark()
        {
            this._dynamic.Visibility = Visibility.Collapsed;
        }

        public void DeleteSelectMark()
        {
            var entity = this.GetSelectMarkEntity();

            entity.markOperateType = ImageMarkOperateType.Delete;

            this.MarkOperate(entity);
        }

        public ImageMarkEntity GetSelectMarkEntity()
        {
            if (this.ViewModel == null) return null;

            var result = this.ViewModel.SampleCollection.ToList().FindAll(l => l.RectangleLayer.First().IsSelected);

            if (result == null || result.Count == 0)
            {
                Debug.WriteLine("没有选中项！");
                return null;
            }

            return result.First().Model;
        }

        public void ImgPlaySpeedDown()
        {
            switch (this.Speed)
            {
                case 8:
                    this.Speed = 4;
                    break;
                case 4:
                    this.Speed = 2;
                    break;
                case 2:
                    this.Speed = 1;
                    break;
                case 1:
                    this.Speed = -1;
                    break;
                case -1:
                    this.Speed = -2;
                    break;
                case -2:
                    this.Speed = -4;
                    break;
                case -4:
                    this.Speed = -8;
                    break;
                default:
                    break;
            }

            this.RefreshSpeedText();

            this.RefreshCacheCapacity();
        }

        void RefreshSpeedText()
        {
            //this.btn_play_speed.Content = $"每秒{this.Speed }张";

            var value = this.ConvertSpeedFunction(this.Speed);

            this.btn_play_speed.Content = $"间隔{value}秒";

        }

       

        public void ImgPlaySpeedUp()
        {
            switch (this.Speed)
            {
                case -8:
                    this.Speed = -4;
                    break;
                case -4:
                    this.Speed = -2;
                    break;
                case 2:
                    this.Speed = 4;
                    break;
                case 4:
                    this.Speed = 8;
                    break;
                case 1:
                    this.Speed = 2;
                    break;
                case -1:
                    this.Speed = 1;
                    break;
                case -2:
                    this.Speed = -1;
                    break;
                default:
                    break;
            }

            this.RefreshSpeedText();

            this.RefreshCacheCapacity();
        }

        public void LoadCodes(Dictionary<string, string> codeDic)
        {
            if (this.ViewModel == null)
            {
                Debug.WriteLine("请先加载图片数据，在添加标定信息");
                return;
            }

            this.ViewModel.CodeCollection = codeDic;
        }

        public void LoadImg(string imgPath)
        {
            this.Collection.AddLast(imgPath);

            this.Current = this.Collection.Last;

            this.LoadImage(imgPath);

            //this.SetFullImage();
        }

        public void LoadImg(List<string> imgPathes)
        {

            if (imgPathes == null || imgPathes.Count == 0)
            {
                this.ClearAllImage();
                return;
            }

            this.ImagePaths = imgPathes;


            //  Do：加载默认图片
            this.Current = this.Collection.First;

            this.LoadImage(this.Current.Value);
        }

        void RefreshCacheCapacity()
        {
            if (_imageCacheEngine == null) return;

            int count = (int)(1 / this.GetCacheCount());

            //  Message：缓存5s的图片
            _imageCacheEngine.RefreshCapacity(count);
        }

        private ImageCacheEngine _imageCacheEngine;

        internal ImageCacheEngine ImageCacheEngine
        {
            get { return _imageCacheEngine; }
        }

        public void SetImageCacheEngine(ImageCacheEngine imageCacheEngine)
        {
            if (_imageCacheEngine != null)
            {
                _imageCacheEngine.Stop();
            }

            _imageCacheEngine = imageCacheEngine;

            var control = this.PlayerToolControl;

            this.RefreshCacheCapacity();

            if (_imageCacheEngine != null)
            {
                _imageCacheEngine.Start();

                _imageCacheEngine.CachePercentAction = () =>
                    {
                        //this.Dispatcher.Invoke(() =>
                        //{
                        //    this.PlayerToolControl?.RefersCacheValue();
                        //});


                        control.RefersCacheValue();
                    };
            }
        }

        public void LoadMarkEntitys(List<ImageMarkEntity> markEntityList)
        {
            if (markEntityList == null)
            {
                Debug.WriteLine("加载标定数据为空");
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (this.ViewModel == null)
                {
                    Debug.WriteLine("请先加载图片数据，在添加标定信息");
                    return;
                }

                foreach (var item in markEntityList)
                {
                    if (item == null) continue;
                    item.markOperateType = ImageMarkOperateType.Insert;
                    this.MarkOperate(item);
                }

                this.RefreshAll();
            });

        }

        public void MarkOperate(ImageMarkEntity entity)
        {
            //  Do：新增
            if (entity.markOperateType == ImageMarkOperateType.Insert)
            {
                MarkEntityViewModel vm = new MarkEntityViewModel(entity);

                //  Message：注册选中事件
                foreach (var item in vm.RectangleLayer)
                {
                    item.Selected += Item_Selected;
                }

                this.ViewModel.SampleCollection.Add(vm);
            }
            else
            {
                //  Do：修改
                if (entity.markOperateType == ImageMarkOperateType.Update)
                {
                    MarkEntityViewModel vm = new MarkEntityViewModel(entity);

                    this.ViewModel.SampleCollection.Add(vm);

                }
                else
                {
                    //var find = this.ViewModel.SampleCollection.ToList().Find(l => l.Name == entity.Name && l.Code == entity.Code);

                    var find = this.ViewModel.SampleCollection.ToList().Find(l => l.Model == entity);

                    if (find == null)
                    {
                        Debug.WriteLine("不存在标记：" + entity.Name);
                        return;
                    }

                    //  Message：注销事件
                    foreach (var item in find.RectangleLayer)
                    {
                        item.Selected -= Item_Selected;

                        item.Clear();
                    }

                    //find.RectangleLayer.First().Clear();

                    this.ViewModel.SampleCollection.Remove(find);
                }
            }

        }

        private void Item_Selected(RectangleShape obj)
        {
            var v = this.GetSelectMarkEntity();

            this.MarkEntitySelectChanged?.Invoke(v, this);
        }

        public void NextImg()
        {
            this.OnNextClick();
        }

        public void PreviousImg()
        {
            this.OnLastClicked();
        }

        ShortCutHookService _shortCutHookService = new ShortCutHookService();

        public bool IsWheelPlay
        {
            get => this.btn_wheel.IsChecked.HasValue && this.btn_wheel.IsChecked.Value;
            set
            {
                this.btn_wheel.IsChecked = value;
            }
        }

        public string DetialText
        {
            get
            {
                return this.txt_detial.Text;
            }
            set
            {
                this.txt_detial.Text = value;
            }
        }

        public bool IsImageLoaded { get; set; }

        public int CurrentIndex
        {
            get
            {

                return this.Dispatcher.Invoke(() =>
                 {
                     if (this.ImagePaths == null)
                     {
                         return 0;
                     }
                     //  Do：设置进度条位置
                     return this.ImagePaths.FindIndex(l => l == this.Current.Value);
                 });
            }
        }

        public double LoadPercent { get; set; } = 0.0;
        Func<int, double> IImageCore.ConvertSpeedFunction { get; set; }
        public Action<ImageCore> LoadedAction { get; set; }

        /// <summary> 此方法的说明 </summary>
        public void RegisterPartShotCut(ShortCutEntitys shortcut)
        {
            bool flag = false;

            //  Message：先清理事件
            this.ShowDefaultDefectPart(flag);

            _shortCutHookService.Clear();

            // Todo ：双击大小写切换 
            ShortCutEntitys s = new ShortCutEntitys();

            s = new ShortCutEntitys();

            KeyEntity up = new KeyEntity();
            up.Key = System.Windows.Forms.Keys.Up;
            s.Add(up);

            _shortCutHookService.RegisterCommand(s, () =>
            {
                Debug.WriteLine("按键：↑");

                if (!flag) return;

                //if (this.control_ImagePartView.Visibility == Visibility.Collapsed) return;

                this.ShowNextShape();
            });

            s = new ShortCutEntitys();

            KeyEntity down = new KeyEntity();
            down.Key = System.Windows.Forms.Keys.Down;
            s.Add(down);

            _shortCutHookService.RegisterCommand(s, () =>
            {
                Debug.WriteLine("按键：↓");

                if (!flag) return;

                //if (this.control_ImagePartView.Visibility == Visibility.Collapsed) return;

                this.ShowNextShape();
            });

            // Todo ：双击Ctrl键 
            ShortCutEntitys d = new ShortCutEntitys();

            KeyEntity c1 = new KeyEntity();
            c1.Key = System.Windows.Forms.Keys.LControlKey;
            d.Add(c1);

            KeyEntity c2 = new KeyEntity();
            c2.Key = System.Windows.Forms.Keys.LControlKey;
            d.Add(c2);


            bool _initFlag = false;

            Action action = () =>
            {
                Debug.WriteLine(shortcut);

                if (this.ViewModel == null) return;

                //if (this.Visibility == Visibility.Visible)
                //{
                //    this.OnClosed();
                //}
                ////else
                ////{
                ////    flag = !flag;

                ////    if(flag)
                ////    {
                ////        Debug.WriteLine("进入模式");
                ////    }
                ////    else
                ////    {
                ////        Debug.WriteLine("退出模式");
                ////    }

                ////    Debug.WriteLine(flag);

                ////    this.control_imageView.ShowDefaultDefectPart(flag);
                ////}

                flag = !flag;

                if (flag)
                {
                    Debug.WriteLine("进入模式");
                }
                else
                {
                    Debug.WriteLine("退出模式");
                }

                this.ShowDefaultDefectPart(flag);

                //  Message：如果是是默认加载第一个
                //if (flag)
                //{
                //    //Action<RectangleShape> mouseEnterAction = l =>
                //    //  {
                //    //      if (!flag) return;

                //    //      if (l == null) return;

                //    //      this.control_imageView.ShowPartWithShape(l);
                //    //  };


                //}


            };

            _shortCutHookService.RegisterCommand(shortcut, action);
        }

        public void RegisterDefaltApi()
        {
            // Todo ：双击Ctrl键 
            ShortCutEntitys d = new ShortCutEntitys();

            KeyEntity c1 = new KeyEntity();
            c1.Key = System.Windows.Forms.Keys.LControlKey;
            d.Add(c1);

            KeyEntity c2 = new KeyEntity();
            c2.Key = System.Windows.Forms.Keys.LControlKey;
            d.Add(c2);

            this.RegisterPartShotCut(d);
        }

        public void Rotate()
        {
            btnRotate_Click(null, null);
        }

        public void ScreenShot(string saveFullName)
        {
            byte[] screenshot = ComponetProvider.Instance.GetScreenShot(this.canvas, 1, 90);
            FileStream fileStream = new FileStream(saveFullName, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(screenshot);
            binaryWriter.Close();
        }

        bool _isFullScreen;

        public void SetFullScreen(bool isFullScreen)
        {
            if (!DoubleClickSetFullScreen)
            {
                this.FullScreenChangedEvent?.Invoke(isFullScreen, this);
                return;
            }


            if (_isFullScreen == isFullScreen) return;

            if (isFullScreen)
            {
                this.ShowFullScreen();
            }
            else
            {

                this.CloseFullScreen();
            }

            _isFullScreen = isFullScreen;
        }

        public void SetImgPlay(ImgPlayMode imgPlayMode)
        {
            //var vm = this.ViewModel; 

            //List<IImgOperate> temp = new List<IImgOperate>();

            //if (this.PlayerToolControl != null)
            //{
            //    temp =  this.PlayerToolControl.IImgOperateCollection;

            //    temp.First().
            //}


            //bool isbuzy = this.PlayerToolControl.IsBuzy;

            //Task.Run(()=>
            //{
            //    //  Message：处于加载状态时不可恶意切换播放模式
            //    while(true)
            //    {
            //        bool isbuzy=false;  

            //         Application.Current.Dispatcher.Invoke(() =>
            //         {
            //             isbuzy=this.PlayerToolControl.IsBuzy;
            //         });

            //        if (!isbuzy) break;

            //        Thread.Sleep(1000);
            //    }

            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        this.ImgPlayMode = imgPlayMode;
            //    });


            //});


            Debug.WriteLine("设置imgPlayMode" + imgPlayMode);


            this.ImgPlayMode = imgPlayMode;




        }

        public void SetSelectMarkEntity(Predicate<ImageMarkEntity> match)
        {
            if (this.ViewModel == null) return;

            var result = this.ViewModel.SampleCollection.ToList().Find(l => match(l.Model));

            if (result == null)
            {
                Debug.WriteLine("没有找到匹配项");
                return;
            }

            result.RectangleLayer.First().SetSelected();
        }

        public void ShowDefects()
        {
            foreach (var item in this.ViewModel.SampleCollection)
            {
                item.Visible = item.Type == "0";
            }
        }

        public void ShowLocates()
        {
            foreach (var item in this.ViewModel.SampleCollection)
            {
                item.Visible = item.Type == "1";
            }
        }

        public void ShowMarks()
        {
            foreach (var item in this.ViewModel.SampleCollection)
            {
                item.Visible = true;
            }
        }

        public void ShowMarks(List<string> markCodes)
        {
            foreach (var item in this.ViewModel.SampleCollection)
            {
                item.Visible = markCodes.Exists(l => l == item.Code);
            }
        }

        public void SetEnlarge()
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            //btnNarrow.IsEnabled = true;

            //if (Scale > 2)
            //    return;

            //this.GetOffSetRate();

            //Scale = Scale + WheelScale;

            //SetbtnActualsizeEnable();

            //if (Scale > 2)
            //{
            //    btnEnlarge.IsEnabled = false;
            //}
            //this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";
            //if (sb_Tip != null)
            //    sb_Tip.Begin();
            //SetImageByScale();

            this.ChangeScale(WheelScale);
        }

        public void SetNarrow()
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            this.ChangeScale(-WheelScale);


        }

        bool ChangeScale(double value)
        {
            matchscale = this.GetFullScale();

            Scale = Scale + value;

            SetbtnActualsizeEnable();

            Debug.WriteLine(Scale);
            Debug.WriteLine(matchscale);

            btnNarrow.IsEnabled = Scale > matchscale;

            btnEnlarge.IsEnabled = Scale < MaxScale;

            this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";

            if (sb_Tip != null)
                sb_Tip.Begin();

            if (Scale < matchscale)
            {
                Scale = matchscale;

                //this.SetAdaptiveSize();
                this.txtZoom.Text = "已最小";
                this.svImg.IsEnabled = false;

                //return false;
            }

            if (Scale > MaxScale)
            {
                Scale = MaxScale;
                this.svImg.IsEnabled = false;
                this.txtZoom.Text = "已最大";
            }
            this.svImg.IsEnabled = true;

            SetImageByScale();

            return true;
        }

        public void SetRotateLeft()
        {
            this.Rotate(-90);
        }

        public void SetRotateRight()
        {
            this.Rotate(90);
        }

        public void SetMarkType(MarkType markType)
        {
            this._markType = markType;

            //  Message：隐藏气泡放大控件
            this.MoveRect.Visibility = Visibility.Collapsed;

            this.popup.Visibility = Visibility.Collapsed;

            this.RefreshCursor();
        }

        void RefreshCursor()
        {
            if (this._markType == MarkType.None)
            {
                _dynamic.Visibility = Visibility.Collapsed;

                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Hand;
            }
            else if (this._markType == MarkType.Enlarge)
            {
                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Pen;

            }
            else if (this._markType == MarkType.Bubble)
            {
                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Hand;

                this.MoveRect.Visibility = Visibility.Visible;
            }
            else
            {
                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Cross;
            }
        }

        public string GetCurrentUrl()
        {
            return this.Current?.Value;
        }

        public void StartSlidePlay()
        {
            this.tool_normal.Visibility = Visibility.Collapsed;

            this.tool_play.Visibility = Visibility.Visible;

            this.RefreshPlay();
        }

        public void StopSlidePlay()
        {
            this.tool_normal.Visibility = Visibility.Visible;
            this.tool_play.Visibility = Visibility.Collapsed;

            this.PlayStop();
        }

        public void SetOriginalSize()
        {
            if (imgWidth == 0 || imgHeight == 0)
                return;

            //Scale = 0.0095;

            //imgBig.Width = imgWidth;
            //imgBig.Height = imgHeight;



            //vb.Width = imgWidth;
            //vb.Height = imgHeight;

            vb.Width = imgBig.ActualWidth;
            vb.Height = imgBig.ActualHeight;




            double result = imgBig.ActualWidth / imgWidth;

            this.Scale = Math.Min(result, imgBig.ActualHeight / imgHeight);



            SetbtnActualsizeEnable();


            this.btnActualsize.Visibility = Visibility.Collapsed;
            this.btnMacthsize.Visibility = Visibility.Visible;
        }

        public void SetAdaptiveSize()
        {
            this.SetFullImage();

            this.btnActualsize.Visibility = Visibility.Visible;

            this.btnMacthsize.Visibility = Visibility.Collapsed;

        }

        public void SetWheelMode(bool value)
        {
            this.btn_wheel.IsChecked = value;
        }

        public void SetBubbleScale(double value)
        {
            //  Do：设置控件大小
            this.popup.Width = value;
            this.popup.Height = value;

            //  Do：设置放大区域大小
            this.MoveRect.Width = value;
            this.MoveRect.Height = value;

            this.BigBox.Width = value;
            this.BigBox.Height = value;

            //  Message：防止当修改时操出范围 引起控件大小变化
            this.MoveRect.SetValue(InkCanvas.LeftProperty, 0.0);
            this.MoveRect.SetValue(InkCanvas.TopProperty, 0.0);

            this.bigrect.Rect = new Rect(0, 0, value, value);
        }

        public void Dispose()
        {
            if (_imageCacheEngine != null)
            {
                _imageCacheEngine.Stop();
            }
        }

        public List<string> GetImageList()
        {
            return this.ImagePaths;
        }

        public void SetImageSource(ImageSource imageSource)
        {
            this.Source = imageSource;

            this.ViewModel.ImageSource = imageSource;
        }

        public ImageSource GetCImageSource()
        {
            return this.Source;
        }

        public void ClearAllImage()
        {

            this.imgBig.Source = null;

            this.imgless.Source = null;

            this.Clear();

            this.ImagePaths?.Clear();

            this.Collection.Clear();

            _imageCacheEngine?.Stop();

            this.IsEnabled = false;
        }

        public void SetToolVisible(bool value)
        {
            Visibility visibility = value ? Visibility.Visible : Visibility.Collapsed;

            this.button_next.Visibility = visibility;
            this.button_last.Visibility = visibility;

            //this.grid_buzy.Visibility = visibility;

            //this.tool_normal.Visibility = visibility;
            //this.tool_play.Visibility = visibility;

            this.border.Visibility = visibility;
        }
    }

    /// <summary> 外部应用的依赖属性和事件 </summary>
    partial class ImageCore
    {
        #region - 依赖属性 -

        /// <summary> 所有图片的路径集合 </summary>
        public List<string> ImagePaths
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (List<string>)GetValue(ImagePathsProperty); ;
                });
            }
            set { SetValue(ImagePathsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePathsProperty =
            DependencyProperty.Register("ImagePaths", typeof(List<string>), typeof(ImageCore), new PropertyMetadata(default(List<string>), (d, e) =>
            {
                ImageCore control = d as ImageCore;

                if (control == null) return;

                List<string> config = e.NewValue as List<string>;

                if (config == null) return;

                if (config.Count == 0) return;

                //if (!File.Exists(config.First())) return;

                control.Collection.Clear();

                //  Do：根据路径加载图片内存集合
                foreach (var item in config)
                {
                    control.Collection.AddLast(item);
                }

                //control.Current = control.Collection.First;

                ////  Do：加载默认图片
                //control.LoadImage(control.Current.Value);


            }));

        /// <summary> 自动播放模式 </summary>
        public ImgPlayMode ImgPlayMode
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (ImgPlayMode)GetValue(ImgPlayModeProperty);
                });
            }
            set { SetValue(ImgPlayModeProperty, value); }
        }


        MarkType _tempMarkType;

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgPlayModeProperty =
            DependencyProperty.Register("ImgPlayMode", typeof(ImgPlayMode), typeof(ImageCore), new PropertyMetadata(ImgPlayMode.停止播放, (d, e) =>
            {
                ImageCore control = d as ImageCore;

                if (control == null) return;

                ImgPlayMode config = (ImgPlayMode)e.NewValue;

                //  Do：设置自动播放模式
                if (config == ImgPlayMode.正序 || config == ImgPlayMode.倒叙)
                {
                    //  Message：刷新缓存播放机制
                    if (control._imageCacheEngine != null)
                        control._imageCacheEngine.RefreshPlayMode(config == ImgPlayMode.正序);
                }

                if ((ImgPlayMode)e.OldValue != ImgPlayMode.停止播放 && (ImgPlayMode)e.NewValue != ImgPlayMode.停止播放) return;

                //  Do：设置自动播放模式
                if (config == ImgPlayMode.正序 || config == ImgPlayMode.倒叙)
                {
                    control.Start();

                    control._tempMarkType = control._markType;

                    control.SetMarkType(MarkType.None);

                    ////  Message：刷新缓存播放机制
                    //if (control._imageCacheEngine != null)
                    //    control._imageCacheEngine.RefreshPlayMode(config == ImgPlayMode.正序);
                }
                else if (config == ImgPlayMode.停止播放)
                {
                    control.Stop();

                    control.SetMarkType(control._tempMarkType);
                }

            }));


        /// <summary> 自动播放速度 </summary>
        public int Speed
        {
            get
            {

                return this.Dispatcher.Invoke(() =>
                {
                    return (int)GetValue(SpeedProperty);
                });
            }
            set
            {
                SetValue(SpeedProperty, value);
            }
        }

        public LinkedList<string> Collection { get => _collection; set => _collection = value; }

        public double Scale
        {
            get => scale;
            set
            {
                scale = value < 0 ? 0 : value;
                SetImageByScale();
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(int), typeof(ImageCore), new PropertyMetadata(1, (d, e) =>
            {
                ImageCore control = d as ImageCore;

                if (control == null) return;

                //var value = (int)(1 / control.ConvertSpeedFunction(control.Speed));


                var value = control.ConvertSpeedFunction(control.Speed);

                control.btn_play_speed.Content = $"间隔{value}秒";

                control.OnSpeedChanged();

            }));


        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource),
 typeof(ImageCore),
 new PropertyMetadata(new PropertyChangedCallback(OnSourcePropertyChanged)));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }

            set { SetValue(SourceProperty, value); }
        }

        static void OnSourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender != null && sender is ImageCore)
            {
                ImageCore view = (ImageCore)sender;
                if (args != null && args.NewValue != null)
                {
                    ImageSource imgSource = args.NewValue as ImageSource;

                    view.imgBig.Source = imgSource;

                    view.imgless.Source = imgSource;

                    view.LoadedAction?.Invoke(view);
                }
            }
        }
        public int MaxScale { get; set; } = 15;

        /// <summary> 用于控件播放是否自动适应 </summary>
        public bool IsAutoFullImage { get; set; }


        MarkType _markType;

        private double matchscale;



        public ImageViewViewModel ViewModel
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    if (this.DataContext is ImageViewViewModel)
                    {
                        return (ImageViewViewModel)this.DataContext;
                    }

                    return null;
                });

            }
            set
            {
                this.DataContext = value;
            }
        }

        public PlayerToolControl PlayerToolControl
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (PlayerToolControl)GetValue(PlayerToolControlProperty);
                });
            }
            set { SetValue(PlayerToolControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayerToolControlProperty =
            DependencyProperty.Register("PlayerToolControl", typeof(PlayerToolControl), typeof(ImageCore), new PropertyMetadata(default(PlayerToolControl), (d, e) =>
            {
                ImageCore control = d as ImageCore;

                if (control == null) return;

                PlayerToolControl config = e.NewValue as PlayerToolControl;

            }));


        //声明和注册路由事件
        public static readonly RoutedEvent SpeedChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("SpeedChanged", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageCore));
        //CLR事件包装
        public event RoutedEventHandler SpeedChanged
        {
            add { this.AddHandler(SpeedChangedRoutedEvent, value); }
            remove { this.RemoveHandler(SpeedChangedRoutedEvent, value); }
        }

        protected void OnSpeedChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SpeedChangedRoutedEvent, this);
            this.RaiseEvent(args);
        }

        #endregion   

        #region - 关键方法 -  

        /// <summary> 图片加载核心方法 </summary>
        public void LoadImage(string imagePath)
        {
            if (imagePath == null) return;

            this.Clear();

            this.RefreshCurrentText();

            if (this.ViewModel == null)
            {
                this.ViewModel = new ImageViewViewModel(this);
            }

            this.ViewModel.IsBuzy = true;

            try
            {
                bool flag = this.PlayerToolControl != null;

                ImgPlayMode imgPlayMode = this.ImgPlayMode;

                int count = this.ImagePaths == null ? 0 : this.ImagePaths.Count;

                Action<bool, int, int> action = (l, k, v) =>
                {
                    this.LoadPercent = Convert.ToDouble(k) / Convert.ToDouble(v);

                    this.Dispatcher.Invoke(() =>
                    {
                        this.PlayerToolControl?.RefreshPercent();
                    });
                };

                this.IsImageLoaded = false;

                var p = imagePath;

                if (this._imageCacheEngine != null)
                {
                    p = this._imageCacheEngine.GetWaitCurrent(imagePath, action);

                    //  Message：空标识取消操作
                    if (p == null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.ViewModel.IsBuzy = false;

                            if (this.PlayerToolControl != null)
                            {
                                this.PlayerToolControl.RefreshPercent();
                            }
                        });
                        return;
                    }
                }

                var s = new BitmapImage();
                s.BeginInit();
                s.CacheOption = BitmapCacheOption.OnLoad;
                //Thread.Sleep(4 * 1000);

                s.UriSource = new Uri(p, UriKind.RelativeOrAbsolute);
                s.EndInit();
                //这一句很重要，少了UI线程就不认了。
                s.Freeze();

                this.IsImageLoaded = true;

                if (flag)
                {
                    this.PlayerToolControl.WaitForAllReady(this.ImgPlayMode, this);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.ViewModel.ImageSource = s;

                    this.Source = s;
                    this.ViewModel.IsBuzy = false;

                    this.IsEnabled = true;

                });


            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);

            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.ViewModel.IsBuzy = false;

                });
            }

        }

        /// <summary> 滚动条放大缩小和新方法 </summary>
        void svImg_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.btn_wheel.IsChecked.HasValue && this.btn_wheel.IsChecked.Value)
            {
                if (e.Delta > 0)
                {
                    this.OnLastClicked();
                }
                else
                {
                    this.OnNextClick();
                }
            }
            else
            {
                if (imgWidth == 0 || imgHeight == 0) return;

                //  Do：找出ViewBox的左边距和上边距
                double x_viewbox_margrn = (this.rootGrid.ActualWidth - this.vb.ActualWidth) / 2;
                double y_viewbox_margrn = (this.rootGrid.ActualHeight - this.vb.ActualHeight) / 2;

                //  Do：计算边距百分比
                double x_viewbox_margrn_percent = x_viewbox_margrn / this.rootGrid.ActualWidth;
                double y_viewbox_margrn_percent = y_viewbox_margrn / this.rootGrid.ActualHeight;

                //  Do：获取鼠标在绘图区域Canvas的位置的百分比
                var position_canvas = e.GetPosition(this.canvas);

                double x_position_canvas_percent = position_canvas.X / this.canvas.ActualWidth;
                double y_position_canvas_percent = position_canvas.Y / this.canvas.ActualHeight;

                //  Do：获取鼠标站显示窗口GridMouse中的位置
                var position_gridMouse = e.GetPosition(this.gridMouse);

                //  Message：设置图片的缩放 
                this.ChangeScale(e.Delta > 0 ? WheelScale : -WheelScale);

                //  Message：根据百分比计算放大后滚轮滚动的位置
                double xvalue = x_viewbox_margrn_percent * this.rootGrid.ActualWidth + x_position_canvas_percent * this.vb.ActualWidth - position_gridMouse.X;
                double yvalue = y_viewbox_margrn_percent * this.rootGrid.ActualHeight + y_position_canvas_percent * this.vb.ActualHeight - position_gridMouse.Y;

                svImg.ScrollToHorizontalOffset(xvalue);
                svImg.ScrollToVerticalOffset(yvalue);
            }
        }

        #endregion
    }

}
