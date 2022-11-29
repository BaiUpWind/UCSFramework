using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class BeltEditor
    {
        public int Belt_id { get; set; }
        public string Work_id { get; set; }
        public int Pos_x { get; set; }
        public int Pos_y { get; set; }
        public int Pos_width { get; set; }
        public int Pos_height { get; set; }
        public int Row_index { get; set; }
        public int X_index { get; set; }
        public int Y_index { get; set; }
        public int Is_rgv { get; set; }
        public int Rgv_db_id { get; set; }
        public int Rgv_sys_id { get; set; }
        public int Rgv_put_isjb { get; set; }
        public int Is_scan { get; set; }
        public int Scan_type { get; set; }
        public string Scan_ip { get; set; }
        public string S_checkscanip1 { get; set; }
        public string S_checkscanip2 { get; set; }
        public string S_name { get; set; }
    }
}
