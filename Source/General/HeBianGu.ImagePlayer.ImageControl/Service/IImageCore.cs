using HeBianGu.ImagePlayer.ImageControl.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary>
    /// 图片操作类
    /// </summary>
    public interface IImageCore : IDisposable
    {
        /// <summary> 左旋转 </summary>
        void SetRotateLeft();

        /// <summary> 右旋转 </summary>
        void SetRotateRight();

        /// <summary> 放大 </summary>
        void SetEnlarge();

        /// <summary> 缩小 </summary>
        void SetNarrow();

        /// <summary> 设置缩放比例 </summary>
        double Scale { get; set; }

        /// <summary>
        /// 是否鼠标滚轮进入播放模式
        /// </summary>
        bool IsWheelPlay { get; set; }

        /// <summary>
        /// 滚轮放大到的最大倍率
        /// </summary>
        int MaxScale { get; set; }

        /// <summary>
        /// 创建图片浏览展示控件
        /// </summary>
        /// <returns></returns>
        Control BuildEntity();

        /// <summary>
        /// 图片控件加载标定使用代码和代码名称
        /// </summary>
        /// <param name="codeDic">key：代码（唯一） value：代码名称</param>
        void LoadCodes(Dictionary<string, string> codeDic);

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="imgPath">图片路径</param>
        void LoadImg(string imgPath);

        /// <summary>
        /// 加载图片列表
        /// </summary>
        /// <param name="imgPathes">图片路径列表</param>
        void LoadImg(List<string> imgPathes);

        /// <summary>
        /// 加载文件夹
        /// </summary>
        /// <param name="path"></param>
        void LoadFolder(string path);

        /// <summary>
        /// 加载图片的标定信息
        /// </summary>
        /// <param name="markEntityList">图片已标定内容</param>
        void LoadMarkEntitys(List<ImageMarkEntity> markEntityList); 

        /// <summary>
        /// 新增/修改/删除图片标定事件
        /// </summary>
        event Action<ImageMarkEntity,IImageCore> ImgMarkOperateEvent;

        /// <summary>
        /// 上一张
        /// </summary>
        event Action PreviousImgEvent;

        /// <summary>
        /// 下一张
        /// </summary>
        event Action NextImgEvent;

        /// <summary>
        /// 绘制矩形框结束
        /// </summary>

        event Action<ImageMarkEntity, MarkType, IImageCore> DrawMarkedMouseUp;

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>

        event Action<string, IImageCore> DeleteImgEvent;

        /// <summary>
        /// 全屏模式切换事件 true=是全屏状态
        /// </summary>

        event Action<bool, IImageCore> FullScreenChangedEvent;



        /// <summary> 选中项改变事件 </summary>
        event Action<ImageMarkEntity, IImageCore> MarkEntitySelectChanged;

        /// <summary>
        /// 展示全部缺陷标注
        /// </summary>
        void ShowDefects();

        /// <summary>
        /// 展示全部区域定位标注
        /// </summary>
        void ShowLocates();

        /// <summary>
        /// 展示全部标注，包括缺陷和定位标注
        /// </summary>
        void ShowMarks();

        /// <summary>
        /// 展示指定编码标注
        /// </summary>
        /// <param name="markCodes"></param>
        void ShowMarks(List<string> markCodes);

        /// <summary>
        /// 是否全屏展示
        /// </summary>
        /// <param name="isFullScreen">true：全屏展示 false：正常展示</param>
        void SetFullScreen(bool isFullScreen);

        /// <summary>
        /// 图片详细信息展示
        /// </summary>
        /// <param name="imgFigures">要展示的指标和相应值</param>
        void AddImgFigure(Dictionary<string, string> imgFigures);

        /// <summary>
        /// 前一张
        /// </summary>
        void PreviousImg();

        /// <summary>
        /// 下一张
        /// </summary>
        void NextImg();

        /// <summary>
        /// 设置图片播放
        /// </summary>
        /// <param name="imgPlayMode">正序，倒叙，停止播放</param>
        void SetImgPlay(ImgPlayMode imgPlayMode);

        /// <summary>
        /// 加快图片播放速度
        /// </summary>
        void ImgPlaySpeedUp();

        /// <summary>
        /// 减慢图片播放速度
        /// </summary>
        void ImgPlaySpeedDown();

        /// <summary>
        /// 设置一个标识位，标识该图片属于检测分析还是样本标定 默认
        /// </summary>
        /// <param name="markType"></param>
        void SetMarkType(MarkType markType);

        /// <summary>
        /// 缺陷集合与样本集合模型要修改
        /// </summary>
        /// <param name="entity"></param>
        void MarkOperate(ImageMarkEntity entity);

        /// <summary>
        /// 获取当前选中图片已标定的矩形框
        /// </summary>
        /// <returns></returns>
        ImageMarkEntity GetSelectMarkEntity();

        /// <summary>
        /// 设置当前选中图片已标定的矩形框
        /// </summary>
        /// <returns></returns>
        void SetSelectMarkEntity(Predicate<ImageMarkEntity> match);


        /// <summary>
        /// 添加标定(在DrawMarkedMouseUp事件时添加标定)
        /// </summary>
        void AddMark(ImageMarkEntity imgMarkEntity);

        /// <summary>
        /// 取消添加标定(在DrawMarkedMouseUp事件时取消标定)
        /// </summary>
        void CancelAddMark();

        /// <summary>
        /// 旋转
        /// </summary>
        void Rotate();

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="saveFullName"></param>
        void ScreenShot(string saveFullName);

        /// <summary>
        /// 删除选中项
        /// </summary>
        void DeleteSelectMark();

        /// <summary>
        /// 注册自动操作放大的快捷键
        /// </summary>
        /// <param name="shortcut"></param>
        void RegisterPartShotCut(ShortCutEntitys shortcut);

        void OnImgMarkOperateEvent(ImageMarkEntity entity);

        string GetCurrentUrl();

        /// <summary> 设置缩放比例 </summary>
        double WheelScale { get; set; }

        /// <summary> 设置幻灯片播放 </summary>
        void StartSlidePlay();

        /// <summary> 设置幻灯片停止 </summary>
        void StopSlidePlay();

        /// <summary> 设置原始尺寸 </summary>
        void SetOriginalSize();

        /// <summary> 设置自适应尺寸 </summary>
        void SetAdaptiveSize();

        /// <summary> 设置滚轮模式 </summary>
        void SetWheelMode(bool value);

        int Speed { get; set; }

        /// <summary> 描述信息 </summary>
        string DetialText { get; set; }

        /// <summary> 设置气泡模式范围大小 </summary>
        void SetBubbleScale(double value);

        /// <summary>
        /// 是否已经缓冲完图片
        /// </summary>
        bool IsImageLoaded { get; set; }

        /// <summary> 当前索引 (用于检查播放是否同步) </summary>
        int CurrentIndex { get; }

        /// <summary> 加载百分比 </summary>
        double LoadPercent { get; set; }

        /// <summary> 获取数据源列表 </summary>
        List<string> GetImageList();
        

        Func<int, double> ConvertSpeedFunction { get; set; }

        Action<ImageCore> LoadedAction { get; set; }

        void SetImageSource(ImageSource imageSource);


        ImageSource GetCImageSource();

        /// <summary>
        /// 提供一个清除图片的接口，原本有图片，调用该接口后不显示任何图片，达到刚初始化时的状态
        /// </summary>
        void ClearAllImage();

        void SetToolVisible(bool value);

    }

    /// <summary>
    /// 标识位，标识该图片属于检测分析还是样本标定
    /// </summary>
    public enum MarkType
    {
        Sample = 0,
        Defect,
        Enlarge,
        Bubble,
        None
    }

}
