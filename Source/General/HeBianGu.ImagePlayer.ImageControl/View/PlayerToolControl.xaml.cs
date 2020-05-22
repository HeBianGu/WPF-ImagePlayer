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
        public List<IImagePlayerService> IImgOperateCollection { get; set; } = new List<IImagePlayerService>();

        public void RefreshPercent()
        {
            var percent = (int)(this.IImgOperateCollection.Sum(m => m.GetImgOperate().LoadPercent) / Convert.ToDouble(this.IImgOperateCollection.Count) * 100);
            this.Message = percent.ToString()=="0"?"Loading": percent.ToString()+"%";
            this.IsBuzy = percent != 100;
        }

        Semaphore _semaphore = new Semaphore(1, 1); 
        public void WaitForAllReady(ImgPlayMode imgPlayMode, IImageCore operate)
        { 

            int count = operate.GetImageList().Count;

            var operates = this.IImgOperateCollection.Select(l=>l.GetImgOperate()).ToList();

            while (true)
            { 
                int cindex = operate.CurrentIndex;
                int max = operates.Max(k => k.CurrentIndex);
                int min = operates.Min(k => k.CurrentIndex);
                if (Math.Abs(cindex - max) > 1) break;
                if (Math.Abs(cindex - min) > 1) break;

                if (imgPlayMode == ImgPlayMode.正序)
                {
                    if (operates.TrueForAll(
                            l => l.IsImageLoaded) && operate.IsImageLoaded && operate.CurrentIndex == operates.Min(k => k.CurrentIndex))
                    {
                        break;
                    }
                }
                else if (imgPlayMode == ImgPlayMode.倒序)
                    
                {
                    //  Message：播放到第一个位置单独处理 如:0 和32
                    if (operates.Exists(l => l.CurrentIndex == 0) && operates.Exists(l => l.CurrentIndex == count - 1))
                    {
                        if (operates.TrueForAll(l => l.IsImageLoaded) && operate.CurrentIndex == operates.Min(k => k.CurrentIndex))
                        {
                            break;
                        }
                    }
                    else
                    {

                        try
                        {
                            if (operates.TrueForAll(l => l.IsImageLoaded) && cindex == min)
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


        public void RefersCacheValue()
        {  
            if (this.IImgOperateCollection.Count == 0)
            {
                this.media_slider_cache.Value = 0;
                return;
            } 

            Action action = () =>
            {
                this.media_slider_cache.Value = this.media_slider_cache.Maximum * this.IImgOperateCollection.Select(l=>l.GetImgOperate())
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

        

            var collection = this.IImgOperateCollection.Select(l=>l.GetImgOperate()).Cast<ImageCore>();

            //int index = collection.Min(l => l.ImagePaths == null ? 0 : l.ImagePaths.FindIndex(k => k == l.Current.Value));

      

            int index = collection.Min(l=>l.CurrentIndex);

            //DateTime time = DateTime.Now;

            this.media_slider.Value = TimeSpan.FromMilliseconds(1000 * index).Ticks;



            //System.Diagnostics.Debug.WriteLine("间隔:"+ (DateTime.Now - time).Ticks);

        }

        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }

        private void Btn_addspeed_Click(object sender, RoutedEventArgs e)
        {
            this.ImgPlaySpeedUp();
        }

        private void Btn_mulspeed_Click(object sender, RoutedEventArgs e)
        {
            this.ImgPlaySpeedDown();
        }

        private void Btn_addstep_Click(object sender, RoutedEventArgs e)
        {
            this.PlayStepUp();
        }

        private void Btn_mulstep_Click(object sender, RoutedEventArgs e)
        {
            this.PlayStepDown();
        }

        public void PlayStepUp()
        {

            if (this.IImgOperateCollection == null || this.IImgOperateCollection.Count == 0) return;

            var find = this.IImgOperateCollection.First();

            var operate = find.GetImgOperate() as ImageCore;

            double speed = operate.ConvertSpeedFunction(operate.Speed);

            this.media_slider.Value += TimeSpan.FromSeconds(speed).Ticks;

            foreach (var item in this.IImgOperateCollection)
            {
                item.PlayStepUp();
            }
        }

        public void PlayStepDown()
        {
            if (this.IImgOperateCollection == null || this.IImgOperateCollection.Count == 0) return;

            var find = this.IImgOperateCollection.First();

            var operate = find.GetImgOperate() as ImageCore; ;

            double speed = operate.ConvertSpeedFunction(operate.Speed);

            //double speed = 5 / find.ImagePlayerService.GetImgOperate().GetSpeedSplitTime();

            this.media_slider.Value -= TimeSpan.FromSeconds(speed).Ticks;

            foreach (var item in this.IImgOperateCollection)
            {
                item.PlayStepDown();
            }
        }

        public void ImgPlaySpeedDown()
        {
            if (this.IImgOperateCollection == null || this.IImgOperateCollection.Count == 0) return;

            foreach (var item in this.IImgOperateCollection)
            {
                item.ImgPlaySpeedDown();
            }
        }

        public void ImgPlaySpeedUp()
        {
            if (this.IImgOperateCollection == null || this.IImgOperateCollection.Count == 0) return;

            foreach (var item in this.IImgOperateCollection)
            {
                item.ImgPlaySpeedUp();
            }
        } 

        public void Next()
        {
            foreach (var item in this.IImgOperateCollection)
            {
                item.NextImg();
            }
        }

        public void Last()
        {
            foreach (var item in this.IImgOperateCollection)
            {
                item.PreviousImg();
            }
        }

        public void Stop()
        {
            if (this.IImgOperateCollection == null || this.IImgOperateCollection.Count == 0) return;

            foreach (var item in this.IImgOperateCollection)
            {
                item.SetImgPlay(ImgPlayMode.停止播放);
                item.SetPositon(0);
            }
        }

        public ImgPlayMode RefreshPlayState()
        {
            this.toggle_play.IsChecked = !this.toggle_play.IsChecked;

            foreach (var item in this.IImgOperateCollection)
            {
                if (this.toggle_play.IsChecked.Value)
                {
                    item.SetImgPlay(ImgPlayMode.停止播放);
                }
                else
                {
                    item.SetImgPlay(ImgPlayMode.正序);
                }

            }

            return this.toggle_play.IsChecked.Value ? ImgPlayMode.停止播放 : ImgPlayMode.正序;
        }

    }
}
