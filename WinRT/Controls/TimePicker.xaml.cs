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
			DependencyProperty.Register("ShowSecond", typeof(bool), typeof(TimePicker), new PropertyMetadata(false, OnShowSecondChanged));

		private static void OnShowSecondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (TimePicker)d;
			ctrl.Second.Visibility = ctrl.ShowSecond ? Visibility.Visible : Visibility.Collapsed; 
		}


		public static readonly DependencyProperty AllowNullProperty =
			DependencyProperty.Register("AllowNull", typeof(bool), typeof(TimePicker), new PropertyMetadata(true));

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		// TODO: change to DateTime when fixed
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(Object), typeof(TimePicker), new PropertyMetadata(null, OnSelectedItemChanged));

        public Object SelectedItem
		{
            get { return (Object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

        public DateTime? SelectedDate
        {
            get { return (DateTime?)SelectedItem; }
            set { SelectedItem = value; }
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
            if (SelectedDate.HasValue)
			{
                UpdateValues(SelectedDate.Value.Year, SelectedDate.Value.Month);

                if (AllowNull)
                {
					Hour.SelectedIndex = SelectedDate.Value.Hour + 1;
					Minute.SelectedIndex = SelectedDate.Value.Minute + 1;
					Second.SelectedIndex = SelectedDate.Value.Second + 1;
                }
                else
                {
					Hour.SelectedIndex = SelectedDate.Value.Day;
					Minute.SelectedIndex = SelectedDate.Value.Month;
					Second.SelectedIndex = SelectedDate.Value.Year;
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

		private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (initializing)
				return;

			var year = SelectedItem != null ? SelectedDate.Value.Year : 0;
			var month = SelectedItem != null ? SelectedDate.Value.Month : 0;
			var day = SelectedItem != null ? SelectedDate.Value.Day : 0;

			initializing = true;
			if (AllowNull && (Hour.SelectedIndex == 0 || Minute.SelectedIndex == 0 || Second.SelectedIndex == 0))
				SelectedItem = null;
			else
			{
				if (AllowNull)
					SelectedItem = new DateTime(year, month, day, Hour.SelectedIndex - 1, Minute.SelectedIndex - 1, Second.SelectedIndex - 1);
				else
					SelectedItem = new DateTime(year, month, day, Hour.SelectedIndex, Minute.SelectedIndex, Second.SelectedIndex);
			}
				
			initializing = false; 
		}
	}
}
