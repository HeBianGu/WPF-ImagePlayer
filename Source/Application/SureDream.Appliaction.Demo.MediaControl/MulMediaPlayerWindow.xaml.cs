using CDTY.DataAnalysis.Entity;
using HeBianGu.ImagePlayer.ImageControl;
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
    /// MulMediaPlayerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MulMediaPlayerWindow : Window
    {
        public MulMediaPlayerWindow()
        {
            InitializeComponent();

            this.media.ImgPlayModeChanged += l =>
              {
                  Debug.WriteLine("ImgPlayModeChanged");
              };

            this.media.ImageIndexChanged += (l, k, j) =>
              {
                  //Debug.WriteLine("ImageIndexChanged");
              };

            this.media.FullScreenStateChanged += l =>
              {
                  Debug.WriteLine("FullScreenStateChanged:" + l);
              };


            this.media.ImageIndexDrawMarkedMouseUp += (l, k, j) =>
             {
                 Debug.WriteLine("ImageIndexDrawMarkedMouseUp");

                 this.media.AddImageIndexMark(l, j);
             };


            this.media.ImageMarkEntitySelectChanged += (l, k) =>
              {
                  Debug.WriteLine("ImageMarkEntitySelectChanged");
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

        private void Btn_loadImages_Click(object sender, RoutedEventArgs e)
        {
            //  Do：根据数量初始化控件
            int c = int.Parse(this.txt_count.Text);

            List<Tuple<List<string>, string>> imageFoders = new List<Tuple<List<string>, string>>();


            for (int i = 0; i < c; i++)
            {
                List<string> folders = new List<string>();

                string filePath1 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
                //string filePath2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images1");

                folders.Add(filePath1);
                //folders.Add(filePath2);

                imageFoders.Add(new Tuple<List<string>, string>(folders, filePath1));

            }

            this.media.LoadImageFolders(imageFoders.ToArray());

            this.media.SetImageIndexMarkType(MarkType.Defect);

            this.media.SetImageIndexMarkType(MarkType.Defect, 1);

            this.media.SetImageIndexBubbleScale(200);

            //this.media.SetImageIndexWheelScale(0.1);

            this.media.SetImagePlayMode(ImgPlayMode.正序);



        }

        private void Btn_loadShareImages_Click(object sender, RoutedEventArgs e)
        {
            ////  Do：根据数量初始化控件
            //int c = int.Parse(this.txt_count.Text);

            //string filePath1 = @"\\192.168.1.22\Document\images1";
            //string filePath2 = @"\\192.168.1.22\Document\images2";
            //string filePath3 = @"\\192.168.1.22\Document\images3";
            //string filePath4 = @"\\192.168.1.22\Document\images4";
            //string filePath5 = @"\\192.168.1.22\Document\images5";

            //List<string> folders = new List<string>();

            //folders.Add(filePath1);
            //folders.Add(filePath2);
            //folders.Add(filePath3);
            //folders.Add(filePath4);
            //folders.Add(filePath5);

            //foreach (var item in imageFoders)
            //{
            //    //var dir = Directory.CreateDirectory(item);

            //    DirectoryInfo dir = new DirectoryInfo(item);

            //    var file = dir.GetFiles().Where(l => ComponetProvider.Instance.IsValidImage(l.FullName)).Select(l => l.FullName).ToList();

            //    if (item == startForder)
            //    {
            //        exist = true;
            //    }

            //    if (!exist)
            //    {
            //        startPostion += file.Count;
            //    }

            //    //Thread.Sleep(500);

            //    files.AddRange(file);
            //}

            //List<Tuple<List<string>, string>> collection = new List<Tuple<List<string>, string>>();

            //for (int i = 0; i < c; i++)
            //{
            //    collection.Add(new Tuple<List<string>, string>(folders, filePath1));
            //}



            //this.media.LoadImageShareFolders("Healthy", "870210lhj", "192.168.1.22", collection.ToArray());

            //this.media.LoadImageList(collection.ToArray(), collection)

        }

        private void Btn_loadFTPImages_Click(object sender, RoutedEventArgs e)
        {

            //string useName = "Healthy";

            //string passWord = "870210lhj";

            string useName = "administrator";

            string passWord = "jkyl123.+";

            FtpHelper.Login(useName, passWord);

            List<string> files = new List<string>();

            string loginParam = $"ftp://{useName}:{passWord}@";

            //  Do：默认加载位置
            int startPostion = 0;

            List<string> folders = new List<string>();

            //folders.Add(@"ftp://127.0.0.1/images2/");
            //folders.Add(@"ftp://127.0.0.1/images1/");
            //folders.Add(@"ftp://127.0.0.1/images2/");
            //folders.Add(@"ftp://127.0.0.1/images3/");
            //folders.Add(@"ftp://127.0.0.1/images4/");
            //folders.Add(@"ftp://127.0.0.1/images5/");
            //folders.Add(@"ftp://127.0.0.1/images6/");
            //folders.Add(@"ftp://127.0.0.1/images7/");
            //folders.Add(@"ftp://127.0.0.1/images8/");  


            //string path =
            //    @"ftp://192.168.5.133:21/images2";

            string path =
                @"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530076_5";

            folders.Add(path);

            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K53085_51");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K529986_1");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530031_3");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530031_4");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530171_9");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530221_11");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530273_13");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530321_15");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530362_17");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530407_19");

            //  Do：根据数量初始化控件
            int c = int.Parse(this.txt_count.Text);

            Task.Run(() => {
                foreach (var item in folders)
                {
                    string url = item.Replace("ftp://", loginParam).Replace("FTP://", loginParam.ToUpper());

                    var file = FtpHelper.GetFileList(item).Select(l => System.IO.Path.Combine(url, l)).ToList();


                    files.AddRange(file);
                } 

                List<Tuple<List<string>, string>> collection = new List<Tuple<List<string>, string>>();

                for (int i = 0; i < c; i++)
                {
                    collection.Add(Tuple.Create(files, files[0]));
                }

                this.Dispatcher.Invoke(() => { this.media.LoadImageList(collection.ToArray()); });

            });

  

            //this.media.LoadImageList("Healthy", "870210lhj", collection.ToArray());

        }

        private void Btn_speedadd_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImageSpeedUp();
        }

        private void Btn_speedmul_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImageSpeedDown();
        }

        private void Btn_playback_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImagePlayMode(ImgPlayMode.倒叙);
        }

        private void Btn_clearallcache_Click(object sender, RoutedEventArgs e)
        {
            this.media.ClearAllCache();
        }

        private void Btn_addStep_Click(object sender, RoutedEventArgs e)
        {
            if (this.media.GetImagePlayMode() == ImgPlayMode.倒叙)
            {
                this.media.SetImagePlayStepDown();
            }
            else
            {
                this.media.SetImagePlayStepUp();
            }
        }

        private void Btn_mulStep_Click(object sender, RoutedEventArgs e)
        {
            if (this.media.GetImagePlayMode() == ImgPlayMode.倒叙)
            {
                this.media.SetImagePlayStepUp();
            }
            else
            {
                this.media.SetImagePlayStepDown();
            }
           
        }

        private void Btn_big_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImageEnlarge();
        }

        private void Btn_small_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImageNarrow();
        }

        private void Btn_loadImages1_Click(object sender, RoutedEventArgs e)
        {

            //  Do：根据数量初始化控件
            int c = int.Parse(this.txt_count.Text);

            List<Tuple<List<string>, string>> collection = new List<Tuple<List<string>, string>>();

            for (int i = 0; i < c; i++)
            {
                string imageFoders = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

                DirectoryInfo dir = new DirectoryInfo(imageFoders);

                var file = dir.GetFiles().Select(l => l.FullName).ToList();

                collection.Add(Tuple.Create(file, file.FirstOrDefault()));
            }

            this.media.LoadImageList(collection.ToArray());


        }

        private void btn_loadFTPefolder_Click(object sender, RoutedEventArgs e)
        {
            //  Do：根据数量初始化控件
            int c = int.Parse(this.txt_count.Text);

            string filePath = @"ftp://127.0.0.1/images2/";

            List<string> folders = new List<string>();

            //folders.Add(@"ftp://127.0.0.1/images2/");
            //folders.Add(@"ftp://127.0.0.1/images1/");
            //folders.Add(@"ftp://127.0.0.1/images2/");
            //folders.Add(@"ftp://127.0.0.1/images3/");
            //folders.Add(@"ftp://127.0.0.1/images4/");
            //folders.Add(@"ftp://127.0.0.1/images5/");
            //folders.Add(@"ftp://127.0.0.1/images6/");
            //folders.Add(@"ftp://127.0.0.1/images7/");
            //folders.Add(@"ftp://127.0.0.1/images8/"); 


            string path =
                @"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530076_5";
            folders.Add(path);

            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K53085_51");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K529986_1");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530031_3");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530031_4");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530171_9");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530221_11");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530273_13");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530321_15");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530362_17");
            folders.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530407_19");

            List<string> folders1 = new List<string>();
            string path1 =
                @"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K545244_32";
            folders1.Add(path1);

            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530506_23");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530555_27");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530605_31");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530655_35");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530705_39");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530755_43");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530805_47");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530895_55");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K532738_2");
            folders1.Add(@"ftp://192.168.5.133:21/201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K544569_2");



            List<Tuple<List<string>, string>> collection = new List<Tuple<List<string>, string>>();

            //for (int i = 0; i < c; i++)
            //{
            //    collection.Add(new Tuple<List<string>, string>(folders, path));
            //}

            collection.Add(new Tuple<List<string>, string>(folders, path));
            collection.Add(new Tuple<List<string>, string>(folders1, path1));

            this.media.LoadImageFtpFolders("administrator", "jkyl123.+", collection.ToArray()); 
        }

        private void btn_loadShareFolder_Click(object sender, RoutedEventArgs e)
        {
            //  Do：根据数量初始化控件
            int c = int.Parse(this.txt_count.Text);

            string filePath1 = @"\\192.168.1.22\Document\images1";
            string filePath2 = @"\\192.168.1.22\Document\images2";
            string filePath3 = @"\\192.168.1.22\Document\images3";
            string filePath4 = @"\\192.168.1.22\Document\images4";
            string filePath5 = @"\\192.168.1.22\Document\images5";

            string filePath6 = @"\\WIN-DS66FFHH1G6\ShareSource\201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K529986_1";
            string filePath7 = @"\\WIN-DS66FFHH1G6\ShareSource\201903121100_西成高铁_上行_汉中高速场_城固北\201903121100_西成高铁_上行_汉中高速场_城固北\图片\K530171_9";

            List<string> folders = new List<string>();

            folders.Add(filePath6);
            folders.Add(filePath7);
            //folders.Add(filePath3);
            //folders.Add(filePath4);
            //folders.Add(filePath5);

            List<Tuple<List<string>, string>> collection = new List<Tuple<List<string>, string>>();

            for (int i = 0; i < c; i++)
            {
                collection.Add(new Tuple<List<string>, string>(folders, filePath6));
            }


            //this.media.LoadImageShareFolders("Healthy", "870210lhj", "192.168.1.22", collection.ToArray());

            this.media.LoadImageShareFolders("administrator", "jkyl123.+", "192.168.5.133", collection.ToArray());
        }

        private void btn_setfullscreen_Click(object sender, RoutedEventArgs e)
        {
            this.media.SetImageIndexFullScreen(1);
        }

        private void Btn_setimagesource_Click(object sender, RoutedEventArgs e)
        {
            //  Do：根据数量初始化控件  


            string imageFoders = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

            DirectoryInfo dir = new DirectoryInfo(imageFoders);

            var file = dir.GetFiles().Select(l => l.FullName).ToList();

            BitmapImage imgSource = new BitmapImage(new Uri(file[new Random().Next(file.Count-1)], UriKind.RelativeOrAbsolute));

            this.media.SetImageIndexImageSource(imgSource);
            //this.media.(collection.ToArray());
        }
    }
}
