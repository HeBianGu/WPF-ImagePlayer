using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeBianGu.ImagePlayer.ImageControl.Hook
{

    /// <summary> 系统快捷键服务 </summary>
    public class ShortCutHookService 
    {

        public static ShortCutHookService Instance = new ShortCutHookService();
        /// <summary> 创建监控引擎 </summary>
        // void CreateMoniter()
        //{
        //    HookKeyboardEngine.KeyUp += HookKeyboardEngine_KeyUp;

        //    HookKeyboardEngine.KeyDown += HookKeyboardEngine_KeyDown;
        //}

        public ShortCutHookService()
        {
            HookKeyboardEngine.KeyUp += HookKeyboardEngine_KeyUp;

            HookKeyboardEngine.KeyDown += HookKeyboardEngine_KeyDown;
        }

        private void HookKeyboardEngine_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEntity k = new KeyEntity();
            k.Key = e.KeyCode;
            k.Time = DateTime.Now;
            _current.AddDown(k);
        }

        private void HookKeyboardEngine_KeyUp(object sender, KeyEventArgs e)
        {
            KeyEntity k = new KeyEntity();
            k.Key = e.KeyCode;
            k.Time = DateTime.Now;
            _current.RemoveDown(k);
            _current.Add(k);

            foreach (var item in _collection)
            {
                // Todo ：匹配规则触发任务
                if (_current.Equals(item.Item1))
                {
                    item.Item2.Invoke();
                }
            }
        }

        // Todo ：当前按钮记录 
        ShortCutEntitys _current = new ShortCutEntitys();

        // Todo ：匹配规则记录 
        List<Tuple<ShortCutEntitys, Action>> _collection = new List<Tuple<ShortCutEntitys, Action>>();

        /// <summary> 注册执行命令 </summary>
        public void RegisterCommand(ShortCutEntitys match, Action action)
        {
            //if (_collection.Count == 0)
            //{
            //    this.CreateMoniter();
            //}

            Tuple<ShortCutEntitys, Action> t = new Tuple<ShortCutEntitys, Action>(match, action);
            _collection.Add(t);
        }

        public void Clear()
        {
            _collection.Clear();
        }
    }

    /// <summary> 配置信息 </summary>
    class ShortCutConfiger
    {

        /// <summary> 间隔字符 </summary>
        public const char SptitChar = '+';

        /// <summary> 间隔字符串 </summary>
        public const string SptitString = "+";

        /// <summary> 触发范围 </summary>
        public const int SplitMilliseconds = 500;

        /// <summary> 按下状态 </summary>
        public const char downChar = '↓';

        private static List<Keys> _alts = new List<Keys>() { Keys.LMenu, Keys.RMenu, Keys.Alt };
        /// <summary> 说明 </summary>
        public List<Keys> Alts
        {
            get { return _alts; }
        }

        private static List<Keys> _shifts = new List<Keys>() { Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey };
        /// <summary> 说明 </summary>
        public static List<Keys> Shifts
        {
            get { return _shifts; }
        }

        private static List<Keys> _ctrls = new List<Keys>() { Keys.LControlKey, Keys.RControlKey, Keys.Control, Keys.ControlKey };
        /// <summary> 说明 </summary>
        public static List<Keys> Ctrls
        {
            get { return _ctrls; }
        }

        /// <summary> 是否匹配系统按键 并且已经按下系统按键 </summary>
        public static bool IsMatchSystem(Keys k, KeyEventArgs key)
        {
            if (_alts.Contains(k))
            {
                if (!key.Alt) return false;
            }
            else if (_shifts.Contains(k))
            {
                if (!key.Shift) return false;
            }
            else if (_ctrls.Contains(k))
            {
                if (!key.Control) return false;
            }

            return true;
        }
    }


    /// <summary> 快捷按钮实体 </summary>
    public class ShortCutEntitys
    {
        List<KeyEntity> _keys = new List<KeyEntity>();

        /// <summary> 正常按键 </summary>
        internal List<KeyEntity> Keys { get => _keys; set => _keys = value; }

        private List<KeyEntity> _downKeys = new List<KeyEntity>();
        /// <summary> 按下的按键 </summary>
        public List<KeyEntity> DownKeys
        {
            get { return _downKeys; }
            set { _downKeys = value; }
        }

        /// <summary> 容器的大小 </summary>
        int capacity = 3;


        /// <summary> 增加正常按键 </summary>
        public void Add(KeyEntity key)
        {
            Keys.Add(key);

            if (Keys.Count > 3)
            {
                Keys.RemoveAt(0);
            }
        }

        /// <summary> 增加按下的按键 </summary>
        public void AddDown(KeyEntity key)
        {
            if (this.DownKeys.Exists(l => l.Key == key.Key)) return;

            this.DownKeys.Add(key);
        }

        /// <summary> 删除按下的按键 </summary>
        public void RemoveDown(KeyEntity key)
        {
            this.DownKeys.RemoveAll(l => l.Key == key.Key);
        }

        /// <summary> 是否包含指定快捷键 </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is ShortCutEntitys)) return false;

            ShortCutEntitys s = obj as ShortCutEntitys;

            // Todo ：比较按下键 
            if (this.DownKeys.Count != s.DownKeys.Count) return false;

            foreach (var item in s.DownKeys)
            {
                if (!this.DownKeys.Exists(l => l.Key == item.Key)) return false;
            }

            // Todo ：非按下键 
            if (Keys.Count < s.Keys.Count) return false;

            for (int i = 0; i < s.Keys.Count; i++)
            {
                // Todo ：比较按键是否一样 
                if (this.Keys[this.Keys.Count - i - 1].Key != s.Keys[s.Keys.Count - i - 1].Key)
                {
                    return false;
                }

                // Todo ：判断时间间隔 
                if (i == s.Keys.Count - 1) continue; 

                if ((this.Keys[this.Keys.Count - i - 1].Time - this.Keys[this.Keys.Count - i - 2].Time).TotalMilliseconds > ShortCutConfiger.SplitMilliseconds)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary> 转换可视化文本 </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in this._downKeys)
            {
                sb.Append(item.Key + ShortCutConfiger.downChar.ToString() + ShortCutConfiger.SptitString.ToString());
            }

            foreach (var item in this.Keys)
            {
                sb.Append(item.Key + ShortCutConfiger.SptitString);
            }

            return sb.ToString().Trim(ShortCutConfiger.SptitChar);
        }


        /// <summary> 根据文本生成规则 </summary>
        public static ShortCutEntitys DeSerilse(string str)
        {
            var ss = str.Split(ShortCutConfiger.SptitChar);

            var ds = ss.ToList().FindAll(l => l.EndsWith(ShortCutConfiger.downChar.ToString()));

            var ns = ss.Except(ds);

            ShortCutEntitys s = new ShortCutEntitys();

            foreach (var item in ds)
            {
                KeyEntity ke = new KeyEntity();
                Keys k = (Keys)Enum.Parse(typeof(Keys), item.Trim(ShortCutConfiger.downChar));
                ke.Key = k;
                s.AddDown(ke);
            }


            foreach (var item in ns)
            {
                KeyEntity ke = new KeyEntity();
                Keys k = (Keys)Enum.Parse(typeof(Keys), item.Trim(ShortCutConfiger.downChar));
                ke.Key = k;
                s.Add(ke);
            }



            return s;
        }
    }


    /// <summary> 键盘按键 </summary>
    public class KeyEntity
    {
        private Keys _key;
        /// <summary> 按下的键 </summary>
        public Keys Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private DateTime _time;
        /// <summary> 按下的时间 </summary>
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }


    }
}
