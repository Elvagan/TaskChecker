using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskChecker.UI.Converters
{
    public class NullToVisibilityConverter : DependencyObject, IValueConverter
    {
        #region Properties

        public Visibility VisibilityIfNull
        {
            get => (Visibility)GetValue(VisibilityIfNullProperty);
            set => SetValue(VisibilityIfNullProperty, value);
        }

        public Visibility VisibilityIfNotNull
        {
            get => (Visibility)GetValue(VisibilityIfNotNullProperty);
            set => SetValue(VisibilityIfNotNullProperty, value);
        }

        public static readonly DependencyProperty VisibilityIfNullProperty = DependencyProperty.Register("VisibilityIfNull", typeof(Visibility),
            typeof(NullToVisibilityConverter), new FrameworkPropertyMetadata(Visibility.Hidden));

        public static readonly DependencyProperty VisibilityIfNotNullProperty = DependencyProperty.Register("VisibilityIfNotNulle", typeof(Visibility),
            typeof(NullToVisibilityConverter), new FrameworkPropertyMetadata(Visibility.Visible));

        #endregion Properties

        #region Convert/ConvertBack methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? VisibilityIfNull : VisibilityIfNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Convert/ConvertBack methods
    }
}