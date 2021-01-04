using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmCharting.WpfFX
{
    /// <summary>
    /// Represents a control that can be highlighted and selected
    /// </summary>
    public class InteractiveControl : Control
    {
        public event Action<InteractiveControl, bool> IsHighlightedChanged;
        public event Action<InteractiveControl, bool> IsSelectedChanged;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            
            base.OnPropertyChanged(e);
            if (e.Property == IsMouseOverProperty)
            {
                this.SetCurrentValue(IsHighlightedProperty, this.IsMouseOver);
                //this.IsHighlighted = this.IsMouseOver;
            }
        }

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(InteractiveControl), new PropertyMetadata(false, OnIsHighlightedPropertyChanged
            ));

        private static void OnIsHighlightedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InteractiveControl)d).OnIsHighlightedChanged();
        }

        protected virtual void OnIsHighlightedChanged()
        {
             this.IsHighlightedChanged?.Invoke(this, this.IsHighlighted);
        }


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(InteractiveControl), new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InteractiveControl)d).OnIsSelectedChanged();
        }

        protected virtual void OnIsSelectedChanged()
        {
            this.IsSelectedChanged?.Invoke(this, this.IsSelected);
        }
    }
}