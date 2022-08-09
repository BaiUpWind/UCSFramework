using DeviceConfig;
using DeviceConfig.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    public class OperationViewModel : ViewModelBase
    {
        public OperationViewModel(OperationBase operation,  DeviceInfo info )
        {
            this.operation = operation; 
            this.info = info;
        }
        private OperationBase operation;
        private ConnectionConfigBase conn = null;
        private CommandBase command;
        private DeviceInfo info; 
        public OperationBase Operation
        {
            get => operation; set
            {
                operation = value;
                RaisePropertyChanged(nameof(OpertaionName));
            }

        }
        public CommandBase Command
        {
            get => command; set
            {
                command = value;
                RaisePropertyChanged(nameof(CommandName));
            }

        }
        public ConnectionConfigBase Conn
        {
            get => conn; set
            {
                conn = value;
                RaisePropertyChanged(nameof(ConnName));
            }
        }

        public int RefreshInterval
        {
            get => info.RefreshInterval; set
            {
                info.RefreshInterval = value; 
                RaisePropertyChanged();
            }
        }

        public string ConnName => Conn == null ? "点击创建" : Conn.GetType().Name;
        public string OpertaionName => Operation == null ? "点击创建" : Operation.GetType().Name;
        public string CommandName => Command == null ? "点击创建" : Command.GetType().Name;


    }
}
