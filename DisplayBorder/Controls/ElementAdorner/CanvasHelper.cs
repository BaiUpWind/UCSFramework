using DisplayBorder.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DisplayBorder
{
    [TemplatePart(Name = "PART_MainGrid", Type = typeof(Grid))]
    public class CanvasHelper : ContentControl
    {

        public CanvasHelper()
        {
            this.Loaded += CanvasHelper_Loaded;
            Unloaded += CanvasHelper_Unloaded;
        }
        public FrameworkElement TargetElement
        {
            get;// { return (FrameworkElement)GetValue(TargetElementProperty); }
            set;//{ SetValue(TargetElementProperty, value); }
        }

        //public static bool GetIsEditable(DependencyObject obj)
        //{
        //    return (bool)obj.GetValue(IsEditableProperty);
        //}

        //public static void SetIsEditable(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(IsEditableProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsEditableProperty =
        //    DependencyProperty.RegisterAttached("IsEditable", typeof(bool), typeof(CanvasHelper), new PropertyMetadata(false));
        public static bool GetIsSelectable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectableProperty);
        }

        public static void SetIsSelectable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectableProperty, value);
        }
        public static readonly DependencyProperty IsSelectableProperty =
           DependencyProperty.RegisterAttached("IsSelectable", typeof(bool), typeof(CanvasHelper), new PropertyMetadata(false));

        private void CanvasHelper_Loaded(object sender, RoutedEventArgs e)
        {
            AttachParentEvents(); 
            Loaded -= CanvasHelper_Loaded;
        }

        private void CanvasHelper_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachParentEvents();
            Unloaded -= CanvasHelper_Unloaded;
        }

        private void AttachParentEvents()
        {
            Canvas CanvasParent = Parent as Canvas;

            if (CanvasParent == null)
            {
                throw new Exception("CanvasHelper Must place into Canvas!");
            }

            CanvasParent.MouseLeftButtonDown += CanvasParent_MouseLeftButtonDown; ;
        }
        private void DetachParentEvents()
        {
            Canvas CanvasParent = Parent as Canvas;

            if (CanvasParent != null)
            {
                CanvasParent.MouseLeftButtonDown -= CanvasParent_MouseLeftButtonDown;
            }
        }
        private void CanvasParent_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement SelectedElement = e.Source as FrameworkElement;
            if (CheckTargetIsSelectable(SelectedElement))
            {
                GlobalPara.EventManager.Fire(this, OnCanvasChildrenClickArgs.Create(e.Source, e.OriginalSource));
                TargetElement = SelectedElement; 
            }
            else
            {
                GlobalPara.EventManager.Fire(this, OnCanvasChildrenClickArgs.Create(null,null));
                TargetElement = null;
            }
            SelectedElement.Focus();
        }

        private bool CheckTargetIsSelectable(FrameworkElement Target)
        {
            return (Target != null) && !Target.Equals(Parent) && !Target.Equals(this) && GetIsSelectable(Target);
        }
    }

 
}
