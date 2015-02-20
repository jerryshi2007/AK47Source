using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace Designer.Commands
{
	public class BindingListener
	{
		public delegate void ChangedHandler(object sender, BindingChangedEventArgs e);
		private class DependencyPropertyListener
		{
			private DependencyProperty property;
			private static int index;
			private FrameworkElement target;
			public event EventHandler<BindingChangedEventArgs> Changed;
		
			public DependencyPropertyListener()
			{
				this.property = DependencyProperty.RegisterAttached("DependencyPropertyListener" + BindingListener.DependencyPropertyListener.index++, typeof(object), typeof(BindingListener.DependencyPropertyListener), new PropertyMetadata(null, new PropertyChangedCallback(this.HandleValueChanged)));
			}
			private void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
			{
				if (this.Changed != null)
				{
					this.Changed(this, new BindingChangedEventArgs(e));
				}
			}
			public void Attach(FrameworkElement element, Binding binding)
			{
				if (this.target != null)
				{
					throw new Exception("Cannot attach an already attached listener");
				}
				this.target = element;
				this.target.SetBinding(this.property, binding);
			}
			public void Detach()
			{
				this.target.ClearValue(this.property);
				this.target = null;
			}
			public void SetValue(object value)
			{
				this.target.SetValue(this.property, value);
			}
		}
		private static List<BindingListener.DependencyPropertyListener> freeListeners = new List<BindingListener.DependencyPropertyListener>();
		private BindingListener.DependencyPropertyListener listener;
		private BindingListener.ChangedHandler changedHandler;
		private Binding binding;
		private FrameworkElement target;
		private object value;
		public Binding Binding
		{
			get
			{
				return this.binding;
			}
			set
			{
				this.binding = value;
				this.Attach();
			}
		}
		public FrameworkElement Element
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
				this.Attach();
			}
		}
		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.listener != null)
				{
					this.listener.SetValue(value);
				}
			}
		}
		public BindingListener(BindingListener.ChangedHandler changedHandler)
		{
			this.changedHandler = changedHandler;
		}
		public BindingListener()
		{
		}
		private void Attach()
		{
			this.Detach();
			if (this.target != null && this.binding != null)
			{
				this.listener = this.GetListener();
				this.listener.Attach(this.target, this.binding);
			}
		}
		private void Detach()
		{
			if (this.listener != null)
			{
				this.ReturnListener();
			}
		}
		private BindingListener.DependencyPropertyListener GetListener()
		{
			BindingListener.DependencyPropertyListener dependencyPropertyListener;
			if (BindingListener.freeListeners.Count != 0)
			{
				dependencyPropertyListener = BindingListener.freeListeners[BindingListener.freeListeners.Count - 1];
				BindingListener.freeListeners.RemoveAt(BindingListener.freeListeners.Count - 1);
				return dependencyPropertyListener;
			}
			dependencyPropertyListener = new BindingListener.DependencyPropertyListener();
			dependencyPropertyListener.Changed += new EventHandler<BindingChangedEventArgs>(this.HandleValueChanged);
			return dependencyPropertyListener;
		}
		private void ReturnListener()
		{
			this.listener.Changed -= new EventHandler<BindingChangedEventArgs>(this.HandleValueChanged);
			BindingListener.freeListeners.Add(this.listener);
			this.listener = null;
		}
		private void HandleValueChanged(object sender, BindingChangedEventArgs e)
		{
			this.value = e.EventArgs.NewValue;
			if (this.changedHandler != null)
			{
				this.changedHandler(this, e);
			}
		}
	}

	public sealed class BindingChangedEventArgs : EventArgs
	{
		public DependencyPropertyChangedEventArgs EventArgs
		{
			get;
			private set;
		}

		public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e)
		{
			this.EventArgs = e;
		}
	}
}
