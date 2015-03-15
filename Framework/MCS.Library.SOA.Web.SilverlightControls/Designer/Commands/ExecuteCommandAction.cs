﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Input;

namespace Designer.Commands
{
	[DefaultTrigger(typeof(UIElement), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonDown")]
	public class ExecuteCommandAction : TargetedTriggerAction<UIElement>
	{
		/// <summary>
		/// Dependency property represents the Command of the behaviour.
		/// </summary>
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter",
		typeof(object), typeof(ExecuteCommandAction), new PropertyMetadata(null));
		/// <summary>
		/// Dependency property represents the Command parameter of the behaviour.
		/// </summary>
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ExecuteCommandAction), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets the Commmand.
		/// </summary>
		public ICommand Command
		{
			get
			{
				return (ICommand)this.GetValue(CommandProperty);
			}
			set
			{
				this.SetValue(CommandProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the CommandParameter.
		/// </summary>
		public object CommandParameter
		{
			get
			{
				return this.GetValue(CommandParameterProperty);
			}
			set
			{
				this.SetValue(CommandParameterProperty, value);
			}
		}

		/// <summary>
		/// Invoke method is called when the given routed event is fired.
		/// </summary>
		/// <param name=”parameter”>
		/// Parameter is the sender of the event.
		/// </param>
		protected override void Invoke(object parameter)
		{
			if (this.Command != null)
			{
				if (this.Command.CanExecute(this.CommandParameter))
				{
					this.Command.Execute(this.CommandParameter);
				}
			}
		}
	}
}
