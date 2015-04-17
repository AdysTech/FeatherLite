using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdysTech.FeatherLite.View
{
     /// <summary>
    /// Couple of simple helpers for walking the visual tree
    /// </summary>
    static class VisualTreeHelper
    {
        /// <summary>
        /// Gets the ancestors of the element, up to the root
        /// </summary>
        /// <param name="node">The element to start from</param>
        /// <returns>An enumerator of the ancestors</returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            FrameworkElement parent = node.GetVisualParent ();
            while ( parent != null )
            {
                yield return parent;
                parent = parent.GetVisualParent ();
            }
        }

        /// <summary>
        /// Gets the visual parent of the element
        /// </summary>
        /// <param name="node">The element to check</param>
        /// <returns>The visual parent</returns>
        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return System.Windows.Media.VisualTreeHelper.GetParent (node) as FrameworkElement;
        }
    }
}
