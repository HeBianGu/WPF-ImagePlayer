using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using HeBianGu.ImagePlayer.ImageControl;

namespace HeBianGu.Appliaction.Demo.ImageControl
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class ShellWindow : Window
    {
        //  Message：接口实现用例
        IImageCore _imgOperate = new ImageViews();

        bool _isload = false;

        public ShellWindow()
        {
            InitializeComponent();

            //  Message：设置初始速度
            _imgOperate.Speed = 1;

            _imgOperate.WheelScale = 1;

            //  Do：加载图片浏览主键
            this.grid_center.Children.Add(_imgOperate.BuildEntity());

            List<ImageMarkEntity> temp = new List<ImageMarkEntity>();

            //  Do：注册编辑标定事件
            _imgOperate.ImgMarkOperateEvent += (l, k) =>
              {
                  string fn = System.IO.Path.GetFileNameWithoutExtension(this._imgOperate.GetCurrentUrl());

                  string file = this.GetMarkFileName(fn);

                  string str = l.markOperateType.ToString();

                  Debug.WriteLine(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}");

                  temp.Add(l);

                  string result = JsonConvert.SerializeObject(temp);

                  File.WriteAllText(file, result);

                  MessageBox.Show(str + "：" + l.Name + "-" + l.Code + $"({l.X},{l.Y}) {l.Width}*{l.Height}", "保存成功");

              };

            ////  Do：注册风格化处理事件
            //_imgOperate.ImgProcessEvent += (l, k) =>
            //  {
            //      Debug.WriteLine("图片路径：" + l);

            //      Debug.WriteLine("操作参数：" + k);

            //      MessageBox.Show(k.ToString());
            //  };

            _imgOperate.PreviousImgEvent += () =>
              {
                  string current = _imgOperate.GetCurrentUrl();


                  string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                  var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                  var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);

                  collection = collection.Take(1);

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

            _imgOperate.NextImgEvent += () =>
            {
                string current = _imgOperate.GetCurrentUrl();

                string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);

                collection = collection.Take(1);

                foreach (var item in collection)
                {
                    string marks = File.ReadAllText(item);

                    var list = JsonConvert.DeserializeObject<List<ImageMarkEntity>>(marks); 
                    _imgOperate.LoadMarkEntitys(list);
                }


            };

            _imgOperate.DrawMarkedMouseUp += (l, k, m) =>
              {
                  Debug.WriteLine(l);
                  Debug.WriteLine(k);

                  

                  _imgOperate.AddMark(l);

                  string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_cache", Guid.NewGuid().ToString() + ".jpg");

                  if (!Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                  {
                      Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                  }

                  BitmapImage bitmapImage = SystemUtils.ByteArrayToBitmapImage(l.PicData);

                  SystemUtils.SaveBitmapImageIntoFile(bitmapImage, path);

                  //_imgOperate.CancelAddMark();
              };


            _imgOperate.MarkEntitySelectChanged += (l, k) =>
              {
                  Debug.WriteLine("MarkEntitySelectChanged" + l);
              };

            _imgOperate.FullScreenChangedEvent += (l, k) =>
                {
                    Debug.WriteLine("DoubleClickFullScreenHandle" + l);
                };

            this.Loaded += ShellWindow_Loaded;


        }

        private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //List<string> images = new List<string>();

            //images.Add(@"F:\GitHub\WPF-Project\SureDream 9.0\Product\Debug\images1\20190103035949.jpg");
            //images.Add(@"F:\GitHub\WPF-Project\SureDream 9.0\Product\Debug\images1\20190103035953.jpg");
            ////_imgOperate.LoadImages(images);

            //_imgOperate.LoadImg(images);
        }

        //  Message：加载图片
        private void CommandBinding_Search_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                DirectoryInfo directory = new DirectoryInfo(dialog.SelectedPath);

                var images = this.GetAllFile(directory, l => l.Extension.EndsWith("jpg") || l.Extension.EndsWith("png")); 

                _imgOperate.LoadImg(images.ToList());

                this._isload = true;
            }


            //OpenFileDialog open = new OpenFileDialog();

            //open.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            //var result = open.ShowDialog();

            //List<string> images = new List<string>();

            //if (result.HasValue && result.Value)
            //{
            //    var files = Directory.GetFiles(System.IO.Path.GetDirectoryName(open.FileName));

            //    foreach (var item in files)
            //    {
            //        if (System.IO.Path.GetExtension(item).EndsWith("jpg") || System.IO.Path.GetExtension(item).EndsWith("png"))
            //        {
            //            images.Add(item);
            //        }
            //    }
            //}



            //_imgOperate.LoadImg(images);

            //this._isload = true;
        }


        IEnumerable<string> GetAllImage(string folder)
        {
            DirectoryInfo directory = new DirectoryInfo(folder);

            var systemfiles = directory.GetFileSystemInfos();

            foreach (var item in systemfiles)
            {
                if (item is DirectoryInfo)
                {
                    GetAllImage(item.FullName);
                }
                else
                {
                    if (item.Extension.EndsWith("jpg") || item.Extension.EndsWith("png"))
                    {
                        yield return item.FullName;
                    }
                }
            }
        }


        /// <summary> 获取当前文件夹下所有匹配的文件 </summary>

        IEnumerable<string> GetAllFile(DirectoryInfo dir, Predicate<FileInfo> match = null)
        {
            foreach (var d in dir.GetFileSystemInfos())
            {
                if (d is DirectoryInfo)
                {
                    DirectoryInfo dd = d as DirectoryInfo;

                    var result = GetAllFile(dd, match);

                    foreach (var item in result)
                    {
                        yield return item;
                    }
                }

                else if (d is FileInfo)
                {
                    FileInfo dd = d as FileInfo;
                    if (match == null || match(dd))
                    {
                        yield return d.FullName;
                    }
                }
            }
        }


        private void CommandBinding_Search_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //  Message：上一页
        private void CommandBinding_Previous_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.PreviousImg();

        }

        private void CommandBinding_Previous_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Message：下一页
        private void CommandBinding_Next_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.NextImg();
        }

        private void CommandBinding_Next_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        private void CommandBinding_Next_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Do：全屏
        private void CommandBinding_FullScreen_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload && !_isfullscreen;
        }

        bool _isfullscreen = false;
        private void CommandBinding_FullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.SetFullScreen(true);
        }

        //  Do：退出全屏
        private void CommandBinding_UnFullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.SetFullScreen(false);
        }

        private void CommandBinding_UnFullScreen_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload && _isfullscreen;
        }

        //  Message：显示缺陷
        private void CommandBinding_ShowLocates_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.ShowLocates();
        }

        private void CommandBinding_ShowDefects_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.ShowDefects();
        }

        //  Message：显示全部标定
        private void CommandBinding_ShowMarks_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.ShowMarks();
        }

        //  Message：加速播放
        private void CommandBinding_ImgPlaySpeedUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.ImgPlaySpeedUp();
        }

        //  Message：减速播放
        private void CommandBinding_ImgPlaySpeedDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.ImgPlaySpeedDown();
        }

        //  Message：显示样本
        private void CommandBinding_ShowLocates_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        private void CommandBinding_ShowDefects_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        private void CommandBinding_ShowMarks_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        private void CommandBinding_ImgPlaySpeedUp_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        private void CommandBinding_ImgPlaySpeedDown_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Message：加载历史标定信息
        private void CommandBinding_LoadMarkEntitys_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Marks");

            var result = open.ShowDialog();

            if (result.HasValue && result.Value)
            {
                string marks = File.ReadAllText(open.FileName);

                var list = JsonConvert.DeserializeObject<List<ImageMarkEntity>>(marks);

                _imgOperate.LoadMarkEntitys(list);
            }


        }

        private void CommandBinding_LoadMarkEntitys_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Message：加载缺陷代码查询列表
        private void CommandBinding_LoadCodes_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            List<KeyValueViewModel> colleciotn = new List<KeyValueViewModel>();

            var count = r.Next(5, 10);

            for (int i = 0; i < count; i++)
            {
                KeyValueViewModel m = new KeyValueViewModel();
                m.Key = (i + 1).ToString();
                m.Value = "D10" + i.ToString();
                colleciotn.Add(m);
            }

            KeyValueWindow key = new KeyValueWindow();
            key.Collection = colleciotn;
            var result = key.ShowDialog();

            if (!result.HasValue) return;

            if (!result.Value) return;

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var item in key.Collection)
            {
                if (dic.ContainsKey(item.Key)) continue;
                dic.Add(item.Key, item.Value);
            }

            _imgOperate.LoadCodes(dic);
        }

        private void CommandBinding_LoadCodes_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        Random r = new Random();
        //  Message：添加图片详情信息
        private void CommandBinding_AddImgFigure_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            List<KeyValueViewModel> colleciotn = new List<KeyValueViewModel>();

            var count = r.Next(5, 10);

            for (int i = 0; i < count; i++)
            {
                KeyValueViewModel m = new KeyValueViewModel();
                m.Key = (i + 1).ToString();
                m.Value = "D10" + i.ToString();
                colleciotn.Add(m);
            }

            KeyValueWindow key = new KeyValueWindow();
            key.Collection = colleciotn;
            var result = key.ShowDialog();

            if (!result.HasValue) return;

            if (!result.Value) return;

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var item in key.Collection)
            {
                if (dic.ContainsKey(item.Key)) continue;
                dic.Add(item.Key, item.Value);
            }

            _imgOperate.AddImgFigure(dic);

        }

        private void CommandBinding_AddImgFigure_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Message：设置播放模式
        private void CommandBinding_SetImgPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.SetImgPlay((ImgPlayMode)this.cb_playmode.SelectedItem);
        }

        private void CommandBinding_SetImgPlay_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }

        //  Message：加载图片
        private void CommandBinding_LoadImg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            var result = open.ShowDialog();

            if (result.HasValue && result.Value)
            {
                _imgOperate.LoadImg(open.FileName);
            }



            this._isload = true;
        }

        private void CommandBinding_LoadImg_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// 获取标定信息存放路径
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns></returns>
        string GetMarkFileName(string imgName)
        {
            string file = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");

            string tempFiles = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks", imgName + "[" + file + "].mark");

            if (!File.Exists(tempFiles)) File.WriteAllText(tempFiles, string.Empty);

            return tempFiles;
        }

        //  Message：删除选中项
        private void CommandBinding_DeleteSelect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var entity = _imgOperate.GetSelectMarkEntity();

            entity.markOperateType = ImageMarkOperateType.Delete;

            _imgOperate.MarkOperate(entity);
        }

        private void CommandBinding_DeleteSelect_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _imgOperate.GetSelectMarkEntity() != null;
        }

        //  Message：设置标识位
        private void Cb_marktype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MarkType mark = (MarkType)this.cb_marktype.SelectedItem;

            _imgOperate.SetMarkType(mark);
        }

        //  Message：设置选中项
        private void CommandBinding_SetSelect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _imgOperate.SetSelectMarkEntity(l => string.IsNullOrEmpty(l.Code));
        }

        private void CommandBinding_SetSelect_CanExecut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this._isload;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._imgOperate.SetBubbleScale(e.NewValue);
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._imgOperate.WheelScale = e.NewValue;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this._imgOperate.SetNarrow();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this._imgOperate.SetEnlarge();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this._imgOperate.ClearAllImage();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this._imgOperate.SetToolVisible(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this._imgOperate.SetToolVisible(false);
        }
    }

}
