using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class BeltReadPlc
    {
        //`work_id`  varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL ,
        //`plc_status`  int (3) NULL DEFAULT NULL ,
        //`plc_have`  int (3) NULL DEFAULT NULL ,
        //`plc_next_work_id`  int (5) NULL DEFAULT NULL ,
        //`plc_load_status`  int (3) NULL DEFAULT NULL ,
        //`plc_load_info_id`  int (5) NULL DEFAULT NULL ,
        //`plc_move_req`  int (3) NULL DEFAULT NULL ,
        //`plc_scan_req`  int (3) NULL DEFAULT NULL ,
        //`plc_get_put_req`  int (3) NULL DEFAULT NULL ,
        //`plc_get_put_ans`  int (3) NULL DEFAULT NULL ,

        public string Work_id { get; set; }
        public int Plc_status { get; set; }
        public int Plc_have { get; set; }
        public int Plc_next_work_id { get; set; }
        public int Plc_load_status { get; set; }
        public int Plc_load_info_id { get; set; }
        public int Plc_move_req { get; set; }
        public int Plc_scan_req { get; set; }
        public int Plc_get_put_req { get; set; }
        public int Plc_get_put_ans { get; set; }
    }
}
