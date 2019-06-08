using HeBianGu.ImagePlayer.ImageControl;
using HeBianGu.ImagePlayer.ImageControl.Hook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    interface IMulMediaPlayer : IDisposable
    {

        //#region - 视频操作 -

        ///// <summary>
        ///// 视频路径
        ///// </summary>
        ///// <param name="videoPath"></param>
        //void LoadVedio(string videoPath);

        ///// <summary>
        ///// 3）视频支持跳转，提供外部跳转接口。（帧/时间）
        ///// </summary>
        ///// <param name="timeSpan"></param>
        //void SetPositon(TimeSpan timeSpan);

        ///// <summary>
        ///// 4）支持A->B点循环播放的功能（如1分10秒到1分25秒内循环播放）
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        //void RepeatFromTo(TimeSpan from, TimeSpan to);

        ///// <summary>
        ///// 7）提供截屏接口
        ///// </summary>
        ///// <param name="from"></param>
        //void ScreenShot(TimeSpan from, string saveFullName);


        ///// <summary>
        ///// 11）提供接口返回当前播放的Url，文件夹时返回文件Url
        ///// </summary>
        ///// <returns></returns>
        //string GetCurrentUrl(int index=0);

        ///// <summary>
        ///// 12）提供接口返回当前的帧数与总帧数
        ///// </summary>
        ///// <returns></returns>
        //TimeSpan GetCurrentFrame();

        ///// <summary>
        ///// 12）提供接口返回当前的帧数与总帧数
        ///// </summary>
        ///// <returns></returns>
        //TimeSpan GetTotalFrame();

        ///// <summary>
        ///// 音量属性控制
        ///// </summary>
        //void SetVolumn(double value);

        //#endregion

        #region - 图像操作 -

        /// <summary>
        /// 9）播放图片集合功能（List<string> ImageUrls），将集合内的图片按顺序反复播放，默认间隔为0.5秒。
        /// </summary>
        /// <param name="ImageUrls"></param>
        void LoadImageList(params Tuple<List<string>, string>[] ImageUrls);

        /// <summary>
        /// 10）播放图片文件夹功能，按照文件名按名称排序正序播放，默认间隔为0.5秒
        /// </summary>
        /// <param name="imageFoder"></param>
        void LoadImageFolders(params Tuple<List<string>, string>[] imageFoders);

        /// <summary>
        /// 10）播放图片文件夹功能，按照文件名按名称排序正序播放，默认间隔为0.5秒
        /// </summary>
        /// <param name="imageFoder"></param>
        void LoadImageShareFolders(string useName, string passWord, string ip, params Tuple<List<string>, string>[] imageFoders);

        /// <summary>
        /// 播放 ftp图片文件夹
        /// </summary>
        /// <param name="imageFoder"> 文件夹路径 </param>
        /// <param name="useName"> 用户名 </param>
        /// <param name="passWord"> 密码 </param>
        void LoadImageFtpFolders(string useName, string passWord, params Tuple<List<string>, string>[] imageFoders);


        /// <summary> 放大 </summary>
        void SetImageEnlarge(int index = 0);

        /// <summary> 缩小 </summary>
        void SetImageNarrow(int index = 0);

        /// <summary> 设置缩放比例 </summary>
        void SetImageScale(double value, int index = 0);

        /// <summary>
        /// 是否鼠标滚轮进入播放模式
        /// </summary>
        void SetImageWeelPlayMode(bool value);

        /// <summary>
        /// 新增/修改/删除图片标定事件
        /// </summary>
        event Action<ImgMarkEntity, int> ImageIndexMarkOperateEvent;

        ///// <summary>
        ///// 图片风格化处理事件
        ///// </summary>
        //event Action<string, ImgProcessType, int> ImageIndexProcessEvent;

        ///// <summary>
        ///// 上一张
        ///// </summary>
        //event Action<int> ImageIndexPreviousEvent;

        ///// <summary>
        ///// 下一张
        ///// </summary>
        //event Action<int> ImageIndexNextEvent;

        /// <summary>
        /// 绘制矩形框结束
        /// </summary>

        event Action<ImgMarkEntity, MarkType, int> ImageIndexDrawMarkedMouseUp;

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>

        event Action<string, int> ImageIndexDeletedClicked;


        /// <summary> 选中项改变事件 </summary>
        event Action<ImgMarkEntity, int> ImageMarkEntitySelectChanged;

        /// <summary> 全屏模式改变触发 true=全屏模式 false=取消全屏 </summary>
        event Action<bool> FullScreenStateChanged;

        /// <summary>
        /// 展示全部缺陷标注
        /// </summary> 
        void ShowAllImageIndexDefects(int index = 0);

        /// <summary>
        /// 展示全部区域定位标注
        /// </summary>
        void ShowImageIndexLocates(int index = 0);

        /// <summary>
        /// 展示全部标注，包括缺陷和定位标注
        /// </summary>
        void ShowImageIndexMarks(int index = 0);

        /// <summary>
        /// 展示指定编码标注
        /// </summary>
        /// <param name="markCodes"></param>
        void ShowImageIndexMarks(List<string> markCodes, int index);


        /// <summary>
        /// 前一张
        /// </summary>
        void SetImagPrevious();

        /// <summary>
        /// 下一张
        /// </summary>
        void SetImagNext();

        /// <summary>
        /// 设置图片播放
        /// </summary>
        /// <param name="imgPlayMode">正序，倒叙，停止播放</param>
        void SetImagePlayMode(ImgPlayMode imgPlayMode);

        /// <summary>
        /// 加快图片播放速度
        /// </summary>
        void SetImageSpeedUp();

        /// <summary>
        /// 减慢图片播放速度
        /// </summary>
        void SetImageSpeedDown();

        #endregion

        /// <summary>
        /// 设置一个标识位，标识该图片属于检测分析还是样本标定 默认
        /// </summary>
        /// <param name="markType"></param>
        void SetImageIndexMarkType(MarkType markType, int index = 0);

        /// <summary>
        /// 缺陷集合与样本集合模型要修改
        /// </summary>
        /// <param name="entity"></param>
        void SetImageIndexMarkOperate(ImgMarkEntity entity, int index = 0);

        /// <summary>
        /// 设置当前图片的资源
        /// </summary>
        /// <param name="entity"></param>
        void SetImageIndexImageSource(ImageSource source, int index = 0);

        /// <summary>
        /// 获取当前图片的资源
        /// </summary>
        /// <param name="entity"></param>
        ImageSource GetImageIndexImageSource(int index = 0);

        /// <summary>
        /// 获取当前选中图片已标定的矩形框
        /// </summary>
        /// <returns></returns>
        ImgMarkEntity GetImageIndexSelectMark(int index = 0);

        /// <summary>
        /// 设置当前选中图片已标定的矩形框
        /// </summary>
        /// <returns></returns>
        void SetImageIndexSelectMark(Predicate<ImgMarkEntity> match, int index = 0);

        /// <summary>
        /// 添加标定(在DrawMarkedMouseUp事件时添加标定)
        /// </summary>
        void AddImageIndexMark(ImgMarkEntity imgMarkEntity, int index = 0);

        /// <summary>
        /// 取消添加标定(在DrawMarkedMouseUp事件时取消标定)
        /// </summary>
        void CancelAddImageIndexMark(int index = 0);

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="saveFullName"></param>
        void ScreenShotImageIndex(string saveFullName, int index = 0);

        /// <summary>
        /// 删除选中项
        /// </summary>
        void DeleteImageIndexSelectMark(int index = 0);

        /// <summary>
        /// 注册自动操作放大的快捷键
        /// </summary>
        /// <param name="shortcut"></param>
        void SetImageIndexPartShotCut(ShortCutEntitys shortcut, int index = 0);

        /// <summary> 设置缩放比例 </summary>
        void SetImageIndexWheelScale(double value, int index = 0);

        /// <summary> 设置原始尺寸 </summary>
        void SetImageIndexOriginalSize(int index = 0);

        /// <summary> 设置自适应尺寸 </summary>
        void SetImageIndexAdaptiveSize(int index = 0);

        /// <summary> 设置滚轮模式 </summary>
        void SetImageIndexWheelMode(bool value, int index = 0);

        void SetImageIndexSpeed(int value, int index = 0);

        /// <summary> 描述信息 </summary>
        void SetImageIndexDetialText(string value, int index = 0);

        /// <summary> 设置气泡模式范围大小 </summary>
        void SetImageIndexBubbleScale(double value, int index = 0);

        /// <summary>
        /// 设置播放位置
        /// </summary>
        /// <param name="index"></param>
        void SetImageIndexPositon(int postion, int index = 0);

        /// <summary>
        /// 图片索引发生变化时触发，P=当前URL
        /// </summary>
        event Action<string, ImgSliderMode,int> ImageIndexChanged;

        /// <summary>
        /// 播放类型变化时触发
        /// </summary>
        event Action<ImgPlayMode> ImgPlayModeChanged;

        /// <summary>
        /// 获取当前播放状态
        /// </summary>
        ImgPlayMode GetImagePlayMode();

        /// <summary> 增加5s </summary>
        void SetImagePlayStepUp();

        /// <summary> 减少5s </summary>
        void SetImagePlayStepDown();

        /// <summary> 音量增加5 </summary>
        void SetImageVoiceStepUp();

        /// <summary> 音量减少5 </summary>
        void SetImageVoiceStepDown();

        /// <summary> 左旋转 </summary>
        void SetImageIndexRotateLeft(int index);

        /// <summary> 右旋转 </summary>
        void SetImageIndexRotateRight(int index);

        /// <summary> 全屏事件 </summary>
        event Action<int> ImageIndexFullScreenEvent;

        /// <summary> 清空所有缓存 </summary>
        void ClearAllCache();

        /// <summary>
        /// 加载图片的标定信息
        /// </summary>
        /// <param name="markEntityList">图片已标定内容</param>
        void ImageIndexLoadMarkEntitys(List<ImgMarkEntity> markEntityList,int index);

        /// <summary> 获取当前播放图片的路径 </summary>
        List<string> GetCurrentUrl();

        /// <summary>
        /// 获取指定数据源列表
        /// </summary>
        /// <param name="index"></param>
        List<string> GetImageIndexImageList(int index = 0);


        /// <summary>
        /// 设置全屏
        /// </summary>
        /// <param name="index"></param>
        void SetImageIndexFullScreen(int index = 0);

    }
}
