using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Specialized;

namespace Designer.Controls
{
	public class CustomTabControl : TabControl
	{
		public CustomTabControl()
			: base()
		{
		}

		/// <summary>
		/// Tab header template
		/// </summary>
		public DataTemplate TabHeaderItemTemplate
		{
			get { return (DataTemplate)GetValue(TabHeaderItemTemplateProperty); }
			set { SetValue(TabHeaderItemTemplateProperty, value); }
		}
		public static readonly DependencyProperty TabHeaderItemTemplateProperty =
			DependencyProperty.Register("TabHeaderItemTemplate", typeof(DataTemplate), typeof(CustomTabControl),
				new PropertyMetadata(
				(sender, e) =>
				{
					((CustomTabControl)sender).InitTabs();
				}));

		/// <summary>
		/// Tab item template
		/// </summary>
		public DataTemplate TabItemTemplate
		{
			get { return (DataTemplate)GetValue(TabItemTemplateProperty); }
			set { SetValue(TabItemTemplateProperty, value); }
		}
		public static readonly DependencyProperty TabItemTemplateProperty =
			DependencyProperty.Register("TabItemTemplate", typeof(DataTemplate), typeof(CustomTabControl),
				new PropertyMetadata(
				(sender, e) =>
				{
					((CustomTabControl)sender).InitTabs();
				}));

		public IEnumerable ItemsDataSource
		{
			get
			{
				return (IEnumerable)GetValue(ItemsDataSourceProperty);
			}
			set
			{
				SetValue(ItemsDataSourceProperty, value);
			}
		}

		public static readonly DependencyProperty ItemsDataSourceProperty =
			DependencyProperty.Register("ItemsDataSource", typeof(IEnumerable), typeof(CustomTabControl),
				new PropertyMetadata(
				(sender, e) =>
				{
					CustomTabControl control = (CustomTabControl)sender;
					INotifyCollectionChanged incc = e.OldValue as INotifyCollectionChanged;
					if (incc != null)
					{
						incc.CollectionChanged -= control.ItemsDataSource_CollectionChanged;
					}
					control.InitTabs();

					incc = e.NewValue as INotifyCollectionChanged;
					if (incc != null)
					{
						incc.CollectionChanged += control.ItemsDataSource_CollectionChanged;
					}
				}));


		internal void InitTabs()
		{
			Items.Clear();
			if (ItemsDataSource != null)
			{
				foreach (var item in ItemsDataSource)
				{
					var newitem = new TabItem();

					if (TabItemTemplate != null)
						newitem.Content = TabItemTemplate.LoadContent();

					if (TabHeaderItemTemplate != null)
						newitem.Header = TabHeaderItemTemplate.LoadContent();

					newitem.DataContext = item;
					Items.Add(newitem);
				}
			}
		}

		void ItemsDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				if (e.NewStartingIndex > -1)
				{
					foreach (var item in e.NewItems)
					{
						var newitem = new TabItem();

						if (TabItemTemplate != null)
							newitem.Content = TabItemTemplate.LoadContent();

						if (TabHeaderItemTemplate != null)
							newitem.Header = TabHeaderItemTemplate.LoadContent();

						newitem.DataContext = item;

						Items.Add(newitem);

						this.SelectedItem = newitem;
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				if (e.OldStartingIndex > -1)
				{
					Items.RemoveAt(e.OldStartingIndex);
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				Items.RemoveAt(e.OldStartingIndex);

				var newitem = new TabItem();

				if (TabItemTemplate != null)
					newitem.Content = TabItemTemplate.LoadContent();

				if (TabHeaderItemTemplate != null)
					newitem.Header = TabHeaderItemTemplate.LoadContent();

				newitem.DataContext = e.NewItems[0];

				Items.Add(newitem);

				Items.Insert(e.NewStartingIndex, newitem);
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				InitTabs();
			}
		}
	}
}
