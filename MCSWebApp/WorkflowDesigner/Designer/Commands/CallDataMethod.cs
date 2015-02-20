using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Designer.Commands
{
	public class CallDataMethod : TriggerAction<FrameworkElement>
	{
		private BindingListener listener = new BindingListener
		{
			Binding = new Binding()
		};

		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Binding), typeof(CallDataMethod), new PropertyMetadata(null, new PropertyChangedCallback(CallDataMethod.HandleTargetBindingChanged)));
		public static readonly DependencyProperty MethodProperty = DependencyProperty.Register("Method", typeof(string), typeof(CallDataMethod), new PropertyMetadata(null));
		
		public Binding Target
		{
			get
			{
				return (Binding)base.GetValue(CallDataMethod.TargetProperty);
			}
			set
			{
				base.SetValue(CallDataMethod.TargetProperty, value);
			}
		}
		
		public string Method
		{
			get
			{
				return (string)base.GetValue(CallDataMethod.MethodProperty);
			}
			set
			{
				base.SetValue(CallDataMethod.MethodProperty, value);
			}
		}
		
		protected override void Invoke(object parameter)
		{
			object obj;
			if (this.Target == null)
			{
				obj = base.AssociatedObject.DataContext;
			}
			else
			{
				obj = this.listener.Value;
			}
			if (obj != null)
			{
				MethodInfo method = obj.GetType().GetMethod(this.Method);
				if (method != null)
				{
					ParameterInfo[] parameters = method.GetParameters();
					if (parameters.Length == 0)
					{
						method.Invoke(obj, null);
						return;
					}
					if (parameters.Length == 2 && base.AssociatedObject != null && parameter != null && parameters[0].ParameterType.IsAssignableFrom(base.AssociatedObject.GetType()) && parameters[1].ParameterType.IsAssignableFrom(parameter.GetType()))
					{
						method.Invoke(obj, new object[]
						{
							base.AssociatedObject,
							parameter
						});
					}
				}
			}
		}
		
		protected override void OnAttached()
		{
			base.OnAttached();
			this.listener.Element = base.AssociatedObject;
		}
		
		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.listener.Element = null;
		}
		
		private static void HandleTargetBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((CallDataMethod)sender).OnTargetBindingChanged(e);
		}
		
		protected virtual void OnTargetBindingChanged(DependencyPropertyChangedEventArgs e)
		{
			this.listener.Binding = (Binding)e.NewValue;
		}
	}
}
