using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public sealed partial class TimePicker
	{
		private bool initializing = true;
		public TimePicker()
		{
			InitializeComponent();

			UpdateValues(0, 0);
			Hour.SelectedIndex = 0;
			Minute.SelectedIndex = 0;
			Second.SelectedIndex = 0;
			initializing = false;

			Second.Visibility = Visibility.Collapsed; 
		}
		
		public bool ShowSecond
		{
			get { return (bool)GetValue(ShowSecondProperty); }
			set { SetValue(ShowSecondProperty, value); }
		}

		public static readonly DependencyProperty ShowSecondProperty =
			DependencyProperty.Register("ShowSecond", typeof(bool), typeof(TimePicker), 
			new PropertyMetadata(false, OnShowSecondChanged));

		private static void OnShowSecondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (TimePicker)d;
			ctrl.Second.Visibility = ctrl.ShowSecond ? Visibility.Visible : Visibility.Collapsed; 
		}


		public static readonly DependencyProperty AllowNullProperty =
			DependencyProperty.Register("AllowNull", typeof(bool), typeof(TimePicker), 
			new PropertyMetadata(true, OnSelectedItemChanged));

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register("SelectedTime", typeof(object), typeof(TimePicker), 
			new PropertyMetadata(null, OnSelectedItemChanged));

        public DateTime? SelectedTime
		{
            get { return (DateTime?)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
		}

		public event RoutedEventHandler SelectedItemChanged; 

		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (TimePicker)d;
			if (ctrl.initializing)
				return;

            ctrl.initializing = true;
			ctrl.UpdateDate();
            ctrl.initializing = false; 
            
			if (ctrl.SelectedItemChanged != null)
				ctrl.SelectedItemChanged(ctrl, new RoutedEventArgs());
		}

		public void UpdateDate()
		{
            if (SelectedTime.HasValue)
			{
                UpdateValues(SelectedTime.Value.Year, SelectedTime.Value.Month);

                if (AllowNull)
                {
					Hour.SelectedIndex = SelectedTime.Value.Hour + 1;
					Minute.SelectedIndex = SelectedTime.Value.Minute + 1;
					Second.SelectedIndex = SelectedTime.Value.Second + 1;
                }
                else
                {
					Hour.SelectedIndex = SelectedTime.Value.Hour;
					Minute.SelectedIndex = SelectedTime.Value.Minute;
					Second.SelectedIndex = SelectedTime.Value.Second;
                }
			}
			else
			{
				UpdateValues(0, 0);
				Hour.SelectedIndex = 0;
				Minute.SelectedIndex = 0;
				Second.SelectedIndex = 0;
			}
		}

		public void UpdateValues(int year, int month)
		{
			var hours = new List<string>();
			if (AllowNull)
				hours.Add(" ");
			for (var i = 0; i <= 23; i++)
				hours.Add(i.ToString());

			var minutes = new List<string>();
			if (AllowNull)
				minutes.Add(" ");
			for (var i = 0; i <= 59; i++)
				minutes.Add(i.ToString());

			var seconds = new List<string>();
			if (AllowNull)
				seconds.Add(" ");
			for (var i = 0; i <= 59; i++)
				seconds.Add(i.ToString());

			Hour.ItemsSource = hours;
			Minute.ItemsSource = minutes;
			Second.ItemsSource = seconds; 
		}

		private void OnUpdateTime(object sender, SelectionChangedEventArgs e)
		{
			if (initializing)
				return;

            var year = SelectedTime != null ? SelectedTime.Value.Year : DateTime.MinValue.Year;
			var month = SelectedTime != null ? SelectedTime.Value.Month : DateTime.MinValue.Month;
			var day = SelectedTime != null ? SelectedTime.Value.Day : DateTime.MinValue.Day;

			initializing = true;
			if (AllowNull && (Hour.SelectedIndex == 0 || Minute.SelectedIndex == 0 || (ShowSecond && Second.SelectedIndex == 0)))
                SelectedTime = null;
			else
			{
				if (AllowNull)
					SelectedTime = new DateTime(year, month, day, Hour.SelectedIndex - 1, Minute.SelectedIndex - 1, ShowSecond ? Second.SelectedIndex - 1 : 0);
				else
					SelectedTime = new DateTime(year, month, day, Hour.SelectedIndex, Minute.SelectedIndex, Second.SelectedIndex);
			}
				
			initializing = false; 
		}
	}
}
