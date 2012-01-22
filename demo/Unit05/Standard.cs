using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace demo.Unit05
{
    public class Standard : DependencyObject
    {
        public static readonly DependencyProperty WidgetudeProperty = DependencyProperty.Register(
            "Property", 
            typeof(int), 
            typeof(Standard),
            new PropertyMetadata(OnPropertyChanged));

        public int Property
        {
            get { return (int)GetValue(WidgetudeProperty); }
            set { SetValue(WidgetudeProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // on-change logic here
        }
    }
}
