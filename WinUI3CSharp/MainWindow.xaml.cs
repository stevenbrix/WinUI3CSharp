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
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    
    public class Parent : Person
    {
        public Parent(string first, string last, Person child): base(first, last)
        {
            this.Child = child;
        }
        public Person Child { get; private set; }
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
                var person1 = new Person("Jon", "Doe");
                var person2 = new Person("Jane", "Doe");
                People.Add(person1);
                People.Add(person2);
                People.Add(new Parent("Bob", "Doe", person1));
                People.Add(new Parent("Rob", "Doe", person2));
            }

            var count = 0;
            var headerBlock = new TextBlock() { Height = 100 };
            var panel = new StackPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(headerBlock);
            var listView = new ListView();
            panel.Children.Add(listView);

            // Another way to update the template is just to use a binding and change visibility
            // of an element, which will accomplish the same visual effect as the above example.
            listView.Bind(People, new ItemTemplate<Person>((person) =>
            {
                headerBlock.Text = $"{++count} created items";
                var firstName = new TextBlock();
                var lastName = new TextBlock();
                var isParent = new TextBlock();
                return new ()
                {
                    Bindings = (person) => {
                        firstName.Text = person.FirstName;
                        lastName.Text = person.LastName;
                        if (person is Parent parent)
                        {
                            isParent.Text = $" is parent of {parent.Child.FirstName} {parent.Child.LastName}";
                            isParent.Visibility = Visibility.Visible;
                        } else
                        {
                            isParent.Visibility = Visibility.Collapsed;
                        }
                    },
                    RootElement = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Children = {
                            firstName,
                            lastName,
                            isParent
                        }
                    }
                };
            }));

            // Use an ItemTemplateSelector to select different item templates based off the item in the 
            // data source. The below selector creates the same visual effect as above, and in this simple example,
            // it is easier to simply bind visibility of the TextBlock.  
            //listView.Bind(People, new ItemTemplateSelector<Person>((person) => person is Parent, new((person) =>
            //{
            //    headerBlock.Text = $"{++count} created items";
            //    var firstName = new TextBlock();
            //    var lastName = new TextBlock();
            //    var isParent = new TextBlock();
            //
            //    return new()
            //    {
            //        Bindings = (person) => {
            //            firstName.Text = person.FirstName;
            //            lastName.Text = person.LastName;
            //            isParent.Text = $" is parent of {((Parent)person).Child.FirstName} {((Parent)person).Child.LastName}";
            //        },
            //        RootElement = new StackPanel()
            //        {
            //            Orientation = Orientation.Horizontal,
            //            Children = {
            //                firstName,
            //                lastName,
            //                new TextBlock() { Text = " is parent" }
            //            }
            //        }
            //    };
            //}), new((person) =>
            //{
            //    headerBlock.Text = $"{++count} created items";
            //    var firstName = new TextBlock();
            //    var lastName = new TextBlock();

            //    return new()
            //    {
            //        Bindings =
            //        {
            //            (person) => firstName.Text = person.FirstName,
            //            (person) => lastName.Text = person.LastName,
            //        },
            //        RootElement = new StackPanel()
            //        {
            //            Orientation = Orientation.Horizontal,
            //            Children = {
            //                firstName,
            //                lastName
            //            }
            //        }
            //    };
            //})));
            listView.Height = 50;
            Content = panel;
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
