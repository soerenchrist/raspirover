using System;
using Xamarin.Forms;

namespace PlayGround.Controls
{
    public class SteerSlider : Grid
    {
        #region Bindable Properties
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(SteerSlider), 0.0, BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);

        private static void ValuePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SteerSlider view)
            {
                var height = view.Height / 2;
                var percentage = (double)newvalue;
                var realHeight = height * percentage / 100;

                view._label.Text = ((int)percentage).ToString();

                if (realHeight > 0)
                {
                    view._rightBoxView.WidthRequest = realHeight;
                    view._leftBoxView.WidthRequest = 0;
                    return;
                }
                if (realHeight < 0)
                {
                    view._leftBoxView.WidthRequest = realHeight * -1;
                    view._rightBoxView.WidthRequest = 0;
                    return;
                }

                view._leftBoxView.WidthRequest = 0;
                view._rightBoxView.WidthRequest = 0;
            }
        }
        public static readonly BindableProperty LeftColorProperty = BindableProperty.Create(nameof(LeftColor), typeof(Color), typeof(SteerSlider), Color.Red, propertyChanged: LeftColorChanged);
        public Color LeftColor {
            get => (Color)GetValue(LeftColorProperty);
            set => SetValue(LeftColorProperty, value);
        }

        private static void LeftColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SteerSlider view)
                view._leftBoxView.BackgroundColor = (Color)newvalue;
        }

        public static readonly BindableProperty RightColorProperty = BindableProperty.Create(nameof(RightColor), typeof(Color), typeof(SteerSlider), Color.Green, propertyChanged: RightColorChanged);

        private static void RightColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SteerSlider view)
                view._rightBoxView.BackgroundColor = (Color)newvalue;
        }

        public Color RightColor {
            get => (Color)GetValue(RightColorProperty);
            set => SetValue(RightColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SteerSlider), Color.White, propertyChanged: TextColorChanged);

        private static void TextColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SteerSlider view)
                view._label.TextColor = (Color)newvalue;
        }

        public Color TextColor {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }


        private static int CalculatePercentage(SteerSlider view, double value)
        {
            var totalHeight = view.Width;
            var center = totalHeight / 2;

            if (value > 0)
            {
                var perc = (int)(value / center * 100);
                return Math.Min(perc, 100);
            }

            if (value < 0)
            {
                var perc = (int)(value / center * 100);
                return Math.Max(perc, -100);
            }

            return 0;
        }


        public double Value {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        private readonly BoxView _rightBoxView;
        private readonly BoxView _leftBoxView;
        public SteerSlider()
        {
            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += PanGestureRecognizerOnPanUpdated;
            GestureRecognizers.Add(panGestureRecognizer);

            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });

            _rightBoxView = new BoxView
            {
                BackgroundColor = RightColor,
                WidthRequest = 0,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _leftBoxView = new BoxView
            {
                BackgroundColor = LeftColor,
                WidthRequest = 0,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            SetColumn(_rightBoxView, 1);
            SetColumn(_leftBoxView, 0);
            Children.Add(_rightBoxView);
            Children.Add(_leftBoxView);

            _label = new Label
            {
                FontSize = 30,
                TextColor = TextColor,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "0"
            };

            SetColumnSpan(_label, 2);
            SetColumn(_label, 0);
            Children.Add(_label);
        }

        private double _currentValue;
        private double _lastValue;
        private readonly Label _label;

        private void PanGestureRecognizerOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!IsEnabled)
                return;
            if (e.StatusType == GestureStatus.Started)
                _lastValue = 0;
            if (e.StatusType != GestureStatus.Running)
                return;

            _currentValue += e.TotalX - _lastValue;
            _lastValue = e.TotalX;

            var percentage = CalculatePercentage(this, _currentValue);

            Value = percentage;
        }

    }
}
