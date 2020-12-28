using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MvvmCharting.WpfFX
{
    public static class VisualTreeHelper2
    {
        public static void PrintVisualTree(this DependencyObject o, string indent)
        {
            indent = "  " + indent;
            Debug.WriteLine($"{indent}{o?.GetType()}");

            var childCount = VisualTreeHelper.GetChildrenCount(o);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                foreach (var childItem in GetAllVisualChildren(child))
                {
                    PrintVisualTree(childItem, indent);
                }
            }
        }

        public static IEnumerable<DependencyObject> GetAllVisualChildren(this DependencyObject o)
        {
            yield return o;

            var childCount = VisualTreeHelper.GetChildrenCount(o);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                foreach (var childItem in GetAllVisualChildren(child))
                {
                    yield return childItem;
                }
            }
        }
    }


    public static class LogicalTreeHelper2
    {
        public static void PrintLogicalTree(this DependencyObject o, string indent)
        {
            if (o == null)
            {
                return;
            }

            indent = "  " + indent;
            Debug.WriteLine($"{indent}{o?.GetType()}");


            foreach (var child in LogicalTreeHelper.GetChildren(o))
            {


                foreach (var childItem in GetAllLogicalChildren(child as DependencyObject))
                {
                    PrintLogicalTree(childItem as DependencyObject, indent);
                }
            }
        }

        public static IEnumerable<object> GetAllLogicalChildren(DependencyObject o)
        {
   

            yield return o;

            foreach (var child in LogicalTreeHelper.GetChildren(o))
            {
                yield return child;
            }


        }
    }
}