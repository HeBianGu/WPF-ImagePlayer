using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeBianGu.ImagePlayer.ImageControl;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary>
    /// 视频播放控件
    /// </summary>
    public partial class MediaPlayerControl : UserControl
    {
        public MediaPlayerControl()
        {
            InitializeComponent();

            this.media_media.MediaEnded += Player_MediaEnded;
            this.media_media.MediaOpened += Player_MediaOpened;
            this.media_media.MediaFailed += Player_MediaFailed;
            this.media_media.Loaded += Player_Loaded;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Interval = 1000;

        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.RefreshSlider();
        }

        Timer _timer = new Timer();

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Player_Loaded");
        }

        void InitSlider()
        {
            if (this.media_media.Source == null) return;

            if (this.media_media.NaturalDuration.HasTimeSpan)
            {
                this.PlayerToolControl.media_slider.Maximum = this.media_media.NaturalDuration.TimeSpan.Ticks;
            }
        }

        void RefreshSlider()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mediaPlayMode == MediaPlayMode.RepeatFromTo)
                {
                    if (this._repeatFromTo == null) return;

                    if (this.media_media.Position < _repeatFromTo.Item1)
                    {
                        this.media_media.Position = _repeatFromTo.Item1;
                    }

                    if (this.media_media.Position > _repeatFromTo.Item2)
                    {
                        this.media_media.Position = _repeatFromTo.Item1;
                    }
                }

                this.PlayerToolControl.media_slider.Value = this.media_media.Position.Ticks;
            });
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
            Debug.WriteLine("Player_MediaFailed");

        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Player_MediaOpened");

            this.InitSlider();

            this.InitSound();

            this._timer.Start();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Player_MediaEnded");

            this.Stop();
        }

        //private void media_slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        //{
        //    if (this.media_media == null) return;

        //    this.media_media.Position = TimeSpan.FromTicks((long)this.media_slider.Value);

        //    this._timer.Start();
        //}

        private void Media_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            ////  Message：当是鼠标点击引起的改变是触发SetPositon
            //if (Mouse.LeftButton != MouseButtonState.Pressed) return;
            //if (!this.media_slider.IsMouseOver) return;

            //int index = (int)((this.media_slider.Value / this.media_slider.Maximum) * this.image_control.ImagePaths.Count);


            //Debug.WriteLine("MouseButtonState.Pressed");

            ////  Do：设置播放位置
            //this.SetPositon(index);

            if (this.media_media == null) return;

            this.media_media.Position = TimeSpan.FromTicks((long)this.PlayerToolControl.media_slider.Value);
        }

        void InitSound()
        {
            this.PlayerToolControl.slider_sound.Value = this.media_media.Volume;
        }

        //private void slider_sound_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        //{

        //}

        private void Slider_sound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.media_media.Volume = this.PlayerToolControl.slider_sound.Value;
        }

        //private void media_slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        //{
        //    this._timer.Stop();
        //}

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PlayerToolControl.toggle_play.IsChecked.Value)
            {
                this.Pause();
            }
            else
            {

                this.Play();
            }
        }

        private void CommandBinding_Executed_Play(object sender, ExecutedRoutedEventArgs e)
        {

            Debug.WriteLine("CommandBinding_Executed_Play");

        }

        private void CommandBinding_CanExecute_Play(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MediaBrower_PlayClick(object sender, RoutedEventArgs e)
        {
            this.media_media.Stop();
            this.media_media.Play();
            this.PlayerToolControl.toggle_play.IsChecked = false;
        }

        internal void Play()
        {
            this.media_media.Play();

            this._timer.Start();

            this.PlayerToolControl.toggle_play.IsChecked = false;}

        internal void Pause()
        {
            this.media_media.Pause();
            this._timer.Stop();
            this.PlayerToolControl.toggle_play.IsChecked = true;
        }

        void Stop()
        {
            this.media_media.Position = TimeSpan.FromTicks(0);
            if (this.PlayerToolControl != null)
                this.PlayerToolControl.media_slider.Value = 0;
            this.media_media.Stop();
            this._timer.Stop();
            if (this.PlayerToolControl != null)
                this.PlayerToolControl.toggle_play.IsChecked = true;
            this.media_media.LoadedBehavior = MediaState.Manual;
        }

        Point start;

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            _dynamic.BegionMatch(true);

            start = e.GetPosition(sender as InkCanvas);

        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (!(this.r_screen.IsChecked.HasValue && this.r_screen.IsChecked.Value)) return;



            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (this.start.X <= 0) return;

            Point end = e.GetPosition(this.canvas);

            //this._isMatch = Math.Abs(start.X - end.X) > 50 && Math.Abs(start.Y - end.Y) > 50;

            _dynamic.Visibility = Visibility.Visible;

            _dynamic.Refresh(start, end);

        }

        /// <summary>
        /// 鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //  Do：检查选择区域是否可用
            if (!_dynamic.IsMatch())
            {
                _dynamic.Visibility = Visibility.Collapsed;
                return;
            };

            if (this.start.X <= 0) return;

            //  Do：如果是选择局部放大
            if (this.r_screen.IsChecked.HasValue && this.r_screen.IsChecked.Value)
            {
                RectangleGeometry rect = new RectangleGeometry(new Rect(0, 0, this.canvas.ActualWidth, this.canvas.ActualHeight));

                //  Do：设置覆盖的蒙版
                var geo = Geometry.Combine(rect, new RectangleGeometry(this._dynamic.Rect), GeometryCombineMode.Exclude, null);

                DynamicShape shape = new DynamicShape(this._dynamic);

                ////  Do：设置形状、用来提供给局部放大页面
                //this.DynamicShape = shape;

                ////  Do：设置提供局部放大在全局的范围的视图
                //this.ImageVisual = this.canvas;

                //this.OnBegionShowPartView();

                ////  Do：设置当前蒙版的剪切区域
                //this.rectangle_clip.Clip = geo;

                _dynamic.Visibility = Visibility.Collapsed;

                MediaPartControl mediaPartControl = new MediaPartControl();
                mediaPartControl.DynamicShape = shape;
                mediaPartControl.ImageVisual = this.canvas;



                Window window = new Window();
                mediaPartControl.Closed += (l, k) =>
                {
                    window.Close();
                };

                window.Content = mediaPartControl;
                window.ShowDialog();



            }


            //  Do：将数据初始化
            start = new Point(-1, -1);


        }

        private void Btn_rotateTransform_Click(object sender, RoutedEventArgs e)
        {
            //TransformGroup transformGroup = this.media_media.RenderTransform as  TransformGroup;
            RotateTransform rotate = this.media_media.RenderTransform as RotateTransform;
            rotate.CenterX = this.media_media.ActualWidth / 2;
            rotate.CenterY = this.media_media.ActualHeight / 2;
            rotate.Angle = rotate.Angle + 90;

        }

        //private void Btn_addspeed_Click(object sender, RoutedEventArgs e)
        //{
        //    this.media_media.SpeedRatio = this.media_media.SpeedRatio * 2;
        //}

        //private void Btn_multipspeed_Click(object sender, RoutedEventArgs e)
        //{
        //    this.media_media.SpeedRatio = this.media_media.SpeedRatio / 2;
        //}

        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        private void r_screen_Checked(object sender, RoutedEventArgs e)
        {
            if (this.r_screen.IsChecked.HasValue && this.r_screen.IsChecked.Value)
            {
                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Cross;
            }
            else
            {
                //  Message：设置光标和区域放大
                this.canvas.Cursor = Cursors.Arrow;
            }
        }

        private void Btn_addspeed_Click(object sender, RoutedEventArgs e)
        {
            //this.PlaySpeedUp();


        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.PlayerToolControl.toggle_play.IsChecked.Value)
            {
                this.Play();

                //this.toggle_play.IsChecked = false;
            }
            else
            {



                this.Pause();

                //this.toggle_play.IsChecked = true;
            }
        }



        public PlayerToolControl PlayerToolControl
        {
            get { return (PlayerToolControl)GetValue(PlayerToolControlProperty); }
            set { SetValue(PlayerToolControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayerToolControlProperty =
            DependencyProperty.Register("PlayerToolControl", typeof(PlayerToolControl), typeof(MediaPlayerControl), new PropertyMetadata(default(PlayerToolControl), (d, e) =>
             {
                 MediaPlayerControl control = d as MediaPlayerControl;

                 if (control == null) return;

                 PlayerToolControl config = e.NewValue as PlayerToolControl;

                 //control.ResgiterPlayerToolControl();

             }));

        public void ResgiterPlayerToolControl()
        {
            //  Message：注册播放事件
            this.PlayerToolControl.toggle_play.Click += this.ToggleButton_Click;

            this.PlayerToolControl.media_slider.ValueChanged += this.Media_slider_ValueChanged;


            this.PlayerToolControl.slider_sound.ValueChanged += this.Slider_sound_ValueChanged;

        }

        public void DisposePlayerToolControl()
        {
            
                //this.Stop();

                //  Message：注册播放事件
                this.PlayerToolControl.toggle_play.Click -= this.ToggleButton_Click;

                this.PlayerToolControl.media_slider.ValueChanged -= this.Media_slider_ValueChanged;

                this.PlayerToolControl.slider_sound.ValueChanged -= this.Slider_sound_ValueChanged;

        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.FullScreenHandle?.Invoke();
        }
    }

    public partial class MediaPlayerControl : IMediaPlayerService
    {

        MediaPlayMode _mediaPlayMode = MediaPlayMode.Normal;

        Tuple<TimeSpan, TimeSpan> _repeatFromTo;

        /// <summary>
        /// 获取当前帧
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentFrame()
        {
            if (this.media_media.Source == null) return TimeSpan.MinValue;

            if (!this.media_media.NaturalDuration.HasTimeSpan) return TimeSpan.MinValue;

            return this.media_media.Position;
        }

        public string GetCurrentUrl()
        {
            return Uri.UnescapeDataString(_url);

            //return this.media_media.Source.OriginalString;
        }

        public TimeSpan GetTotalFrame()
        {
            if (this.media_media.Source == null) return TimeSpan.MinValue;

            if (!this.media_media.NaturalDuration.HasTimeSpan) return TimeSpan.MinValue;

            return TimeSpan.FromTicks(this.media_media.NaturalDuration.TimeSpan.Ticks);
        }

        string _url;

        public event Action FullScreenHandle;

        public void Load(string mediaPath)
        {
            Uri uri = new Uri(mediaPath, UriKind.Absolute);

            _url = mediaPath;

            this.media_media.Source = uri;

            this.Play();
        }

        //public void LoadImageFolder(string imageFoder)
        //{
        //    throw new NotImplementedException();
        //}

        //List<string> _imageUrls = new List<string>();

        //public void LoadImages(List<string> ImageUrls)
        //{
        //    this._mediaPlayType = MediaPlayType.ImageList;

        //    _imageUrls = ImageUrls;

        //    Uri uri = new Uri(ImageUrls.First(), UriKind.Absolute);

        //    this.media_media.Source = uri;

        //    this.Play();
        //}

        public double Volumn
        {
            get
            {
                return this.media_media.Volume;
            }
            set
            {
                this.PlayerToolControl.slider_sound.Value = this.media_media.Volume = value;
            }
        }

        public void VoiceStepUp()
        {
            this.PlayerToolControl.slider_sound.Value += 0.1;
        }

        public void VoiceStepDown()
        {
            this.PlayerToolControl.slider_sound.Value -= 0.1;
        }

        public void RepeatFromTo(TimeSpan from, TimeSpan to)
        {
            if (from > to) return;

            this._mediaPlayMode = MediaPlayMode.RepeatFromTo;

            _repeatFromTo = new Tuple<TimeSpan, TimeSpan>(from, to);
        }

        public void ScreenShot(TimeSpan from, string saveFullName)
        {
            byte[] screenshot = this.media_media.GetScreenShot(1, 90);
            FileStream fileStream = new FileStream(saveFullName, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(screenshot);
            binaryWriter.Close();
        }

        public void SetPositon(TimeSpan timeSpan)
        {
            this.media_media.Position = timeSpan;
        }

        public void Rotate(double value)
        {
            RotateTransform rotate = this.media_media.RenderTransform as RotateTransform;
            rotate.CenterX = this.media_media.ActualWidth / 2;
            rotate.CenterY = this.media_media.ActualHeight / 2;
            rotate.Angle = rotate.Angle + value;
        }

        public void PlaySpeedUp()
        {
            this.media_media.SpeedRatio = this.media_media.SpeedRatio * 2;

            this.PlayerToolControl.media_speed.Text = this.media_media.SpeedRatio.ToString() + "X";
        }

        public void PlaySpeedDown()
        {
            this.media_media.SpeedRatio = this.media_media.SpeedRatio / 2;

            this.PlayerToolControl.media_speed.Text = this.media_media.SpeedRatio.ToString() + "X";
        }

        public void PlayStepUp()
        {
            double v = this.PlayerToolControl.media_slider.Value + TimeSpan.FromSeconds(5).Ticks;

            if (v > this.PlayerToolControl.media_slider.Maximum)
            {
                this.PlayerToolControl.media_slider.Value = this.PlayerToolControl.media_slider.Maximum;
            }
            else
            {
                this.PlayerToolControl.media_slider.Value = v;
            }
        }

        public void PlayStepDown()
        {
            double v = this.PlayerToolControl.media_slider.Value - TimeSpan.FromSeconds(5).Ticks;

            if (v < 0)
            {
                this.PlayerToolControl.media_slider.Value = 0;
            }
            else
            {
                this.PlayerToolControl.media_slider.Value = v;
            }
        }

        public void RotateLeft()
        {
            this.Rotate(-90);
        }

        public void RotateRight()
        {
            this.Rotate(90);
        }
    }

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            //if (value.ToString() == "0") return "0";
            //if (value.ToString() == "100") return "100";

            var d = double.Parse(value.ToString());

            var sp = TimeSpan.FromTicks((long)d);

            return sp.ToString().Split('.')[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    enum MediaPlayMode
    {
        Normal = 0, RepeatFromTo
    }

    #region Extension Methods
    public static class ScreenShot
    {
        public static byte[] GetScreenShot(this UIElement source, double scale, int quality)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;
            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;
            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth,
                (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
                    new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            Byte[] imageArray;
            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                imageArray = outputStream.ToArray();
            }
            return imageArray;
        }
    }
    #endregion
}
