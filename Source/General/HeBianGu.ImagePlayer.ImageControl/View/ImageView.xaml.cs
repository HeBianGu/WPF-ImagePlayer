using System;
using System.Collections.Generic;
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

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary> 图片浏览控件 </summary>
    public partial class ImageView : UserControl
    {
        IImageCore _core;

        public ImageView()
        {
            InitializeComponent();

            this._core = this.imagecore;


            this._core.PreviousImgEvent += () =>
            {
                this.OnImageIndexChanged();
            };


            this._core.NextImgEvent += () =>
            {
                this.OnImageIndexChanged();
            };

            this._core.DrawMarkedMouseUp += (l, j, n) =>
              {
                  this.OnDrawMarked();

                  this._core.AddMark(l);
              };

            this._core.ImgMarkOperateEvent += (l, k) =>
              {
                  this.OnMarkChanged(l);
              };
        }

        #region - 依赖属性 -

        public List<string> ImageDataSouce
        {
            get { return (List<string>)GetValue(ImageDataSouceProperty); }
            set { SetValue(ImageDataSouceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageDataSouceProperty =
            DependencyProperty.Register("ImageDataSouce", typeof(List<string>), typeof(ImageView), new PropertyMetadata(default(List<string>), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 List<string> config = e.NewValue as List<string>;

                 control._core.LoadImg(config);

             }));


        public string LoadFolderPath
        {
            get { return (string)GetValue(LoadFolderPathProperty); }
            set { SetValue(LoadFolderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadFolderPathProperty =
            DependencyProperty.Register("LoadFolderPath", typeof(string), typeof(ImageView), new PropertyMetadata(default(string), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 string config = e.NewValue as string;

                 control._core.LoadFolder(config);

             }));

        public string LoadFile
        {
            get { return (string)GetValue(LoadFileProperty); }
            set { SetValue(LoadFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadFileProperty =
            DependencyProperty.Register("LoadFile", typeof(string), typeof(ImageView), new PropertyMetadata(default(string), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 string config = e.NewValue as string;

                 control._core.LoadImg(config);

             }));

        public ImgPlayMode ImgPlayMode
        {
            get { return (ImgPlayMode)GetValue(ImgPlayModeProperty); }
            set { SetValue(ImgPlayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgPlayModeProperty =
            DependencyProperty.Register("ImgPlayMode", typeof(ImgPlayMode), typeof(ImageView), new PropertyMetadata(default(ImgPlayMode), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 ImgPlayMode config = (ImgPlayMode)e.NewValue;

                 control._core.SetImgPlay(config);

             }));

        public MarkType MarkType
        {
            get { return (MarkType)GetValue(MarkTypeProperty); }
            set { SetValue(MarkTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkTypeProperty =
            DependencyProperty.Register("MarkType", typeof(MarkType), typeof(ImageView), new PropertyMetadata(default(MarkType), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 MarkType config = (MarkType)e.NewValue;

                 control._core.SetMarkType(config);

             }));



        public ImageMarkEntity ImageMarkEntity
        {
            get { return (ImageMarkEntity)GetValue(ImageMarkEntityProperty); }
            set { SetValue(ImageMarkEntityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageMarkEntityProperty =
            DependencyProperty.Register("ImageMarkEntity", typeof(ImageMarkEntity), typeof(ImageView), new PropertyMetadata(default(ImageMarkEntity), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 ImageMarkEntity config = e.NewValue as ImageMarkEntity;

                 control._core.MarkOperate(config);

             }));


        public ImageMarkEntity SelectImageMarkEntity
        {
            get
            {
                return this._core.GetSelectMarkEntity();
                //return (ImageMarkEntity)GetValue(SelectImageMarkEntityProperty);
            }
            set { SetValue(SelectImageMarkEntityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectImageMarkEntityProperty =
            DependencyProperty.Register("SelectImageMarkEntity", typeof(ImageMarkEntity), typeof(ImageView), new PropertyMetadata(default(ImageMarkEntity), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 ImageMarkEntity config = e.NewValue as ImageMarkEntity;

                 control._core.SetSelectMarkEntity(l => l == config);
             }));




        public double BubbleScale
        {
            get { return (double)GetValue(BubbleScaleProperty); }
            set { SetValue(BubbleScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleScaleProperty =
            DependencyProperty.Register("BubbleScale", typeof(double), typeof(ImageView), new PropertyMetadata(default(double), (d, e) =>
             {
                 ImageView control = d as ImageView;

                 if (control == null) return;

                 double config = (double)e.NewValue;

                 control._core.SetBubbleScale(config);

             }));



        #endregion

        #region - 依赖事件 -



        //声明和注册路由事件
        public static readonly RoutedEvent ImageIndexChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("ImageIndexChanged", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageView));
        //CLR事件包装
        public event RoutedEventHandler ImageIndexChanged
        {
            add { this.AddHandler(ImageIndexChangedRoutedEvent, value); }
            remove { this.RemoveHandler(ImageIndexChangedRoutedEvent, value); }
        }

        //激发路由事件,借用Click事件的激发方法

        protected void OnImageIndexChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(ImageIndexChangedRoutedEvent, this);

            this.RaiseEvent(args);
        }




        //声明和注册路由事件
        public static readonly RoutedEvent DrawMarkedRoutedEvent =
            EventManager.RegisterRoutedEvent("DrawMarked", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageView));
        //CLR事件包装
        public event RoutedEventHandler DrawMarked
        {
            add { this.AddHandler(DrawMarkedRoutedEvent, value); }
            remove { this.RemoveHandler(DrawMarkedRoutedEvent, value); }
        }

        //激发路由事件,借用Click事件的激发方法

        protected void OnDrawMarked()
        {
            RoutedEventArgs args = new RoutedEventArgs(DrawMarkedRoutedEvent, this);
            this.RaiseEvent(args);
        }



        //声明和注册路由事件
        public static readonly RoutedEvent MarkChangedRoutedEvent =
            EventManager.RegisterRoutedEvent("MarkChanged", RoutingStrategy.Bubble, typeof(EventHandler<ObjectArgs<ImageMarkEntity>>), typeof(ImageView));
        //CLR事件包装
        public event RoutedEventHandler MarkChanged
        {
            add { this.AddHandler(MarkChangedRoutedEvent, value); }
            remove { this.RemoveHandler(MarkChangedRoutedEvent, value); }
        }

        //激发路由事件,借用Click事件的激发方法

        protected void OnMarkChanged(ImageMarkEntity mark)
        {
            ObjectArgs<ImageMarkEntity> args = new ObjectArgs<ImageMarkEntity>(MarkChangedRoutedEvent, this);
            args.Value = mark;
            this.RaiseEvent(args);
        }


        #endregion


        public string GetCurrentImage()
        {
           return this._core.GetCurrentUrl();
        }

        public void LoadMarks(params ImageMarkEntity[] marks)
        {
            this._core.LoadMarkEntitys(marks?.ToList());
        }

        #region - 操作菜单 -

        private void CommandBinding_OpenFolder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               this._core.LoadFolder(dialog.SelectedPath);
            }
        } 

        private void CommandBinding_OpenFolder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton.Tag == null) return;

            MarkType markType = (MarkType)radioButton.Tag;

            this._core.SetMarkType(markType);
        }
        #endregion
    }

    public class ObjectArgs<T>: RoutedEventArgs
    {
        public ObjectArgs(RoutedEvent routedEvent, object source) : base( routedEvent,source)
        {

        }
        public T Value { get; set; }
    }
}


