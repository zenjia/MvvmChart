using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MvvmCharting
{
    internal class ItemsControlEx : ItemsControl
    {
        public event Action<object, DependencyObject> ItemTemplateApplied;
 

        protected override DependencyObject GetContainerForItemOverride()
        {
            var container = new NotifyTemplateAppliedPresenter();
            container.ItemContainerTemplateApplied += ItemContainerTemplateApplied;
            return container;
        }

        private void ItemContainerTemplateApplied(object arg1, DependencyObject root)
        {
            //Debug.WriteLine(this.Name + $"  ItemsControlEx: arg1={arg1},  root={root} !!!");
            this.ItemTemplateApplied?.Invoke(this, root);
        }
    }
}