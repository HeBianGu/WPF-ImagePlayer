using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeBianGu.ImagePlayer.ImageControl
{
    /// <summary> 缓存FTP和共享图片引擎 </summary>
    public class ImageCacheEngine : IDisposable
    {
        /// <summary> 唯一标识 </summary>
        public string ID { get; set; }

        /// <summary> 容器量 </summary>
        public int Capacity { get; set; } = 10;

        /// <summary> 容器量 </summary>
        public int CapacityTotal { get; set; } = 10;

        /// <summary> 同时下载的任务数量 </summary>
        public int TaskCount { get; set; } = 5;

        public string LocalFolder { get; set; }

        //  Message：所有的文件列表
        List<ImageCacheEntity> _fileCollection = new List<ImageCacheEntity>();

        //  Message：下载完成等待播放的队列
        List<ImageCacheEntity> _cache = new List<ImageCacheEntity>(); 

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"> 要下载的资源列表 </param>
        /// <param name="localFolder"> 本地缓存的路径 </param>
        /// <param name="startFile"> 默认要播放的起始位置 </param>
        /// <param name="sourceType"> 资源类型 FTP 或共享 </param>
        public ImageCacheEngine(List<string> filePath, string localFolder, string startFile, SourceType sourceType)
        {
            this.ID = Guid.NewGuid().ToString();

            //this.LocalFolder = Path.Combine(localFolder, this.ID);

            if (!Directory.Exists(localFolder))
            {
                Directory.CreateDirectory(localFolder);

            }

            foreach (var item in filePath)
            {
                //string localPath = Path.Combine(localFolder, this.ID, Path.GetFileName(item));

                string localPath = Path.Combine(localFolder, Path.GetFileName(item));

                ImageCacheEntity entity = new ImageCacheEntity(item, localPath, sourceType);

                _fileCollection.Add(entity);
            }

            _current = _fileCollection.Find(l => l.FilePath == startFile);

            this.Init();
        }

        private Task _task;

        /// <summary> 初始化后台一直运行的下载线程 </summary>
        void Init()
        {
            //  Message：启动当前位置的顺序下载任务
            _task = Task.Run(() =>
             {
                 while (true)
                 {
                     if (cts.IsCancellationRequested)
                     {
                         break;
                     }

                     if (_isEnbled == false)
                     {
                         Thread.Sleep(1000); continue;
                     }

                     int index = _fileCollection.FindIndex(l => l.FilePath == _current.FilePath);

                     this._cache = _fileCollection.Skip(index).Take(this.CapacityTotal).ToList();

                     foreach (var item in _cache.Where(l => l.IsLoaded == 0 || l.IsLoaded == -1))
                     {
                         item.Start();

                         //  Do：触发缓存百分比改变事件
                         CachePercentAction?.Invoke();
                     }

                     Thread.Sleep(500);
                 }

                 //_semaphore.Release();

             }, cts.Token);

        }

        public Action CachePercentAction; 

        public void RefreshCapacity(int count)
        {  
            this.Capacity = count * 5; 

            this.CapacityTotal = count * 5 * 5;
        }


        /// <summary> 当位置改变时触发，停止上一个下载任务，开始新位置的下载任务 </summary>
        void RefreshPosition(string current)
        {
            this._current = this._fileCollection.Find(l => l.FilePath == current);
        }


        private bool _isEnbled = false;

        public void Start()
        {
            this.RefreshPosition(this._current.FilePath);

            _isEnbled = true;
        }

        public void Stop()
        {
            _isEnbled = false;
        }

        //  Message：当前播放的节点
        ImageCacheEntity _current; 
 
        bool flag = true;

        Semaphore _semaphoreWait = new Semaphore(1, 1);

        /// <summary> 获取下好的文件 返回null则需要等待 </summary>
        public string GetWaitCurrent(string file, Action<bool, int, int> action)
        {
            _semaphoreWait.WaitOne();

            var result = this._fileCollection.Find(l => l.FilePath == file);

            this.RefreshPosition(file);

            if (result.IsLoaded == 2)
            {
                _semaphoreWait.Release();
                action(false, 100, 100);
                return result.LocalPath;
            }
            else
            {

                while (true)
                {
                    int index = this._fileCollection.FindIndex(l => l.FilePath == file);

                    var waitCache = _fileCollection.Skip(index).Take(this.Capacity).ToList();

                    if (waitCache.TrueForAll(l => l.IsLoaded == 2 || l.IsLoaded == -1))
                    {
                        action(true, waitCache.FindAll(l => l.IsLoaded == 2 || l.IsLoaded == -1).Count, waitCache.Count);

                        _semaphoreWait.Release();
                        return result.LocalPath;
                    }

                    if (_waitCurrentCancel)
                    {
                        _semaphoreWait.Release();
                        return result.LocalPath;
                    }

                    Debug.WriteLine("线程ID:" + Thread.CurrentThread.ManagedThreadId.ToString());

                    action(false, waitCache.FindAll(l => l.IsLoaded == 2 || l.IsLoaded == -1).Count, waitCache.Count);

                    Thread.Sleep(1000);
                }

             
            }
        }

        /// <summary> 获取当前缓存完的位置 </summary>
        public double GetBufferPercent()
        {
            string path = this._current.FilePath;

            int index = this._fileCollection.FindIndex(l => l.FilePath == path);

            var isdown = _fileCollection.Skip(index).LastOrDefault(l => l.IsLoaded == 2);

            if (isdown == null) return 0;

            return Convert.ToDouble(this._fileCollection.FindIndex(l => l == isdown)) / Convert.ToDouble(this._fileCollection.Count);
        }

        /// <summary> 清理缓存数据 </summary>
        public void Clear()
        {
            Directory.Delete(this.LocalFolder);
        }

        //  Message：是否是向前播放
        bool _isForward = true;

        public void RefreshPlayMode(bool forward)
        {
            if (_isForward == forward) return;

            _isForward = forward;


            Debug.WriteLine(forward ? "正序播放切换" : "倒序播放切换");


            _fileCollection.Reverse();

            //_waitCurrentCancel = true;
            //_semaphore.WaitOne();
            //_waitCurrentCancel = false;

            this.RefreshPosition(this._current.FilePath);
        }

        Semaphore _semaphore = new Semaphore(1, 1);
        CancellationTokenSource cts = new CancellationTokenSource();

        private bool _waitCurrentCancel = false;
        public void Dispose()
        {

            this.CachePercentAction = null;

            if (cts != null)
            {
                cts.Cancel();

                _waitCurrentCancel = true;

                this._fileCollection.Clear();

                //_semaphore.WaitOne();
            }

        }
    }

    class ImageCacheEntity
    {
        public ImageCacheEntity(string path, string localPath, SourceType sourceType)
        {
            this.CacheType = sourceType;
            this.FilePath = path;
            this.LocalPath = localPath;

        }

        object locker = new object();

        int _isLoaded = 0;

        /// <summary> 下载状态 0 未开始 1 正在下载 2 已经下载完成 </summary>
        public int IsLoaded
        {
            get
            {
                //  Message：加锁
                lock (locker)
                {
                    return _isLoaded;
                };
            }
            set
            {
                lock (locker)
                {
                    _isLoaded = value;
                };
            }
        }

        public SourceType CacheType { get; set; }

        public string FilePath { get; set; }

        public string LocalPath { get; set; }

        public void Start()
        {
            this.IsLoaded = 1;

            if (File.Exists(this.LocalPath))
            {
                this.IsLoaded = 2;
                return;
            }

            if (this.CacheType == SourceType.Ftp)
            {
                if (!File.Exists(this.LocalPath))
                {
                    try
                    {
                        FtpHelper.DownLoadFile(this.FilePath, this.LocalPath);
                        this.IsLoaded = 2;
                    }
                    catch (Exception ex)
                    {
                        this.IsLoaded = -1;
                        Debug.WriteLine("下载错误:" + ex);
                    }
                }
            }
            else
            {
                try
                {
                    File.Copy(this.FilePath, this.LocalPath, false);
                    this.IsLoaded = 2;
                }
                catch (Exception ex)
                {
                    this.IsLoaded = -1;
                    Debug.WriteLine("下载错误:" + ex);

                }
            }

            //Thread.Sleep(500);

            Debug.WriteLine("下载完成:" + this.FilePath);
        }

        public void Stop()
        {

        }
    }

    public enum SourceType
    {
        Local = 0, Ftp, Share
    }
}
