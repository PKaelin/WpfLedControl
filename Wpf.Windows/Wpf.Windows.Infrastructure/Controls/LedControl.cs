using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wpf.Windows.Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for LedControl
    /// </summary>
    /// <example>
    /// controls:LedControl ActiveLed="{Binding ActiveLedItem}" Height="60"
    /// controls:LedControl ActiveLed="{Binding ActiveLedItem}" Leds="{Binding Colors}" Height="60" 
    /// controls:LedControl ActiveLed="{Binding ActiveLedItem}" Leds="{Binding Colors}" LedOrientation="Vertical" Width="60" 
    /// </example>
    public class LedControl : ContentControl
    {

        /// <summary>
        /// Ellipses controls
        /// </summary>
        private List<Ellipse> ellipses = new List<Ellipse>();


        /// <summary>
        /// Orientation of the leds
        /// </summary>
        public Orientation LedOrientation
        {
            get { return (Orientation)GetValue(LedOrientationProperty); }
            set { SetValue(LedOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LedOrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LedControl), new PropertyMetadata(Orientation.Horizontal));


        /// <summary>
        /// Colors of the leds in on mode. Amount of colors equal the amount of leds displayed
        /// </summary>
        public List<Color> Leds
        {
            get { return (List<Color>)GetValue(LedsProperty); }
            set { SetValue(LedsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Leds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LedsProperty =
            DependencyProperty.Register("Leds", typeof(List<Color>), typeof(LedControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = new List<Color> { Colors.Green, Colors.Orange, Colors.Red },
                    PropertyChangedCallback = LedsChanged,
                });


        /// <summary>
        /// Opacity of led in off mode
        /// </summary>
        public double OffOpacity
        {
            get { return (double)GetValue(OffOpacityProperty); }
            set { SetValue(OffOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffOpacityProperty =
            DependencyProperty.Register("OffOpacity", typeof(double), typeof(LedControl), new PropertyMetadata(0.4));


        /// <summary>
        /// Ative index of the leds
        /// Value 0 = nothing active
        /// Value 1 = first led active
        /// Value 2 = second led active etc.
        /// </summary>
        public int ActiveLed
        {
            get { return (int)GetValue(ActiveLedProperty); }
            set { SetValue(ActiveLedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveLed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveLedProperty =
            DependencyProperty.Register("ActiveLed", typeof(int), typeof(LedControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true,
                    DefaultValue = 0,
                    PropertyChangedCallback = ActiveLedChanged
                });


        /// <summary>
        /// Constructor of the led control
        /// </summary>
        public LedControl()
        {
            Loaded += LoadLeds;
        }


        /// <summary>
        /// Property changed callback for LEDs
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs</param>
        private static void LedsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LedControl c = d as LedControl;
            c.Leds = (List<Color>)e.NewValue;
            c.LoadLeds(d, null);
        }


        /// <summary>
        /// Property changed callback for the active LED index
        /// </summary>
        /// <param name="d">The DependencyObject</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs</param>
        private static void ActiveLedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LedControl c = d as LedControl;
            c.ActiveLed = (int)e.NewValue;
            c.LedOff();
            c.LedOn();
        }


        /// <summary>
        /// Load led into the panel
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Routed event atguments</param>
        private void LoadLeds(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = Parent as FrameworkElement;
            StackPanel panel = new StackPanel();
            Content = panel;
            panel.Orientation = LedOrientation;
            panel.Children.Clear();
            ellipses.Clear();
            double size;

            if (LedOrientation == Orientation.Horizontal)
            {
                size = Height;
            }
            else
            {
                size = Width;
            }
            // Give it some size if forgotten to define width or height in combination with orientation
            if ((size.Equals(double.NaN)) && (parent != null) && (Leds.Count != 0))
            {
                if (parent.ActualWidth != double.NaN)
                {
                    size = parent.ActualWidth / Leds.Count;
                }
                else if (parent.ActualHeight != double.NaN)
                {
                    size = parent.ActualHeight / Leds.Count;
                }
            }

            foreach (Color color in Leds)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Height = size > 4 ? size - 4 : size;
                ellipse.Width = size > 4 ? size - 4 : size;
                ellipse.Margin = new Thickness(2);
                ellipse.Style = null;
                // Border for led
                RadialGradientBrush srgb = new RadialGradientBrush(new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(255, 211, 211, 211), 0.8d),
                    new GradientStop(Color.FromArgb(255, 169, 169, 169), 0.9d),
                    new GradientStop(Color.FromArgb(255, 150, 150, 150), 0.95d),
                });

                if (size <= 50)
                {
                    ellipse.StrokeThickness = 5;
                }
                else if (size <= 100)
                {
                    ellipse.StrokeThickness = 10;
                }
                else
                {
                    ellipse.StrokeThickness = 20;
                }

                srgb.GradientOrigin = new System.Windows.Point(0.5d, 0.5d);
                srgb.Center = new System.Windows.Point(0.5d, 0.5d);
                srgb.RadiusX = 0.5d;
                srgb.RadiusY = 0.5d;
                ellipse.Stroke = srgb;
                // Color of led
                RadialGradientBrush rgb = new RadialGradientBrush(new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb(150, color.R, color.G, color.B), 0.1d),
                    new GradientStop(Color.FromArgb(200, color.R, color.G, color.B), 0.4d),
                    new GradientStop(Color.FromArgb(255, color.R, color.G, color.B), 1.0d),
                });

                rgb.GradientOrigin = new System.Windows.Point(0.5d, 0.5d);
                rgb.Center = new System.Windows.Point(0.5d, 0.5d);
                rgb.RadiusX = 0.5d;
                rgb.RadiusY = 0.5d;
                ellipse.Fill = rgb;
                ellipse.Fill.Opacity = OffOpacity;
                panel.Children.Add(ellipse);
                ellipses.Add(ellipse);
            }

            LedOn();
        }

        /// <summary>
        /// Switch on the active led
        /// </summary>
        private void LedOn()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = OffOpacity;
            animation.To = 1.0d;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = false;

            for (int i = 0; i < ellipses.Count; i++)
            {
                if ((ActiveLed - 1 == i) && (ellipses[i].Fill.Opacity < 1.0))
                {
                    ellipses[i].Fill.BeginAnimation(Brush.OpacityProperty, animation);
                }
            }
        }

        /// <summary>
        /// Switch off all but the active led
        /// </summary>
        private void LedOff()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1.0d;
            animation.To = OffOpacity;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = false;

            for (int i = 0; i < ellipses.Count; i++)
            {
                if ((ActiveLed - 1 != i) && (ellipses[i].Fill.Opacity > OffOpacity))
                {
                    ellipses[i].Fill.BeginAnimation(Brush.OpacityProperty, animation);
                }
            }
        }

    }
}

