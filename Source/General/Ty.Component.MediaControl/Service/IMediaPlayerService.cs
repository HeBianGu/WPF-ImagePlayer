using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeBianGu.ImagePlayer.ImagePlayerControl
{
    /// <summary>
    /// 视频播放服务类
    /// </summary>
    public interface IMediaPlayerService
    {
        /// <summary>
        /// 视频路径
        /// </summary>
        /// <param name="videoPath"></param>
        void Load(string videoPath);

        /// <summary>
        /// 3）视频支持跳转，提供外部跳转接口。（帧/时间）
        /// </summary>
        /// <param name="timeSpan"></param>
        void SetPositon(TimeSpan timeSpan);

        /// <summary>
        /// 4）支持A->B点循环播放的功能（如1分10秒到1分25秒内循环播放）
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void RepeatFromTo(TimeSpan from, TimeSpan to);

        /// <summary>
        /// 7）提供截屏接口
        /// </summary>
        /// <param name="from"></param>
        void ScreenShot(TimeSpan from,string saveFullName);

        ///// <summary>
        ///// 9）播放图片集合功能（List<string> ImageUrls），将集合内的图片按顺序反复播放，默认间隔为0.5秒。
        ///// </summary>
        ///// <param name="ImageUrls"></param>
        //void LoadImages(List<string> ImageUrls);

        ///// <summary>
        ///// 10）播放图片文件夹功能，按照文件名按名称排序正序播放，默认间隔为0.5秒
        ///// </summary>
        ///// <param name="imageFoder"></param>
        //void LoadImageFolder(string imageFoder);

        /// <summary>
        /// 11）提供接口返回当前播放的Url，文件夹时返回文件Url
        /// </summary>
        /// <returns></returns>
        string GetCurrentUrl();

        /// <summary>
        /// 12）提供接口返回当前的帧数与总帧数
        /// </summary>
        /// <returns></returns>
        TimeSpan GetCurrentFrame();

        /// <summary>
        /// 12）提供接口返回当前的帧数与总帧数
        /// </summary>
        /// <returns></returns>
        TimeSpan GetTotalFrame();

        /// <summary>
        /// 音量属性控制
        /// </summary>
        double Volumn
        {
            get;set;
        }

        /// <summary>
        /// 加快播放速度
        /// </summary>
        void PlaySpeedUp();

        /// <summary>
        /// 减慢播放速度
        /// </summary>
        void PlaySpeedDown();

        /// <summary> 增加5s </summary>
        void PlayStepUp();

        /// <summary> 减少5s </summary>
        void PlayStepDown();

        /// <summary> 音量增加5 </summary>
        void VoiceStepUp();

        /// <summary> 音量减少5 </summary>
        void VoiceStepDown();

        /// <summary> 左旋转 </summary>
        void RotateLeft();

        /// <summary> 右旋转 </summary>
        void RotateRight();

        /// <summary> 全屏事件 </summary>
        event Action FullScreenHandle;


    }
}
