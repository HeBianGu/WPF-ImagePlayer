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

namespace HeBianGu.ImagePlayer.ImageView
{
    /// <summary>
    /// ImageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageWindow 
    {
        public ImageWindow()
        {
            InitializeComponent();

            this.InitHandle();
        }

        string[] _paths;
        public ImageWindow(params string[] paths)
        {
            InitializeComponent();

            this.InitHandle();


            if (_paths == null) return;

            foreach (var item in _paths)
            {
                if (File.Exists(item))
                {
                    this.imageview.LoadFile = item;
                    return;
                }

                if (Directory.Exists(item))
                {
                    this.imageview.LoadFolderPath = item;
                    return;
                }
            }


        }

        void InitHandle()
        {

            //  Do：注册编辑标定事件
            this.imageview.MarkChanged += (l, k) =>
            {
                string fn = System.IO.Path.GetFileNameWithoutExtension(this.imageview.GetCurrentImage());

                ObjectArgs<ImageMarkEntity> args = k as ObjectArgs<ImageMarkEntity>;

                ImageMarkEntity entity = args.Value;


                string file = this.GetMarkFileName(fn, entity.ID);

                string str = entity.markOperateType.ToString();

                if (entity.markOperateType == ImageMarkOperateType.Delete)
                {
                    File.Delete(file);
                    MessageBox.Show(str + "：" + entity.Name + "-" + entity.Code + $"({entity.X},{entity.Y}) {entity.Width}*{entity.Height}", "保存成功");
                    return;
                }
                else
                {

                    entity.ID = Guid.NewGuid().ToString();

                    Debug.WriteLine(str + "：" + entity.ID + "-" + entity.Name + "-" + entity.Code + $"({entity.X},{entity.Y}) {entity.Width}*{entity.Height}");

                    string result = JsonConvert.SerializeObject(entity);

                    File.WriteAllText(file, result);

                    MessageBox.Show(str + "：" + entity.Name + "-" + entity.Code + $"({entity.X},{entity.Y}) {entity.Width}*{entity.Height}", "保存成功");
                }
            };

            this.imageview.ImageIndexChanged += (s, e) =>
            {
                string current = this.imageview.GetCurrentImage();


                string fileName = System.IO.Path.GetFileNameWithoutExtension(current);

                var foder = Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Marks"));

                var collection = foder.GetFiles().Where(l => l.Name.StartsWith(fileName)).Select(l => l.FullName);

                foreach (var item in collection)
                {
                    string marks = File.ReadAllText(item);

                    var list = JsonConvert.DeserializeObject<ImageMarkEntity>(marks);

                    this.imageview.LoadMarks(list);
                }
            };
        }


        string GetMarkFileName(string imgName, string id)
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
