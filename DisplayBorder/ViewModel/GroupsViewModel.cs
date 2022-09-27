using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceConfig;
using DeviceConfig.Core;
using HandyControl.Controls; 
using System.Collections.ObjectModel;
using CommonApi;
using System.IO;

namespace DisplayBorder.ViewModel
{
    [Obsolete("暂时弃用220830")]
    public class GroupsViewModel : ViewModelBase
    {

        public GroupsViewModel()
        {
            addGroup = new RelayCommand(AddGroup);
            saveGroup = new RelayCommand(SaveGroup);


         
            //if (!File.Exists(GlobalPara.GroupsFilePath))
            //{
            //    File.Create(GlobalPara.GroupsFilePath);
            //}
            var result = GlobalPara.Groups;// JsonHelper.ReadJson<Group>(InitializationBase.JsonFilePath);
            if (result != null)
            {
                groups = new ObservableCollection<Group>(result);
            }
        }

        private int groupID = 1001;
        private string groupName = "第1001组";
        private ObservableCollection<Group> groups = new ObservableCollection<Group>();
        private Group currentGroup;
        private Device curretnDvice;

        public RelayCommand addGroup { get; set; }

        public RelayCommand saveGroup { get; set; }

        public int GroupID
        {
            get => groupID; set
            {
                groupID = value;
                RaisePropertyChanged();
            }
        }
        public string GroupName
        {
            get => groupName; set
            {
                groupName = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Group> Groups
        {
            get => groups; set
            {
                groups = value;
                RaisePropertyChanged(() => Groups);
            }
        }

        //----------------------  主界面显示

        public int GroupCount
        {
            get => Groups.Count;
        }

        public int DeviceCount
        {
            get
            {
                if (groups == null)
                {
                    return 0;
                }
                int count = 0;
                foreach (var item in groups)
                {
                    count += item.DeviceConfigs.Count;
                }

                return count;
            }
        }




        //当前组的信息

        public Group CurrentGroup
        {
            get => currentGroup; set
            {
                currentGroup = value;
                RaisePropertyChanged();
            }
        }

        public Device CurretnDevice
        {
            get => curretnDvice; set
            {
                curretnDvice = value;
                RaisePropertyChanged();
            }
        }


        public void AddGroup()
        {
            var result = Groups.Where(a => a.GroupID == GroupID).FirstOrDefault();
            if (result == null)
            {
                Group group = new Group();
                group.GroupID = GroupID;
                group.GroupName = GroupName;
                group.DeviceConfigs = new List<Device>();
                groups.Add(group);
                Growl.Info($"添加'{GroupID}'组成功");
            }
            else
            {
                Growl.Error($"已经包含对应的组'{GroupID}',请勿重复添加");
            }
            GroupID += 1;
            GroupName = $"第{GroupID}组";
        }


        private void SaveGroup()
        {

            GlobalPara.Groups = groups;
            Growl.Success("保存成功!");

        }
    }
}
