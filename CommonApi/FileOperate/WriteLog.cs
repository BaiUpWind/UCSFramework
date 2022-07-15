using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonApi.FileOperate
{
    public class WriteLog
    {
        public WriteLog()
        {
            logNet = new LogNetSingle(@"Log\log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
           
        }
        public WriteLog(string OtherType)
        {
            logNet_HaveName = new LogNetSingle(@"Log\"+ OtherType + "_log" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

        }
        private static ILog logNet;               // 日志
        private ILog logNet_HaveName;               // 日志
        /// <summary>
        /// 写入一条记录
        /// </summary>
        /// <param name="hmd"></param>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        public void WriteRecord(MessageDegree hmd, string key, string text,Exception ex = null)
        {
            if(ex != null)
            {
                logNet.WriteException(text, key, ex);
            }
            else
            {
                logNet.RecordMessage(hmd, key, text);

            } 
        }
        /// <summary>
        /// 写入一条记录
        /// </summary>
        /// <param name="hmd"></param>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="ex"></param>
        public void WriteRecord_HaveName(MessageDegree hmd, string key, string text, Exception ex = null)
        {
            if (ex != null)
            {
                logNet_HaveName.WriteException(text, key, ex);
            }
            else
            {
                logNet_HaveName.RecordMessage(hmd, key, text);

            }
        }



    }
    /// <summary>
    /// 记录消息的等级
    /// </summary>
    public enum MessageDegree
    {
        /// <summary>
        /// 一条消息都不记录
        /// </summary>
        None = 1,
        /// <summary>
        /// 记录致命等级及以上日志的消息
        /// </summary>
        FATAL = 2,
        /// <summary>
        /// 记录异常等级及以上日志的消息
        /// </summary>
        ERROR = 3,
        /// <summary>
        /// 记录警告等级及以上日志的消息
        /// </summary>
        WARN = 4,
        /// <summary>
        /// 记录信息等级及以上日志的消息
        /// </summary>
        INFO = 5,
        /// <summary>
        /// 记录调试等级及以上日志的信息
        /// </summary>
        DEBUG = 6
    }
    internal class LogNetSingle : LogNetBase, ILog
    {
        public LogNetSingle(string filePath)
        {
            LogSaveMode = LogSaveModeBySingleFile;

            m_fileName = filePath;

            FileInfo fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
         }
        /// <summary>
        /// 存储文件的时候指示单文件存储
        /// </summary>
        internal const int LogSaveModeBySingleFile = 1;
        private string m_fileName = string.Empty;

        /// <summary>
        /// 单日志文件允许清空日志内容
        /// </summary>
        public void ClearLog()
        {
            m_fileSaveLock.Enter();
            if (!string.IsNullOrEmpty(m_fileName))
            {
                File.Create(m_fileName).Dispose();
            }
            m_fileSaveLock.Leave();
        }

        /// <summary>
        /// 获取单日志文件的所有保存记录
        /// </summary>
        /// <returns>字符串信息</returns>
        public string GetAllSavedLog()
        {
            string result = string.Empty;
            m_fileSaveLock.Enter();
            if (!string.IsNullOrEmpty(m_fileName))
            {
                if (File.Exists(m_fileName))
                {
                    StreamReader stream = new StreamReader(m_fileName, Encoding.UTF8);
                    result = stream.ReadToEnd();
                    stream.Dispose();
                }
            }
            m_fileSaveLock.Leave();
            return result;
        }
        public string[] GetExistLogFileNames()
        {
            return new string[]
           {
                m_fileName,
           };
        }
        protected override string GetFileSaveName()
        {
            return m_fileName;
        }

    }

    /// <summary>
    /// 日志存储类的基类，提供一些基础的服务
    /// </summary>
    /// <remarks>
    /// 基于此类可以实现任意的规则的日志存储规则，欢迎大家补充实现，本组件实现了3个日志类
    /// <list type="number">
    /// <item>单文件日志类 <see cref="LogNetSingle"/></item>
    /// <item>根据文件大小的类 <see cref="LogNetFileSize"/></item>
    /// <item>根据时间进行存储的类 <see cref="LogNetDateTime"/></item>
    /// </list>
    /// </remarks>
    public abstract class LogNetBase : IDisposable
    {
        #region Constructor

        /// <summary>
        /// 实例化一个日志对象
        /// </summary>
        public LogNetBase()
        {
            m_fileSaveLock = new SimpleHybirdLock();
            m_simpleHybirdLock = new SimpleHybirdLock();
            m_WaitForSave = new Queue<MessageItem>();
            filtrateKeyword = new List<string>();
            filtrateLock = new SimpleHybirdLock();
        }

        #endregion

        #region Private Member

        private MessageDegree m_messageDegree = MessageDegree.DEBUG;                     // 默认的存储规则
        private Queue<MessageItem> m_WaitForSave;                                          // 待存储数据的缓存
        private SimpleHybirdLock m_simpleHybirdLock;                                          // 缓存列表的锁
        private int m_SaveStatus = 0;                                                          // 存储状态
        private List<string> filtrateKeyword;                                                  // 需要过滤的存储对象
        private SimpleHybirdLock filtrateLock;                                                 // 过滤列表的锁

        #endregion

        #region Protect Member

        /// <summary>
        /// 文件存储的锁
        /// </summary>
        protected SimpleHybirdLock m_fileSaveLock;                                             // 文件的锁

        #endregion

        #region Event Handle

        /// <summary>
        /// 在存储到文件的时候将会触发的事件
        /// </summary>
        public event EventHandler<EventArgs> BeforeSaveToFile = null;

        private void OnBeforeSaveToFile(EventArgs args)
        {
            BeforeSaveToFile?.Invoke(this, args);
        }

        #endregion

        #region Public Member

        /// <summary>
        /// 日志存储模式，1:单文件，2:按大小存储，3:按时间存储
        /// </summary>
        public int LogSaveMode { get; protected set; }


        #endregion

        #region Log Method

        /// <summary>
        /// 写入一条调试信息
        /// </summary>
        /// <param name="text"></param>
        public void WriteDebug(string text)
        {
            WriteDebug(string.Empty, text);
        }

        /// <summary>
        /// 写入一条调试信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteDebug(string keyWord, string text)
        {
            RecordMessage(MessageDegree.DEBUG, keyWord, text);
        }

        /// <summary>
        /// 写入一条普通信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteInfo(string text)
        {
            WriteInfo(string.Empty, text);
        }

        /// <summary>
        /// 写入一条普通信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteInfo(string keyWord, string text)
        {
            RecordMessage(MessageDegree.INFO, keyWord, text);
        }

        /// <summary>
        /// 写入一条警告信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteWarn(string text)
        {
            WriteWarn(string.Empty, text);
        }

        /// <summary>
        /// 写入一条警告信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteWarn(string keyWord, string text)
        {
            RecordMessage(MessageDegree.WARN, keyWord, text);
        }


        /// <summary>
        /// 写入一条错误消息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteError(string text)
        {
            WriteError(string.Empty, text);
        }

        /// <summary>
        /// 写入一条错误消息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteError(string keyWord, string text)
        {
            RecordMessage(MessageDegree.ERROR, keyWord, text);
        }

        /// <summary>
        /// 写入一条致命错误信息
        /// </summary>
        /// <param name="text">文本内容</param>
        public void WriteFatal(string text)
        {
            WriteFatal(string.Empty, text);
        }


        /// <summary>
        /// 写入一条致命错误信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本内容</param>
        public void WriteFatal(string keyWord, string text)
        {
            RecordMessage(MessageDegree.FATAL, keyWord, text);
        }

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="ex">异常信息</param>
        public void WriteException(string keyWord, Exception ex)
        {
            WriteException(keyWord, string.Empty, ex);
        }

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">内容</param>
        /// <param name="ex">异常</param>
        public void WriteException(string keyWord, string text, Exception ex)
        {
            RecordMessage(MessageDegree.FATAL, keyWord, ExceptionOperater.GetSaveStringFromException(text, ex));
        }

        /// <summary>
        /// 记录一条自定义的消息
        /// </summary>
        /// <param name="degree">消息的等级</param>
        /// <param name="keyWord">关键字</param>
        /// <param name="text">文本</param>
        public void RecordMessage(MessageDegree degree, string keyWord, string text)
        {
            WriteToFile(degree, keyWord, text);
        }

        /// <summary>
        /// 写入一条解释性的消息，不需要带有回车键
        /// </summary>
        /// <param name="description">解释性的文本</param>
        public void WriteDescrition(string description)
        {
            if (string.IsNullOrEmpty(description)) return;

            // 和上面的文本之间追加一行空行
            StringBuilder stringBuilder = new StringBuilder("\u0002");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("\u0002/");

            int count = 118 - CalculateStringOccupyLength(description);
            if (count >= 8)
            {
                int count_1 = (count - 8) / 2;
                AppendCharToStringBuilder(stringBuilder, '*', count_1);
                stringBuilder.Append("   ");
                stringBuilder.Append(description);
                stringBuilder.Append("   ");
                if (count % 2 == 0)
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1);
                }
                else
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1 + 1);
                }
            }
            else if (count >= 2)
            {
                int count_1 = (count - 2) / 2;
                AppendCharToStringBuilder(stringBuilder, '*', count_1);
                stringBuilder.Append(description);
                if (count % 2 == 0)
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1);
                }
                else
                {
                    AppendCharToStringBuilder(stringBuilder, '*', count_1 + 1);
                }
            }
            else
            {
                stringBuilder.Append(description);
            }

            stringBuilder.Append("/");
            stringBuilder.Append(Environment.NewLine);
            RecordMessage(MessageDegree.None, string.Empty, stringBuilder.ToString());
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveText), stringBuilder.ToString());
        }

        /// <summary>
        /// 写入一条任意字符
        /// </summary>
        /// <param name="text">内容</param>
        public void WriteAnyString(string text)
        {
            RecordMessage(MessageDegree.None, string.Empty, text);
        }

        /// <summary>
        /// 写入一条换行符
        /// </summary>
        public void WriteNewLine()
        {
            RecordMessage(MessageDegree.None, string.Empty, "\u0002" + Environment.NewLine);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveText), "\u0002" + Environment.NewLine);
        }

        /// <summary>
        /// 设置日志的存储等级，高于该等级的才会被存储
        /// </summary>
        /// <param name="degree">消息等级</param>
        public void SetMessageDegree(MessageDegree degree)
        {
            m_messageDegree = degree;
        }

        #endregion

        #region Filtrate Keyword

        /// <summary>
        /// 过滤指定的关键字存储
        /// </summary>
        /// <param name="keyWord">关键字</param>
        public void FiltrateKeyword(string keyWord)
        {
            filtrateLock.Enter();
            if (!filtrateKeyword.Contains(keyWord))
            {
                filtrateKeyword.Add(keyWord);
            }
            filtrateLock.Leave();
        }

        #endregion

        #region File Write

        private void WriteToFile(MessageDegree degree, string keyWord, string text)
        {
            // 过滤事件
            if (degree <= m_messageDegree)
            {
                // 需要记录数据
                MessageItem item = GetHslMessageItem(degree, keyWord, text);
                AddItemToCache(item);
            }
        }


        private void AddItemToCache(MessageItem item)
        {
            m_simpleHybirdLock.Enter();

            m_WaitForSave.Enqueue(item);

            m_simpleHybirdLock.Leave();

            StartSaveFile();
        }


        private void StartSaveFile()
        {
            if (Interlocked.CompareExchange(ref m_SaveStatus, 1, 0) == 0)
            {
                //启动存储
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolSaveFile), null);
            }
        }

        private MessageItem GetAndRemoveLogItem()
        {
            MessageItem result = null;

            m_simpleHybirdLock.Enter();

            result = m_WaitForSave.Count > 0 ? m_WaitForSave.Dequeue() : null;

            m_simpleHybirdLock.Leave();

            return result;
        }
 
        private void ThreadPoolSaveFile(object obj)
        {
            // 获取需要存储的日志
            MessageItem current = GetAndRemoveLogItem();
            // 进入文件操作的锁
            m_fileSaveLock.Enter();


            // 获取要存储的文件名称
            string LogSaveFileName = GetFileSaveName();

            if (!string.IsNullOrEmpty(LogSaveFileName))
            {
                // 保存
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(LogSaveFileName, true, Encoding.UTF8);
                    while (current != null)
                    {
                        // 触发事件
                        OnBeforeSaveToFile(new LogEventArgs() { LogMessage = current });

                        // 检查是否需要真的进行存储
                        bool isSave = true;
                        filtrateLock.Enter();
                        isSave = !filtrateKeyword.Contains(current.KeyWord);
                        filtrateLock.Leave();

                        // 检查是否被设置为强制不存储
                        if (current.Cancel) isSave = false;

                        // 如果需要存储的就过滤掉
                        if (isSave)
                        {
                            sw.Write(MessageFormate(current));
                            sw.Write(Environment.NewLine);
                        }

                        current = GetAndRemoveLogItem();
                    }
                }
                catch (Exception ex)
                {
                    AddItemToCache(current);
                    AddItemToCache(new MessageItem()
                    {
                        Degree = MessageDegree.FATAL,
                        Text = ExceptionOperater.GetSaveStringFromException("LogNetSelf", ex),
                    });
                }
                finally
                {
                    sw?.Dispose();
                }
            }

            // 释放锁
            m_fileSaveLock.Leave();
            Interlocked.Exchange(ref m_SaveStatus, 0);

            // 再次检测锁是否释放完成
            if (m_WaitForSave.Count > 0)
            {
                StartSaveFile();
            }
        }
     
        private string GetDegreeDescription(MessageDegree degree)
        {
            switch (degree)
            {
                case MessageDegree.None:

                    return "放弃"; 
                case MessageDegree.FATAL:
                    return "致命";
                case MessageDegree.ERROR:
                    return "错误";
                case MessageDegree.WARN:
                    return "警告";
                case MessageDegree.INFO:
                    return "信息";
                case MessageDegree.DEBUG:
                    return "调试";
                default:
                    return "放弃";
            }
        }

        private string MessageFormate(MessageItem hslMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (hslMessage.Degree != MessageDegree.None)
            {
                stringBuilder.Append("\u0002");
                stringBuilder.Append("[");
                stringBuilder.Append(GetDegreeDescription(hslMessage.Degree));
                stringBuilder.Append("] ");

                stringBuilder.Append(hslMessage.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                stringBuilder.Append(" thread:[");
                stringBuilder.Append(hslMessage.ThreadId.ToString("D2"));
                stringBuilder.Append("] ");

                if (!string.IsNullOrEmpty(hslMessage.KeyWord))
                {
                    stringBuilder.Append(hslMessage.KeyWord);
                    stringBuilder.Append(" : ");
                }
            }
            stringBuilder.Append(hslMessage.Text);

            return stringBuilder.ToString();
        }

        private void ThreadPoolSaveText(object obj)
        {
            // 进入文件操作的锁
            m_fileSaveLock.Enter();

            //获取要存储的文件名称
            string LogSaveFileName = GetFileSaveName();

            if (!string.IsNullOrEmpty(LogSaveFileName))
            {
                // 保存
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(LogSaveFileName, true, Encoding.UTF8);
                    string str = obj as string;
                    sw.Write(str);
                }
                catch (Exception ex)
                {
                    AddItemToCache(new MessageItem()
                    {
                        Degree = MessageDegree.FATAL,
                        Text = ExceptionOperater.GetSaveStringFromException("LogNetSelf", ex),
                    });
                }
                finally
                {
                    sw?.Dispose();
                }
            }

            // 释放锁
            m_fileSaveLock.Leave();
        }

        #endregion

        #region Helper Method


        /// <summary>
        /// 获取要存储的文件的名称
        /// </summary>
        /// <returns>完整的文件路径信息，带文件名</returns>
        protected virtual string GetFileSaveName()
        {
            return string.Empty;
        }

        /// <summary>
        /// 返回检查的路径名称，将会包含反斜杠
        /// </summary>
        /// <param name="filePath">路径信息</param>
        /// <returns>检查后的结果对象</returns>
        protected string CheckPathEndWithSprit(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (!filePath.EndsWith(@"\"))
                {
                    return filePath + @"\";
                }
            }
            return filePath;
        }



        private MessageItem GetHslMessageItem(MessageDegree degree, string keyWord, string text)
        {
            return new MessageItem()
            {
                KeyWord = keyWord,
                Degree = degree,
                Text = text,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
            };
        }


        private int CalculateStringOccupyLength(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            int result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] >= 0x4e00 && str[i] <= 0x9fbb)
                {
                    result += 2;
                }
                else
                {
                    result += 1;
                }
            }
            return result;
        }

        private void AppendCharToStringBuilder(StringBuilder sb, char c, int count)
        {
            for (int i = 0; i < count; i++)
            {
                sb.Append(c);
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否初次调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。

                    m_simpleHybirdLock.Dispose();
                    m_WaitForSave.Clear();
                    m_fileSaveLock.Dispose();

                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                m_simpleHybirdLock = null;
                m_WaitForSave = null;
                m_fileSaveLock = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~LogNetBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }


        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    #region 简单的混合锁

    /// <summary>
    /// 一个简单的混合线程同步锁，采用了基元用户加基元内核同步构造实现
    /// </summary>
    /// <example>
    /// 以下演示常用的锁的使用方式，还包含了如何优雅的处理异常锁
    /// <code lang="cs" source="" region="SimpleHybirdLockExample1" title="SimpleHybirdLock示例" />
    /// </example>
    public sealed class SimpleHybirdLock : IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                m_waiterLock.Close();

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~SimpleHybirdLock() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// 基元用户模式构造同步锁
        /// </summary>
        private int m_waiters = 0;
        /// <summary>
        /// 基元内核模式构造同步锁
        /// </summary>
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);

        /// <summary>
        /// 获取锁
        /// </summary>
        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiters) == 1) return;//用户锁可以使用的时候，直接返回，第一次调用时发生
            //当发生锁竞争时，使用内核同步构造锁
            m_waiterLock.WaitOne();
        }

        /// <summary>
        /// 离开锁
        /// </summary>
        public void Leave()
        {
            if (Interlocked.Decrement(ref m_waiters) == 0) return;//没有可用的锁的时候
            m_waiterLock.Set();
        }

        /// <summary>
        /// 获取当前锁是否在等待当中
        /// </summary>
        public bool IsWaitting => m_waiters != 0;
    }

    /// <summary>
    /// 单个日志的记录信息
    /// </summary>
    public class MessageItem
    {
        private static long IdNumber = 0;

        /// <summary>
        /// 默认的无参构造器
        /// </summary>
        public   MessageItem()
        {
            Id = Interlocked.Increment(ref IdNumber);
            Time = DateTime.Now;
        }

        /// <summary>
        /// 单个记录信息的标识ID，程序重新运行时清空
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// 消息的等级
        /// </summary>
        public MessageDegree Degree { get; set; } = MessageDegree.DEBUG;

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// 消息文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 消息发生的事件
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 消息的关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 是否取消写入到文件中去，在事件BeforeSaveToFile触发的时候捕获即可设置。
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 返回表示当前对象的字符串
        /// </summary>
        /// <returns>字符串信息</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(KeyWord))
            {
                return $"[{Degree}] {Time.ToString("yyyy-MM-dd HH:mm:ss.fff")} Thread [{ThreadId.ToString("D3")}] {Text}";
            }
            else
            {
                return $"[{Degree}] {Time.ToString("yyyy-MM-dd HH:mm:ss.fff")} Thread [{ThreadId.ToString("D3")}] {KeyWord} : {Text}";
            }
        }

        /// <summary>
        /// 返回表示当前对象的字符串，剔除了关键字
        /// </summary>
        /// <returns>字符串信息</returns>
        public string ToStringWithoutKeyword()
        {
            return $"[{Degree}] {Time.ToString("yyyy-MM-dd HH:mm:ss.fff")} Thread [{ThreadId.ToString("D3")}] {Text}";
        }
    }

    /// <summary>
    /// 带有日志消息的事件
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// 消息信息
        /// </summary>
        public MessageItem LogMessage { get; set; }

    }
    #endregion
}
