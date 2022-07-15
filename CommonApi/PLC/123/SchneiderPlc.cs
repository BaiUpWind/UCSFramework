using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WchCommon.Core.IManger;

namespace WchCommon.PLC
{
    /// <summary>
    /// 施耐德plc连接
    /// modelbus
    /// 
    /// 日期：2020年7月30日
    /// 
    /// </summary>
    public class SchneiderPlc : WchCommon.Core.IManger.IPLCConfig
    {
        public string PLCName { get ; set ; }
        public bool IsConnectioned { get ; set ; }
        public List<IDBRead> NListItem { get ; set ; }
        public Dictionary<string, IDBRead> DirItem { get ; set ; }

        public event Action<string, int> ShowMsg;

        public void AddItemList(string key, List<string> value)
        {
            throw new NotImplementedException();
        }

        public void AddItemList(List<string> list)
        {
            throw new NotImplementedException();
        }

        public bool Connection()
        {
            throw new NotImplementedException();
        }

        public void DisConnection()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDBRead GetDBRead(string key)
        {
            throw new NotImplementedException();
        }

        public IDBRead GetDBRead(int index)
        {
            throw new NotImplementedException();
        }
    }

    public class Schneider : WchCommon.Core.IManger.IDBRead
    {
        public int ListCount { get; }

        public event Action<string, int> ShowMsg;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public object Read(int index)
        {
            throw new NotImplementedException();
        }

        public void Write(object values, int index)
        {
            throw new NotImplementedException();
        }

        public void Write(object[] values)
        {
            throw new NotImplementedException();
        }
    }

}
