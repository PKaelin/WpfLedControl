![MultipleControls.png](https://github.com/PKaelin/WpfLedControl/blob/master/Docu/MultipleControls.png)
## Introduction

We needed a control for a WPF windows infrastructure library that visually showed us the state of different components within our application. The most obvious thing that popped into my mind was a traffic signal that, per default, showed me three LEDs (green, orange, red) that indicate the components state (Ok, Warning, Error) of which only one LED could be active.  

For pleasing the end user, we wanted to make them animated. For my own sake, I wanted to make them animated, configurable, not using images and not using resources. Therefore, I created an LED control with control creation and animation functionality in code behind.

## Using the code

For the sake of good practice, I have created two assemblies. One that simulates the common infrastructure library that contains the "LedControl" and one assembly that shows the control within a WPF application. I usually use Prism for composite pattern, injections, MVVM, etc. but to keep this example simple I have created two classes in the MVVM folder and assigned the ViewModel directly within the MainWindow. Not a good practice but I wanted to keep the sample simple. The LED control was created with a certain contract in mind and was therefore only tested within that contract.

## Controls properties

The control has the following configurable dependency properties:  

**List<Color> Leds**  
A list of colors that is used to display LEDs in those defined colors. The default is green, orange and red.  
Images below show the default LEDs in their on/off mode

![Green](https://github.com/PKaelin/WpfLedControl/blob/master/Docu/Green.png)    ![Orange](https://github.com/PKaelin/WpfLedControl/blob/master/Docu/Orange.png)   ![Red](https://github.com/PKaelin/WpfLedControl/blob/master/Docu/Red.png)

**double OffOpacity**  
The opacity of the LED color in off mode. The default is 0.4.  

**int ActiveLed**  
Active index of the LED. Eg. 0=nothing active, 1=first LED within the LEDs list is active, 2=second, etc.  

**Orientation LedOrientation**  
The orientation of the LEDs. Horizontal or Vertical. The default is horizontal.

<div style="color: crimson;">The importance of the orientation is that one should define a height on the custom control when the orientation is horizontal and a width if the orientation is vertical. This gets used to control the size of the LEDs.  
If not set the custom control will use the size available.</div>

The control uses a StackPanel that contains a list of Ellipse (LED) controls. The brush of the ellipse is used to display the color of the LED and to simulate the on/off behavior.  

The loading of the LEDs happens after the control is loaded so that we know the sizes of the framework elements. The loaded event gets registered in the constructor

<pre lang="cs">        public LedControl()
        {
            Loaded += LoadLeds;
        }
</pre>

The control creation happens in the LoadLeds method that is called either after the control is loaded or the property Leds has changed.

<pre lang="cs">        private void LoadLeds(object sender, RoutedEventArgs e)
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
            // Create LED for each defined color in Leds
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

                if (size  <= 50)
                {
                    ellipse.StrokeThickness = 5;
                }
                else if (size  <= 100)
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
                // ellipse.Fill is used as animation target
                ellipse.Fill = rgb;
                ellipse.Fill.Opacity = OffOpacity;
                panel.Children.Add(ellipse);
                ellipses.Add(ellipse);
            }

            LedOn();
        }
</pre>

The two methods LedOn()

<pre lang="cs">        private void LedOn()
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
</pre>

and LedOff()

<pre lang="cs">        private void LedOff()
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1.0d;
            animation.To = OffOpacity;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.AutoReverse = false;

            for (int i = 0; i  < ellipses.Count; i++)
            {
                if ((ActiveLed - 1 != i) && (ellipses[i].Fill.Opacity > OffOpacity))
                {
                    ellipses[i].Fill.BeginAnimation(Brush.OpacityProperty, animation);
                }
            }
        }
</pre>

run an animation (after ActiveLed has changed) by changing the opacity, on the ellipses brush. The check for the ActiveLed and the current Opacity is done so we don’t switch on and off the same LED at the same time.

## How to use the control

In the MainWindowxaml you can find some example of how to use the control and to test your own creations. Some are as follow:

<pre lang="xml">    <controls:LedControl ActiveLed="{Binding ActiveLedItem1}" LedOrientation="Vertical" Width="50" Margin="20" />
    <controls:LedControl ActiveLed="{Binding ActiveLedItem1}" Height="125" Margin="20" />
    <controls:LedControl ActiveLed="{Binding ActiveLedItem1}" LedOrientation="Vertical" Width="50" Leds="{Binding Colors}" Margin="20" />
</pre>

