using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Media;

namespace Designer.Commands
{
	public class SetProperty : TargetedTriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(SetProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(SetProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty EaseProperty = DependencyProperty.Register("Ease", typeof(IEasingFunction), typeof(SetProperty), new PropertyMetadata(null));
		public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(Duration), typeof(SetProperty), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.25))));
		public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(bool), typeof(SetProperty), new PropertyMetadata(null));

		public object Value
		{
			get
			{
				return base.GetValue(SetProperty.ValueProperty);
			}
			set
			{
				base.SetValue(SetProperty.ValueProperty, value);
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)base.GetValue(SetProperty.PropertyNameProperty);
			}
			set
			{
				base.SetValue(SetProperty.PropertyNameProperty, value);
			}
		}

		public IEasingFunction Ease
		{
			get
			{
				return (IEasingFunction)base.GetValue(SetProperty.EaseProperty);
			}
			set
			{
				base.SetValue(SetProperty.EaseProperty, value);
			}
		}

		public Duration Duration
		{
			get
			{
				return (Duration)base.GetValue(SetProperty.DurationProperty);
			}
			set
			{
				base.SetValue(SetProperty.DurationProperty, value);
			}
		}

		public bool Increment
		{
			get
			{
				return (bool)base.GetValue(SetProperty.IncrementProperty);
			}
			set
			{
				base.SetValue(SetProperty.IncrementProperty, value);
			}
		}

		/// <summary>
		/// Sets the property
		/// </summary>
		/// <param name="parameter">Unused.</param>
		protected override void Invoke(object parameter)
		{
			if (!string.IsNullOrEmpty(this.PropertyName) && (this.Target != null))
			{
				Type c = this.Target.GetType();
				PropertyInfo property = c.GetProperty(this.PropertyName);
				if (property == null)
					throw new ArgumentException("Cannot find property " + this.PropertyName + " on object " + this.Target.GetType().Name);

				if (!property.CanWrite)
					throw new ArgumentException("Property is read-only " + this.PropertyName + " on object " + this.Target.GetType().Name);

				object toValue = this.Value;
				TypeConverter typeConverter = ConverterHelper.GetTypeConverter(property.PropertyType);
				Exception innerException = null;
				try
				{
					if (((typeConverter != null) && (this.Value != null)) && typeConverter.CanConvertFrom(this.Value.GetType()))
						toValue = typeConverter.ConvertFrom(this.Value);

					object fromValue = property.GetValue(this.Target, null);

					if (this.Increment)
						toValue = SetProperty.Add(toValue, fromValue);

					if (this.Duration.HasTimeSpan)
					{
						Timeline timeline;
						if ((typeof(FrameworkElement).IsAssignableFrom(c) && ((this.PropertyName == "Width") || (this.PropertyName == "Height"))) && double.IsNaN((double)fromValue))
						{
							FrameworkElement target = (FrameworkElement)this.Target;
							if (this.PropertyName == "Width")
								fromValue = target.ActualWidth;
							else
								fromValue = target.ActualHeight;
						}

						Storyboard storyboard = new Storyboard();
						if (typeof(double).IsAssignableFrom(property.PropertyType))
						{
							DoubleAnimation animation = new DoubleAnimation();
							animation.From = new double?((double)fromValue);
							animation.To = new double?((double)toValue);
							animation.EasingFunction = this.Ease;
							timeline = animation;
						}
						else if (typeof(Color).IsAssignableFrom(property.PropertyType))
						{
							ColorAnimation animation2 = new ColorAnimation();
							animation2.From = new Color?((Color)fromValue);
							animation2.To = new Color?((Color)toValue);
							animation2.EasingFunction = this.Ease;
							timeline = animation2;
						}
						else if (typeof(Point).IsAssignableFrom(property.PropertyType))
						{
							PointAnimation animation3 = new PointAnimation();
							animation3.From = new Point?((Point)fromValue);
							animation3.To = new Point?((Point)toValue);
							animation3.EasingFunction = this.Ease;
							timeline = animation3;
						}
						else
						{
							ObjectAnimationUsingKeyFrames frames = new ObjectAnimationUsingKeyFrames();
							DiscreteObjectKeyFrame frame = new DiscreteObjectKeyFrame();
							frame.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0L));
							frame.Value = fromValue;
							DiscreteObjectKeyFrame frame2 = new DiscreteObjectKeyFrame();
							frame2.KeyTime = KeyTime.FromTimeSpan(this.Duration.TimeSpan);
							frame2.Value = toValue;
							frames.KeyFrames.Add(frame);
							frames.KeyFrames.Add(frame2);
							timeline = frames;
						}
						timeline.Duration = this.Duration;
						storyboard.Children.Add(timeline);
						Storyboard.SetTarget(storyboard, this.Target);
						Storyboard.SetTargetProperty(storyboard, new PropertyPath(property.Name, new object[0]));
						storyboard.Begin();
					}
					else
						property.SetValue(this.Target, toValue, new object[0]);
				}
				catch (FormatException exception2)
				{
					innerException = exception2;
				}
				catch (ArgumentException exception3)
				{
					innerException = exception3;
				}
				catch (MethodAccessException exception4)
				{
					innerException = exception4;
				}

				if (innerException != null)
					throw new ArgumentException(innerException.Message);

			}
		}

		private static object Add(object a, object b)
		{
			if (a.GetType() != b.GetType())
			{
				throw new Exception("Types must match");
			}
			Type type = a.GetType();
			if (type == typeof(double))
			{
				return (double)a + (double)b;
			}
			if (type == typeof(int))
			{
				return (int)a + (int)b;
			}
			if (type == typeof(string))
			{
				return (string)a + (string)b;
			}
			if (type == typeof(float))
			{
				return (float)a + (float)b;
			}
			MethodInfo method = type.GetMethod("op_Addition");
			if (method != null)
			{
				return method.Invoke(null, new object[]
				{
					a,
					b
				});
			}
			throw new Exception("Unable to add type " + type);
		}
	}
}
