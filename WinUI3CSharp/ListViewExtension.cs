using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
namespace WinUI3CSharp
{
    internal static class ListViewExtension
    {
        public static void Bind<T>(this ListView listView, ObservableCollection<T> items, ItemTemplate<T> template)
        {
            listView.ItemsSource = items;
            listView.ItemTemplate = template;
        }

        public static void Bind<T>(this ListView listView, ObservableCollection<T> items, ItemTemplateSelector<T> templateSelector)
        {
            listView.ItemsSource = items;
            listView.ItemTemplateSelector = templateSelector;
        }
    }
}
