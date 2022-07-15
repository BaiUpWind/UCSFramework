using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonApi
{
    /*******************************************************************************
    * 
    *    用于显示和保存的数据信息，未来支持中英文
    *
    *    Used to the return result class in the synchronize communication and communication for industrial Ethernet
    * 
    *******************************************************************************/
     

    /// <summary>
    /// 系统的字符串资源及多语言管理中心 ->
    /// System string resource and multi-language management Center
    /// </summary>
    public static class StringResources
    {
        #region Constractor

        static StringResources()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.ToString().StartsWith("zh"))
            {
                SetLanguageChinese();
            }
            else
            {
                SeteLanguageEnglish();
            }
        }

        #endregion


        /// <summary>
        /// 获取或设置系统的语言选项 ->
        /// Gets or sets the language options for the system
        /// </summary>
        public static DefaultLanguage Language = new DefaultLanguage();

        /// <summary>
        /// 将语言设置为中文 ->
        /// Set the language to Chinese
        /// </summary>
        public static void SetLanguageChinese()
        {
            Language = new DefaultLanguage();
        }

        /// <summary>
        /// 将语言设置为英文 ->
        /// Set the language to English
        /// </summary>
        public static void SeteLanguageEnglish()
        {
            Language = new English();
        }
    }

    /// <summary>
    /// 系统的语言基类，默认也即是中文版本
    /// </summary>
    public class DefaultLanguage
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /***********************************************************************************
         * 
         *    一般的错误信息
         * 
         ************************************************************************************/

        public virtual string ConnectedFailed => "连接失败：";
        public virtual string ConnectedSuccess => "连接成功！";
        public virtual string UnknownError => "未知错误";
        public virtual string ErrorCode => "错误代号";
        public virtual string TextDescription => "文本描述";
        public virtual string ExceptionMessage => "错误信息：";
        public virtual string ExceptionSourse => "错误源：";
        public virtual string ExceptionType => "错误类型：";
        public virtual string ExceptionStackTrace => "错误堆栈：";
        public virtual string ExceptopnTargetSite => "错误方法：";
        public virtual string ExceprionCustomer => "用户自定义方法出错：";
        public virtual string SuccessText => "成功";
        public virtual string TwoParametersLengthIsNotSame => "两个参数的个数不一致";
        public virtual string NotSupportedDataType => "输入的类型不支持，请重新输入";
        public virtual string NotSupportedFunction => "当前的功能逻辑不支持";
        public virtual string DataLengthIsNotEnough => "接收的数据长度不足，应该值:{0},实际值:{1}";
        public virtual string ReceiveDataTimeout => "接收数据超时：";
        public virtual string ReceiveDataLengthTooShort => "接收的数据长度太短：";
        public virtual string MessageTip => "消息提示：";
        public virtual string Close => "关闭";
        public virtual string Time => "时间：";
        public virtual string SoftWare => "软件：";
        public virtual string BugSubmit => "Bug提交";
        public virtual string MailServerCenter => "邮件发送系统";
        public virtual string MailSendTail => "邮件服务系统自动发出，请勿回复！";
        public virtual string IpAddresError => "Ip地址输入异常，格式不正确";
        public virtual string Send => "发送";
        public virtual string Receive => "接收";

        /***********************************************************************************
         * 
         *    系统相关的错误信息
         * 
         ************************************************************************************/

        public virtual string SystemInstallOperater => "安装新系统：IP为";
        public virtual string SystemUpdateOperater => "更新新系统：IP为";


        /***********************************************************************************
         * 
         *    套接字相关的信息描述
         * 
         ************************************************************************************/

        public virtual string SocketIOException => "套接字传送数据异常：";
        public virtual string SocketSendException => "同步数据发送异常：";
        public virtual string SocketHeadReceiveException => "指令头接收异常：";
        public virtual string SocketContentReceiveException => "内容数据接收异常：";
        public virtual string SocketContentRemoteReceiveException => "对方内容数据接收异常：";
        public virtual string SocketAcceptCallbackException => "异步接受传入的连接尝试";
        public virtual string SocketReAcceptCallbackException => "重新异步接受传入的连接尝试";
        public virtual string SocketSendAsyncException => "异步数据发送出错:";
        public virtual string SocketEndSendException => "异步数据结束挂起发送出错";
        public virtual string SocketReceiveException => "异步数据发送出错:";
        public virtual string SocketEndReceiveException => "异步数据结束接收指令头出错";
        public virtual string SocketRemoteCloseException => "远程主机强迫关闭了一个现有的连接";


        /***********************************************************************************
         * 
         *    文件相关的信息
         * 
         ************************************************************************************/


        public virtual string FileDownloadSuccess => "文件下载成功";
        public virtual string FileDownloadFailed => "文件下载异常";
        public virtual string FileUploadFailed => "文件上传异常";
        public virtual string FileUploadSuccess => "文件上传成功";
        public virtual string FileDeleteFailed => "文件删除异常";
        public virtual string FileDeleteSuccess => "文件删除成功";
        public virtual string FileReceiveFailed => "确认文件接收异常";
        public virtual string FileNotExist => "文件不存在";
        public virtual string FileSaveFailed => "文件存储失败";
        public virtual string FileLoadFailed => "文件加载失败";
        public virtual string FileSendClientFailed => "文件发送的时候发生了异常";
        public virtual string FileWriteToNetFailed => "文件写入网络异常";
        public virtual string FileReadFromNetFailed => "从网络读取文件异常";
        public virtual string FilePathCreateFailed => "文件夹路径创建失败：";
        public virtual string FileRemoteNotExist => "对方文件不存在，无法接收！";

        /***********************************************************************************
         * 
         *    服务器的引擎相关数据
         * 
         ************************************************************************************/

        public virtual string TokenCheckFailed => "接收验证令牌不一致";
        public virtual string TokenCheckTimeout => "接收验证超时:";
        public virtual string CommandHeadCodeCheckFailed => "命令头校验失败";
        public virtual string CommandLengthCheckFailed => "命令长度检查失败";
        public virtual string NetClientAliasFailed => "客户端的别名接收失败：";
        public virtual string NetEngineStart => "启动引擎";
        public virtual string NetEngineClose => "关闭引擎";
        public virtual string NetClientOnline => "上线";
        public virtual string NetClientOffline => "下线";
        public virtual string NetClientBreak => "异常掉线";
        public virtual string NetClientFull => "服务器承载上限，收到超出的请求连接。";
        public virtual string NetClientLoginFailed => "客户端登录中错误：";
        public virtual string NetHeartCheckFailed => "心跳验证异常：";
        public virtual string NetHeartCheckTimeout => "心跳验证超时，强制下线：";
        public virtual string DataSourseFormatError => "数据源格式不正确";
        public virtual string ServerFileCheckFailed => "服务器确认文件失败，请重新上传";
        public virtual string ClientOnlineInfo => "客户端 [ {0} ] 上线";
        public virtual string ClientOfflineInfo => "客户端 [ {0} ] 下线";
        public virtual string ClientDisableLogin => "客户端 [ {0} ] 不被信任，禁止登录";

        /***********************************************************************************
         * 
         *    Client 相关
         * 
         ************************************************************************************/

        public virtual string ReConnectServerSuccess => "重连服务器成功";
        public virtual string ReConnectServerAfterTenSeconds => "在10秒后重新连接服务器";
        public virtual string KeyIsNotAllowedNull => "关键字不允许为空";
        public virtual string KeyIsExistAlready => "当前的关键字已经存在";
        public virtual string KeyIsNotExist => "当前订阅的关键字不存在";
        public virtual string ConnectingServer => "正在连接服务器...";
        public virtual string ConnectFailedAndWait => "连接断开，等待{0}秒后重新连接";
        public virtual string AttemptConnectServer => "正在尝试第{0}次连接服务器";
        public virtual string ConnectServerSuccess => "连接服务器成功";
        public virtual string GetClientIpaddressFailed => "客户端IP地址获取失败";
        public virtual string ConnectionIsNotAvailable => "当前的连接不可用";
        public virtual string DeviceCurrentIsLoginRepeat => "当前设备的id重复登录";
        public virtual string DeviceCurrentIsLoginForbidden => "当前设备的id禁止登录";
        public virtual string PasswordCheckFailed => "密码验证失败";
        public virtual string DataTransformError => "数据转换失败，源数据：";
        public virtual string RemoteClosedConnection => "远程关闭了连接";

        /***********************************************************************************
         * 
         *    日志 相关
         * 
         ************************************************************************************/
        public virtual string LogNetDebug => "调试";
        public virtual string LogNetInfo => "信息";
        public virtual string LogNetWarn => "警告";
        public virtual string LogNetError => "错误";
        public virtual string LogNetFatal => "致命";
        public virtual string LogNetAbandon => "放弃";
        public virtual string LogNetAll => "全部";

        /***********************************************************************************
         * 
         *    Modbus相关
         * 
         ************************************************************************************/

        public virtual string ModbusTcpFunctionCodeNotSupport => "不支持的功能码";
        public virtual string ModbusTcpFunctionCodeOverBound => "读取的数据越界";
        public virtual string ModbusTcpFunctionCodeQuantityOver => "读取长度超过最大值";
        public virtual string ModbusTcpFunctionCodeReadWriteException => "读写异常";
        public virtual string ModbusTcpReadCoilException => "读取线圈异常";
        public virtual string ModbusTcpWriteCoilException => "写入线圈异常";
        public virtual string ModbusTcpReadRegisterException => "读取寄存器异常";
        public virtual string ModbusTcpWriteRegisterException => "写入寄存器异常";
        public virtual string ModbusAddressMustMoreThanOne => "地址值在起始地址为1的情况下，必须大于1";
        public virtual string ModbusAsciiFormatCheckFailed => "Modbus的ascii指令检查失败，不是modbus-ascii报文";
        public virtual string ModbusCRCCheckFailed => "Modbus的CRC校验检查失败";
        public virtual string ModbusLRCCheckFailed => "Modbus的LRC校验检查失败";
        public virtual string ModbusMatchFailed => "不是标准的modbus协议";


        /***********************************************************************************
         * 
         *    Melsec PLC 相关
         * 
         ************************************************************************************/
        public virtual string MelsecPleaseReferToManulDocument => "请查看三菱的通讯手册来查看报警的具体信息";
        public virtual string MelsecReadBitInfo => "读取位变量数组只能针对位软元件，如果读取字软元件，请调用Read方法";
        public virtual string MelsecCurrentTypeNotSupportedWordOperate => "当前的类型不支持字读写";
        public virtual string MelsecCurrentTypeNotSupportedBitOperate => "当前的类型不支持位读写";
        public virtual string MelsecFxReceiveZore => "接收的数据长度为0";
        public virtual string MelsecFxAckNagative => "PLC反馈的数据无效";
        public virtual string MelsecFxAckWrong => "PLC反馈信号错误：";
        public virtual string MelsecFxCrcCheckFailed => "PLC反馈报文的和校验失败！";

        /***********************************************************************************
         * 
         *    Siemens PLC 相关
         * 
         ************************************************************************************/

        public virtual string SiemensDBAddressNotAllowedLargerThan255 => "DB块数据无法大于255";
        public virtual string SiemensReadLengthMustBeEvenNumber => "读取的数据长度必须为偶数";
        public virtual string SiemensWriteError => "写入数据异常，代号为：";
        public virtual string SiemensReadLengthCannotLargerThan19 => "读取的数组数量不允许大于19";
        public virtual string SiemensDataLengthCheckFailed => "数据块长度校验失败，请检查是否开启put/get以及关闭db块优化";
        public virtual string SiemensFWError => "发生了异常，具体信息查找Fetch/Write协议文档";

        /***********************************************************************************
         * 
         *    Omron PLC 相关
         * 
         ************************************************************************************/

        public virtual string OmronAddressMustBeZeroToFiveteen => "输入的位地址只能在0-15之间";
        public virtual string OmronReceiveDataError => "数据接收异常";
        public virtual string OmronStatus0 => "通讯正常";
        public virtual string OmronStatus1 => "消息头不是FINS";
        public virtual string OmronStatus2 => "数据长度太长";
        public virtual string OmronStatus3 => "该命令不支持";
        public virtual string OmronStatus20 => "超过连接上限";
        public virtual string OmronStatus21 => "指定的节点已经处于连接中";
        public virtual string OmronStatus22 => "尝试去连接一个受保护的网络节点，该节点还未配置到PLC中";
        public virtual string OmronStatus23 => "当前客户端的网络节点超过正常范围";
        public virtual string OmronStatus24 => "当前客户端的网络节点已经被使用";
        public virtual string OmronStatus25 => "所有的网络节点已经被使用";



        /***********************************************************************************
         * 
         *    AB PLC 相关
         * 
         ************************************************************************************/


        public virtual string AllenBradley04 => "它没有正确生成或匹配标记不存在。";
        public virtual string AllenBradley05 => "引用的特定项（通常是实例）无法找到。";
        public virtual string AllenBradley06 => "请求的数据量不适合响应缓冲区。 发生了部分数据传输。";
        public virtual string AllenBradley0A => "尝试处理其中一个属性时发生错误。";
        public virtual string AllenBradley13 => "命令中没有提供足够的命令数据/参数来执行所请求的服务。";
        public virtual string AllenBradley1C => "与属性计数相比，提供的属性数量不足。";
        public virtual string AllenBradley1E => "此服务中的服务请求出错。";
        public virtual string AllenBradley26 => "IOI字长与处理的IOI数量不匹配。";

        public virtual string AllenBradleySessionStatus00 => "成功";
        public virtual string AllenBradleySessionStatus01 => "发件人发出无效或不受支持的封装命令。";
        public virtual string AllenBradleySessionStatus02 => "接收器中的内存资源不足以处理命令。 这不是一个应用程序错误。 相反，只有在封装层无法获得所需内存资源的情况下才会导致此问题。";
        public virtual string AllenBradleySessionStatus03 => "封装消息的数据部分中的数据形成不良或不正确。";
        public virtual string AllenBradleySessionStatus64 => "向目标发送封装消息时，始发者使用了无效的会话句柄。";
        public virtual string AllenBradleySessionStatus65 => "目标收到一个无效长度的信息。";
        public virtual string AllenBradleySessionStatus69 => "不支持的封装协议修订。";

        /***********************************************************************************
         * 
         *    Panasonic PLC 相关
         * 
         ************************************************************************************/
        public virtual string PanasonicReceiveLengthMustLargerThan9 => "接收数据长度必须大于9";
        public virtual string PanasonicAddressParameterCannotBeNull => "地址参数不允许为空";
        public virtual string PanasonicMewStatus20 => "错误未知";
        public virtual string PanasonicMewStatus21 => "NACK错误，远程单元无法被正确识别，或者发生了数据错误。";
        public virtual string PanasonicMewStatus22 => "WACK 错误:用于远程单元的接收缓冲区已满。";
        public virtual string PanasonicMewStatus23 => "多重端口错误:远程单元编号(01 至 16)设置与本地单元重复。";
        public virtual string PanasonicMewStatus24 => "传输格式错误:试图发送不符合传输格式的数据，或者某一帧数据溢出或发生了数据错误。";
        public virtual string PanasonicMewStatus25 => "硬件错误:传输系统硬件停止操作。";
        public virtual string PanasonicMewStatus26 => "单元号错误:远程单元的编号设置超出 01 至 63 的范围。";
        public virtual string PanasonicMewStatus27 => "不支持错误:接收方数据帧溢出. 试图在不同的模块之间发送不同帧长度的数据。";
        public virtual string PanasonicMewStatus28 => "无应答错误:远程单元不存在. (超时)。";
        public virtual string PanasonicMewStatus29 => "缓冲区关闭错误:试图发送或接收处于关闭状态的缓冲区。";
        public virtual string PanasonicMewStatus30 => "超时错误:持续处于传输禁止状态。";
        public virtual string PanasonicMewStatus40 => "BCC 错误:在指令数据中发生传输错误。";
        public virtual string PanasonicMewStatus41 => "格式错误:所发送的指令信息不符合传输格式。";
        public virtual string PanasonicMewStatus42 => "不支持错误:发送了一个未被支持的指令。向未被支持的目标站发送了指令。";
        public virtual string PanasonicMewStatus43 => "处理步骤错误:在处于传输请求信息挂起时,发送了其他指令。";
        public virtual string PanasonicMewStatus50 => "链接设置错误:设置了实际不存在的链接编号。";
        public virtual string PanasonicMewStatus51 => "同时操作错误:当向其他单元发出指令时,本地单元的传输缓冲区已满。";
        public virtual string PanasonicMewStatus52 => "传输禁止错误:无法向其他单元传输。";
        public virtual string PanasonicMewStatus53 => "忙错误:在接收到指令时,正在处理其他指令。";
        public virtual string PanasonicMewStatus60 => "参数错误:在指令中包含有无法使用的代码,或者代码没有附带区域指定参数(X, Y, D), 等以外。";
        public virtual string PanasonicMewStatus61 => "数据错误:触点编号,区域编号,数据代码格式(BCD,hex,等)上溢出, 下溢出以及区域指定错误。";
        public virtual string PanasonicMewStatus62 => "寄存器错误:过多记录数据在未记录状态下的操作（监控记录、跟踪记录等。)。";
        public virtual string PanasonicMewStatus63 => "PLC 模式错误:当一条指令发出时，运行模式不能够对指令进行处理。";
        public virtual string PanasonicMewStatus65 => "保护错误:在存储保护状态下执行写操作到程序区域或系统寄存器。";
        public virtual string PanasonicMewStatus66 => "地址错误:地址（程序地址、绝对地址等）数据编码形式（BCD、hex 等）、上溢、下溢或指定范围错误。";
        public virtual string PanasonicMewStatus67 => "丢失数据错误:要读的数据不存在。（读取没有写入注释寄存区的数据。。";


#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }

    /// <summary>
    /// English Version Text
    /// </summary>
    public class English : DefaultLanguage
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /***********************************************************************************
         * 
         *    Normal Info
         * 
         ************************************************************************************/

        public override string ConnectedFailed => "Connected Failed: ";
        public override string ConnectedSuccess => "Connect Success !";
        public override string UnknownError => "Unknown Error";
        public override string ErrorCode => "Error Code: ";
        public override string TextDescription => "Description: ";
        public override string ExceptionMessage => "Exception Info: ";
        public override string ExceptionSourse => "Exception Sourse：";
        public override string ExceptionType => "Exception Type：";
        public override string ExceptionStackTrace => "Exception Stack: ";
        public override string ExceptopnTargetSite => "Exception Method: ";
        public override string ExceprionCustomer => "Error in user-defined method: ";
        public override string SuccessText => "Success";
        public override string TwoParametersLengthIsNotSame => "Two Parameter Length is not same";
        public override string NotSupportedDataType => "Unsupported DataType, input again";
        public override string NotSupportedFunction => "The current feature logic does not support";
        public override string DataLengthIsNotEnough => "Receive length is not enough，Should:{0},Actual:{1}";
        public override string ReceiveDataTimeout => "Receive timeout: ";
        public override string ReceiveDataLengthTooShort => "Receive length is too short: ";
        public override string MessageTip => "Message prompt:";
        public override string Close => "Close";
        public override string Time => "Time:";
        public override string SoftWare => "Software:";
        public override string BugSubmit => "Bug submit";
        public override string MailServerCenter => "Mail Center System";
        public override string MailSendTail => "Mail Service system issued automatically, do not reply";
        public override string IpAddresError => "IP address input exception, format is incorrect";
        public override string Send => "Send";
        public override string Receive => "Receive";

        /***********************************************************************************
         * 
         *    System about
         * 
         ************************************************************************************/

        public override string SystemInstallOperater => "Install new software: ip address is";
        public override string SystemUpdateOperater => "Update software: ip address is";


        /***********************************************************************************
         * 
         *    Socket-related Information description
         * 
         ************************************************************************************/

        public override string SocketIOException => "Socket transport error: ";
        public override string SocketSendException => "Synchronous Data Send exception: ";
        public override string SocketHeadReceiveException => "Command header receive exception: ";
        public override string SocketContentReceiveException => "Content Data Receive exception: ";
        public override string SocketContentRemoteReceiveException => "Recipient content Data Receive exception: ";
        public override string SocketAcceptCallbackException => "Asynchronously accepts an incoming connection attempt: ";
        public override string SocketReAcceptCallbackException => "To re-accept incoming connection attempts asynchronously";
        public override string SocketSendAsyncException => "Asynchronous Data send Error: ";
        public override string SocketEndSendException => "Asynchronous data end callback send Error";
        public override string SocketReceiveException => "Asynchronous Data send Error: ";
        public override string SocketEndReceiveException => "Asynchronous data end receive instruction header error";
        public override string SocketRemoteCloseException => "An existing connection was forcibly closed by the remote host";


        /***********************************************************************************
         * 
         *    File related information
         * 
         ************************************************************************************/


        public override string FileDownloadSuccess => "File Download Successful";
        public override string FileDownloadFailed => "File Download exception";
        public override string FileUploadFailed => "File Upload exception";
        public override string FileUploadSuccess => "File Upload Successful";
        public override string FileDeleteFailed => "File Delete exception";
        public override string FileDeleteSuccess => "File deletion succeeded";
        public override string FileReceiveFailed => "Confirm File Receive exception";
        public override string FileNotExist => "File does not exist";
        public override string FileSaveFailed => "File Store failed";
        public override string FileLoadFailed => "File load failed";
        public override string FileSendClientFailed => "An exception occurred when the file was sent";
        public override string FileWriteToNetFailed => "File Write Network exception";
        public override string FileReadFromNetFailed => "Read file exceptions from the network";
        public override string FilePathCreateFailed => "Folder path creation failed: ";
        public override string FileRemoteNotExist => "The other file does not exist, cannot receive!";

        /***********************************************************************************
         * 
         *    Engine-related data for the server
         * 
         ************************************************************************************/

        public override string TokenCheckFailed => "Receive authentication token inconsistency";
        public override string TokenCheckTimeout => "Receive authentication timeout: ";
        public override string CommandHeadCodeCheckFailed => "Command header check failed";
        public override string CommandLengthCheckFailed => "Command length check failed";
        public override string NetClientAliasFailed => "Client's alias receive failed: ";
        public override string NetEngineStart => "Start engine";
        public override string NetEngineClose => "Shutting down the engine";
        public override string NetClientOnline => "Online";
        public override string NetClientOffline => "Offline";
        public override string NetClientBreak => "Abnormal offline";
        public override string NetClientFull => "The server hosts the upper limit and receives an exceeded request connection.";
        public override string NetClientLoginFailed => "Error in Client logon: ";
        public override string NetHeartCheckFailed => "Heartbeat Validation exception: ";
        public override string NetHeartCheckTimeout => "Heartbeat verification timeout, force offline: ";
        public override string DataSourseFormatError => "Data source format is incorrect";
        public override string ServerFileCheckFailed => "Server confirmed file failed, please re-upload";
        public override string ClientOnlineInfo => "Client [ {0} ] Online";
        public override string ClientOfflineInfo => "Client [ {0} ] Offline";
        public override string ClientDisableLogin => "Client [ {0} ] is not trusted, login forbidden";

        /***********************************************************************************
         * 
         *    Client related
         * 
         ************************************************************************************/

        public override string ReConnectServerSuccess => "Re-connect server succeeded";
        public override string ReConnectServerAfterTenSeconds => "Reconnect the server after 10 seconds";
        public override string KeyIsNotAllowedNull => "The keyword is not allowed to be empty";
        public override string KeyIsExistAlready => "The current keyword already exists";
        public override string KeyIsNotExist => "The keyword for the current subscription does not exist";
        public override string ConnectingServer => "Connecting to Server...";
        public override string ConnectFailedAndWait => "Connection disconnected, wait {0} seconds to reconnect";
        public override string AttemptConnectServer => "Attempting to connect server {0} times";
        public override string ConnectServerSuccess => "Connection Server succeeded";
        public override string GetClientIpaddressFailed => "Client IP Address acquisition failed";
        public override string ConnectionIsNotAvailable => "The current connection is not available";
        public override string DeviceCurrentIsLoginRepeat => "ID of the current device duplicate login";
        public override string DeviceCurrentIsLoginForbidden => "The ID of the current device prohibits login";
        public override string PasswordCheckFailed => "Password validation failed";
        public override string DataTransformError => "Data conversion failed, source data: ";
        public override string RemoteClosedConnection => "Remote shutdown of connection";

        /***********************************************************************************
         * 
         *    Log related
         * 
         ************************************************************************************/
        public override string LogNetDebug => "Debug";
        public override string LogNetInfo => "Info";
        public override string LogNetWarn => "Warn";
        public override string LogNetError => "Error";
        public override string LogNetFatal => "Fatal";
        public override string LogNetAbandon => "Abandon";
        public override string LogNetAll => "All";


        /***********************************************************************************
         * 
         *    Modbus related
         * 
         ************************************************************************************/

        public override string ModbusTcpFunctionCodeNotSupport => "Unsupported function code";
        public override string ModbusTcpFunctionCodeOverBound => "Data read out of bounds";
        public override string ModbusTcpFunctionCodeQuantityOver => "Read length exceeds maximum value";
        public override string ModbusTcpFunctionCodeReadWriteException => "Read and Write exceptions";
        public override string ModbusTcpReadCoilException => "Read Coil anomalies";
        public override string ModbusTcpWriteCoilException => "Write Coil exception";
        public override string ModbusTcpReadRegisterException => "Read Register exception";
        public override string ModbusTcpWriteRegisterException => "Write Register exception";
        public override string ModbusAddressMustMoreThanOne => "The address value must be greater than 1 in the case where the start address is 1";
        public override string ModbusAsciiFormatCheckFailed => "Modbus ASCII command check failed, not MODBUS-ASCII message";
        public override string ModbusCRCCheckFailed => "The CRC checksum check failed for Modbus";
        public override string ModbusLRCCheckFailed => "The LRC checksum check failed for Modbus";
        public override string ModbusMatchFailed => "Not the standard Modbus protocol";


        /***********************************************************************************
         * 
         *    Melsec PLC related
         * 
         ************************************************************************************/
        public override string MelsecPleaseReferToManulDocument => "Please check Mitsubishi's communication manual for details of the alarm.";
        public override string MelsecReadBitInfo => "The read bit variable array can only be used for bit soft elements, if you read the word soft component, call the Read method";
        public override string MelsecCurrentTypeNotSupportedWordOperate => "The current type does not support word read and write";
        public override string MelsecCurrentTypeNotSupportedBitOperate => "The current type does not support bit read and write";
        public override string MelsecFxReceiveZore => "The received data length is 0";
        public override string MelsecFxAckNagative => "Invalid data from PLC feedback";
        public override string MelsecFxAckWrong => "PLC Feedback Signal Error: ";
        public override string MelsecFxCrcCheckFailed => "PLC Feedback message and check failed!";

        /***********************************************************************************
         * 
         *    Siemens PLC related
         * 
         ************************************************************************************/

        public override string SiemensDBAddressNotAllowedLargerThan255 => "DB block data cannot be greater than 255";
        public override string SiemensReadLengthMustBeEvenNumber => "The length of the data read must be an even number";
        public override string SiemensWriteError => "Writes the data exception, the code name is: ";
        public override string SiemensReadLengthCannotLargerThan19 => "The number of arrays read does not allow greater than 19";
        public override string SiemensDataLengthCheckFailed => "Block length checksum failed, please check if Put/get is turned on and DB block optimization is turned off";
        public override string SiemensFWError => "An exception occurred, the specific information to find the Fetch/write protocol document";

        /***********************************************************************************
         * 
         *    Omron PLC related
         * 
         ************************************************************************************/

        public override string OmronAddressMustBeZeroToFiveteen => "The bit address entered can only be between 0-15";
        public override string OmronReceiveDataError => "Data Receive exception";
        public override string OmronStatus0 => "Communication is normal.";
        public override string OmronStatus1 => "The message header is not fins";
        public override string OmronStatus2 => "Data length too long";
        public override string OmronStatus3 => "This command does not support";
        public override string OmronStatus20 => "Exceeding connection limit";
        public override string OmronStatus21 => "The specified node is already in the connection";
        public override string OmronStatus22 => "Attempt to connect to a protected network node that is not yet configured in the PLC";
        public override string OmronStatus23 => "The current client's network node exceeds the normal range";
        public override string OmronStatus24 => "The current client's network node is already in use";
        public override string OmronStatus25 => "All network nodes are already in use";



        /***********************************************************************************
         * 
         *    AB PLC related
         * 
         ************************************************************************************/


        public override string AllenBradley04 => "The IOI could not be deciphered. Either it was not formed correctly or the match tag does not exist.";
        public override string AllenBradley05 => "The particular item referenced (usually instance) could not be found.";
        public override string AllenBradley06 => "The amount of data requested would not fit into the response buffer. Partial data transfer has occurred.";
        public override string AllenBradley0A => "An error has occurred trying to process one of the attributes.";
        public override string AllenBradley13 => "Not enough command data / parameters were supplied in the command to execute the service requested.";
        public override string AllenBradley1C => "An insufficient number of attributes were provided compared to the attribute count.";
        public override string AllenBradley1E => "A service request in this service went wrong.";
        public override string AllenBradley26 => "The IOI word length did not match the amount of IOI which was processed.";

        public override string AllenBradleySessionStatus00 => "success";
        public override string AllenBradleySessionStatus01 => "The sender issued an invalid or unsupported encapsulation command.";
        public override string AllenBradleySessionStatus02 => "Insufficient memory resources in the receiver to handle the command. This is not an application error. Instead, it only results if the encapsulation layer cannot obtain memory resources that it need.";
        public override string AllenBradleySessionStatus03 => "Poorly formed or incorrect data in the data portion of the encapsulation message.";
        public override string AllenBradleySessionStatus64 => "An originator used an invalid session handle when sending an encapsulation message.";
        public override string AllenBradleySessionStatus65 => "The target received a message of invalid length.";
        public override string AllenBradleySessionStatus69 => "Unsupported encapsulation protocol revision.";

        /***********************************************************************************
         * 
         *    Panasonic PLC related
         * 
         ************************************************************************************/
        public override string PanasonicReceiveLengthMustLargerThan9 => "The received data length must be greater than 9";
        public override string PanasonicAddressParameterCannotBeNull => "Address parameter is not allowed to be empty";
        public override string PanasonicMewStatus20 => "Error unknown";
        public override string PanasonicMewStatus21 => "Nack error, the remote unit could not be correctly identified, or a data error occurred.";
        public override string PanasonicMewStatus22 => "WACK Error: The receive buffer for the remote unit is full.";
        public override string PanasonicMewStatus23 => "Multiple port error: The remote unit number (01 to 16) is set to repeat with the local unit.";
        public override string PanasonicMewStatus24 => "Transport format error: An attempt was made to send data that does not conform to the transport format, or a frame data overflow or a data error occurred.";
        public override string PanasonicMewStatus25 => "Hardware error: Transport system hardware stopped operation.";
        public override string PanasonicMewStatus26 => "Unit Number error: The remote unit's numbering setting exceeds the range of 01 to 63.";
        public override string PanasonicMewStatus27 => "Error not supported: Receiver data frame overflow. An attempt was made to send data of different frame lengths between different modules.";
        public override string PanasonicMewStatus28 => "No answer error: The remote unit does not exist. (timeout).";
        public override string PanasonicMewStatus29 => "Buffer Close error: An attempt was made to send or receive a buffer that is in a closed state.";
        public override string PanasonicMewStatus30 => "Timeout error: Persisted in transport forbidden State.";
        public override string PanasonicMewStatus40 => "BCC Error: A transmission error occurred in the instruction data.";
        public override string PanasonicMewStatus41 => "Malformed: The sent instruction information does not conform to the transmission format.";
        public override string PanasonicMewStatus42 => "Error not supported: An unsupported instruction was sent. An instruction was sent to a target station that was not supported.";
        public override string PanasonicMewStatus43 => "Processing Step Error: Additional instructions were sent when the transfer request information was suspended.";
        public override string PanasonicMewStatus50 => "Link Settings Error: A link number that does not actually exist is set.";
        public override string PanasonicMewStatus51 => "Simultaneous operation error: When issuing instructions to other units, the transmit buffer for the local unit is full.";
        public override string PanasonicMewStatus52 => "Transport suppression Error: Unable to transfer to other units.";
        public override string PanasonicMewStatus53 => "Busy error: Other instructions are being processed when the command is received.";
        public override string PanasonicMewStatus60 => "Parameter error: Contains code that cannot be used in the directive, or the code does not have a zone specified parameter (X, Y, D), and so on.";
        public override string PanasonicMewStatus61 => "Data error: Contact number, area number, Data code format (BCD,HEX, etc.) overflow, overflow, and area specified error.";
        public override string PanasonicMewStatus62 => "Register ERROR: Excessive logging of data in an unregistered state of operations (Monitoring records, tracking records, etc.). )。";
        public override string PanasonicMewStatus63 => "PLC mode error: When an instruction is issued, the run mode is not able to process the instruction.";
        public override string PanasonicMewStatus65 => "Protection Error: Performs a write operation to the program area or system register in the storage protection state.";
        public override string PanasonicMewStatus66 => "Address Error: Address (program address, absolute address, etc.) Data encoding form (BCD, hex, etc.), overflow, underflow, or specified range error.";
        public override string PanasonicMewStatus67 => "Missing data error: The data to be read does not exist. (reads data that is not written to the comment register.)";

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

    }
}
