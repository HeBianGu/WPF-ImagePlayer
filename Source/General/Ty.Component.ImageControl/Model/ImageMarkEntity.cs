
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
    public class ImageMarkEntity
    { 
        public string ID { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 区分标定信息的来源
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
        /// 标定类型代码
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

        public ImageMarkEntity()
        {
            markOperateType = ImgMarkOperateType.Insert;

            ID = Guid.NewGuid().ToString();
        }  

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
    public delegate void ImgMarkHandler(ImageMarkEntity markEntity,IImageView operate);

    /// <summary>
    /// 图片风格化处理委托，
    /// </summary>
    /// <param name="imgPath">图片路径</param>
    /// <param name="imgProcessType">风格化处理类型</param>
    public delegate void ImgProcessHandler(string imgPath, ImgProcessType imgProcessType);



}
