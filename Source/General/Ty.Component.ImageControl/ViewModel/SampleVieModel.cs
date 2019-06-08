using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// 标定绑定模型
    /// </summary>
    public partial class SampleVieModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="imgMarkEntity"> 标定实体 </param>
        public SampleVieModel(ImgMarkEntity imgMarkEntity)
        {
            //  Do：注册明林
            RelayCommand = new RelayCommand(RelayMethod);

            //  Do：初始化标定信息
            Model = imgMarkEntity;
            this.Flag = "\xe76c";
            this.Name = imgMarkEntity.Name;
            this.Code = imgMarkEntity.Code;
            this.Type = imgMarkEntity.MarkType.ToString();

            if(imgMarkEntity.MarkType==0)
            {
                DefectShape defect = new DefectShape(imgMarkEntity.X, imgMarkEntity.Y, imgMarkEntity.Width, imgMarkEntity.Height);

                //  Do：添加到图层列表
                this.RectangleLayer.Add(defect);
            }
            else 
            {
                SampleShape sample = new SampleShape(imgMarkEntity.X, imgMarkEntity.Y, imgMarkEntity.Width, imgMarkEntity.Height);

                //  Do：添加到图层列表
                this.RectangleLayer.Add(sample);
            }

            
        }

        #region - 成员属性 -

        ImgMarkEntity _model = new ImgMarkEntity();

        /// <summary> 标定实体模型 </summary>
        public ImgMarkEntity Model { get => _model; set => _model = value; }

        private bool _visible = true;
        /// <summary> 是否可见  </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;

                RaisePropertyChanged("Visible");

                _rectangleLayerLayer.IsVisible = value;
            }
        }

        private RectangleLayer _rectangleLayerLayer = new RectangleLayer();
        /// <summary> 当前图层  </summary>
        public RectangleLayer RectangleLayer
        {
            get { return _rectangleLayerLayer; }
            set
            {
                _rectangleLayerLayer = value;
                RaisePropertyChanged("RectangleLayer");
            }
        }

        /// <summary> 名称  </summary>
        public string Name
        {
            get { return this.Model.Name; }
            set
            {
                this.Model.Name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _flag;
        /// <summary> 显示的符号Icon标识 </summary>
        public string Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                RaisePropertyChanged("Flag");
            }
        }

        private string _type;
        /// <summary> 标定类型 0=缺陷 1=样本  </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        /// <summary> 代码  </summary>
        public string Code
        {
            get { return _model.Code; }
            set
            {
                _model.Code = value;
                RaisePropertyChanged("Code");
            }
        }

        #endregion


        /// <summary>
        /// 添加形状
        /// </summary>
        /// <param name="shape"> 形状 </param>
        public void Add(RectangleShape shape)
        {
            this._model.X = (int)shape.Position.X;
            this._model.Y = (int)shape.Position.Y;

            this._model.Width = (int)shape.Width;
            this._model.Height = (int)shape.Height;

            this.RectangleLayer.Add(shape);
        }

        /// <summary>
        /// 命令触发的参数方法
        /// </summary>
        /// <param name="obj"></param>
        public void RelayMethod(object obj)
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

    partial class SampleVieModel : INotifyPropertyChanged
    {
        public RelayCommand RelayCommand { get; set; }

        public SampleVieModel()
        {
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
