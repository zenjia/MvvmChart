using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MvvmCharting
{
    /// <summary>
    /// A ContentPresenter that can notify when its Template is applied and expose
    /// its template child(the root child of it).
    /// This will be used as the ItemContainer for <see cref="ItemsControlEx"/>.
    /// </summary>
    internal class NotifyTemplateAppliedPresenter : ContentPresenter
    {
        internal event Action<object, DependencyObject> ItemContainerTemplateApplied;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DependencyObject root = VisualTreeHelper.GetChild(this, 0);
            this.ItemContainerTemplateApplied?.Invoke(this, root);
        }
    }

}
