using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using HeBianGu.Base.WpfBase;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary>
    /// 图片标定信息绑定模型
    /// </summary>
    public partial class ImageViewViewModel
    {
        #region - 成员属性 -

        private ImageSource _imageSource;
        /// <summary>
        /// 图片资源
        /// </summary>
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }

        private ObservableCollection<MarkEntityViewModel> _samplecollection = new ObservableCollection<MarkEntityViewModel>();
        /// <summary> 样本标定集合  </summary>
        public ObservableCollection<MarkEntityViewModel> SampleCollection
        {
            get { return _samplecollection; }
            set
            {
                _samplecollection = value;
                RaisePropertyChanged("SampleCollection");
            }
        }


        private MarkEntityViewModel _selectSample;
        /// <summary> 当前选择的样本标定  </summary>
        public MarkEntityViewModel SelectSample
        {
            get { return _selectSample; }
            set
            {
                _selectSample = value;
                RaisePropertyChanged("SelectSample");
            }
        }

        private Dictionary<string, string> _codeCollection = new Dictionary<string, string>();
        /// <summary> 设置缺陷列表集合  </summary>
        public Dictionary<string, string> CodeCollection
        {
            get { return _codeCollection; }
            set
            {
                _codeCollection = value;
                RaisePropertyChanged("CodeCollection");
            }
        }


        private Dictionary<string, string> _figureCollection = new Dictionary<string, string>();
        /// <summary> 是指的图片详情信息列表  </summary>
        public Dictionary<string, string> FigureCollection
        {
            get { return _figureCollection; }
            set
            {
                _figureCollection = value;
                RaisePropertyChanged("FigureCollection");
            }
        }


        private bool _isBuzy=false;
        /// <summary> 说明  </summary>
        public bool IsBuzy
        {
            get { return _isBuzy; }
            set
            {
                _isBuzy = value;
                RaisePropertyChanged("IsBuzy");
            }
        }


        #endregion

        /// <summary>
        /// 添加标定
        /// </summary>
        /// <param name="entity"></param>
        public void Add(MarkEntityViewModel entity)
        {
            this.SampleCollection.Add(entity);
            
        }

        /// <summary>
        /// 命令触发参数方法
        /// </summary>
        /// <param name="obj"></param>
        public void RelayMethod(object obj)
        {
            string command = obj.ToString();
            


            //  Do：测试
            if (command == "text")
            {
                this.SampleCollection.Clear();
                for (int i = 0; i < 10; i++)
                {
                    MarkEntityViewModel sample = new MarkEntityViewModel();

                    sample.Name = "Name" + i;
                    sample.Flag = i % 3 == 0 ? "\xe688" : "\xeaf3";
                    sample.Code = "Code" + i;

                    this.SampleCollection.Add(sample);
                }
            }
            //  Do：删除标定
            else if (command == "delete")
            {
                if (this.SelectSample == null) return;

                this.SelectSample.Flag = "\xe743";
                this.SelectSample.Model.markOperateType = ImageMarkOperateType.Delete;
                this.SelectSample.Visible = false;
            }
            //  Do：更新标定
            else if (command == "update")
            {
                if (this.SelectSample == null) return;

                this.SelectSample.Flag = "\xe6b5";
                this.SelectSample.Model.markOperateType = ImageMarkOperateType.Update;
            }

            //  Do：删除标定
            else if (command == "menu_delete")
            {
                this.SelectSample = this.SampleCollection.ToList().Find(l => l.RectangleLayer.First().IsSelected);

                if (this.SelectSample == null) return;

                this.SelectSample.Model.markOperateType = ImageMarkOperateType.Delete;

                //  Do：触发删除事件
                this._iImgOperate.OnImgMarkOperateEvent(this.SelectSample.Model);


                this.SelectSample.RectangleLayer.First().Clear();

                this.SampleCollection.Remove(this.SelectSample);

               

                //this.SelectSample.Flag = "\xe6b5";
                //this.SelectSample.Model.markOperateType = ImgMarkOperateType.Update;
            }
        }

    }

    partial class ImageViewViewModel : INotifyPropertyChanged
    {

        public RelayCommand RelayCommand { get; set; }

        IImageCore _iImgOperate;

        public ImageViewViewModel(IImageCore iImgOperate)
        {
            _iImgOperate = iImgOperate;

            RelayCommand = new RelayCommand(RelayMethod);

        }
        
        #region - MVVM -

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    
}
