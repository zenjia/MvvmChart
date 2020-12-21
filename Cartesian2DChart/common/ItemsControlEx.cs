using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MvvmCharting
{
    /// <summary>
    /// An ItemsControl which can notify when the ItemTemplate is
    /// Applied by the ItemContainer it generated for each item, so
    /// the owner of the ItemsControl could see the root element in
    /// the ItemTemplate when it is applied.
    /// See <see cref="NotifyTemplateAppliedPresenter"/>
    /// </summary>
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
            this.ItemTemplateApplied?.Invoke(this, root);
        }
    }
}