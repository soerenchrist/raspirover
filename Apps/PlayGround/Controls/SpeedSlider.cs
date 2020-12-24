using System;
using Xamarin.Forms;

namespace PlayGround.Controls
{
    public class SpeedSlider : Grid
    {
        #region Bindable Properties
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(SpeedSlider), 0.0, BindingMode.TwoWay, propertyChanged: ValuePropertyChanged);

        private static void ValuePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SpeedSlider view)
            {
                var height = view.Height / 2;
                var percentage = (double)newvalue;
                var realHeight = height * percentage / 100;
                var oldPercentage = (double)oldvalue;
                var oldHeight = height * oldPercentage / 100;

                if (realHeight > 0)
                {
                    var animation = new Animation(d => view._positiveBoxView.HeightRequest = d, oldHeight > 0 ? oldHeight : 0, realHeight, Easing.Linear);
                    animation.Commit(view._positiveBoxView, "AnimatePositiveBox", 16, 100);

                    var negativeAnimation = new Animation(d => view._negativeBoxView.HeightRequest = d, oldHeight < 0 ? oldHeight : 0, 0, Easing.Linear);
                    negativeAnimation.Commit(view._negativeBoxView, "AnimateNegativeBox", 16, 100);
                    return;
                }
                if (realHeight < 0)
                {

                    var animation = new Animation(d => view._positiveBoxView.HeightRequest = d, oldHeight > 0 ? oldHeight : 0, 0, Easing.Linear);
                    animation.Commit(view._positiveBoxView, "AnimatePositiveBox", 16, 100);

                    var negativeAnimation = new Animation(d => view._negativeBoxView.HeightRequest = d, oldHeight < 0 ? oldHeight * -1 : 0, realHeight * -1, Easing.Linear);
                    negativeAnimation.Commit(view._negativeBoxView, "AnimateNegativeBox", 16, 100);
                    return;
                }

                if (oldHeight > 0)
                {
                    var animation = new Animation(d => view._positiveBoxView.HeightRequest = d, oldHeight, 0, Easing.Linear);
                    animation.Commit(view._positiveBoxView, "AnimatePositiveBox", 16, 100);
                }

                if (oldHeight < 0)
                {
                    var negativeAnimation = new Animation(d => view._negativeBoxView.HeightRequest = d, oldHeight * -1, 0, Easing.Linear);
                    negativeAnimation.Commit(view._negativeBoxView, "AnimateNegativeBox", 16, 100);
                }
            }
        }

        public static readonly BindableProperty PositiveColorProperty = BindableProperty.Create(nameof(PositiveColor), typeof(Color), typeof(SpeedSlider), Color.Green, propertyChanged: PositiveColorChanged);
        public Color PositiveColor {
            get => (Color)GetValue(PositiveColorProperty);
            set => SetValue(PositiveColorProperty, value);
        }

        private static void PositiveColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SpeedSlider view)
                view._positiveBoxView.BackgroundColor = (Color)newvalue;
        }

        public static readonly BindableProperty NegativeColorProperty = BindableProperty.Create(nameof(NegativeColor), typeof(Color), typeof(SteerSlider), Color.Green, propertyChanged: NegativeColorChanged);

        private static void NegativeColorChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is SpeedSlider view)
                view._negativeBoxView.BackgroundColor = (Color)newvalue;
        }

        public Color NegativeColor {
            get => (Color)GetValue(NegativeColorProperty);
            set => SetValue(NegativeColorProperty, value);
        }

        public double Value {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        private readonly BoxView _positiveBoxView;
        private readonly BoxView _negativeBoxView;
        public SpeedSlider()
        {
            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += PanGestureRecognizerOnPanUpdated;
            GestureRecognizers.Add(panGestureRecognizer);
            RowSpacing = 0.0;
            RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Star
            });
            RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Star
            });

            _positiveBoxView = new BoxView
            {
                BackgroundColor = PositiveColor,
                HeightRequest = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };

            _negativeBoxView = new BoxView
            {
                BackgroundColor = NegativeColor,
                HeightRequest = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };
            SetRow(_positiveBoxView, 0);
            SetRow(_negativeBoxView, 1);
            Children.Add(_positiveBoxView);
            Children.Add(_negativeBoxView);

            var divider = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromHex("#eeeeee"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            SetRow(divider, 0);
            SetRowSpan(divider, 2);
            Children.Add(divider);
        }

        private double _lastValue;
        private double _currentValue;

        private void PanGestureRecognizerOnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (!IsEnabled)
                return;
            if (e.StatusType == GestureStatus.Completed)
            {
                Value = 0;
            }
            if (e.StatusType == GestureStatus.Started)
            {
                _lastValue = 0;
                _currentValue = 0;
            }

            if (e.StatusType != GestureStatus.Running)
                return;

            _currentValue -= e.TotalY - _lastValue;
            _lastValue = e.TotalY;

            var percentage = CalculatePercentage(this, _currentValue);

            Value = percentage;
        }
        private static double CalculatePercentage(SpeedSlider view, double value)
        {
            var totalHeight = view.Height;
            var height = totalHeight / 2;

            if (value > 0)
            {
                var perc = (value / height * 100);
                return Math.Min(perc, 100);
            }

            if (value < 0)
            {
                var perc = (value / height * 100);
                return Math.Max(perc, -100);
            }

            return 0;
        }

    }
}
