using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MvvmCharting.WpfFX
{
    public class LegendControl : Control
    {
        static LegendControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendControl), new FrameworkPropertyMetadata(typeof(LegendControl)));
        }

        private ItemsControl PART_ItemsControl;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_ItemsControl = (ItemsControl)this.GetTemplateChild("PART_ItemsControl");
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(LegendControl), new PropertyMetadata(null));


        public ItemsPanelTemplate LegendPanelTemplate
        {
            get { return (ItemsPanelTemplate)GetValue(LegendPanelTemplateProperty); }
            set { SetValue(LegendPanelTemplateProperty, value); }
        }
        public static readonly DependencyProperty LegendPanelTemplateProperty =
            DependencyProperty.Register("LegendPanelTemplate", typeof(ItemsPanelTemplate), typeof(LegendControl), new PropertyMetadata(null));



        public DataTemplate LegendItemTemplate
        {
            get { return (DataTemplate)GetValue(LegendItemTemplateProperty); }
            set { SetValue(LegendItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty LegendItemTemplateProperty =
            DependencyProperty.Register("LegendItemTemplate", typeof(DataTemplate), typeof(LegendControl), new PropertyMetadata(null));



        public DataTemplateSelector LegendItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LegendItemTemplateSelectorProperty); }
            set { SetValue(LegendItemTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty LegendItemTemplateSelectorProperty =
            DependencyProperty.Register("LegendItemTemplateSelector", typeof(DataTemplateSelector), typeof(LegendControl), new PropertyMetadata(null));


        public void OnItemHighlightChanged(object item, bool newValue)
        {
            var container = PART_ItemsControl.ItemContainerGenerator.ContainerFromItem(item);

            var legendItem = container?.GetAllVisualChildren().OfType<LegendItemControl>().FirstOrDefault();

            if (legendItem != null)
            {
                legendItem.IsHighlighted = newValue;
            }


        }
    }
}
