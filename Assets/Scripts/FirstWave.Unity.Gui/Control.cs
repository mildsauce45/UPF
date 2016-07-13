using FirstWave.Messaging;
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

        #endregion

        public Control()
        {
            dependencyPropertyValues = new Dictionary<string, object>();

            InitDependencyProperties();
        }

        protected void SetValue(DependencyProperty property, object value)
        {
            if (property == null)
                throw new ArgumentNullException("You must supply a DependencyProperty object.");

            object oldValue = GetValue(property);

            if (!Equals(oldValue, value))
            {
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

            return dependencyPropertyValues.ContainsKey(property.Name) ? dependencyPropertyValues[property.Name] : property.Metadata.DefaultValue;
        }

        public abstract Vector2 Measure();
        public abstract void Layout(Rect r);
        public abstract void Draw();

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

        protected virtual void OnKeyDown(string key)
        {
        }

        protected virtual void OnKeyPressed(string key)
        {
        }

        protected virtual void OnKeyReleased(string key)
        {
        }

        internal virtual void InvalidateLayout(Control source)
        {
            Size = null;
            Location = null;

            if (this != source)
                return;

            // Now let's invalidate the entire leaf of this tree so we can properly recalculate/redraw
            var ctrl = Parent;
            while (ctrl != null)
            {
                ctrl.InvalidateLayout(this);
                ctrl = ctrl.Parent;
            }
        }

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
