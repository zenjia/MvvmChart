using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MvvmCharting.WpfFX
{
    public class VisualTreeHelper2
    {
        public static IEnumerable<DependencyObject> GetAllChildren(DependencyObject o)
        {
            yield return o;

            var childCount = VisualTreeHelper.GetChildrenCount(o);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                foreach (var childItem in GetAllChildren(child))
                {
                    yield return childItem;
                }
            }
        }
    }


    public class LogicalTreeHelper2
    {
        public static IEnumerable<object> GetAllChildren(DependencyObject o)
        {
            yield return o;

            foreach (var child in LogicalTreeHelper.GetChildren(o))
            {
                yield return child;
            }

 
        }
    }
}