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
			DependencyProperty.Register("AllowNull", "Object", typeof(DatePicker).FullName, new PropertyMetadata(true));

		public bool AllowNull
		{
			get { return (bool)GetValue(AllowNullProperty); }
			set { SetValue(AllowNullProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", "Object", typeof(DatePicker).FullName, new PropertyMetadata(null, OnSelectedItemChanged));

		public DateTime? SelectedItem
		{
			get { return (DateTime?)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public event RoutedEventHandler SelectedItemChanged; 

		private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (DatePicker)d;
			if (ctrl.initializing)
				return;
			ctrl.UpdateDate();


			if (ctrl.SelectedItemChanged != null)
				ctrl.SelectedItemChanged(ctrl, new RoutedEventArgs());
		}

		public void UpdateDate()
		{
			if (SelectedItem.HasValue)
			{
				UpdateValues(SelectedItem.Value.Year, SelectedItem.Value.Month);
				Day.SelectedIndex = SelectedItem.Value.Day;
				Month.SelectedIndex = SelectedItem.Value.Month;
				Year.SelectedIndex = SelectedItem.Value.Year - 2000;
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
				days.Add("");
			for (var i = 1; i <= 31; i++)//(year != 0 && month != 0 ? DateTime.DaysInMonth(year, month) : 31); i++)
				days.Add(i.ToString());

			var months = new List<string>();
			if (AllowNull)
				months.Add("");
			for (var i = 1; i <= 12; i++)
				months.Add(i.ToString());

			var years = new List<string>();
			if (AllowNull)
				years.Add("");
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

			initializing = true;
			if (AllowNull && (Day.SelectedIndex == 0 || Month.SelectedIndex == 0 || Year.SelectedIndex == 0))
				SelectedItem = null;
			else
			{
				if (AllowNull)
					SelectedItem = new DateTime(Year.SelectedIndex + 2000, Month.SelectedIndex, Day.SelectedIndex);
				else
					SelectedItem = new DateTime(Year.SelectedIndex + 2000 + 1, Month.SelectedIndex + 1, Day.SelectedIndex + 1);
			}

			//if (SelectedItem.HasValue)
			//	UpdateValues(SelectedItem.Value.Year, SelectedItem.Value.Month);
			//else
			//	UpdateValues(0, 0);
				
			initializing = false; 
		}
	}
}
