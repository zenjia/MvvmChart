
//#define DEBUG_SlimItemsControl

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

    /// <summary>
    /// This is not an ItemsControl
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


        public SlimItemsControl()
        {
  
            this.Loaded += this.SlimItemsControl_Loaded;

 

        }

        

        private void SlimItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
          
            LoadAllItems();
        }

        private IList _handledItemsSource;
 
 
 

        public event Action<object, FrameworkElement> ItemTemplateContentLoaded;

        private Dictionary<object, FrameworkElement> _itemsDictionary = new Dictionary<object, FrameworkElement>();

        #region internal items access routines
        public bool ContainsItem(object item)
        {
            return this._itemsDictionary.ContainsKey(item);
        }

        public int ItemCount => this._itemsDictionary.Count;

        public FrameworkElement TryGetTemplateElementForItem(object item)
        {
            if (!this._itemsDictionary.ContainsKey(item))
            {
                return null;
            }
            return this._itemsDictionary[item];
        }

        public IEnumerable<FrameworkElement> GetAllTemplateElements()
        {
            if (this._itemsDictionary.Count != this.PART_Root.Children.Count)
            {
                throw new NotImplementedException();
            }

            return this._itemsDictionary.Values;
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
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemsSourceChanged....");
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


        // Called when an item is added to the items collection
        void OnItemAdded(object item, int index)
        {

#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemAdded....{item}");
#endif

            var treeRoot = LoadTemplateContentForItem(item);

            _itemsDictionary.Add(item, treeRoot);

            this.PART_Root.Children.Insert(index, treeRoot);

            this.ItemTemplateContentLoaded?.Invoke(this, treeRoot);



        }

        // Called when an item is removed from the items collection
        void OnItemRemoved(object item, int itemIndex)
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnItemRemoved....{item}");
#endif
            _itemsDictionary.Remove(item);
            this.PART_Root.Children.RemoveAt(itemIndex);

        }

        void OnItemReplaced(object oldItem, object newItem, int index)
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

            _itemsDictionary.Remove(oldItem);
            _itemsDictionary.Add(newItem, treeRoot);
            //treeRoot = LoadTemplateContentForItem(newItem);
            //this.PART_Root.Children.RemoveAt(index);
            //this.PART_Root.Children.Insert(index, treeRoot);

            this.ItemTemplateContentLoaded?.Invoke(this, treeRoot);
        }

        void OnItemMoved(object item, int oldIndex, int newIndex)
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

        // Called when the items collection is refreshed
        void OnRefresh()
        {
#if DEBUG_SlimItemsControl
            Debug.WriteLine($"{this.Name}({this.GetHashCode()})....OnRefresh....");
#endif
            
            ReloadAllItems();
        }

        private void ClearItems()
        {
#if DEBUG_SlimItemsControl
            if (this.PART_Root != null && this.PART_Root.Children.Count != 0)
            {
                Debug.WriteLine($"{this.Name}({this.GetHashCode()})....ClearItems....ct={this.PART_Root.Children.Count}");
            }
#endif

            this._itemsDictionary.Clear();

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
            if (this.ItemsSource == null)
            {
             
                return;
            }


            if (!CanAddItem())
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