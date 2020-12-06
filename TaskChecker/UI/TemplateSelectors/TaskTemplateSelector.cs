using System.Windows;
using System.Windows.Controls;
using TaskChecker.ViewModels;

namespace TaskChecker.UI.TemplateSelectors
{
    internal class TaskTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the default template.
        /// </summary>
        public DataTemplate DefaultTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for enum values.
        /// </summary>
        public DataTemplate CreationTemplate { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate" /> or <see langword="null" />. The default value is <see langword="null" />.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null || !(item is TaskVm taskVm)) return null;

            return taskVm.IsFake ? CreationTemplate : DefaultTemplate;
        }
    }
}