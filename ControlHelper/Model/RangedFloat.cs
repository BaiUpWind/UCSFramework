using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Model
{
    /// <summary>
    /// 设定一个浮点数的区间
    /// </summary>
    [System.Serializable]
    public class RangedFloat
    {
        public float minValue;
        public float maxValue;
        public RangedFloat()
        {
            this.minValue = 0;
            this.maxValue = 0;
        }
        public RangedFloat(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary> 判断进入的值是否在区间内 </summary>
        public bool IsInRange(float value) => value >= minValue && value <= maxValue;
    }
}
