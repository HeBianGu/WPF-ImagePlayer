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

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary>
    /// MediaFullScreenControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaFullScreenControl : UserControl
    {
        public MediaFullScreenControl()
        {
            InitializeComponent();
        }

        public List<IVdeioImagePlayerService> MediaSources
        {
            get { return (List<IVdeioImagePlayerService>)GetValue(MediaSourcesProperty); }
            set { SetValue(MediaSourcesProperty, value); }
        } 

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaSourcesProperty =
            DependencyProperty.Register("MediaSources", typeof(List<IVdeioImagePlayerService>), typeof(MediaFullScreenControl), new PropertyMetadata(default(List<IVdeioImagePlayerService>), (d, e) =>
            {
                MediaFullScreenControl control = d as MediaFullScreenControl;

                if (control == null) return;

                List<IVdeioImagePlayerService> config = e.NewValue as List<IVdeioImagePlayerService>; 

                control.Init(); 

            }));

        void Init()
        {
            this.sp_list.Children.Clear();

            this.grid_center.Children.Clear(); 

            if (this.MediaSources == null) return;

            if (this.MediaSources.Count == 0) return;

            this.grid_center.Children.Add(this.MediaSources[Index] as UIElement);

            var collection = this.MediaSources.Where(l=>l!= this.MediaSources[Index]);

            foreach (var item in collection)
            {
                Control element = item as Control;

                element.Height = 200;

                this.sp_list.Children.Add(element);
            }
            
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(MediaFullScreenControl), new PropertyMetadata(default(int), (d, e) =>
             {
                 MediaFullScreenControl control = d as MediaFullScreenControl;

                 if (control == null) return;

                 //int config = e.NewValue as int;

                 control.Init();

             }));



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    this.CloseClicked?.Invoke();
        //}

        public event Action CloseClicked;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseClicked?.Invoke();
        }
    }
}
