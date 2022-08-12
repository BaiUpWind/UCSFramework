using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    public class TitleViewModel : ViewModelBase
    {



        private string titleName;
        private double startX;
        private double startY;
        private double endX;
        private double endY;

        public TitleViewModel( )
        {
            
        }

        public string TitleName
        {
            get => titleName; set
            {
                titleName = value;
                RaisePropertyChanged();
            }
        }

        public double StartX
        {
            get => startX;
            set
            {
                startX = value; 
                RaisePropertyChanged();
            }

        }
        public double StartY
        {
            get => startY; set
            {
                startY = value; 
                RaisePropertyChanged();
            }
        }
        public double EndX
        {
            get => endX; set
            {
                endX = value; 
                RaisePropertyChanged();
            }
        }
        public double EndY
        {
            get => endY; set
            {
                endY = value; 
                RaisePropertyChanged();
            }
        }
    }
}
