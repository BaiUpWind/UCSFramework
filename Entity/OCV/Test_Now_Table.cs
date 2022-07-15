using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.OCV
{
    /// <summary>
    /// 测试框架可行性
    /// </summary>
    public class Test_Now_Table
    {
        //        DROP TABLE IF EXISTS `test_now_table`;
        //CREATE TABLE `test_now_table` (
        //  `position` int (11) NOT NULL,
        //  `id` int (2) NOT NULL,
        //  `code` varchar(25) DEFAULT NULL,
        //  `v` varchar(6) DEFAULT NULL,
        //  `r` varchar(6) DEFAULT NULL,
        //  `t` varchar(6) DEFAULT NULL,
        //  `grade` varchar(6) DEFAULT NULL,
        //  `ttime` datetime DEFAULT NULL,
        //  `tray_code` varchar(10) DEFAULT NULL,
        //  `test_type` varchar(10) DEFAULT NULL,
        //  `r_bc` varchar(10) DEFAULT '0',
        //  `ngcount` varchar(2) DEFAULT NULL,
        //  PRIMARY KEY(`position`,`id`)
        //) ENGINE=InnoDB DEFAULT CHARSET=utf8;

        public string POSITION { get; set; }

        /// <summary>
        /// 1-36 一共两层
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 对应的编码
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// 电池电压V
        /// </summary>
        public string V { get; set; }


        /// <summary>
        /// 电池内阻（电流I）
        /// </summary>
        public string R { get; set; }
        /// <summary>
        /// 暂时无用
        /// </summary>
        public string T { get; set; }
        /// <summary>
        /// 测试结果 等级
        /// </summary>
        public string GRADE { get; set; }
        /// <summary>
        /// 测试时间
        /// </summary>
        public string TTIME { get; set; }
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string TRAY_CODE { get; set; }
        /// <summary>
        /// 测试类型 由扫描抢扫码后获取
        /// </summary>
        public string TEST_TYPE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string R_BC { get; set; }
        /// <summary>
        /// NG次数，重新测试次数
        /// </summary>
        public string NGCOUNT { get; set; }
    }
}
