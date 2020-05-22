using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeBianGu.General.ImageView
{
    /// <summary>
    /// 标识位，标识该图片属于检测分析还是样本标定
    /// </summary>
    public enum MarkType
    {
        None=0,
        Sample,
        Enlarge,
        Bubble
        
    }
}
