using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3CSharp
{

    public struct TemplateContent<Item>
    {
        public TemplateContent() { }
        public UIElement RootElement { get; set; } = null;
        public List<Action<Item>> Bindings { get; } = new List<Action<Item>>();
    }
    internal class DataTemplateComponent<Item> :
          global::Microsoft.UI.Xaml.Markup.IDataTemplateComponent
    {
        // Fields for each control that has bindings.
        private global::Microsoft.UI.Xaml.Controls.ContentPresenter presenter;
        Func<Item, TemplateContent<Item>> builder;
        TemplateContent<Item>? content;
        public DataTemplateComponent(ContentPresenter presenter, Func<Item, TemplateContent<Item>> builder)
        {
            this.presenter = presenter;
            this.builder = builder;
        }

        // IDataTemplateComponent
        public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
        {
            nextPhase = -1;
            if (content == null)
            {
                content = this.builder((Item)item);
            }
            if (this.presenter.Content == null)
            {
                this.presenter.Content = content.Value.RootElement;
            }

            foreach (var binding in content.Value.Bindings)
            {
                binding((Item)item);
            }
        }

        public void Recycle()
        {
            return;
        }
    }

    internal class ItemTemplate<Item> : DataTemplate, IComponentConnector
    {
        Func<Item, TemplateContent<Item>> builder;
        public ItemTemplate(Func<Item, TemplateContent<Item>> builder)
        {
            this.builder = builder;
            Application.LoadComponent(this, new Uri("ms-appx:///DataTemplate.xaml"));
        }

        // When Application.LoadComponent is called, the item that is passed into the call is expected to implement
        // the IComponentConnector interface. The Connect method is called when the XAML runtime parses an element with
        // an associated connection ID. For DataTemplates, an x:ConnectionId is placed on the root element and so when
        // the runtime creates an instance of the template, it invokes this method.
        public void Connect(int connectionId, object target)
        {
            var item = target as ContentPresenter;

            // Set the DataTemplateComponent on the ListViewItem, this will allow us to create the UI via the builder when the ProcessBindings
            // method is called with the appropriate data item.
            Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(item, new DataTemplateComponent<Item>(item, builder));
        }

        public IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            return null;
        }
    }
}
