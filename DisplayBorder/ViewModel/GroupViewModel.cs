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
using DisplayBorder.Model;
using System.Collections.ObjectModel;

namespace DisplayBorder.ViewModel
{
    public class GroupViewModel : ViewModelBase
    {
        public GroupViewModel()
        {
            addGroup = new RelayCommand(AddGroup); 
        }

        public RelayCommand addGroup { get; set; } 
  
        private int groupID =1001; 
        private string groupName="第1001组"; 
        private ObservableCollection<Group> groups = new ObservableCollection<Group>()
        {
            //new Group()
            //{
            //     GroupID = 1001,
            //     GroupName="第一组"
            //},
            //new Group()
            //{
            //     GroupID = 1002,
            //     GroupName="第二组"
            //},
        };

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
                RaisePropertyChanged(()=> Groups);
            }
        }
         
        public void AddGroup()
        {
            var result = Groups.Where(a => a.GroupID == GroupID).FirstOrDefault();
            if (result == null)
            {
                //if (string.IsNullOrEmpty(GroupName))
                //{
                //    GroupName = "";
                //}
                Group group = new Group();
                group.GroupID = GroupID;
                group.GroupName = GroupName   ;
                groups.Add(group); 
                Growl.Info($"添加'{GroupID}'组成功");
            }
            else
            {
                Growl.Error($"已经包含对应的组'{GroupID}',请勿重复添加");
            }
            GroupID +=1;
            GroupName = $"第{GroupID}组";
        }

        //public bool RemoveGroup(int groupId)
        //{
        //    var result = Groups.Where(a => a.GroupID == groupId).FirstOrDefault();
        //    if (result != null)
        //    {
              
        //        Groups.Remove(result);
        //        return true;
        //    }
        //    else
        //    { 
        //        return false;

        //    }
        //}
    }
}
