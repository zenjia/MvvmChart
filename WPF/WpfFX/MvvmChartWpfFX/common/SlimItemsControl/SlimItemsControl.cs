//#define DEBUG_SlimItemsControl

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MvvmCharting.Common;

namespace MvvmCharting.WpfFX
{

    /// <summary>
    /// This is just a panel wrapper, which can be used through MVVM mode
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Panel))]
    public class SlimItemsControl : Control
    {
        static SlimItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SlimItemsControl), new FrameworkPropertyMetadata(typeof(SlimItemsControl)));
        }

        private static string RangeActionsNotSupported = "RangeActionsNotSupported";
        private static string UnexpectedCollectionChangeAction = "UnexpectedCollectionChangeAction";

        /// <summary>
        /// Cached ItemsSource reference which has been added
        /// </summary>
        private IList _handledItemsSource;

        /// <summary>
        /// Fired when ItemTemplate is applied for an item
        /// </summary>
        public event Action<object, FrameworkElement, int> ElementGenerated;

        public event Action<object, FrameworkElement> ChildRemoved;
        public event Action<object, FrameworkElement> ChildReplaced;
        public event Action<object, FrameworkElement> ChildAdded;
        public event Action<object> Reset;

        private Dictionary<object, FrameworkElement> _itemChildMap = new Dictionary<object, FrameworkElement>();

        private Panel PART_Root;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_Root = (Panel)this.GetTemplateChild("PART_Root");


            if (this.PART_Root == null)
            {
                throw new MvvmChartException($"'PART_Root' is not found!");
            }


            LoadAllItems();



        }

        public override string ToString()
        {
            return this.Name ?? GetHashCode().ToString();

        }

        public SlimItemsControl()
        {
            this.Loaded += this.SlimItemsControl_Loaded;
        }

        private void SlimItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllItems();
        }


        #region public routines
        public bool ContainsItem(object item)
        {
            return this._itemChildMap.ContainsKey(item);
        }

        public int ItemCount => this._itemChildMap.Count;

        public FrameworkElement TryGetChildForItem(object item)
        {
            if (!this._itemChildMap.ContainsKey(item))
            {
                return null;
            }
            return this._itemChildMap[item];
        }

        public IEnumerable<FrameworkElement> GetChildren()
        {
            if (this.PART_Root == null)
            {
                Debug.Assert(this._itemChildMap.Count == 0);
                return Enumerable.Empty<FrameworkElement>();
            }

            Debug.Assert(this._itemChildMap.Count == this.PART_Root.Children.Count);

            return this.PART_Root.Children.OfType<FrameworkElement>();
        }

        public int GetChildIndex(FrameworkElement child)
        {
            return this.PART_Root.Children.IndexOf(child);
        }
        #endregion

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
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemsSourceChanged....newValue={newValue}");
#endif


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
            //If the ItemTemplate or ItemTemplateSelector of an ItemsControl is replaced, then its
            //ItemContainer will re-apply its Template(i.e. ItemTemplate), which will regenerate
            //the TemplateChild of its ItemContainer. We should ReloadAllItems.
            ReloadAllItems();
        }

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

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {

            if (!CanAddItem())
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

                    if (this.ItemsSource.Count != this.PART_Root.Children.Count)
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

        private void OnItemAdded(object item, int index)
        {

#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemAdded....{item}");
#endif

            var treeRoot = LoadTemplateContentForItem(item);

            this._itemChildMap.Add(item, treeRoot);

            this.PART_Root.Children.Insert(index, treeRoot);

            this.ElementGenerated?.Invoke(this, treeRoot, index);

            this.ChildAdded?.Invoke(this, treeRoot);

        }

        private void OnItemRemoved(object item, int itemIndex)
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemRemoved....{item}");
#endif
            this._itemChildMap.Remove(item);

            FrameworkElement treeRoot = null;
            if (this.ChildRemoved != null)
            {
                treeRoot = (FrameworkElement)this.PART_Root.Children[itemIndex];
            }
            this.PART_Root.Children.RemoveAt(itemIndex);

            this.ChildRemoved?.Invoke(this, treeRoot);
        }

        private void OnItemReplaced(object oldItem, object newItem, int index)
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemReplaced....{oldItem}->{newItem}");
#endif

            FrameworkElement treeRoot;
            if (this.ItemTemplate != null)
            {
#if DEBUG_SlimItemsControl
                Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemReplaced2....");
#endif
                treeRoot = (FrameworkElement)this.PART_Root.Children[index];
                treeRoot.DataContext = newItem;

            }
            else
            {
                treeRoot = LoadTemplateContentForItem(newItem);
                this.PART_Root.Children.RemoveAt(index);
                this.PART_Root.Children.Insert(index, treeRoot);
            }




            this._itemChildMap.Remove(oldItem);
            this._itemChildMap.Add(newItem, treeRoot);

            this.ChildReplaced?.Invoke(this, treeRoot);

            //treeRoot = LoadTemplateContentForItem(newItem);
            //this.PART_Root.Children.RemoveAt(index);
            //this.PART_Root.Children.Insert(index, treeRoot);

            this.ElementGenerated?.Invoke(this, treeRoot, index);
        }

        private void OnItemMoved(object item, int oldIndex, int newIndex)
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemMoved....{item}");
#endif
            var old = this.PART_Root.Children[oldIndex];
            int insertIndex = newIndex;
            if (oldIndex < insertIndex)
            {
                insertIndex--;
            }
            this.PART_Root.Children.RemoveAt(oldIndex);
            this.PART_Root.Children.Insert(insertIndex, old);

        }

        private void OnRefresh()
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnRefresh....");
#endif

            ReloadAllItems();

            this.Reset?.Invoke(this);
        }

        private void ClearItems()
        {
#if DEBUG_SlimItemsControl
            if (this.PART_Root != null && this.PART_Root.Children.Count != 0)
            {
                Debug.WriteLine($"{this.Name}({this.GetHashCode()})....ClearItems....ct={this.PART_Root.Children.Count}");
            }
#endif

            this._itemChildMap.Clear();

            this.PART_Root?.Children.Clear();

            _handledItemsSource = null;

        }

        private bool CanAddItem()
        {
            return this.IsLoaded &&
                   this.PART_Root != null &&
                   //this.Visibility == Visibility.Visible &&
                   (this.ItemTemplate != null || this.ItemTemplateSelector != null);
        }

        private void ReloadAllItems()
        {

#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....ReloadAllItems....");
#endif
            ClearItems();

            LoadAllItems();
        }

        public void LoadAllItems()
        {
            if (this.ItemsSource == null || !CanAddItem())
            {
                return;
            }

            if (object.ReferenceEquals(this._handledItemsSource, this.ItemsSource))
            {

                return;
            }

            if (this.PART_Root.Children.Count != 0)
            {
                throw new MvvmChartException($"this.PART_Root.Children.Count=={this.PART_Root.Children.Count}");
            }


            this.PART_Root.Children.Capacity = this.ItemsSource.Count;


            Debug.WriteLine($"LoadAllItems: {this.Name}..........ct = {ItemsSource.Count}....!!!!!!!!!!");

            for (int i = 0; i < this.ItemsSource.Count; i++)
            {
                OnItemAdded(this.ItemsSource[i], i);
            }

            this._handledItemsSource = this.ItemsSource;
        }
        #endregion










    }


}