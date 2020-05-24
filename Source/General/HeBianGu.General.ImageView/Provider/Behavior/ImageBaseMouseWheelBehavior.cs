using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HeBianGu.General.ImageView
{
    /// <summary> 但点击当前控件时ListBoxItem项值也选中 </summary>
    public class ImageBaseMouseWheelBehavior : Behavior<ImageBase>
    {
        public ImageBaseMouseWheelBehavior(ImageBase imageBase) : base(imageBase)
        {

        }
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            ////gridMouse.MouseWheel += svImg_MouseWheel;

            AssociatedObject.svImg.ScrollChanged += svImg_ScrollChanged;

            AssociatedObject.rootGrid.MouseWheel += svImg_MouseWheel;

            AssociatedObject.mask.LoationChanged += Mask_LoationChanged;
        }

        protected override void OnDetaching()
        {

            AssociatedObject.svImg.ScrollChanged -= svImg_ScrollChanged;

            AssociatedObject.rootGrid.MouseWheel -= svImg_MouseWheel;

            AssociatedObject.mask.LoationChanged -= Mask_LoationChanged;


            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void Mask_LoationChanged(object sender, LoactionArgs e)
        {
            if (AssociatedObject.OperateType == OperateType.Bubble) return;

            var result = GetScrollWidthAndHeight();

            double xleft = (AssociatedObject.rootGrid.ActualWidth - AssociatedObject.vb.ActualWidth) / 2;
            double ytop = (AssociatedObject.rootGrid.ActualHeight - AssociatedObject.vb.ActualHeight) / 2;

            AssociatedObject.svImg.ScrollToHorizontalOffset(e.Left * AssociatedObject.svImg.ExtentWidth * result.Item1 + xleft);
            AssociatedObject.svImg.ScrollToVerticalOffset(e.Top * AssociatedObject.svImg.ExtentHeight * result.Item2 + ytop);
        }


        /// <summary> 滚动条放大缩小和新方法 </summary>
        void svImg_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (AssociatedObject.OperateType == OperateType.Bubble) return;

            if (AssociatedObject.imgWidth == 0 || AssociatedObject.imgHeight == 0) return;

            //  Do：找出ViewBox的左边距和上边距
            double x_viewbox_margrn = (AssociatedObject.rootGrid.ActualWidth - AssociatedObject.vb.ActualWidth) / 2;
            double y_viewbox_margrn = (AssociatedObject.rootGrid.ActualHeight - AssociatedObject.vb.ActualHeight) / 2;

            //  Do：计算边距百分比
            double x_viewbox_margrn_percent = x_viewbox_margrn / AssociatedObject.rootGrid.ActualWidth;
            double y_viewbox_margrn_percent = y_viewbox_margrn / AssociatedObject.rootGrid.ActualHeight;

            //  Do：获取鼠标在绘图区域Canvas的位置的百分比
            var position_canvas = e.GetPosition(AssociatedObject._centerCanvas);

            double x_position_canvas_percent = position_canvas.X / AssociatedObject._centerCanvas.ActualWidth;
            double y_position_canvas_percent = position_canvas.Y / AssociatedObject._centerCanvas.ActualHeight;

            //  Do：获取鼠标站显示窗口GridMouse中的位置
            var position_gridMouse = e.GetPosition(AssociatedObject.grid_Mouse_drag);

            //  Message：设置图片的缩放 
            this.ChangeScale(e.Delta > 0 ? AssociatedObject.WheelScale : -AssociatedObject.WheelScale);

            //  Message：根据百分比计算放大后滚轮滚动的位置
            double xvalue = x_viewbox_margrn_percent * AssociatedObject.rootGrid.ActualWidth + x_position_canvas_percent * AssociatedObject.vb.ActualWidth - position_gridMouse.X;
            double yvalue = y_viewbox_margrn_percent * AssociatedObject.rootGrid.ActualHeight + y_position_canvas_percent * AssociatedObject.vb.ActualHeight - position_gridMouse.Y;

            AssociatedObject.svImg.ScrollToHorizontalOffset(xvalue);
            AssociatedObject.svImg.ScrollToVerticalOffset(yvalue);
        }


        int thumbWidth;
        int thumbHeight;

        void svImg_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (AssociatedObject.OperateType == OperateType.Bubble) return;

            if (AssociatedObject.imgWidth == 0 || AssociatedObject.imgHeight == 0)
                return;

            thumbWidth = (int)AssociatedObject.controlmask.ActualWidth;
            thumbHeight = (int)AssociatedObject.controlmask.ActualHeight;

            double xleft = (AssociatedObject.rootGrid.ActualWidth - AssociatedObject.vb.ActualWidth) / 2;
            double ytop = (AssociatedObject.rootGrid.ActualHeight - AssociatedObject.vb.ActualHeight) / 2;


            var result = this.GetScrollWidthAndHeight();

            double scroll_width = AssociatedObject.svImg.ViewportWidth + AssociatedObject.svImg.ScrollableWidth;

            double timeH = AssociatedObject.svImg.ViewportHeight / (AssociatedObject.svImg.ViewportHeight + AssociatedObject.svImg.ScrollableHeight);
            double timeW = AssociatedObject.svImg.ViewportWidth / (scroll_width - 2 * xleft);

            double w = thumbWidth * timeW;
            double h = thumbHeight * timeH;

            double offsetx = 0;
            double offsety = 0;

            if (AssociatedObject.svImg.ScrollableWidth == 0)
            {
                offsetx = 0;
            }
            else
            {
                offsetx = (w - thumbWidth) / AssociatedObject.svImg.ScrollableWidth * (AssociatedObject.svImg.HorizontalOffset);
            }

            offsetx = -(AssociatedObject.svImg.HorizontalOffset - xleft) / (scroll_width - xleft * 2) * thumbWidth;

            if (AssociatedObject.svImg.ScrollableHeight == 0)
            {
                offsety = 0;
            }
            else
            {
                offsety = (h - thumbHeight) / AssociatedObject.svImg.ScrollableHeight * AssociatedObject.svImg.VerticalOffset;
            }

            Rect rect = new Rect(-offsetx, -offsety, w, h);

            AssociatedObject.mask.UpdateSelectionRegion(rect);
        }


        Tuple<double, double> GetScrollWidthAndHeight()
        {
            double xleft = 1 - (AssociatedObject.rootGrid.ActualWidth - AssociatedObject.vb.ActualWidth) / AssociatedObject.rootGrid.ActualWidth;
            double ytop = 1 - (AssociatedObject.rootGrid.ActualHeight - AssociatedObject.vb.ActualHeight) / AssociatedObject.rootGrid.ActualHeight;

            return Tuple.Create(xleft, ytop);
        }

        double matchscale;

        bool ChangeScale(double value)
        {
            matchscale = AssociatedObject.GetFullScale();

            AssociatedObject.Scale = AssociatedObject.Scale + value;

            //SetbtnActualsizeEnable();

            Debug.WriteLine(AssociatedObject.Scale);
            Debug.WriteLine(matchscale);

            //btnNarrow.IsEnabled = Scale > matchscale;

            //btnEnlarge.IsEnabled = Scale < MaxScale;

            AssociatedObject.IsMaxScaled = !(AssociatedObject.Scale < AssociatedObject.MaxScale);

            AssociatedObject.IsMinScaled = !(AssociatedObject.Scale > matchscale);

            //this.txtZoom.Text = ((int)(Scale * 100)).ToString() + "%";

            AssociatedObject.OnNoticeMessaged(((int)(AssociatedObject.Scale * 100)).ToString() + "%");

            //if (sb_Tip != null)
            //    sb_Tip.Begin();

            if (AssociatedObject.Scale < matchscale)
            {
                AssociatedObject.Scale = matchscale;

                AssociatedObject.OnNoticeMessaged("已最小");

                AssociatedObject.svImg.IsEnabled = false;
            }

            if (AssociatedObject.Scale > AssociatedObject.MaxScale)
            {
                AssociatedObject.Scale = AssociatedObject.MaxScale;

                AssociatedObject.svImg.IsEnabled = false;

                AssociatedObject.OnNoticeMessaged("已最大");
            }
            AssociatedObject.svImg.IsEnabled = true;

            AssociatedObject.RefreshImageByScale();

            return true;
        }
    }
}
