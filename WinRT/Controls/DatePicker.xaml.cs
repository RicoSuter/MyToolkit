using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public sealed partial class DatePicker
	{
		private bool initializing = true; 
		public DatePicker()
		{
			InitializeComponent();

			UpdateValues(0, 0);
			Day.SelectedIndex = 0;
			Month.SelectedIndex = 0;
			Year.SelectedIndex = 0;
			initializing = false; 
		}

		public static readonly DependencyProperty AllowNullProperty =
			DependencyProperty.Register("AllowNull", typeof(bool), typeof(DatePicker), new PropertyMetadata(true));

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		// TODO (beta) change to DateTime when fixed
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(Object), typeof(DatePicker), new PropertyMetadata(null, OnSelectedItemChanged));

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
			var ctrl = (DatePicker)d;
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
                    Day.SelectedIndex = SelectedDate.Value.Day;
                    Month.SelectedIndex = SelectedDate.Value.Month;
                    Year.SelectedIndex = SelectedDate.Value.Year - 2000 + 1;
                }
                else
                {
                    Day.SelectedIndex = SelectedDate.Value.Day - 1;
                    Month.SelectedIndex = SelectedDate.Value.Month - 1;
                    Year.SelectedIndex = SelectedDate.Value.Year - 2000;
                }
			}
			else
			{
				UpdateValues(0, 0);
				Day.SelectedIndex = 0;
				Month.SelectedIndex = 0;
				Year.SelectedIndex = 0;
			}
		}

		public void UpdateValues(int year, int month)
		{
			var days = new List<string>();
			if (AllowNull)
				days.Add(" ");
			for (var i = 1; i <= 31; i++)//(year != 0 && month != 0 ? DateTime.DaysInMonth(year, month) : 31); i++)
				days.Add(i.ToString());

			var months = new List<string>();
			if (AllowNull)
				months.Add(" ");
			for (var i = 1; i <= 12; i++)
				months.Add(i.ToString());

			var years = new List<string>();
			if (AllowNull)
				years.Add(" ");
			for (var i = 2000; i <= 2020; i++)
				years.Add(i.ToString());

			if (Month.SelectedIndex > months.Count - 1)
				Month.SelectedIndex = months.Count - 1; 

			Day.ItemsSource = days;
			Month.ItemsSource = months;
			Year.ItemsSource = years; 
		}

		private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (initializing)
				return;

			var hour = SelectedItem != null ? SelectedDate.Value.Hour : 0;
			var minute = SelectedItem != null ? SelectedDate.Value.Minute : 0;
			var second = SelectedItem != null ? SelectedDate.Value.Second : 0;

			initializing = true;
			if (AllowNull && (Day.SelectedIndex == 0 || Month.SelectedIndex == 0 || Year.SelectedIndex == 0))
				SelectedItem = null;
			else
			{
				if (AllowNull)
					SelectedItem = new DateTime(Year.SelectedIndex + 2000 - 1, Month.SelectedIndex, Day.SelectedIndex, hour, minute, second);
				else
					SelectedItem = new DateTime(Year.SelectedIndex + 2000, Month.SelectedIndex + 1, Day.SelectedIndex + 1, hour, minute, second);
			}

			//if (SelectedItem.HasValue)
			//	UpdateValues(SelectedItem.Value.Year, SelectedItem.Value.Month);
			//else
			//	UpdateValues(0, 0);
				
			initializing = false; 
		}
	}
}
