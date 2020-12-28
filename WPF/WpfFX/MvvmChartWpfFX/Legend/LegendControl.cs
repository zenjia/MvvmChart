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

    [TemplatePart(Name = "PART_ItemsControl", Type = typeof(SlimItemsControl))]
    public class LegendControl : Control
    {
        static LegendControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendControl), new FrameworkPropertyMetadata(typeof(LegendControl)));
        }

        public event Action<LegendItemControl, bool> LegendItemHighlighChanged; 

        private SlimItemsControl PART_ItemsControl;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_ItemsControl = (SlimItemsControl)this.GetTemplateChild("PART_ItemsControl");
            if (this.PART_ItemsControl!=null)
            {
                this.PART_ItemsControl.ElementGenerated += PART_ItemsControl_ElementGenerated;
            }
        }

        private void PART_ItemsControl_ElementGenerated(object arg1, FrameworkElement treeRoot)
        {
            var legendItemControl = treeRoot as LegendItemControl;
            if (legendItemControl != null)
            {
                legendItemControl.PropertyChanged += LegendItemControl_PropertyChanged;
            }
        }

        private void LegendItemControl_PropertyChanged(object sender, string propName)
        {
            if (propName == "IsHighlighted")
            {
                var legendItem = (LegendItemControl)sender;
     
                this.LegendItemHighlighChanged?.Invoke(legendItem, legendItem.IsHighlighted);
            }
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(LegendControl), new PropertyMetadata(null));


 

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
            var legendItem = PART_ItemsControl?.TryGetElementForItem(item) as LegendItemControl;

 
            if (legendItem != null)
            {
                legendItem.IsHighlighted = newValue;
            }


        }
    }
}
