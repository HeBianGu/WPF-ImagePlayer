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
using HeBianGu.ImagePlayer.ImageControl;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary> 视频播放控件 </summary>
    public partial class VedioImagePlayerControl : UserControl, IVdeioImagePlayerService
    {
        public VedioImagePlayerControl()
        {
            InitializeComponent();

            this.MediaPlayerService = this.control_media;

            this.ImagePlayerService = this.control_image;

            this.MediaPlayerService.FullScreenHandle += () =>
            {
                this.FullScreenHandle?.Invoke(this);
            };

            this.ImagePlayerService.FullScreenHandle += () =>
            {
                this.FullScreenHandle?.Invoke(this);

                //  Do：设置自适应大小
                this.ImagePlayerService.GetImgOperate().SetAdaptiveSize();
                
            };

            //this.SizeChanged += (l, k) =>
            //{
            //    if (_init>=0)
            //    {
            //        _init --;
            //        return;
            //    }
            //    this.ImagePlayerService.GetImgOperate().SetAdaptiveSize();

            //};

        }

        private int _init = 2;

        public PlayerToolControl PlayerToolControl
        {
            get { return (PlayerToolControl)GetValue(PlayerToolControlProperty); }
            set { SetValue(PlayerToolControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayerToolControlProperty =
            DependencyProperty.Register("PlayerToolControl", typeof(PlayerToolControl), typeof(VedioImagePlayerControl), new PropertyMetadata(default(PlayerToolControl), (d, e) =>
             {
                 VedioImagePlayerControl control = d as VedioImagePlayerControl;

                 if (control == null) return;

                 PlayerToolControl config = e.NewValue as PlayerToolControl;

                 control.control_media.PlayerToolControl = config;

                 control.control_image.PlayerToolControl = config;

             }));


        public IMediaPlayerService MediaPlayerService { get; set; }

        public IImagePlayerService ImagePlayerService { get; set; }


        MediaPlayType _type;

        public event Action<IVdeioImagePlayerService> FullScreenHandle;

        /// <summary> 更新播放类型 </summary>
        void RefreshPlayType(MediaPlayType type)
        {
            _type = type;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (type == MediaPlayType.Video)
                {

                    this.control_image.DisposePlayerToolControl();
                    this.control_media.ResgiterPlayerToolControl();
                }
                else
                {

                    this.control_media.DisposePlayerToolControl();
                    this.control_image.ResgiterPlayerToolControl();
                }

                this.control_media.Visibility = type == MediaPlayType.Video ? Visibility.Visible : Visibility.Collapsed;

                this.control_image.Visibility = type == MediaPlayType.Image ? Visibility.Visible : Visibility.Collapsed;
            });

        }

        public void LoadVedio(string path)
        {
            this.RefreshPlayType(MediaPlayType.Video);

            this.MediaPlayerService.Load(path);
        }

        public void LoadImages(List<string> paths,string start)
        {
            this.RefreshPlayType(MediaPlayType.Image);

            this.ImagePlayerService.LoadImages(paths, start);
        }

        public  void LoadImageFolder(List<string> paths, string start)
        {
            this.RefreshPlayType(MediaPlayType.Image);

             this.ImagePlayerService.LoadImageFolder(paths, start);
        }

        public void LoadShareImageFolder(List<string> paths, string start, string user, string password, string ip)
        {
            this.RefreshPlayType(MediaPlayType.Image);

            this.ImagePlayerService.LoadShareImageFolder(paths, start, user, password, ip);
        }

        public void LoadFtpImageFolder(List<string> paths, string start, string user, string password)
        {
            this.RefreshPlayType(MediaPlayType.Image);

            this.ImagePlayerService.LoadFtpImageFolder(paths, start, user, password);
        }

        public void PlaySpeedUp()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.PlaySpeedUp();
            }
            else
            {
                this.ImagePlayerService.ImgPlaySpeedUp();
            }
        }

        public void PlaySpeedDown()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.PlaySpeedDown();
            }
            else
            {
                this.ImagePlayerService.ImgPlaySpeedDown();
            }
        }

        public void PlayStepUp()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.PlayStepUp();
            }
            else
            {
                this.ImagePlayerService.PlayStepUp();
            }
        }

        public void PlayStepDown()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.PlayStepDown();
            }
            else
            {
                this.ImagePlayerService.PlayStepDown();
            }
        }

        public void VoiceStepUp()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.VoiceStepUp();
            }
            else
            {
                this.ImagePlayerService.VoiceStepUp();
            }
        }

        public void VoiceStepDown()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.VoiceStepDown();
            }
            else
            {
                this.ImagePlayerService.VoiceStepDown();
            }
        }

        public void RotateLeft()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.RotateLeft();
            }
            else
            {
                this.ImagePlayerService.RotateLeft();
            }
        }

        public void RotateRight()
        {
            if (_type == MediaPlayType.Video)
            {
                this.MediaPlayerService.RotateRight();
            }
            else
            {
                this.ImagePlayerService.RotateRight();
            }
        }

        public void Dispose()
        {
            this.ImagePlayerService?.Dispose();
        }
    }
}
