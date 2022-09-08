/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DisplayBorder"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;
//using Microsoft.Practices.ServiceLocation;

namespace DisplayBorder.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}


            SimpleIoc.Default.Register<GroupViewModel>(); 
            SimpleIoc.Default.Register<WindowStateViewModel>();  
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static ViewModelLocator Instance = new Lazy<ViewModelLocator>(() =>
    Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;
        public GroupViewModel Group
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GroupViewModel>();
            }
        }
     
 
        public WindowStateViewModel WindowState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WindowStateViewModel>();

            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>(); 
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}