using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskChecker.UI
{
    /// <summary>
    /// Logique d'interaction pour TaskListControl.xaml
    /// </summary>
    public partial class TaskListControl : UserControl
    {
        public TaskListControl()
        {
            InitializeComponent();

            Splitter.DragDelta += OnSplitterDragDelta;
        }

        private void OnSplitterDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double newWidth = ContentGrid.ColumnDefinitions[0].ActualWidth + e.HorizontalChange;

            if (newWidth <= ContentGrid.ColumnDefinitions[0].MinWidth) return;

            ContentGrid.ColumnDefinitions[0].Width = new GridLength(newWidth);
        }
    }
}