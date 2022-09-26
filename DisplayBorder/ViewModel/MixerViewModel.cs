using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    [Obsolete("不再使用  找个机会删掉")]
    public class MixerViewModel : ViewModelBase
    {

        public MixerViewModel()
        {

        }
        public MixerViewModel(int id ,string name ,string status)
        {
            this .id = id;
            this.name = name;
            this.status = status;
        }

        private int id;
        private string name;
        private string status;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Status { get => status; set => status = value; }
    }
}
