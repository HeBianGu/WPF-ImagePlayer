
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
        public ImageMarkOperateType markOperateType { get; set; }

        public ImageMarkEntity()
        {
            markOperateType = ImageMarkOperateType.Insert;

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
    /// 标定实体操作类型
    /// </summary>
    public enum ImageMarkOperateType
    {
        /// <summary>
        /// 新增
        /// </summary>
        Insert = 0,

        /// <summary>
        /// 修改
        /// </summary>
        Update = 1,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
    }  
}
