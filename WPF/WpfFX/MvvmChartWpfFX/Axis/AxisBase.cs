using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MvvmCharting.Axis;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX.Axis
{
    /// <summary>
    /// The base class for <see cref="NumericAxis"/> and <see cref="CategoryAxis"/>.
    /// </summary>
    [TemplatePart(Name = "PART_AxisItemsControl", Type = typeof(SlimItemsControl))]
    public abstract class AxisBase : Control, IAxisNS
    {
        static AxisBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisBase), new FrameworkPropertyMetadata(typeof(AxisBase)));
        }

        private static readonly string sPART_AxisItemsControl = "PART_AxisItemsControl";

        private SlimItemsControl PART_AxisItemsControl;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PART_AxisItemsControl != null)
            {
                this.PART_AxisItemsControl.ElementGenerated -= AxisItemsControlItemTemplateApplied;
            }

            this.PART_AxisItemsControl = (SlimItemsControl)GetTemplateChild(sPART_AxisItemsControl);
            if (this.PART_AxisItemsControl != null)
            {

                this.PART_AxisItemsControl.ElementGenerated += AxisItemsControlItemTemplateApplied;

                this.PART_AxisItemsControl.SetBinding(SlimItemsControl.ItemTemplateProperty,
                    new Binding(nameof(this.ItemTemplate)) { Source = this });
                this.PART_AxisItemsControl.SetBinding(SlimItemsControl.ItemTemplateSelectorProperty,
                    new Binding(nameof(this.ItemTemplateSelector)) { Source = this });

                this.PART_AxisItemsControl.ItemsSource = this.ItemDrawingParams;
                TryLoadAxisItemDrawingParams();
            }


        }



        protected AxisBase()
        {
            this.ItemDrawingParams = new ObservableCollection<IAxisItemDrawingBaseParams>();

        }




        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(AxisBase), new PropertyMetadata(null));



        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register("TitleStyle", typeof(Style), typeof(AxisBase), new PropertyMetadata(null));



        public AxisPlacement Placement
        {
            get { return (AxisPlacement)GetValue(PlacementProperty); }
            set { SetValue(PlacementProperty, value); }
        }
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(AxisPlacement), typeof(AxisBase), new PropertyMetadata(AxisPlacement.None, OnPlacementPropertyChange));

        private static void OnPlacementPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnPlacementChange();
        }

        private IEnumerable<IAxisItem> GetAllAxisItems()
        {
            if (this.PART_AxisItemsControl == null)
            {
                return Enumerable.Empty<IAxisItem>();
            }

            return this.PART_AxisItemsControl.GetChildren().OfType<IAxisItem>();
        }

        private void OnPlacementChange()
        {
            foreach (var axisItem in GetAllAxisItems())
            {
                axisItem.SetAxisPlacement(this.Placement);
            }

            this.AxisPlacementChanged?.Invoke(this);
        }






        public int TickCount
        {
            get { return (int)GetValue(TickCountProperty); }
            set { SetValue(TickCountProperty, value); }
        }
        public static readonly DependencyProperty TickCountProperty =
            DependencyProperty.Register("TickCount", typeof(int), typeof(AxisBase), new PropertyMetadata(10, OnTickCountPropertyChanged));

        private static void OnTickCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).UpdateAxisDrawingSettings();
        }


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(AxisBase));


        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(AxisBase));


        /// <summary>
        /// A double to string convert.
        /// The Axis only receive double values, so its the user's responsibility to provide a proper
        /// converter in order to correctly display the Label text. 
        /// If the double is converted from DateTime or DateTimeOffset, then it should be
        /// convert back to DateTime or DateTimeOffset first before it can be convert to a user-formatted string
        /// </summary>
        public IValueConverterNS LabelTextConverter
        {
            get { return (IValueConverterNS)GetValue(LabelTextConverterProperty); }
            set { SetValue(LabelTextConverterProperty, value); }
        }
        public static readonly DependencyProperty LabelTextConverterProperty =
            DependencyProperty.Register("LabelTextConverter", typeof(IValueConverterNS), typeof(AxisBase), new PropertyMetadata(null, OnLabelTextConverterPropertyChanged));

        private static void OnLabelTextConverterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisBase)d).OnLabelTextConverterChanged((IValueConverterNS)e.OldValue, (IValueConverterNS)e.NewValue);
        }

        private void OnLabelTextConverterChanged(IValueConverterNS oldValue, IValueConverterNS newValue)
        {

            foreach (var axisItem in GetAllAxisItems())
            {
                axisItem.SetLabelTextConverter(this.LabelTextConverter);
            }
        }


        private IAxisOwner _owner;
        public IAxisOwner Owner
        {
            get { return this._owner; }
            set
            {

                if (this._owner != value)
                {
                    this._owner = value;
                    AttachHandler();
                }

            }
        }






        private AxisType _orientation;
        public AxisType Orientation
        {
            get { return this._orientation; }
            set
            {
                if (this._orientation != value)
                {
                    this._orientation = value;
                    AttachHandler();
                }

            }
        }

        private void AttachHandler()
        {
            if (this.Owner == null ||
                this.Orientation == AxisType.None)
            {
                return;
            }

            switch (this.Orientation)
            {
                case AxisType.X:
                    ((IXAxisOwner)this.Owner).HorizontalSettingChanged += AxisBase_CanvasSettingChanged;
                    break;
                case AxisType.Y:
                    ((IYAxisOwner)this.Owner).VerticalSettingChanged += AxisBase_CanvasSettingChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateAxisDrawingSettings();
        }

        private void AxisBase_CanvasSettingChanged(IPlottingSettingsBase obj)
        {
            if (obj == null)
            {
                return;
            }
            switch (obj.Orientation)
            {
                case AxisType.X:
                    this.RenderSize = new Size(obj.RenderSize, this.RenderSize.Height);
                    this.Margin = new Thickness(obj.Margin.X, this.Margin.Top, obj.Margin.Y, this.Margin.Bottom);
                    this.Padding = new Thickness(obj.Padding.X, this.Padding.Top, obj.Padding.Y, this.Padding.Bottom);
                    this.BorderThickness = new Thickness(obj.BorderThickness.X, this.BorderThickness.Top, obj.BorderThickness.Y, this.BorderThickness.Bottom);

                    break;
                case AxisType.Y:
                    this.RenderSize = new Size(this.RenderSize.Width, obj.RenderSize);
                    this.Margin = new Thickness(this.Margin.Left, obj.Margin.X, this.Margin.Right, obj.Margin.Y);
                    this.Padding = new Thickness(this.Padding.Left, obj.Padding.X, this.Padding.Right, obj.Padding.Y);
                    this.BorderThickness = new Thickness(this.BorderThickness.Left, obj.BorderThickness.X, this.BorderThickness.Right, obj.BorderThickness.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.PlottingSetting = obj;


        }

        private IPlottingSettingsBase _plottingSetting;
        protected IPlottingSettingsBase PlottingSetting
        {
            get { return this._plottingSetting; }

            set
            {
                if (this._plottingSetting != value )
                {
                    this._plottingSetting = value;
                    UpdateAxisDrawingSettings();
                }
            }
        }


        private IAxisDrawingSettingsBase _drawingSettings;
        internal IAxisDrawingSettingsBase DrawingSettings
        {
            get { return this._drawingSettings; }
            set
            {

                if (this._drawingSettings == null || !Equals(this._drawingSettings, value))
                {
                    this._drawingSettings = value;

                    TryLoadAxisItemDrawingParams();
                }


            }
        }


        public ObservableCollection<IAxisItemDrawingBaseParams> ItemDrawingParams { get; }



        protected abstract void UpdateAxisDrawingSettings();


        public abstract IEnumerable<double> GetAxisItemCoordinates();


        private void OnAxisItemCoordinateChanged()
        {

            var coordinates = GetAxisItemCoordinates();


            this.Owner.OnAxisItemsCoordinateChanged(this.Orientation, coordinates);





        }

        protected IAxisDrawingSettingsBase _currentDrawingSettings;

        protected abstract bool DoLoadAxisItemDrawingParams();

        public bool TryLoadAxisItemDrawingParams()
        {
            if (this.PART_AxisItemsControl == null)
            {
                return false;
            }

            bool succeed = DoLoadAxisItemDrawingParams();

            if (!succeed)
            {
                return false;
            }

            UpdateAxisItemsCoordinate();

            return true;
        }

        protected abstract void DoUpdateAxisItemsCoordinate();

        private void UpdateAxisItemsCoordinate()
        {

            if (!this.DrawingSettings.CanUpdateAxisItemsCoordinate())
            {
                throw new NotImplementedException();
            }

            DoUpdateAxisItemsCoordinate();

            OnAxisItemCoordinateChanged();
        }


        private void AxisItemsControlItemTemplateApplied(object sender, DependencyObject root, int index)
        {
            if (!(root is IAxisItem axisItem))
            {
                throw new MvvmChartUnexpectedTypeException($"The root item of ItemTemplate of an axis must be based on '{typeof(AxisItem)}'!");
            }

            axisItem.SetLabelTextConverter(this.LabelTextConverter);
            axisItem.SetAxisPlacement(this.Placement);

        }


        protected void UpdateItemDrawingParams(IList<object> source)
        {

            var oldCt = this.ItemDrawingParams.Count;
            var newCt = source.Count;
            if (oldCt > newCt)
            {
                this.ItemDrawingParams.RemoveRange(newCt, oldCt - newCt);
            }

            for (int i = 0; i < source.Count; i++)
            {
                var newValue = source[i];
                if (i < oldCt)
                {
                    var item = this.ItemDrawingParams[i];
                    item.Value = newValue;
                }
                else
                {
                    IAxisItemDrawingBaseParams item = new AxisItemDrawingParam();
                    item.Value = newValue;
                    this.ItemDrawingParams.Add(item);
                }
            }




        }



        public event Action<IAxisNS> AxisPlacementChanged;



    }

}
