// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using System.Numerics;
using Microsoft.UI.Xaml.Hosting;
using Windows.UI.WebUI;
using WinUI3CSharp;
using Microsoft.UI.Xaml.Markup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3CSharp
{
    class MyDataTemplate : DataTemplate, IElementFactory
    {
        public MyDataTemplate() { }
        new public UIElement GetElement(ElementFactoryGetArgs args)
        {
            return new ListViewItem()
            {
                Content = new TextBlock()
                {
                    Text = "Hello"
                }
            };
        }
    }
    public class Person
    {
        public Person(string first, string last)
        {
            FirstName = first;
            LastName = last;
        }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            for (int i = 0; i < 1000; i++) {
                People.Add(new("Steve", "Kirbach"));
                People.Add(new("Heidi", "Heiser"));
            }

            var id = WinRT.GuidGenerator.GetIID(typeof(Microsoft.UI.Xaml.Thickness));

            var grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition()
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            var rect1 = new Rectangle();
            rect1.Width = 100;
            rect1.Height = 100;
            rect1.Fill = new SolidColorBrush(Colors.Red);
            Grid.SetColumn(rect1, 0);
            grid.Children.Add(rect1);

            var rect2 = new Rectangle();
            rect2.Width = 100;
            rect2.Height = 100;
            rect2.Fill = new SolidColorBrush(Colors.Blue);

            Grid.SetColumn(rect2, 1);
            grid.Children.Add(rect2);
            
            var rect3 = new Rectangle();
            rect3.Width = 100;
            rect3.Height = 100;
            rect3.Fill = new SolidColorBrush(Colors.HotPink);

            Grid.SetColumn(rect3, 2);
            grid.Children.Add(rect3);

            var listView = new ListView();
            listView.ItemsSource = People;
            listView.ItemTemplate = new ItemTemplate<Person>((person) =>
            {
                var firstName = new TextBlock();
                var lastName = new TextBlock();

                return new TemplateContent<Person>()
                {
                    Bindings =
                    {
                        (person) => firstName.Text = person.FirstName,
                        (person) => lastName.Text = person.LastName,
                    },
                    RootElement = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Children = {
                            firstName,
                            lastName
                        }
                    }
                };
            });
            listView.Height = 50;
            Content = listView;
        }

        public System.Collections.ObjectModel.ObservableCollection<Person> People { get; } = new ();
        Compositor _compositor = Microsoft.UI.Xaml.Media.CompositionTarget.GetCompositorForCurrentThread();
        private SpringVector3NaturalMotionAnimation _springAnimation;
        private void UpdateSpringAnimation(float finalValue)
        {
            if (_springAnimation == null)
            {
                _springAnimation = _compositor.CreateSpringVector3Animation();
                _springAnimation.Target = "Scale";
            }

            _springAnimation.FinalValue = new Vector3(finalValue);
            _springAnimation.DampingRatio = 0.6f;
            _springAnimation.Period = TimeSpan.FromMilliseconds(50);
        }
        private void StartAnimation(UIElement sender, Microsoft.UI.Composition.CompositionAnimation animation)
        {
            sender.StartAnimation(animation);
        }
        private void Button_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            UpdateSpringAnimation(1.5f);

            StartAnimation((sender as UIElement), _springAnimation);
        }

        private void StackPanel_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UpdateSpringAnimation(1.0f);
        }

        private void Button_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            UpdateSpringAnimation(1.0f);

            StartAnimation((sender as UIElement), _springAnimation);
        }

    }
}
