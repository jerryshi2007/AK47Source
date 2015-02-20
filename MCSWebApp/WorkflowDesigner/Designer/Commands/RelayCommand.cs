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
using System.Diagnostics;
using System.Threading;

namespace Designer.Commands
{
	public class RelayCommand<T> : ICommand
	{
		/// <summary>
		/// Occurs when changes occur that affect whether the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		Predicate<T> canExecute;
		Action<T> executeAction;
		bool canExecuteCache;

		/// <summary>
		/// 创建一个实例  <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="executeAction">The execute action.</param>
		public RelayCommand(Action<T> executeAction) : this(executeAction, null) { }

		/// <summary>
		/// 创建一个实例  <see cref="RelayCommand"/> class.
		/// </summary>
		/// <param name="executeAction">The execute action.</param>
		/// <param name="canExecute">The can execute.</param>
		public RelayCommand(Action<T> executeAction,
							   Predicate<T> canExecute)
		{
			this.executeAction = executeAction;
			this.canExecute = canExecute;
		}

		#region ICommand Members
		/// <summary>
		/// Defines the method that determines whether the command 
		/// can execute in its current state.
		/// </summary>
		/// <param name="parameter">
		/// Data used by the command. 
		/// If the command does not require data to be passed,
		/// this object can be set to null.
		/// </param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		public bool CanExecute(object parameter)
		{
			if (canExecute == null)
				return true;

			bool tempCanExecute = canExecute((T)parameter);

			if (canExecuteCache != tempCanExecute)
			{
				canExecuteCache = tempCanExecute;
				if (CanExecuteChanged != null)
				{
					CanExecuteChanged(this, EventArgs.Empty);
				}
			}

			return canExecuteCache;
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">
		/// Data used by the command. 
		/// If the command does not require data to be passed, 
		/// this object can be set to null.
		/// </param>
		public void Execute(object parameter)
		{
			executeAction((T)parameter);
		}
		#endregion
	}
}
