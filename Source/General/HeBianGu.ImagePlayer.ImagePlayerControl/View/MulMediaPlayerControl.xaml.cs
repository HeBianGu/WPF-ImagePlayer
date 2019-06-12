using System;
using System.Collections;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HeBianGu.ImagePlayer.ImageControl;
using HeBianGu.ImagePlayer.ImageControl.Hook;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary>
    /// MulMediaPlayerControl.xaml 的交互逻辑
    /// </summary>
    public partial class MulMediaPlayerControl : UserControl
    {
        public MulMediaPlayerControl()
        {
            InitializeComponent();

            ////  Do：改变窗口自适应大小
            //this.SizeChanged += (l, k) =>
            //{

            //    foreach (var item in this.MediaSources)
            //    {
            //        //var operate = item.ImagePlayerService.GetImgOperate().SetAdaptiveSize();
            //        var operate = item.ImagePlayerService.GetImgOperate();

            //        operate.SetAdaptiveSize();

            //        //operate.Scale = operate.GetFullScale();

            //        //operate.SetAdaptiveSize();
            //    }
            //};

        }

        private void MulMediaPlayerControl_ImageIndexFullScreenEvent(int obj)
        {

            Debug.WriteLine("全屏点击了");

        }

        public List<IVdeioImagePlayer> MediaSources
        {
            get { return (List<IVdeioImagePlayer>)GetValue(MediaSourcesProperty); }
            set { SetValue(MediaSourcesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaSourcesProperty =
            DependencyProperty.Register("MediaSources", typeof(List<IVdeioImagePlayer>), typeof(MulMediaPlayerControl), new PropertyMetadata(new List<IVdeioImagePlayer>(), (d, e) =>
             {
                 MulMediaPlayerControl control = d as MulMediaPlayerControl;

                 if (control == null) return;

                 List<IVdeioImagePlayer> config = e.NewValue as List<IVdeioImagePlayer>;

                 control.Init(config);

                 if (config == null) return;

                 if (config.Count == 1)
                 {
                     control.RowCount = 1;
                     control.ColCount = 1;
                 }

                 if (config.Count == 2)
                 {
                     control.RowCount = 1;
                     control.ColCount = 2;
                 }

                 if (config.Count == 3 || config.Count == 4)
                 {
                     control.RowCount = 2;
                     control.ColCount = 2;
                 }

                 if (config.Count == 5 || config.Count == 6)
                 {
                     control.RowCount = 3;
                     control.ColCount = 2;
                 }

                 if (config.Count == 7 || config.Count == 8 || config.Count == 9)
                 {
                     control.RowCount = 3;
                     control.ColCount = 3;
                 }

                 control.SetNormal();

             }));

        void Init(List<IVdeioImagePlayer> services)
        {

            for (int i = 0; i < services.Count; i++)
            {
                var item = services[i];

                item.PlayerToolControl = this.playtool;

                item.FullScreenHandle += Item_FullScreenHandle;

                var operate = item.ImagePlayerService?.GetImgOperate();

                if (operate != null)
                {
                    operate.DrawMarkedMouseUp += Item_DrawMarkedMouseUp;

                    operate.DeleteImgEvent += Operate_DeleteImgEvent;

                    operate.ImgMarkOperateEvent += Operate_ImgMarkOperateEvent;

                    operate.MarkEntitySelectChanged += Operate_MarkEntitySelectChanged;

                    operate.FullScreenChangedEvent += Operate_FullScreenChangedEvent;
                }


                if (item.ImagePlayerService != null)
                {
                    item.ImagePlayerService.ImgPlayModeChanged += ImagePlayerService_ImgPlayModeChanged;

                    item.ImagePlayerService.ImageIndexChanged += ImagePlayerService_ImageIndexChanged;
                }




            }

        }




        #region - 注册事件 -

        private void ImagePlayerService_ImageIndexChanged(string arg1, ImgSliderMode arg2, IImagePlayerService arg3)
        {


            Application.Current.Dispatcher.Invoke(() =>
            {
                int index = this.MediaSources.FindIndex(l => l.ImagePlayerService == arg3);

                this.ImageIndexChanged?.Invoke(arg1, arg2, index);

                //Debug.WriteLine("ImageIndexChanged");
            });

        }

        private void ImagePlayerService_ImgPlayModeChanged(ImgPlayMode obj, IImagePlayerService imagePlayer)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService == imagePlayer);

            this.ImgPlayModeChanged?.Invoke(obj);

            Debug.WriteLine("ImgPlayModeChanged");
        }

        private void Operate_FullScreenChangedEvent(bool obj, IImageCore operate)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService?.GetImgOperate() == operate);

            this.ImageIndexFullScreenEvent?.Invoke(index);

            Debug.WriteLine("Operate_FullScreenChangedEvent");
        }

        private void Operate_MarkEntitySelectChanged(ImageMarkEntity obj, IImageCore operate)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService?.GetImgOperate() == operate);

            this.ImageMarkEntitySelectChanged?.Invoke(obj, index);

            Debug.WriteLine("Operate_MarkEntitySelectChanged");
        }

        private void Operate_ImgMarkOperateEvent(ImageMarkEntity markEntity, IImageCore operate)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService?.GetImgOperate() == operate);

            this.ImageIndexMarkOperateEvent?.Invoke(markEntity, index);

            Debug.WriteLine("Operate_ImgMarkOperateEvent");
        }

        private void Operate_DeleteImgEvent(string obj, IImageCore operate)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService?.GetImgOperate() == operate);

            this.ImageIndexDeletedClicked?.Invoke(obj, index);

            Debug.WriteLine("Operate_DeleteImgEvent");
        }

        private void Item_DrawMarkedMouseUp(ImageMarkEntity markEntity, MarkType type, IImageCore operate)
        {
            int index = this.MediaSources.FindIndex(l => l.ImagePlayerService?.GetImgOperate() == operate);

            this.ImageIndexDrawMarkedMouseUp?.Invoke(markEntity, type, index);

            Debug.WriteLine("Item_DrawMarkedMouseUp");
        }

        //  Message：全屏事件
        private void Item_FullScreenHandle(IVdeioImagePlayer obj)
        {
            int index = this.MediaSources.FindIndex(l => l == obj);

            this.ImageIndexFullScreenEvent?.Invoke(index);

            this.SetFullScreen(index);

            Debug.WriteLine("Item_FullScreenHandle");

        }
        #endregion

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(MulMediaPlayerControl), new PropertyMetadata(1, (d, e) =>
             {
                 MulMediaPlayerControl control = d as MulMediaPlayerControl;

                 if (control == null) return;

                 //int config = e.NewValue as int;

             }));


        public int ColCount
        {
            get { return (int)GetValue(ColCountProperty); }
            set { SetValue(ColCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColCountProperty =
            DependencyProperty.Register("ColCount", typeof(int), typeof(MulMediaPlayerControl), new PropertyMetadata(1, (d, e) =>
             {
                 MulMediaPlayerControl control = d as MulMediaPlayerControl;

                 if (control == null) return;

                 //int config = e.NewValue as int;

             }));


        void SetFullScreen(int index)
        {
            this.control_normal.ItemsSource = null;

            this.control_fullscreen.Visibility = Visibility.Visible;

            this.control_normal.Visibility = Visibility.Collapsed;

            this.control_fullscreen.MediaSources = this.MediaSources;

            this.control_fullscreen.Index = index;

            (this.MediaSources[index] as Control).Height=double.NaN;

            var collection = this.MediaSources.Where(l => l != this.MediaSources[index]);

            foreach (var item in collection)
            {
                Control element = item as Control;

                element.Height = 200; 
            }

            this.RefreshSize();

            this.FullScreenStateChanged?.Invoke(true);

        }

        void SetNormal()
        {
            this.control_fullscreen.MediaSources = null;

            this.control_fullscreen.Visibility = Visibility.Collapsed;

            this.control_normal.Visibility = Visibility.Visible;


            foreach (var item in this.MediaSources.Cast<Control>())
            {
                item.Height = double.NaN;
            }

            this.control_normal.ItemsSource = this.MediaSources;

            this.RefreshSize();

            this.FullScreenStateChanged?.Invoke(false);
        }


        void RefreshSize()
        {
            for (int i = 0; i < this.MediaSources.Count; i++)
            {
                this.SetImageIndexAdaptiveSize(i);
            }
        }


        private void Control_fullscreen_CloseClicked()
        {
            this.SetNormal();
        }
    }

    public partial class MulMediaPlayerControl : IMulMediaPlayer
    {
        MediaPlayType _type;


        #region - 图片操作 -

        #region - 加载功能 -

        List<IVdeioImagePlayer> services = new List<IVdeioImagePlayer>();

        /// <summary> 初始化控件 </summary>
        void InitControl(int count)
        {
            //  Message：注销事件
            this.Dispose();

            services = new List<IVdeioImagePlayer>();

            for (int i = 0; i < count; i++)
            {

                VedioImagePlayerControl control = new VedioImagePlayerControl();

                services.Add(control);


            }

            this.MediaSources = services;

            //  Do：设置默认缩放灵敏度为0.1
            for (int i = 0; i < services.Count; i++)
            {
                this.SetImageIndexWheelScale(0.1, i);
            }


        }

        public void LoadImageFolders(params Tuple<List<string>, string>[] imageFoders)
        {

            Action<IVdeioImagePlayer, int> action = (l, i) =>
               {
                   l.LoadImageFolder(imageFoders[i].Item1, imageFoders[i].Item2);
               };

            this.AsyncLoadImages(action, imageFoders);

            return;


            //if (imageFoders == null) return;

            //this.InitControl(imageFoders.Length);

            ////for (int i = 0; i < imageFoders.Length; i++)
            ////{    
            ////    services[i].LoadImageFolder(imageFoders[i].Item1, imageFoders[i].Item2); 
            ////}

            //this.grid_load.Visibility = Visibility.Visible;

            //Task.Run(() =>
            //{

            //    for (int i = 0; i < imageFoders.Length; i++)
            //    {
            //        Thread.Sleep(2000);

            //        //this.Dispatcher.Invoke(() =>
            //        //{
            //        //    this.text_loadMessage.Text = ((int)(Convert.ToDouble(i) * 100 / Convert.ToDouble(imageFoders.Length))).ToString();
            //        //});

            //        services[i].LoadImageFolder(imageFoders[i].Item1, imageFoders[i].Item2);

            //    }

            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        this.grid_load.Visibility = Visibility.Collapsed;
            //    });


            //});

        }

        /// <summary> 异步加载 </summary>
        async void AsyncLoadImages(Action<IVdeioImagePlayer, int> action, Array imageFoders)
        {

            if (imageFoders == null) return;

            this.InitControl(imageFoders.Length);

            this.grid_load.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {

                for (int i = 0; i < imageFoders.Length; i++)
                {
                    //Thread.Sleep(2000);

                    action(services[i], i);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.grid_load.Visibility = Visibility.Collapsed;
                });

            });
        }

        public void LoadImageFtpFolders(string useName, string passWord, params Tuple<List<string>, string>[] imageFoders)
        {

            Action<IVdeioImagePlayer, int> action = (l, i) =>
            {
                l.LoadFtpImageFolder(imageFoders[i].Item1, imageFoders[i].Item2, useName, passWord);

            };

            this.AsyncLoadImages(action, imageFoders);



            return;

            //if (imageFoders == null) return;

            //this.InitControl(imageFoders.Length);

            //for (int i = 0; i < imageFoders.Length; i++)
            //{
            //    services[i].LoadFtpImageFolder(imageFoders[i].Item1, imageFoders[i].Item2, useName, passWord);
            //}

        }

        public void LoadImageList(params Tuple<List<string>, string>[] ImageUrls)
        {
            if (ImageUrls == null) return;

            bool check = ImageUrls.ToList().TrueForAll(l => l.Item1.Count == ImageUrls.First().Item1.Count);

            if (!check)
            {
                Debug.WriteLine("参数错误！请检查，传入的多个数组中，数量必须相等"); return;
            }

            Action<IVdeioImagePlayer, int> action = (l, i) =>
            {
                l.LoadImages(ImageUrls[i].Item1, ImageUrls[i].Item2);
            };

            this.AsyncLoadImages(action, ImageUrls);

            return;


            //this.InitControl(ImageUrls.Length);

            //for (int i = 0; i < ImageUrls.Length; i++)
            //{
            //    services[i].LoadImages(ImageUrls[i]);
            //}
        }

        public void LoadImageShareFolders(string useName, string passWord, string ip, params Tuple<List<string>, string>[] imageFoders)
        {
            if (imageFoders == null) return;

            this.InitControl(imageFoders.Length);

            for (int i = 0; i < imageFoders.Length; i++)
            {
                services[i].LoadShareImageFolder(imageFoders[i].Item1, imageFoders[i].Item2, useName, passWord, ip);

            }
        }

        #endregion

        #region - 事件功能 -

        public event Action<ImageMarkEntity, int> ImageIndexMarkOperateEvent;

        //public event Action<string, ImgProcessType, int> ImageIndexProcessEvent;

        //public event Action<int> ImageIndexPreviousEvent;

        //public event Action<int> ImageIndexNextEvent;

        public event Action<ImageMarkEntity, MarkType, int> ImageIndexDrawMarkedMouseUp;

        public event Action<string, int> ImageIndexDeletedClicked;

        public event Action<ImageMarkEntity, int> ImageMarkEntitySelectChanged;

        public event Action<string, ImgSliderMode, int> ImageIndexChanged;

        public event Action<ImgPlayMode> ImgPlayModeChanged;

        public event Action<int> ImageIndexFullScreenEvent;

        public event Action<bool> FullScreenStateChanged;

        #endregion

        #region - 图片交互 -

        public void AddImageIndexMark(ImageMarkEntity imgMarkEntity, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().AddMark(imgMarkEntity);
        }

        bool CheckCount(int index)
        {
            if (this.services.Count <= index) return false;

            return true;
        }

        public void CancelAddImageIndexMark(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().CancelAddMark();
        }

        public void DeleteImageIndexSelectMark(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().DeleteSelectMark();
        }

        public ImageMarkEntity GetImageIndexSelectMark(int index = 0)
        {
            if (!CheckCount(index)) return null;

            IVdeioImagePlayer service = services[index];

            return service.ImagePlayerService.GetImgOperate().GetSelectMarkEntity();
        }



        public void ScreenShotImageIndex(string saveFullName, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().ScreenShot(saveFullName);
        }

        public void SetImageEnlarge(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetEnlarge();
        }

        public void SetImageIndexAdaptiveSize(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetAdaptiveSize();
        }

        public void SetImageIndexBubbleScale(double value, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetBubbleScale(value);
        }

        public void SetImageIndexDetialText(string value, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().DetialText = value;
        }

        public void SetImageIndexMarkOperate(ImageMarkEntity entity, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().MarkOperate(entity);
        }

        public void SetImageIndexFullScreen(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.SetFullScreen(true);
        }

        public void SetImageIndexMarkType(MarkType markType, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetMarkType(markType);
        }

        public void SetImageIndexOriginalSize(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetOriginalSize();
        }

        public void SetImageIndexPartShotCut(ShortCutEntitys shortcut, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().RegisterPartShotCut(shortcut);
        }

        public void SetImageIndexPositon(int postion, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.SetPositon(postion);
        }

        public void SetImageIndexRotateLeft(int index)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetRotateLeft();
        }

        public void SetImageIndexRotateRight(int index)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetRotateRight();
        }

        public void SetImageIndexSelectMark(Predicate<ImageMarkEntity> match, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetSelectMarkEntity(match);
        }

        public void SetImageIndexSpeed(int value, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().Speed = value;
        }

        public void SetImageIndexWheelMode(bool value, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetWheelMode(value);
        }

        public void SetImageIndexWheelScale(double value, int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().WheelScale = value;
        }

        public void SetImageNarrow(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().SetNarrow();
        }



        public void ShowAllImageIndexDefects(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().ShowDefects();
        }

        public void ShowImageIndexLocates(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().ShowLocates();
        }

        public void ShowImageIndexMarks(int index = 0)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().ShowMarks();
        }

        public void ShowImageIndexMarks(List<string> markCodes, int index)
        {
            if (!CheckCount(index)) return;

            IVdeioImagePlayer service = services[index];

            service.ImagePlayerService.GetImgOperate().ShowMarks(markCodes);
        }

        #endregion

        #region - 播放控制 - 

        public ImgPlayMode GetImagePlayMode()
        {
            if (this.services == null) return ImgPlayMode.正序;

            return this.services.First().ImagePlayerService.ImgPlayMode;
        }

        public void SetImagePlayMode(ImgPlayMode imgPlayMode)
        {
            if (this.services == null) return;

            //foreach (var item in this.services)
            //{
            //    item.ImagePlayerService.SetImgPlay(ImgPlayMode.停止播放);
            //}

            //Thread.Sleep(5000);

            foreach (var item in this.services)
            {
                item.ImagePlayerService.SetImgPlay(imgPlayMode);
            }
        }

        public void SetImagePlayStepDown()
        {
            if (this.services == null || this.services.Count == 0) return;

            var find = this.services.First();

            var operate = find.ImagePlayerService.GetImgOperate();

            double speed = operate.ConvertSpeedFunction(operate.Speed);

            //double speed = 5 / find.ImagePlayerService.GetImgOperate().GetSpeedSplitTime();

            this.playtool.media_slider.Value -= TimeSpan.FromSeconds(speed).Ticks;

            foreach (var item in this.services)
            {
                item.PlayStepUp();
            }
        }

        public void SetImagePlayStepUp()
        {
            if (this.services == null || this.services.Count == 0) return;

            var find = this.services.First();

            //double speed = 5 / find.ImagePlayerService.GetImgOperate().GetSpeedSplitTime();

            var operate = find.ImagePlayerService.GetImgOperate();

            double speed = operate.ConvertSpeedFunction(operate.Speed);


            this.playtool.media_slider.Value += TimeSpan.FromSeconds(speed).Ticks;

            foreach (var item in this.services)
            {
                item.PlayStepUp();
            }
        }

        public void SetImageScale(double value, int index = 0)
        {
            if (!this.CheckCount(index)) return;

            var service = services[index];

            service.ImagePlayerService.GetImgOperate().Scale = value;
        }

        public void SetImageSpeedDown()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.ImgPlaySpeedDown();
            }
        }

        public void SetImageVoiceStepDown()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.VoiceStepDown();
            }
        }

        public void SetImageVoiceStepUp()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.VoiceStepUp();
            }
        }

        public void SetImageWeelPlayMode(bool value)
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.GetImgOperate().IsWheelPlay = value;
            }
        }

        public void SetImagNext()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.NextImg();
            }
        }

        public void SetImagPrevious()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.PreviousImg();
            }
        }

        public void SetImageSpeedUp()
        {
            if (this.services == null) return;

            foreach (var item in this.services)
            {
                item.ImagePlayerService.ImgPlaySpeedUp();
            }
        }

        public void Dispose()
        {

            this.SetImagePlayMode(ImgPlayMode.停止播放);

            foreach (var item in this.services)
            {


                //  Message：注销事件
                item.FullScreenHandle -= Item_FullScreenHandle;

                var operate = item.ImagePlayerService?.GetImgOperate();

                if (operate != null)
                {
                    operate.DrawMarkedMouseUp -= Item_DrawMarkedMouseUp;

                    operate.DeleteImgEvent -= Operate_DeleteImgEvent;

                    operate.ImgMarkOperateEvent -= Operate_ImgMarkOperateEvent;

                    operate.MarkEntitySelectChanged -= Operate_MarkEntitySelectChanged;

                    operate.FullScreenChangedEvent -= Operate_FullScreenChangedEvent;
                }

                if (item.ImagePlayerService != null)
                {
                    item.ImagePlayerService.ImgPlayModeChanged -= ImagePlayerService_ImgPlayModeChanged;

                    item.ImagePlayerService.ImageIndexChanged -= ImagePlayerService_ImageIndexChanged;
                }
                //  Message：清理子项
                item.Dispose();

            }

            services.Clear();
        }

        public void ClearAllCache()
        {
            try
            {
                string local = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_cache");

                Directory.Delete(local, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


        }

        public void ImageIndexLoadMarkEntitys(List<ImageMarkEntity> markEntityList, int index)
        {
            if (!this.CheckCount(index)) return;

            var service = services[index];

            service.ImagePlayerService.GetImgOperate().LoadMarkEntitys(markEntityList);
        }

        public List<string> GetCurrentUrl()
        {
            if (this.services == null) return null;

            return this.services.Select(l => l.ImagePlayerService?.GetCurrentUrl()).ToList();

        }

        public List<string> GetImageIndexImageList(int index = 0)
        {
            if (!this.CheckCount(index)) return null;

            var service = services[index];

            return service.ImagePlayerService.GetImgOperate().GetImageList();
        }

        public void SetImageIndexImageSource(ImageSource source, int index = 0)
        {
            if (!this.CheckCount(index)) return;

            var service = services[index];

            service.ImagePlayerService.GetImgOperate().SetImageSource(source);
        }

        public ImageSource GetImageIndexImageSource(int index = 0)
        {
            if (!this.CheckCount(index)) return null;

            var service = services[index];

            return service.ImagePlayerService.GetImgOperate().GetCImageSource();
        }
        #endregion

        #endregion

    }

}
