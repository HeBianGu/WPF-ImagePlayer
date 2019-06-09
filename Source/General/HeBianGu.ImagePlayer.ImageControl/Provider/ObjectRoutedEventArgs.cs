using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HeBianGu.ImagePlayer.ImageControl
{
    public class ObjectRoutedEventArgs<T> : RoutedEventArgs
    {
        public T Object { get; set; }

        public ObjectRoutedEventArgs(RoutedEvent routedEvent, object source, T t):base(routedEvent,source)
        {
            Object = t;
        }

    }
}
