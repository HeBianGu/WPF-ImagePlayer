using HeBianGu.Base.WpfBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HeBianGu.General.ImageView
{
    /// <summary> 但点击当前控件时ListBoxItem项值也选中 </summary>
    public class ImageBaseMouseDragBehavior : Behavior<ImageBase>
    {

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.grid_Mouse_drag.PreviewMouseDown += control_MouseLeftButtonDown;
            AssociatedObject.grid_Mouse_drag.PreviewMouseUp += control_MouseLeftButtonUp;
            AssociatedObject.grid_Mouse_drag.PreviewMouseMove += control_MouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.grid_Mouse_drag.PreviewMouseDown -= control_MouseLeftButtonDown;
            AssociatedObject.grid_Mouse_drag.PreviewMouseUp -= control_MouseLeftButtonUp;
            AssociatedObject.grid_Mouse_drag.PreviewMouseMove -= control_MouseMove;

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        #region - 鸟撖图操作 -

        private bool mouseDown;

        private System.Windows.Point mouseXY;


        MarkType _markType;

        void control_MouseMove(object sender, MouseEventArgs e)
        {

            if ((this._markType != MarkType.None) && e.MiddleButton == MouseButtonState.Released)
            {
                return;
            }

            if (AssociatedObject.imgWidth == 0 || AssociatedObject.imgHeight == 0)
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
                AssociatedObject.svImg.ScrollToHorizontalOffset(AssociatedObject.svImg.HorizontalOffset + X);
            if (Y != 0)
                AssociatedObject.svImg.ScrollToVerticalOffset(AssociatedObject.svImg.VerticalOffset + Y);

            AssociatedObject.GetOffSetRate();
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


            //this.RefreshCursor();
        }

        void control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                AssociatedObject.Cursor = Cursors.Hand;
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
    }
}
