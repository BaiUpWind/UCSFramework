namespace CommonApi.PLC
{
    public interface IPLCInterface
    {
        /// <summary>
        /// 连接plc
        /// </summary>
        /// <returns></returns>
        bool Connect();
        /// <summary>
        /// 断开连接
        /// </summary>
        void DisConnect();
        /// <summary>
        /// 读取DB块
        /// </summary>
        /// <param name="dataBlockNum">DB块地址，DB1 ，其中的1 </param>
        /// <param name="startValue">起始位置 DB1.WO  0是起始位置  W122 122是起始位置</param>
        /// <param name="length">W 长度为 2 Dint 长度为 4</param>
        /// <param name="value">返回的结果集</param>
        /// <returns></returns>
        bool Read(int dataBlockNum, int startValue, int length, ref byte[] value);
        /// <summary>
        /// 写入DB块
        /// </summary>
        /// <param name="dataBlockNum"></param>
        /// <param name="startValue"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Write(int dataBlockNum, int startValue, int length, byte[] value);

 
    }
}
