using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ty.Component.ImageControl
{
    /// <summary>
    /// 图片播放操作服务类
    /// </summary>
    public interface IImagePlayerService
    {
        /// <summary>
        /// 9）播放图片集合功能（List<string> ImageUrls），将集合内的图片按顺序反复播放，默认间隔为0.5秒。
        /// </summary>
        /// <param name="ImageUrls"></param>
        void LoadImages(List<string> ImageUrls);

        /// <summary>
        /// 10）播放图片文件夹功能，按照文件名按名称排序正序播放，默认间隔为0.5秒
        /// </summary>
        /// <param name="imageFoder"></param>
        void LoadImageFolder(List<string> imageFoders, string startForder);


        /// <summary>
        /// 10）播放图片文件夹功能，按照文件名按名称排序正序播放，默认间隔为0.5秒
        /// </summary>
        /// <param name="imageFoder"></param>
        void LoadShareImageFolder(List<string> imageFoders, string startForder, string useName, string passWord,string ip);

        /// <summary>
        /// 播放 ftp图片文件夹
        /// </summary>
        /// <param name="imageFoder"> 文件夹路径 </param>
        /// <param name="useName"> 用户名 </param>
        /// <param name="passWord"> 密码 </param>
        void LoadFtpImageFolder(List<string> imageFoders, string startForder, string useName, string passWord);

        /// <summary>
        /// 获取当前图片URL
        /// </summary>
        /// <returns></returns>
        string GetCurrentUrl();

        /// <summary>
        /// 获取当前索引和总数
        /// </summary>
        /// <returns></returns>
        Tuple<int, int> GetIndexWithTotal();

        /// <summary>
        /// 获取图片操作控件类
        /// </summary>
        /// <returns></returns>
        IImgOperate GetImgOperate();

        /// <summary>
        /// 设置播放模式
        /// </summary>
        /// <param name="imgPlayMode"></param>
        void SetImgPlay(ImgPlayMode imgPlayMode);

        /// <summary>
        /// 设置播放位置
        /// </summary>
        /// <param name="index"></param>
        void SetPositon(int index);

        /// <summary>
        /// 图片索引发生变化时触发，P=当前URL
        /// </summary>
        event Action<string,ImgSliderMode> ImageIndexChanged;

        /// <summary>
        /// 播放类型变化时触发
        /// </summary>
        event Action<ImgPlayMode> ImgPlayModeChanged;

        ///// <summary>
        ///// 拖拽进度条触发 P1= 索引 P2=路径
        ///// </summary>
        //event Action<int,string> SliderDragCompleted;

        /// <summary>
        /// 获取当前播放状态
        /// </summary>
        ImgPlayMode ImgPlayMode
        {
            get;
        }

        /// <summary>
        /// 7）提供截屏接口
        /// </summary>
        /// <param name="from"></param>
        void ScreenShot(string saveFullName);

        #region 设置方法
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
        /// 加快图片播放速度
        /// </summary>
        void ImgPlaySpeedUp();

        /// <summary>
        /// 减慢图片播放速度
        /// </summary>
        void ImgPlaySpeedDown();

        /// <summary>
        /// 旋转
        /// </summary>
        void Rotate();

        /// <summary>
        /// 删除选中项
        /// </summary>
        void DeleteSelectMark();

        #endregion
    }
}
