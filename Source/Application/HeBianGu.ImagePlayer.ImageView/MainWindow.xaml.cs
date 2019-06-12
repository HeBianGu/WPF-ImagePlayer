﻿using HeBianGu.Base.WpfBase.Color;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeBianGu.ImagePlayer.ImageView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        string[] _paths;
        public MainWindow(params string[] paths)
        {
            InitializeComponent();

            //  Do：设置默认主题
            ThemeService.Current.AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);

            //  Do：注册编辑标定事件
            this.imageview.ImgMarkOperateEvent += (l, k) =>
            {
                string fn = System.IO.Path.GetFileNameWithoutExtension(this.imageview.GetCurrentUrl());

                string file = this.GetMarkFileName(fn, l.ID);

                string str = l.markOperateType.ToString();


                if (l.markOperateType == ImageMarkOperateType.Delete)
                {
                    File.Delete(file);
                    MessageBox.Show(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}", "保存成功");
                    return;
                }
                else
                {

                    l.ID = Guid.NewGuid().ToString(); 

                    Debug.WriteLine(str + "：" + l.ID + "-" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}");

                    string result = JsonConvert.SerializeObject(l);

                    File.WriteAllText(file, result);

                    MessageBox.Show(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}", "保存成功");
                }
            };

            this.imageview.PreviousImgEvent += () =>
            {
                string current = this.imageview.GetCurrentUrl();


                string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);

                foreach (var item in collection)
                {
                    string marks = File.ReadAllText(item);

                    var list = JsonConvert.DeserializeObject<ImageMarkEntity>(marks);

                    this.imageview.LoadMarkEntitys(new List<ImageMarkEntity>() { list });
                }
            };
           

            this.imageview.NextImgEvent += () =>
            {
                string current = this.imageview.GetCurrentUrl();

                string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);


                foreach (var item in collection)
                {
                    string marks = File.ReadAllText(item);

                    var list = JsonConvert.DeserializeObject<ImageMarkEntity>(marks);

                    this.imageview.LoadMarkEntitys(new List<ImageMarkEntity>() { list });
                }


            };

            this.imageview.DrawMarkedMouseUp += (l, k, m) =>
            {
                this.imageview.AddMark(l);

                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_cache", Guid.NewGuid().ToString() + ".jpg");

                if (!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                }

                BitmapImage bitmapImage = SystemUtils.ByteArrayToBitmapImage(l.PicData);

                SystemUtils.SaveBitmapImageIntoFile(bitmapImage, path);
            };

            this.imageview.MarkEntitySelectChanged += (l, k) =>
            {
                Debug.WriteLine("MarkEntitySelectChanged" + l);
            };

            this.imageview.FullScreenChangedEvent += (l, k) =>
            {
                Debug.WriteLine("DoubleClickFullScreenHandle" + l);
            };

            _paths = paths;

            this.Loaded += MainWindow_Loaded;

           
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_paths == null) return;

            foreach (var item in _paths)
            {
                if (File.Exists(item))
                {
                    this.imageview.LoadImage(item);
                    return;
                }

                if (Directory.Exists(item))
                {
                    this.imageview.LoadFolder(item);
                    return;
                }
            }
        }

        string GetMarkFileName(string imgName,string id)
        {

            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Marks";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string tempFiles = System.IO.Path.Combine(folder, imgName + "[" + id + "].mark");

            if (!File.Exists(tempFiles)) File.WriteAllText(tempFiles, string.Empty);

            return tempFiles;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

        }


    
 
    }
}
