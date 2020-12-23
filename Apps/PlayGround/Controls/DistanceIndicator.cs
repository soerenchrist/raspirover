using System;
using Xamarin.Forms;

namespace PlayGround.Controls
{
    public class DistanceIndicator : Grid
    {
        #region BindableProperties

        public static readonly BindableProperty DistanceProperty = BindableProperty.Create(nameof(Distance), typeof(double), typeof(DistanceIndicator), 0.0, BindingMode.TwoWay, propertyChanged: DistancePropertyChanged);

        private static void DistancePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DistanceIndicator control)
            {
                var value = (double)newvalue;
                var oldValue = (double)oldvalue;
                var animation = new Animation(d => control._boxView.WidthRequest = d, oldValue, value, Easing.Linear);
                animation.Commit(control._boxView, "AnimateSize", 16, 200);
                var text = $"{Math.Round(value, 1)}cm";
                control._label.Text = text;
            }
        }
        public double Distance {
            get => (double)GetValue(DistanceProperty);
            set => SetValue(DistanceProperty, value);
        }

        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(nameof(MaxValue), typeof(double), typeof(DistanceIndicator), 100.0, BindingMode.OneWay, propertyChanged: MaxValuePropertyChanged);

        private static void MaxValuePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DistanceIndicator control)
            {
                control._boxView.WidthRequest = control.CalculateBoxWidth(control.Distance, (double)newvalue);
            }
        }
        public double MaxValue {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly BindableProperty IndicatorColorProperty =
            BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(DistanceIndicator), Color.Blue,
                propertyChanged: IndicatorColorPropertyChanged);

        private static void IndicatorColorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is DistanceIndicator control)
            {
                control._boxView.BackgroundColor = (Color)newvalue;
            }
        }

        public Color IndicatorColor {
            get => (Color)GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        #endregion

        private readonly BoxView _boxView;
        private readonly Label _label;

        public DistanceIndicator()
        {
            _boxView = new BoxView
            {
                BackgroundColor = IndicatorColor,
                WidthRequest = 0,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Start
            };

            _label = new Label
            {
                TextColor = Color.White,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "0cm"
            };

            Children.Add(_boxView);
            Children.Add(_label);
        }

        private double CalculateBoxWidth(double value, double max)
        {
            if (value >= max) return Width;
            var percentage = value / max;

            return Width * percentage;
        }
    }
}
