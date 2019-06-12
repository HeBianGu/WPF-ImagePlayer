using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary>
    /// PlayerToolControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerToolControl : UserControl
    {
        public PlayerToolControl()
        {
            InitializeComponent();
        }

        public event DragCompletedEventHandler DragCompleted;

        //  Message：标识拖动条是否随播放变化
        internal bool SliderFlag { get; set; } = false;

        private void media_slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            this.SliderFlag = true;
        }

        private void media_slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            this.SliderFlag = false;

            this.DragCompleted?.Invoke(sender, e);
        }

        /// <summary> 多个图片播放时用于检测播放同步 </summary>
        public List<IImageCore> IImgOperateCollection { get; set; } = new List<IImageCore>();

        public void RefreshPercent()
        {
            var percent = (int)(this.IImgOperateCollection.Sum(m => m.LoadPercent) / Convert.ToDouble(this.IImgOperateCollection.Count) * 100);
            this.Message = percent.ToString()=="0"?"Loading": percent.ToString()+"%";
            this.IsBuzy = percent != 100;
        }

        Semaphore _semaphore = new Semaphore(1, 1);

        //private bool _cancel = false;
        public void WaitForAllReady(ImgPlayMode imgPlayMode, IImageCore operate)
        {
            //_cancel = true;

            //_semaphore.WaitOne();

            int count = operate.GetImageList().Count;

            while (true)
            {
                //if (_cancel)
                //{
                //    _cancel = false;
                //    break;
                //}
                int cindex = operate.CurrentIndex;
                int max = this.IImgOperateCollection.Max(k => k.CurrentIndex);
                int min = this.IImgOperateCollection.Min(k => k.CurrentIndex);
                if (Math.Abs(cindex - max) > 1) break;
                if (Math.Abs(cindex - min) > 1) break;

                if (imgPlayMode == ImgPlayMode.正序)
                {
                    if (this.IImgOperateCollection.TrueForAll(
                            l => l.IsImageLoaded) && operate.IsImageLoaded && operate.CurrentIndex == this.IImgOperateCollection.Min(k => k.CurrentIndex))
                    {
                        break;
                    }
                }
                else if (imgPlayMode == ImgPlayMode.倒叙)
                {
                    //  Message：播放到第一个位置单独处理 如:0 和32
                    if (this.IImgOperateCollection.Exists(l => l.CurrentIndex == 0) && this.IImgOperateCollection.Exists(l => l.CurrentIndex == count - 1))
                    {
                        if (this.IImgOperateCollection.TrueForAll(l => l.IsImageLoaded) && operate.CurrentIndex == this.IImgOperateCollection.Min(k => k.CurrentIndex))
                        {
                            break;
                        }
                    }
                    else
                    {

                        try
                        {
                            if (this.IImgOperateCollection.TrueForAll(l => l.IsImageLoaded) && cindex == min)
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                    }
                }
                else
                {
                    break;
                }

                Thread.Sleep(20);
            }

            //_semaphore.Release();
        }


        public bool IsBuzy
        {
            get { return (bool)GetValue(IsBuzyProperty); }
            set { SetValue(IsBuzyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBuzyProperty =
            DependencyProperty.Register("IsBuzy", typeof(bool), typeof(PlayerToolControl), new PropertyMetadata(default(bool), (d, e) =>
             {
                 PlayerToolControl control = d as PlayerToolControl;

                 if (control == null) return;

                 //bool config = e.NewValue as bool;

             }));


        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(PlayerToolControl), new PropertyMetadata(default(string), (d, e) =>
             {
                 PlayerToolControl control = d as PlayerToolControl;

                 if (control == null) return;

                 string config = e.NewValue as string;

             }));

        //public void SetCacheValue(double percent)
        //{
        //    this.media_slider_cache.Value = this.media_slider_cache.Maximum * percent;
        //}


        public void RefersCacheValue()
        {  
            if (this.IImgOperateCollection.Count == 0)
            {
                this.media_slider_cache.Value = 0;
                return;
            }

            //this.Dispatcher.Invoke(() =>
            //{
            //    //this.media_slider_cache.Value = this.media_slider_cache.Maximum * this.IImgOperateCollection
            //    //                                    .Cast<ImageViews>().Min(l =>
            //    //                                        l.ImageCacheEngine == null
            //    //                                            ? 0
            //    //                                            : l.ImageCacheEngine.GetBufferPercent());
            //});

            Action action = () =>
            {
                this.media_slider_cache.Value = this.media_slider_cache.Maximum * this.IImgOperateCollection
                                                    .Cast<ImageCore>().Min(l =>
                                                        l.ImageCacheEngine == null
                                                            ? 0
                                                            : l.ImageCacheEngine.GetBufferPercent());
            };

            this.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, action);
        }


        public void RefersValue()
        {
            if (this.IImgOperateCollection.Count == 0)
            {
                this.media_slider.Value = 0;
                return;
            }

            var collection = this.IImgOperateCollection.Cast<ImageCore>();

            int index = collection.Min(l => l.ImagePaths == null ? 0 : l.ImagePaths.FindIndex(k => k == l.Current.Value));
            this.media_slider.Value = TimeSpan.FromMilliseconds(1000 * index).Ticks;

            //if (collection.FirstOrDefault().ImgPlayMode==ImgPlayMode.倒叙)
            //{
            //    int index = collection.Min(l => l.ImagePaths.FindIndex(k => k == l.Current.Value));
            //    this.media_slider.Value = TimeSpan.FromMilliseconds(1000 * index).Ticks;
            //}
            //else
            //{

            //    int index = collection.Max(l => l.ImagePaths.FindIndex(k => k == l.Current.Value));
            //    this.media_slider.Value = TimeSpan.FromMilliseconds(1000 * index).Ticks;
            //}







            ////  Do：设置进度条位置
            //var index = this.image_control.ImagePaths.FindIndex(l => l == this.image_control.Current.Value);

            //TimeSpan.FromMilliseconds(1000 * index).Ticks
        }

    }
}
