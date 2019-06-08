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
using System.Windows.Shapes;
using Ty.Base.WpfBase.Service;

namespace HeBianGu.Appliaction.Demo.ImageControl
{
    /// <summary>
    /// 键值对输入窗口
    /// </summary>
    public partial class KeyValueWindow : Window
    {
        public KeyValueWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 键值对实体集合
        /// </summary>
        public List<KeyValueViewModel> Collection
        {
            get { return (List<KeyValueViewModel>)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(List<KeyValueViewModel>), typeof(KeyValueWindow), new PropertyMetadata(default(List<KeyValueViewModel>), (d, e) =>
             {
                 KeyValueWindow control = d as KeyValueWindow;

                 if (control == null) return;

                 List<KeyValueViewModel> config = e.NewValue as List<KeyValueViewModel>;

             }));

        /// <summary>
        /// 提交确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
                this.Close();
        }
    }

    /// <summary>
    /// 键值对实体
    /// </summary>
    public class KeyValueViewModel : NotifyPropertyChanged
    {

        private string _key;
        /// <summary> 键  </summary>
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                RaisePropertyChanged("Key");
            }
        }


        private string _value;
        /// <summary> 值  </summary>
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }


        protected override void RelayMethod(object obj)
        {
            string command = obj.ToString();

            //  Do：应用
            if (command == "Sumit")
            {


            }
            //  Do：取消
            else if (command == "Cancel")
            {


            }
        }

    }
}
