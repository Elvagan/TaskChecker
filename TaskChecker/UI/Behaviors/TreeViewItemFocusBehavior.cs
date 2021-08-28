using Microsoft.Xaml.Behaviors;
using System.Windows;
using TaskChecker.Interfaces;

namespace TaskChecker.UI.Behaviors
{
    internal class TreeViewItemFocusBehavior : Behavior<FrameworkElement>
    {
        #region Properties

        public static readonly DependencyProperty SelectableItemProperty = DependencyProperty.Register(
            "SelectableItem",
            typeof(ISelectableItemInList),
            typeof(TreeViewItemFocusBehavior),
            new FrameworkPropertyMetadata(null));

        public ISelectableItemInList SelectableItem
        {
            get => (ISelectableItemInList)GetValue(SelectableItemProperty);
            set => SetValue(SelectableItemProperty, value);
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// behavior qui dans une grille permet de sélectionner l'elt courant (et deselectionner les autres elements lorsque le
        /// focus est positionné sur un de ses éléments.
        /// </summary>
        public TreeViewItemFocusBehavior()
        { }

        #endregion Constructor

        #region OnAttached / OnDetaching

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += OnAssociatedObjectGotFocus;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnAssociatedObjectGotFocus;
        }

        /// <summary>
        /// Called when [associated object got focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectGotFocus(object sender, RoutedEventArgs e)
        {
            if (SelectableItem != null)
                SelectableItem.IsSelected = true;
        }

        #endregion OnAttached / OnDetaching
    }
}