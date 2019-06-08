//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using Ty.Base.WpfBase;

//namespace Ty.Component.ImageControl
//{
//    /// <summary>
//    /// ImageControl.xaml 的交互逻辑
//    /// </summary>
//    public partial class ImageControl : UserControl
//    {

//        public ImageControl()
//        {
//            InitializeComponent();
//        }


//        public ImageControlViewModel ViewModel
//        {
//            get
//            {
//                return (ImageControlViewModel)this.DataContext;
//            }
//            set
//            {
//                this.DataContext = value;
//            }
//        }




//        //声明和注册路由事件
//        public static readonly RoutedEvent LastClickedRoutedEvent =
//            EventManager.RegisterRoutedEvent("LastClicked", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageControl));
//        //CLR事件包装
//        public event RoutedEventHandler LastClicked
//        {
//            add { this.AddHandler(LastClickedRoutedEvent, value); }
//            remove { this.RemoveHandler(LastClickedRoutedEvent, value); }
//        }

//        //激发路由事件,借用Click事件的激发方法

//        protected void OnLastClicked()
//        {
//            RoutedEventArgs args = new RoutedEventArgs(LastClickedRoutedEvent, this);
//            this.RaiseEvent(args);
//        }


//        //声明和注册路由事件
//        public static readonly RoutedEvent NextClickRoutedEvent =
//            EventManager.RegisterRoutedEvent("NextClick", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ImageControl));
//        //CLR事件包装
//        public event RoutedEventHandler NextClick
//        {
//            add { this.AddHandler(NextClickRoutedEvent, value); }
//            remove { this.RemoveHandler(NextClickRoutedEvent, value); }
//        }

//        //激发路由事件,借用Click事件的激发方法

//        protected void OnNextClick()
//        {
//            RoutedEventArgs args = new RoutedEventArgs(NextClickRoutedEvent, this);
//            this.RaiseEvent(args);
//        }


//        Point start;

//        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            //if (e.LeftButton != MouseButtonState.Pressed) return;

//            _isMatch = false;

//            start = e.GetPosition(sender as InkCanvas);

//            //this.popup.IsOpen = false;




//        }



//        bool _isMatch = false;

//        //DynamicShape _dynamic;

//        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
//        {
//            if (e.LeftButton != MouseButtonState.Pressed) return;

//            Point end = e.GetPosition(this.canvas);

//            this._isMatch = Math.Abs(start.X - end.X) > 50 && Math.Abs(start.Y - end.Y) > 50;

//            //this._isMatch = Math.Abs(start.X - end.X) * Math.Abs(start.Y - end.Y) > 50;

//            //Rectangle path = new Rectangle();
//            //path.Width = Math.Abs(start.X - end.X);
//            //path.Height = Math.Abs(start.Y - end.Y);
//            //path.Stroke = Brushes.Red;
//            ////path.Fill = Brushes.Black;
//            //this._isMatch = Math.Abs(start.X - end.X) > 50 && Math.Abs(start.Y - end.Y) > 50;

//            //InkCanvas.SetLeft(path, Math.Min(start.X, end.X));
//            //InkCanvas.SetTop(path, Math.Min(start.Y, end.Y));

//            //this.canvas.Children.Add(path);


//            _dynamic.Visibility = Visibility.Visible;
//            _dynamic.Refresh(start, end);


//            //if (_dynamic != null)
//            //{
//            //    _dynamic.Clear(this.canvas);
//            //}

//            //_dynamic = new DynamicShape(start, end);
//            //_dynamic.ContextMenu = this.FindResource("Resource.ContextMenu.DynamicMenu") as ContextMenu;

//            //_dynamic.Draw(this.canvas);

//        }

//        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
//        {

//            if (!_isMatch)
//            {
//                _dynamic.Visibility = Visibility.Collapsed;
//                return;
//            };

//            _isMatch = false;

//            if (this.r_screen.IsChecked.HasValue && this.r_screen.IsChecked.Value)
//            {

//                //this._dynamic.Clear(this.canvas);
//                //this.VisualBrush.Viewbox = _dynamic.Rect;


//                //this.Screen_Rectangle.Visibility = Visibility.Visible;
//                //this.rectangle_clip.Visibility = Visibility.Visible;
//                //this.btn_close.Visibility = Visibility.Visible;
//                //this.rectangle_show.Visibility = Visibility.Visible;


//                //Rect rect = new Rect(this._dynamic.Position, new Size(this._dynamic.Width,
//                //     this._dynamic.Height));

//                //this.c_showScreen.IsChecked = true;
//                //this.rectangle_clip.Visibility = Visibility.Visible;
//                //this.rectangle_clip_background.Visibility = Visibility.Visible;

//                var geo = Geometry.Combine(this.rectangle_clip.RenderedGeometry, new RectangleGeometry(this._dynamic.Rect), GeometryCombineMode.Exclude, null);

//                this.ShowScreen();

//                this.VisualBrush.Viewbox = this._dynamic.Rect;

//                this.rectangle_clip.Clip = geo;



//                //this.rectangle_clip_background.Visibility = Visibility.Collapsed;

//                _dynamic.Visibility = Visibility.Collapsed;

//            }
//            else
//            {
//                this.popup.IsOpen = true;
//            }


//            start = e.GetPosition(sender as InkCanvas);





//            //MessageBoxResult result = MessageBox.Show("是否保存?", "提示！", MessageBoxButton.YesNo);

//            //if (result == MessageBoxResult.No)
//            //{
//            //    _dynamic.Clear(this.canvas);
//            //    return;
//            //}



//            //Point end = e.GetPosition(this.canvas);

//            //SampleVieModel sample = new SampleVieModel();
//            //sample.Name = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
//            //sample.Flag = "\xeac4";
//            //sample.Code = sample.Name;

//            //DefectShape resultStroke = new DefectShape(this._dynamic);

//            //resultStroke.Draw(this.canvas);

//            //sample.RectangleLayer.Add(resultStroke);

//            //this._vm.SampleCollection.Add(sample);

//            //_dynamic.Clear(this.canvas);

//        }

//        void ShowScreen()
//        {
//            this.c_showScreen.IsChecked = true;
//            this.rectangle_clip.Visibility = Visibility.Visible;
//            //this.rectangle_clip_background.Visibility = Visibility.Visible;
//        }

//        void HideScreen()
//        {
//            this.c_showScreen.IsChecked = false;
//            this.rectangle_clip.Visibility = Visibility.Collapsed;
//            //this.rectangle_clip_background.Visibility = Visibility.Collapsed;
//        }

//        private void CommandBinding_CanExecute_AddSample(object sender, CanExecuteRoutedEventArgs e)
//        {
//            e.CanExecute = this._isMatch;
//        }

//        private void CommandBinding_Executed_AddSample(object sender, ExecutedRoutedEventArgs e)
//        {


//        }

//        private void Button_Click(object sender, RoutedEventArgs e)
//        {

//            SampleVieModel sample = new SampleVieModel();
//            sample.Name = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

//            sample.Code = sample.Name;

//            RectangleShape resultStroke;

//            if (this.r_defect.IsChecked.HasValue && this.r_defect.IsChecked.Value)
//            {
//                resultStroke = new DefectShape(this._dynamic);
//                sample.Flag = "\xeac4";
//            }
//            else
//            {
//                resultStroke = new SampleShape(this._dynamic);
//                sample.Flag = "\xeac5";
//            }

//            resultStroke.Draw(this.canvas);

//            sample.RectangleLayer.Add(resultStroke);

//            this.ViewModel.SampleCollection.Add(sample);

//            //_dynamic.Clear(this.canvas);

//            //_dynamic.Visibility = Visibility.Visible;

//            this.popup.IsOpen = false;
//        }

//        private void btn_close_MouseEnter(object sender, MouseEventArgs e)
//        {
//            UIElement element = sender as UIElement;

//            element.Opacity = 1;
//        }

//        private void btn_close_MouseLeave(object sender, MouseEventArgs e)
//        {
//            UIElement element = sender as UIElement;

//            element.Opacity = 0.2;
//        }

//        private void btn_close_Click(object sender, RoutedEventArgs e)
//        {
//            //this.Screen_Rectangle.Visibility = Visibility.Collapsed;
//            //this.rectangle_clip.Visibility = Visibility.Collapsed;
//            //this.btn_close.Visibility = Visibility.Collapsed;
//            //this.rectangle_show.Visibility = Visibility.Collapsed;

//            //this.c_showScreen.IsChecked = false;

//            this.HideScreen();
//        }

//        private void Button_Click_1(object sender, RoutedEventArgs e)
//        {
//            this.Refresh();
//            this.OnLastClicked();
//        }

//        private void Button_Click_2(object sender, RoutedEventArgs e)
//        {
//            this.Refresh();
//            this.OnNextClick();
//        }

//        public void Refresh()
//        {
//            this.c_showScreen.IsChecked = false;
//            this._dynamic.Visibility = Visibility.Collapsed;



//            foreach (var sample in this.ViewModel.SampleCollection)
//            {
//                foreach (var item in sample.RectangleLayer)
//                {
//                    item.Clear(this.canvas);
//                }
//            }


//        }
//    }



//    public partial class ImageControlViewModel
//    {
//        private ImageSource _imageSource;
//        /// <summary> 说明  </summary>
//        public ImageSource ImageSource
//        {
//            get { return _imageSource; }
//            set
//            {
//                _imageSource = value;
//                RaisePropertyChanged("ImageSource");
//            }
//        }


//        private ObservableCollection<SampleVieModel> _samplecollection = new ObservableCollection<SampleVieModel>();
//        /// <summary> 说明  </summary>
//        public ObservableCollection<SampleVieModel> SampleCollection
//        {
//            get { return _samplecollection; }
//            set
//            {
//                _samplecollection = value;
//                RaisePropertyChanged("SampleCollection");
//            }
//        }



//        private StrokeCollection _strokeCollection = new StrokeCollection();
//        /// <summary> 说明  </summary>
//        public StrokeCollection StrokeCollection
//        {
//            get { return _strokeCollection; }
//            set
//            {
//                _strokeCollection = value;

//                RaisePropertyChanged("StrokeCollection");
//            }
//        }



//        public void RelayMethod(object obj)
//        {
//            string command = obj.ToString();

//            //  Do：应用
//            if (command == "text")
//            {
//                this.SampleCollection.Clear();
//                for (int i = 0; i < 10; i++)
//                {
//                    SampleVieModel sample = new SampleVieModel();

//                    sample.Name = "Name" + i;
//                    sample.Flag = i % 3 == 0 ? "\xeac5" : "\xeac3";
//                    sample.Code = "Code" + i;

//                    this.SampleCollection.Add(sample);
//                }
//            }
//            //  Do：取消
//            else if (command == "Cancel")
//            {


//            }
//        }

//    }

//    partial class ImageControlViewModel : INotifyPropertyChanged
//    {

//        public RelayCommand RelayCommand { get; set; }

//        public ImageControlViewModel()
//        {
//            RelayCommand = new RelayCommand(RelayMethod);

//        }
//        #region - MVVM -

//        public event PropertyChangedEventHandler PropertyChanged;

//        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
//        {
//            if (PropertyChanged != null)
//                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//        }

//        #endregion
//    }


//    public partial class SampleVieModel
//    {

//        private bool _visible = true;
//        /// <summary> 说明  </summary>
//        public bool Visible
//        {
//            get { return _visible; }
//            set
//            {
//                _visible = value;

//                RaisePropertyChanged("Visible");

//                _rectangleLayerLayer.IsVisible = value;
//            }
//        }


//        private RectangleLayer _rectangleLayerLayer = new RectangleLayer();
//        /// <summary> 说明  </summary>
//        public RectangleLayer RectangleLayer
//        {
//            get { return _rectangleLayerLayer; }
//            set
//            {
//                _rectangleLayerLayer = value;
//                RaisePropertyChanged("RectangleLayer");
//            }
//        }


//        private string _name;
//        /// <summary> 说明  </summary>
//        public string Name
//        {
//            get { return _name; }
//            set
//            {
//                _name = value;
//                RaisePropertyChanged("Name");
//            }
//        }


//        private string _flag;
//        /// <summary> 说明  </summary>
//        public string Flag
//        {
//            get { return _flag; }
//            set
//            {
//                _flag = value;
//                RaisePropertyChanged("Flag");
//            }
//        }


//        private string _type;
//        /// <summary> 说明  </summary>
//        public string Type
//        {
//            get { return _type; }
//            set
//            {
//                _type = value;
//                RaisePropertyChanged("Type");
//            }
//        }


//        private string _code;
//        /// <summary> 说明  </summary>
//        public string Code
//        {
//            get { return _code; }
//            set
//            {
//                _code = value;
//                RaisePropertyChanged("Code");
//            }
//        }



//        public void RelayMethod(object obj)
//        {
//            string command = obj.ToString();

//            //  Do：应用
//            if (command == "Sumit")
//            {


//            }
//            //  Do：取消
//            else if (command == "Cancel")
//            {


//            }
//        }
//    }

//    partial class SampleVieModel : INotifyPropertyChanged
//    {
//        public RelayCommand RelayCommand { get; set; }

//        public SampleVieModel()
//        {
//            RelayCommand = new RelayCommand(RelayMethod);

//        }
//        #region - MVVM -

//        public event PropertyChangedEventHandler PropertyChanged;

//        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
//        {
//            if (PropertyChanged != null)
//                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//        }

//        #endregion
//    }


//}
