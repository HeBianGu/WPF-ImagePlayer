using CDTY.DataAnalysis.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary>
    /// 图片标定实体
    /// </summary>
    public class ImgMarkEntity
    {
        /// <summary>
        /// 由大括号包起来的全部大写的36位guidID，与表中的记录保持一致
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 图片路径 Add FROM LYQ BY LHJ
        /// </summary>
        public string ImagePath { get; set; }
        /// <summary>
        /// 区分标定信息的来源 Add FROM LYQ BY LHJ
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 标定矩形框左上角X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 标定矩形框左上角Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 标定矩形框高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 标定矩形框宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 标定类型代码（代码唯一，从代码表中查询）
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 标定类型名称（通过标定类型代码到表中查询）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标注操作类型，枚举值，默认是新增
        /// </summary>
        public ImgMarkOperateType markOperateType { get; set; }

        public ImgMarkEntity()
        {
            markOperateType = ImgMarkOperateType.Insert;

            ID = Guid.NewGuid().ToString();
        }

        #region - 新增字段 -

        public int UsedIn4C { get; set; }
        public int UsedIn3C { get; set; }
        public int UsedIn2C { get; set; }
        public int UsedIn1C { get; set; }
        public int IsDelete { get; set; }
        public int OrderNo { get; set; }
        public string Unit { get; set; }
        public int UsedIn5C { get; set; }
        public int ValueType { get; set; }
        public int ParamState { get; set; }
        //public string Code { get; set; }
        public string RootCode { get; set; }
        public string NamePY { get; set; }
        //public string Name { get; set; }
        public string ParentID { get; set; }
        //public string ID { get; set; }
        public string DefaultRate { get; set; }
        public int UsedIn6C { get; set; }

        #endregion

        /// <summary>
        /// 公里号
        /// </summary>
        public long KmMark { get; set; }

        /// <summary>
        /// 杆号
        /// </summary>
        public string Pole { get; set; }

        /// <summary>
        /// 站区
        /// </summary>
        public string StationArea { get; set; }


        #region - 缺陷标定管理新增 -
        
        /// <summary> 选择的责任工区  </summary>
        public TyeBaseDepartmentEntity SelectResponsibilityWorkArea { get; set; }
        
        /// <summary> 选择的责任车间  </summary>
        public TyeBaseDepartmentEntity SelectResponsibilityWorkshop { get; set; }
        
        /// <summary> 选择的单元  </summary>
        public TyeBasePillarEntity SelectBasicUnit { get; set; }
        
        /// <summary> 选择的站  </summary>
        public TyeBaseSiteEntity SelectDedicatedStation { get; set; }
        
        /// <summary> 选择的段  </summary>
        public TyeBaseLineEntity SelectDedicatedLine { get; set; }
        
        /// <summary> 选择的铁路局顺序码  </summary>
        public TyeBaseRailwaystationEntity SelectRailwaySsequence { get; set; }
        
        /// <summary> 选择的数据采集方式  </summary>
        public TyeBaseDatacollecttypeEntity SelectDataAcquisitionMode { get; set; }

        /// <summary>
        /// PHM编码（基本由界面属性组合而成）
        /// </summary>
        public string PHMCodes { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>

        public TyeAdminUserEntity tyeAdminUserEntity { set; get; }

        /// <summary>
        /// 检测日期
        /// <summary>

        public DateTime? DetectDate { get; set; }

        /// <summary>
        /// 公里标
        /// <summary>

        public string KmLog { get; set; }

        /// <summary>
        /// 检测车辆
        /// </summary>
        public string DetectionVehicles { get; set; }

        /// <summary>
        /// 选择的缺陷
        /// </summary>
        public TyeEncodeDeviceEntity SelectDefectOrMarkCodes { get; set; }

        /// <summary>
        /// 选择的历史信息
        /// </summary>
        public DefectCommonUsed SelectCommonHistoricalDefectsOrMark { get; set; }

        #endregion

        /// <summary> 标识当前标动属于哪个索引 </summary>
        public int Index { get; set; }

        /// <summary> 标定图片数据 </summary>
        public byte[] PicData { get; set; }

        /// <summary> 1：样本标定 0：缺陷标定 </summary>
        public int MarkType { get; set; } = 0;
    }

    /// <summary>
    /// 图片自动播放顺序
    /// </summary>
    public enum ImgPlayMode
    {
        正序 = 0,
        倒叙 = 1,
        停止播放 = 2,
    }

    /// <summary>
    /// 进度条改变模式
    /// </summary>
    public enum ImgSliderMode
    {
        /// <summary>
        /// 系统
        /// </summary>
        System = 0,
        /// <summary>
        /// 用户
        /// </summary>
        User = 1
    }

    /// <summary>
    /// 图片处理类型
    /// </summary>
    public enum ImgProcessType
    {
        滤镜 = 0,
        对比度 = 1,
        曝光补偿 = 2,
        夜视 = 3,
        锐化 = 4,
        边缘锐化 = 5,
    }




    /// <summary>
    /// 标定实体操作类型
    /// </summary>
    public enum ImgMarkOperateType
    {
        /// <summary>
        /// 新增标定
        /// </summary>
        Insert = 0,

        /// <summary>
        /// 修改已有标定
        /// </summary>
        Update = 1,

        /// <summary>
        /// 删除标定
        /// </summary>
        Delete = 2,
    }

    /// <summary>
    /// 标定内容变更委托，包括：新增、修改、删除
    /// </summary>
    /// <param name="markEntity">标定实体，实体内包含了实体是新增、修改还是删除的相关信息</param>
    public delegate void ImgMarkHandler(ImgMarkEntity markEntity,IImgOperate operate);

    /// <summary>
    /// 图片风格化处理委托，
    /// </summary>
    /// <param name="imgPath">图片路径</param>
    /// <param name="imgProcessType">风格化处理类型</param>
    public delegate void ImgProcessHandler(string imgPath, ImgProcessType imgProcessType);



}
