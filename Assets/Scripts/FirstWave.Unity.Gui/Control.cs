using FirstWave.Unity.Core.Utilities;
using FirstWave.Unity.Gui.Data;
using FirstWave.Unity.Gui.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstWave.Unity.Gui
{
	// Base class for all controls, contains the common properties such as location, size, alignments, etc
	public abstract class Control
	{
		private static readonly Type OBJECT_TYPE = typeof(object);

		#region Dependency Properties

		public static readonly DependencyProperty VisibilityProperty =
			DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Control), new PropertyMetadata(Visibility.Visible));

		public static readonly DependencyProperty HorizontalAlignmentProperty =
			DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment?), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty VerticalAlignmentProperty =
			DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment?), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty PaddingProperty =
			DependencyProperty.Register("Padding", typeof(Thickness), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty MarginProperty =
			DependencyProperty.Register("Margin", typeof(Thickness), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.Register("DataContext", typeof(object), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty StyleProperty =
			DependencyProperty.Register("Style", typeof(Style), typeof(Control), new PropertyMetadata(null));

		public static readonly DependencyProperty EnabledProperty =
			DependencyProperty.Register("Enabled", typeof(bool), typeof(Control), new PropertyMetadata(true));

		#endregion

		private IDictionary<string, object> dependencyPropertyValues;

		public Vector2? Location { get; set; }

		/// <summary>
		/// If set, Measure() is not called
		/// </summary>
		public Vector2? Size;

		public Control Parent { get; internal set; }

		#region Properties

		public Visibility Visibility
		{
			get { return (Visibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}

		public HorizontalAlignment? HorizontalAlignment
		{
			get { return (HorizontalAlignment?)GetValue(HorizontalAlignmentProperty); }
			set { SetValue(HorizontalAlignmentProperty, value); }
		}

		public VerticalAlignment? VerticalAlignment
		{
			get { return (VerticalAlignment?)GetValue(VerticalAlignmentProperty); }
			set { SetValue(VerticalAlignmentProperty, value); }
		}

		public Thickness Padding
		{
			get { return (Thickness)GetValue(PaddingProperty) ?? Thickness.ZERO; }
			set { SetValue(PaddingProperty, value); }
		}

		public Thickness Margin
		{
			get { return (Thickness)GetValue(MarginProperty) ?? Thickness.ZERO; }
			set { SetValue(MarginProperty, value); }
		}

		public object DataContext
		{
			get { return GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}

		public Style Style
		{
			get { return (Style)GetValue(StyleProperty); }
			set { SetValue(StyleProperty, value); }
		}

		public bool Enabled
		{
			get { return (bool)GetValue(EnabledProperty); }
			set { SetValue(EnabledProperty, value); }
		}

		public string Name { get; set; }

		#endregion

		public Control()
		{
			dependencyPropertyValues = new Dictionary<string, object>();

			InitDependencyProperties();
		}

		#region Dependency Property Helpers

		protected void SetValue(DependencyProperty property, object value)
		{
			if (property == null)
				throw new ArgumentNullException("You must supply a DependencyProperty object.");

			object oldValue = GetValue(property);

			if (!Equals(oldValue, value))
			{
                if (dependencyPropertyValues[property.Name] is Binding)
                    (dependencyPropertyValues[property.Name] as Binding).UpdateSource(value);
                else
				    dependencyPropertyValues[property.Name] = value;

				// If the property is supposed to recalculate the the layout of the control call the InvalidateLayout
				if (property.Metadata != null && property.Metadata.AffectsLayout)
					InvalidateLayout(this);

				// Call the change handler for the property if one exists
				if (property.Metadata.OnChangeHandler != null)
					property.Metadata.OnChangeHandler(this, oldValue, value);
			}
		}

		protected object GetValue(DependencyProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("You must supply a DependencyProperty object.");

			if (!dependencyPropertyValues.ContainsKey(property.Name))
				return property.PropType.Default();

			// If we set a binding on this property, then we need to query the binding for the value, not the pass back the binding
			var obj = dependencyPropertyValues[property.Name];
			if (obj is Binding)
				return (obj as Binding).GetValue();

			// Otherwise return what's here
			return obj;
		}

		#endregion

		#region Binding Helpers

		public void SetBinding(DependencyProperty property, Binding binding)
		{
			if (property == null)
				throw new ArgumentNullException("You must supply a DependencyProperty object.");

			// We don't want to go through SetValue because we're not passing in a concrete object, we're passing in an object
			// that know's how to retrieve the item of that type.
			dependencyPropertyValues[property.Name] = binding;
		}

		// Right now this is needed because I'm still having that weird issue with the System.Type hashcode not matching up in the Xaml processor
		// I'd prefer to remove this and just let the typing handle everything
		internal DependencyProperty GetDependencyProperty(string name)
		{
			var type = GetType();

			do
			{
				if (DependencyProperty.registeredPropertiesByOwner.ContainsKey(type))
				{
					var depProps = DependencyProperty.registeredPropertiesByOwner[type];
					foreach (var dp in depProps)
					{
						if (dp.Name == name)
							return dp;
					}
	            }

				type = type.BaseType;
			} while (type != OBJECT_TYPE);

			return null;
		}

		internal virtual void ResolveDataContext(object viewModel)
		{
			if (Parent == null)
				DataContext = viewModel;
			else if (dependencyPropertyValues[DataContextProperty.Name] != null)
			{
				// These should auto-resolve
			}
			else
				DataContext = Parent.DataContext;
		}

		#endregion

		#region UPF Methods

		public abstract Vector2 Measure();
		public abstract void Layout(Rect r);
		public abstract void Draw();

		#endregion

		#region Layout Helpers

		protected float GetStartingXCoordinate(Rect r)
		{
			float x = r.x;

			if (!HorizontalAlignment.HasValue || HorizontalAlignment == Enums.HorizontalAlignment.Left || HorizontalAlignment == Enums.HorizontalAlignment.Stretch)
			{
				x = r.x + Margin.Left;
			}
			else if (HorizontalAlignment == Enums.HorizontalAlignment.Right)
			{
				x = r.x + r.width - Margin.Right - (Size.HasValue ? Size.Value.x : 0);
			}
			else if (HorizontalAlignment == Enums.HorizontalAlignment.Center)
			{
				x = r.x + r.width / 2 - (Size.HasValue ? Size.Value.x : 0) / 2;
			}

			return x;
		}

		protected float GetStartingYCoordinate(Rect r)
		{
			float y = r.y;

			if (!VerticalAlignment.HasValue || VerticalAlignment == Enums.VerticalAlignment.Top || VerticalAlignment == Enums.VerticalAlignment.Stretch)
			{
				y = r.y + Margin.Top;
			}
			else if (VerticalAlignment == Enums.VerticalAlignment.Bottom)
			{
				y = r.y + r.height - (Size.HasValue ? Size.Value.y : 0) - Margin.Bottom;
			}
			else if (VerticalAlignment == Enums.VerticalAlignment.Center)
			{
				y = r.y + r.height / 2 - (Size.HasValue ? Size.Value.y : 0) / 2;
			}

			return y;
		}

		internal virtual void InvalidateLayout(Control source)
		{
			Size = null;
			Location = null;

			if (this != source)
				return;

			// Now let's invalidate the entire branch of this tree so we can properly recalculate/redraw
			var ctrl = Parent;
			while (ctrl != null)
			{
				ctrl.InvalidateLayout(this);
				ctrl = ctrl.Parent;
			}
		}

		#endregion

		#region Input Handlers

		protected virtual void OnKeyDown(string key)
		{
		}

		protected virtual void OnKeyPressed(string key)
		{
		}

		protected virtual void OnKeyReleased(string key)
		{
		}

		#endregion

		private void InitDependencyProperties()
		{
			var type = GetType();

			do
			{
				if (DependencyProperty.registeredPropertiesByOwner.ContainsKey(type))
				{
					var depProps = DependencyProperty.registeredPropertiesByOwner[type];
					foreach (var dp in depProps)
					{
						dependencyPropertyValues.Add(dp.Name, dp.Metadata.DefaultValue);
					}
				}

				type = type.BaseType;

			} while (type != OBJECT_TYPE);
		}
	}
}
