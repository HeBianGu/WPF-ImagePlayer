using HeBianGu.Base.WpfBase;
using HeBianGu.ImagePlayer.ImageControl;
using HeBianGu.ImagePlayer.ImageControl.Hook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes; 

namespace HeBianGu.Appliaction.Demo.MediaControl
{
    /// <summary>
    /// ShellWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellWindow : Window
    {

        ShellWindowViewModel _vm = new ShellWindowViewModel();

        public ShellWindow()
        {
            InitializeComponent();

            this.DataContext = _vm;

            IImageView _imgOperate = this.media.ImagePlayerService.GetImgOperate();

            _imgOperate.SetMarkType(MarkType.Defect);

            List<ImageMarkEntity> temp = new List<ImageMarkEntity>();

            this.media.ImagePlayerService.ImgPlayModeChanged += (l,k) =>
              {
                  Debug.WriteLine("ImgPlayModeChanged:" + this.media.ImagePlayerService.ImgPlayMode);
                  Debug.WriteLine("ImgPlayModeChanged:" + l);
              };

            this.media.FullScreenHandle += l =>
              {
                  Debug.WriteLine("FullScreenHandle");
              };


            this.media.ImagePlayerService.ImageIndexChanged += (k, j,m) =>
              {
                  //Debug.WriteLine("ImageIndexChanged:" + k);
                  Debug.WriteLine("ImgSliderMode:" + j);


                  //  Message：加载Mark 20190105050908[2019-01-06-01-58-42].mark

                  //string current1 = _imgOperate.BuildEntity().Current.Value;

                  string current = k;

                  var tuple = this.media.ImagePlayerService.GetIndexWithTotal();

                  string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                  var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                  var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);

                  foreach (var item in collection)
                  {
                      string marks = File.ReadAllText(item);

                      var list = JsonConvert.DeserializeObject<List<ImageMarkEntity>>(marks);

                      //foreach (var c in list)
                      //{
                      //    c.Code = Guid.NewGuid().ToString();
                      //}

                      _imgOperate.LoadMarkEntitys(list);
                  }
              };

            //  Do：1、注册编辑标定事件 包括新增、删除
            _imgOperate.ImgMarkOperateEvent += (l,k) =>
            {
                temp.Clear();

                string fn = System.IO.Path.GetFileNameWithoutExtension(_imgOperate.GetCurrentUrl());

                string file = this.GetMarkFileName(fn);

                string str = l.markOperateType.ToString();

                Debug.WriteLine(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}");

                temp.Add(l);

                string result = JsonConvert.SerializeObject(temp);

                File.WriteAllText(file, result);

                MessageBox.Show(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}", "保存成功");

            };

      

            //  Do：5、注册绘制矩形框结束事件 需要在此处弹出缺陷管理控件，并设置如下参数
            _imgOperate.DrawMarkedMouseUp += (l, k,m) =>
            {
                //Debug.WriteLine(l);
                //Debug.WriteLine(k);

                

                _imgOperate.AddMark(l);

                //_imgOperate.CancelAddMark();
            };

            _imgOperate.MarkEntitySelectChanged += (l,k) =>
              {
                  Debug.WriteLine("MarkEntitySelectChanged:" );
              };
        }

        /// <summary> 获取标定信息存放路径 </summary>
        string GetMarkFileName(string imgName)
        {
            string file = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

            string tempFiles = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks", imgName + "[" + file + "].mark");

            if (!File.Exists(tempFiles)) File.WriteAllText(tempFiles, string.Empty);

            return tempFiles;
        }

        //  Message：播放avi
        private void Btn_play_avi_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi";

            this.media.LoadVedio(filePath);
        }

        //  Message：播放mkv
        private void Btn_play_mkv_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.mkv");

            this.media.LoadVedio(filePath);

        }
        //  Message：播放MP4
        private void Btn_play_mp4_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.mp4");

            this.media.LoadVedio(filePath);
        }

        //  Message：播放本地
        private void Btn_play_local_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.mp4");

            this.media.LoadVedio(filePath);
        }

        //  Message：播放局域网共享
        private void Btn_play_localarea_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"\\Desktop-bem7r0b\视频格式大全\6-9+有关梯度下降法的更多深入讨论.mp4";

            this.media.LoadVedio(filePath);
        }

        //  Message：播放http
        private void Btn_play_http_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi";

            this.media.LoadVedio(filePath);
        }

        //  Message：设置视频位置
        private void Btn_play_setpostion_Click(object sender, RoutedEventArgs e)
        {
            this.media.MediaPlayerService.SetPositon(TimeSpan.FromSeconds(60));
        }

        //  Message：在指定区间重复播放
        private void Btn_play_repeat_Click(object sender, RoutedEventArgs e)
        {
            this.media.MediaPlayerService.RepeatFromTo(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(70));
        }

        //  Message：视频截屏
        private void Btn_play_screen_Click(object sender, RoutedEventArgs e)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", fileName + ".jpg");

            this.media.MediaPlayerService.ScreenShot(TimeSpan.FromSeconds(60), filePath);

            Process.Start(filePath);
        }

        //  Message：视频获取当前路径
        private void Btn_play_currentUrl_Click(object sender, RoutedEventArgs e)
        {
            var result = this.media.MediaPlayerService.GetCurrentUrl();

            MessageBox.Show(result);
        }

        //  Message：视频获取当前帧和总帧
        private void Btn_play_currentframe_Click(object sender, RoutedEventArgs e)
        {
            var result = "当前：" + this.media.MediaPlayerService.GetCurrentFrame();

            result += " - 总计：" + this.media.MediaPlayerService.GetTotalFrame();

            MessageBox.Show(result);
        }

        //  Message：播放ftp视频
        private void Btn_play_localftp_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("ftp");

            string filePath = "ftp://192.168.0.104/media.mkv";

            this.media.MediaPlayerService.Load(filePath);
        }

        //  Message：播放图片文件夹
        private void btn_imageplay_imagefoder_Click(object sender, RoutedEventArgs e)
        {
            //string filePath1 = @"\\192.168.1.19\Document\images1";
            //string filePath2 = @"\\192.168.1.19\Document\images2";
            //string filePath3 = @"\\192.168.1.19\Document\images3";
            //string filePath4 = @"\\192.168.1.19\Document\images4";
            //string filePath5 = @"\\192.168.1.19\Document\images5";
            //string filePath6 = @"\\192.168.1.19\Document\images6";

            //List<string> folders = new List<string>();

            //folders.Add(filePath1);
            //folders.Add(filePath2);
            //folders.Add(filePath3);
            //folders.Add(filePath4);
            //folders.Add(filePath5);
            //folders.Add(filePath6);


            string filePath1 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
            string filePath2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images1");
            List<string> folders = new List<string>();

            folders.Add(filePath1);
            folders.Add(filePath2);


            this.media.LoadImageFolder(folders, filePath1);
        }

        //  Message：获取图片列表当前帧
        private void btn_imageplay_currentframe_Click(object sender, RoutedEventArgs e)
        {
            var t = this.media.ImagePlayerService.GetIndexWithTotal();

            MessageBox.Show($"当前：{t.Item1} - 总数：{t.Item2}");

        }

        //  Message：获取图片当前路径
        private void btn_imageplay_currentUrl_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.media.ImagePlayerService.GetCurrentUrl());
        }

        //  Message：播放ftp图片文件夹路径
        private void btn_imageftpplay_imagefoder_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"ftp://127.0.0.1/images/";

            List<string> folders = new List<string>();

            folders.Add(@"ftp://127.0.0.1/images/");
            folders.Add(@"ftp://127.0.0.1/images1/");
            folders.Add(@"ftp://127.0.0.1/images2/");
            folders.Add(@"ftp://127.0.0.1/images3/");
            folders.Add(@"ftp://127.0.0.1/images4/");
            folders.Add(@"ftp://127.0.0.1/images5/");
            folders.Add(@"ftp://127.0.0.1/images6/");
            folders.Add(@"ftp://127.0.0.1/images7/");
            folders.Add(@"ftp://127.0.0.1/images8/");

            this.media.LoadFtpImageFolder(folders, filePath, "Healthy", "870210lhj");
        }

        //  Message：播放图片集合
        private void Btn_play_imagelist_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

            var forder = Directory.CreateDirectory(filePath);

            List<string> imgs = forder.GetFiles().Select(l => l.FullName).ToList();

            this.media.LoadImages(imgs,imgs.FirstOrDefault());

        }

        private void Btn_imageplay_start1_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.SetImgPlay(ImgPlayMode.倒叙);
        }

        private void Btn_imageplay_stop_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.SetImgPlay(ImgPlayMode.停止播放);
        }

        private void Btn_imageplay_start_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.SetImgPlay(ImgPlayMode.正序);
        }

        private void Btn_imageplay_setposition_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.SetPositon(9);
        }

        //  Message：图片旋转
        private void Btn_imageplay_rotate_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.Rotate();
        }

        private void Btn_imageplay_fullscreen_Click(object sender, RoutedEventArgs e)
        {
            this.media.ImagePlayerService.SetFullScreen(true);
        }

        private void Btn_imageplay_shotcut_Click(object sender, RoutedEventArgs e)
        {
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss");

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", fileName + ".jpg");

            this.media.ImagePlayerService.ScreenShot(filePath);

            Process.Start(filePath);
        }

        private void Btn_imageplay_deleteselect_Click(object sender, RoutedEventArgs e)
        {
            //var mark= this.media.ImagePlayerService.GetImgOperate().GetSelectMarkEntity();


            // this.media.ImagePlayerService.GetImgOperate().SetSelectMarkEntity()

            this.media.ImagePlayerService.DeleteSelectMark();

            //var entity =this.media.ImagePlayerService.GetImgOperate().GetSelectMarkEntity();

            //entity.markOperateType = ImgMarkOperateType.Delete;

            //this.media.ImagePlayerService.GetImgOperate().MarkOperate(entity);

        }

        private void Btn_imageplay_imagesharefoder_Click(object sender, RoutedEventArgs e)
        {
            string filePath1 = @"\\192.168.1.22\Document\images1";
            string filePath2 = @"\\192.168.1.22\Document\images2";
            string filePath3 = @"\\192.168.1.22\Document\images3";
            string filePath4 = @"\\192.168.1.22\Document\images4";
            string filePath5 = @"\\192.168.1.22\Document\images5";

            List<string> folders = new List<string>();

            folders.Add(filePath1);
            folders.Add(filePath2);
            folders.Add(filePath3);
            folders.Add(filePath4);
            folders.Add(filePath5);

            this.media.LoadShareImageFolder(folders, filePath2, "administrator", "123456", "192.168.1.22");
        }

        private void btn_image_shotcut_Click(object sender, RoutedEventArgs e)
        {

            ShortCutEntitys shortcut = new ShortCutEntitys();

            KeyEntity keyEntity = new KeyEntity();
            keyEntity.Key = System.Windows.Forms.Keys.LControlKey;
            shortcut.AddDown(keyEntity);

            keyEntity = new KeyEntity();
            keyEntity.Key = System.Windows.Forms.Keys.D;
            shortcut.Add(keyEntity);

            this.media.ImagePlayerService.GetImgOperate().RegisterPartShotCut(shortcut);
        }

        private void Btn_media_speedup_Click(object sender, RoutedEventArgs e)
        {
            this.media.PlaySpeedUp();
        }

        private void Btn_media_speeddown_Click(object sender, RoutedEventArgs e)
        {
            this.media.PlaySpeedDown();
        }

        private void Btn_play_stepadd_Click(object sender, RoutedEventArgs e)
        {
            this.media.PlayStepUp();
        }

        private void Btn_play_stepmul_Click(object sender, RoutedEventArgs e)
        {
            this.media.PlayStepDown();
        }

        private void Btn_voice_stepadd_Click(object sender, RoutedEventArgs e)
        {
            this.media.VoiceStepUp();
        }

        private void Btn_voice_stepmul_Click(object sender, RoutedEventArgs e)
        {
            this.media.VoiceStepDown();
        }

        private void Btn_voice_left_Click(object sender, RoutedEventArgs e)
        {
            this.media.RotateLeft();
        }

        private void Btn_voice_tight_Click(object sender, RoutedEventArgs e)
        {
            this.media.RotateRight();
        }
    }

    class ShellWindowViewModel : NotifyPropertyChanged
    {


    }
}
