using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Louis
{
    public class RevealBorderBrushExtension : MarkupExtension
    {
        [ThreadStatic]
        private static Dictionary<RadialGradientBrush, WeakReference<FrameworkElement>> _globalRevealingElement;

        /// <summary>
        /// The Color to use for rendering in case the <see cref="MarkupExtension"/> can't work correctly.
        /// </summary>
        public Color FallbackColor { get; set; } = Colors.White;

        /// <summary>
        /// Gets or sets a value that specifies the base background color for the bursh.
        /// </summary>
        public Color Color { get; set; } = Colors.White;

        public Transform Transform { get; set; } = Transform.Identity;

        public Transform RelativeTransform { get; set; } = Transform.Identity;

        public double Opacity { get; set; } = 1.0;

        public double Radius { get; set; } = 100.0;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)) return null;
            if (service.TargetObject.GetType().Name.EndsWith("SharedDp")) return this;
            if (!(service.TargetObject is FrameworkElement element)) return this;
            if (DesignerProperties.GetIsInDesignMode(element)) return new SolidColorBrush();

            var brush = CreateGlobalBrush(element);
            return brush;
        }

        private Brush CreateBrush(UIElement rootVisual, FrameworkElement element)
        {
            var brush = CreateRadialGradientBrush();
            rootVisual.MouseMove += OnMouseMove;
            return brush;

            void OnMouseMove(object sender, MouseEventArgs e)
            {
                UpdateBrush(brush, e.GetPosition(element));
            }
        }

        private Brush CreateGlobalBrush(FrameworkElement element)
        {
            var brush = CreateRadialGradientBrush();
            if (_globalRevealingElement is null)
            {
                CompositionTarget.Rendering -= OnRendering;
                CompositionTarget.Rendering += OnRendering;
                _globalRevealingElement = new Dictionary<RadialGradientBrush, WeakReference<FrameworkElement>>();
            }

            _globalRevealingElement.Add(brush, new WeakReference<FrameworkElement>(element));
            return brush;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (_globalRevealingElement is null)
            {
                return;
            }

            var toCollect = new List<RadialGradientBrush>();
            foreach (var pair in _globalRevealingElement)
            {
                var brush = pair.Key;
                var weak = pair.Value;
                if (weak.TryGetTarget(out var element))
                {
                    Reveal(brush, element);
                }
                else
                {
                    toCollect.Add(brush);
                }
            }

            foreach (var brush in toCollect)
            {
                _globalRevealingElement.Remove(brush);
            }

            void Reveal(RadialGradientBrush brush, FrameworkElement element)
            {
                UpdateBrush(brush, Mouse.GetPosition(element));
            }
        }

        private void UpdateBrush(RadialGradientBrush brush, Point origin)
        {
            IInputElement element;
            if (IsUsingMouseOrStylus())
            {
                brush.GradientOrigin = origin;
                brush.Center = origin;
            }
            else
            {
                brush.Center = new Point(double.NegativeInfinity, double.NegativeInfinity);
            }
        }

        private RadialGradientBrush CreateRadialGradientBrush()
        {
            var brush = new RadialGradientBrush(Color, Colors.Transparent)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = Radius,
                RadiusY = Radius,
                Opacity = Opacity,
                Transform = Transform,
                RelativeTransform = RelativeTransform,
                Center = new Point(double.NegativeInfinity, double.NegativeInfinity),
            };
            return brush;
        }

        private bool IsUsingMouseOrStylus()
        {
            var device = Stylus.CurrentStylusDevice;
            if (device is null)
            {
                return true;
            }

            if (device.TabletDevice.Type == TabletDeviceType.Stylus)
            {
                return true;
            }

            return false;
        }
    }
}