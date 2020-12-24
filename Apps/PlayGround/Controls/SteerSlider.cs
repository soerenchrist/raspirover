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
                var realHeight = height * percentage / 10;
                var oldPercentage = (double)oldvalue;
                var oldHeight = height * oldPercentage / 10;

                if (realHeight > 0)
                {
                    var animation = new Animation(d => view._rightBoxView.WidthRequest = d, oldHeight > 0 ? oldHeight : 0, realHeight, Easing.Linear);
                    animation.Commit(view._rightBoxView, "AnimateRightBox", 16, 100);

                    var leftAnimation = new Animation(d => view._leftBoxView.WidthRequest = d, oldHeight < 0 ? oldHeight : 0, 0, Easing.Linear);
                    leftAnimation.Commit(view._leftBoxView, "AnimateLeftBox", 16, 100);
                    return;
                }
                if (realHeight < 0)
                {

                    var animation = new Animation(d => view._rightBoxView.WidthRequest = d, oldHeight > 0 ? oldHeight : 0, 0, Easing.Linear);
                    animation.Commit(view._rightBoxView, "AnimateRightBox", 16, 100);

                    var leftAnimation = new Animation(d => view._leftBoxView.WidthRequest = d, oldHeight < 0 ? oldHeight * -1 : 0, realHeight * -1, Easing.Linear);
                    leftAnimation.Commit(view._leftBoxView, "AnimateLeftBox", 16, 100);
                    return;
                }

                if (oldHeight > 0)
                {
                    var animation = new Animation(d => view._rightBoxView.WidthRequest = d, oldHeight, 0, Easing.Linear);
                    animation.Commit(view._rightBoxView, "AnimateRightBox", 16, 100);
                }

                if (oldHeight < 0)
                {

                    var leftAnimation = new Animation(d => view._leftBoxView.WidthRequest = d, oldHeight * -1, 0, Easing.Linear);
                    leftAnimation.Commit(view._leftBoxView, "AnimateLeftBox", 16, 100);
                }

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


        private static int CalculatePercentage(SteerSlider view, double value)
        {
            var totalHeight = view.Width;
            var center = totalHeight / 2;

            if (value > 0)
            {
                var perc = (int)(value / center * 10);
                return Math.Min(perc, 10);
            }

            if (value < 0)
            {
                var perc = (int)(value / center * 10);
                return Math.Max(perc, -10);
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
            ColumnSpacing = 0;
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

            var divider = new BoxView
            {
                BackgroundColor = Color.FromHex("#eeeeee"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 1
            };
            SetColumn(divider, 0);
            SetColumnSpan(divider, 2);
            Children.Add(divider);

        }

        private double _currentValue;
        private double _lastValue;

        private void PanGestureRecognizerOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!IsEnabled)
                return;
            if (e.StatusType == GestureStatus.Completed)
            {
                Value = 0;
                _currentValue = 0;
                _lastValue = 0;
                return;
            }

            if (e.StatusType == GestureStatus.Started)
            {
                _lastValue = 0;
                _currentValue = 0;
            }

            if (e.StatusType != GestureStatus.Running)
                return;

            _currentValue += e.TotalX - _lastValue;
            _lastValue = e.TotalX;

            var percentage = CalculatePercentage(this, _currentValue);

            Value = percentage;
        }

    }
}
