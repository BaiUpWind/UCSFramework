using System;

namespace CommonApi
{
    public abstract class BaseEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 获取类型编号。
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// 清理引用。
        /// </summary>
        public abstract void Clear();
    }
}
