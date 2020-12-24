using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MvvmChart.Common;

namespace MvvmCharting
{

    public class SlimItemsControl : Control
    {
        private static string RangeActionsNotSupported = "RangeActionsNotSupported";
        private static string UnexpectedCollectionChangeAction = "UnexpectedCollectionChangeAction";
        static SlimItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlimItemsControl), new FrameworkPropertyMetadata(typeof(SlimItemsControl)));
        }

        private bool _loaded;
        public SlimItemsControl()
        {
            this.Loaded += this.SlimItemsControl_Loaded;
            this.Unloaded += this.SlimItemsControl_Unloaded;

           


        }

        private void SlimItemsControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this._loaded = false;
        }

        private void SlimItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            this._loaded = true;
            ReloadAllItems();
        }

        private Panel PART_Root;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Root = VisualTreeHelper2.GetAllChildren(this).OfType<Panel>().FirstOrDefault();


            if (this.PART_Root == null)
            {
                throw new Cartesian2DChartException($"The Template of SlimItemsControl contains no Panel!");
            }

            ReloadAllItems();



        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == VisibilityProperty)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    ReloadAllItems();
                }
            }
        }


        public event Action<object, FrameworkElement> ItemTemplateContentLoaded;

        private Dictionary<object, FrameworkElement> _itemsDictionary = new Dictionary<object, FrameworkElement>();

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(typeof(SlimItemsControl),
                new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((SlimItemsControl)d).OnItemsSourceChanged((IList)e.OldValue, (IList)e.NewValue);
        }

        private void OnItemsSourceChanged(IList oldValue, IList newValue)
        {
            ReloadAllItems();

            if (oldValue is INotifyCollectionChanged oldItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(oldItemsSource, "CollectionChanged", ItemsSource_CollectionChanged);
            }

            if (newValue is INotifyCollectionChanged newItemsSource)
            {
                WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventArgs>
                    .AddHandler(newItemsSource, "CollectionChanged", ItemsSource_CollectionChanged);
            }

        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);

        }


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            ItemsControl.ItemTemplateProperty.AddOwner(typeof(SlimItemsControl),
                new PropertyMetadata(null, OnItemTemplatePropertyChanged));

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SlimItemsControl)d).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            ReloadAllItems();
        }





        private FrameworkElement LoadTemplateContentForItem(object item)
        {
            var template = this.ItemTemplate;
            if (template == null)
            {
                if (this.ItemTemplateSelector != null)
                {
                    template = this.ItemTemplateSelector.SelectTemplate(item, null);
                }
            }

            if (template == null)
            {
                throw new NotImplementedException();
            }

            var treeRoot = template.LoadContent();

            var FE = treeRoot as FrameworkElement;
            if (FE == null)
            {
                throw new NotSupportedException($"{FE}");
            }


            FE.DataContext = item;


            return FE;
        }


        #region Collection operation

        void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
           
            if (!CanLoadItem())
            {
                return;
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems.Count != 1)
                        throw new NotSupportedException(RangeActionsNotSupported);
                    OnItemAdded(args.NewItems[0], args.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems.Count != 1)
                        throw new NotSupportedException(RangeActionsNotSupported);
                    OnItemRemoved(args.OldItems[0], args.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:

                    if (/*this.ItemsSource.Count != this._itemsDictionary.Count
                        || */this.ItemsSource.Count != this.PART_Root.Children.Count)
                    {
                        throw new NotImplementedException();
                        
                    }
 
                    if (args.OldItems.Count != 1)
                        throw new NotSupportedException(RangeActionsNotSupported);

                    OnItemReplaced(args.OldItems[0], args.NewItems[0], args.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:

                    if (args.OldItems.Count != 1)
                        throw new NotSupportedException(RangeActionsNotSupported);

                    OnItemMoved(args.OldItems[0], args.OldStartingIndex, args.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnRefresh();
                    break;

                default:
                    throw new NotSupportedException($"{UnexpectedCollectionChangeAction}: {args.Action}");
            }

        }

         
        // Called when an item is added to the items collection
        void OnItemAdded(object item, int index)
        {
            var treeRoot = LoadTemplateContentForItem(item);
        
            this.PART_Root.Children.Insert(index, treeRoot);

            this.ItemTemplateContentLoaded?.Invoke(this, treeRoot);

           

        }

        // Called when an item is removed from the items collection
        void OnItemRemoved(object item, int itemIndex)
        {
            //this._itemsDictionary.Remove(item);
            this.PART_Root.Children.RemoveAt(itemIndex);

        }

        void OnItemReplaced(object oldItem, object newItem, int index)
        {
            var treeRoot = LoadTemplateContentForItem(newItem);

            this.PART_Root.Children.RemoveAt(index);  
            this.PART_Root.Children.Insert(index, treeRoot);

            //this._itemsDictionary.Remove(oldItem);
            //this._itemsDictionary.Add(newItem, treeRoot);

            this.ItemTemplateContentLoaded?.Invoke(this, treeRoot);
        }

        void OnItemMoved(object item, int oldIndex, int newIndex)
        {
            var old = this.PART_Root.Children[oldIndex];
            int insertIndex = newIndex;
            if (oldIndex < insertIndex)
            {
                insertIndex--;
            }
            this.PART_Root.Children.RemoveAt(oldIndex);
            this.PART_Root.Children.Insert(insertIndex, old);
        }

        // Called when the items collection is refreshed
        void OnRefresh()
        {
            ReloadAllItems();
        }

        private void ClearItems()
        {
            this.PART_Root?.Children.Clear();
        }

        private bool CanLoadItem()
        {
            return this._loaded &&
                   this.Visibility == Visibility.Visible &&
                   (this.ItemTemplate != null || this.ItemTemplateSelector != null);
        }

        private void ReloadAllItems()
        {
            ClearItems();

            if (this.ItemsSource == null ||
                this.ItemsSource.Count == 0)
            {
                return;
            }



            if (!CanLoadItem())
            {
                return;
            }

            Debug.Assert(this.PART_Root.Children.Count==0);

            if (this.ItemsSource.Count == this.PART_Root.Children.Count)
            {
                return;
            }

            PART_Root.Children.Capacity = this.PART_Root.Children.Count + this.ItemsSource.Count;

            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                OnItemAdded(this.ItemsSource[i], i);
            }
        }
        #endregion

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            ItemsControl.ItemTemplateSelectorProperty.AddOwner(typeof(SlimItemsControl), new PropertyMetadata(null, OnItemTemplateSelectorPropertyChanged));

        private static void OnItemTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SlimItemsControl)d).OnItemTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        private void OnItemTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue)
        {
            if (this.ItemTemplate == null)
            {
                ReloadAllItems();
            }
        }








    }
}