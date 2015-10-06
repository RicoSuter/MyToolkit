//-----------------------------------------------------------------------
// <copyright file="MemoryDiagnosticsHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WPF

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Phone.Info;

// from https://blogs.msdn.com/b/ptorr/archive/2010/10/30/that-memory-thing-i-promised-you.aspx

namespace MyToolkit.Performance
{
	/// <summary>
	/// Helper class for showing current memory usage
	/// </summary>
	public static class MemoryDiagnosticsHelper
	{
		static Popup popup;
		static TextBlock currentMemoryBlock;
		static TextBlock peakMemoryBlock;
		static DispatcherTimer timer;
		static bool forceGc;
		const long MAX_MEMORY = 90 * 1024 * 1024; // 90MB, per marketplace
		static int lastSafetyBand = -1; // to avoid needless changes of colour

		const long MAX_CHECKPOINTS = 10; // adjust as needed
		static Queue<MemoryCheckpoint> recentCheckpoints;

		static bool alreadyFailedPeak = false; // to avoid endless Asserts

		/// <summary>
		/// Starts the memory diagnostic timer and shows the counter
		/// </summary>
		/// <param name="timespan">The timespan between counter updates</param>
		/// <param name="forceGc">Whether or not to force a GC before collecting memory stats</param>
		[Conditional("DEBUG")]
		public static void Start(TimeSpan timespan, bool forceGc)
		{
			if (timer != null)
				throw new InvalidOperationException("Diagnostics already running");

			MemoryDiagnosticsHelper.forceGc = forceGc;
			recentCheckpoints = new Queue<MemoryCheckpoint>();

			StartTimer(timespan);
			ShowPopup();
		}

		/// <summary>
		/// Stops the timer and hides the counter
		/// </summary>
		[Conditional("DEBUG")]
		public static void Stop()
		{
			HidePopup();
			StopTimer();
			recentCheckpoints = null;
		}

		/// <summary>
		/// Add a checkpoint to the system to help diagnose failures. Ignored in retail mode
		/// </summary>
		/// <param name="text">Text to describe the most recent thing that happened</param>
		[Conditional("DEBUG")]
		public static void Checkpoint(string text)
		{
			if (recentCheckpoints == null)
				return;

			if (recentCheckpoints.Count >= MAX_CHECKPOINTS - 1)
				recentCheckpoints.Dequeue();

			recentCheckpoints.Enqueue(new MemoryCheckpoint(text, GetCurrentMemoryUsage()));
		}

		/// <summary>
		/// Recent checkpoints stored by the app; will always be empty in retail mode
		/// </summary>
		public static IEnumerable<MemoryCheckpoint> RecentCheckpoints
		{
			get
			{
				if (recentCheckpoints == null)
					yield break;

				foreach (MemoryCheckpoint checkpoint in recentCheckpoints)
					yield return checkpoint;
			}
		}

		/// <summary>
		/// Gets the current memory usage, in bytes. Returns zero in non-debug mode
		/// </summary>
		/// <returns>Current usage</returns>
		public static long GetCurrentMemoryUsage()
		{
#if DEBUG
			// don't use DeviceExtendedProperties for release builds (requires a capability)
			return (long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage");
#else
			return 0;
#endif
		}

		/// <summary>
		/// Gets the peak memory usage, in bytes. Returns zero in non-debug mode
		/// </summary>
		/// <returns>Peak memory usage</returns>
		public static long GetPeakMemoryUsage()
		{
#if DEBUG
			// don't use DeviceExtendedProperties for release builds (requires a capability)
			return (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
#else
			return 0;
#endif
		}

		private static void ShowPopup()
		{
			popup = new Popup();
			double fontSize = (double)Application.Current.Resources["PhoneFontSizeSmall"] - 2;
			Brush foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
			StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Background = (Brush)Application.Current.Resources["PhoneSemitransparentBrush"] };
			currentMemoryBlock = new TextBlock { Text = "---", FontSize = fontSize, Foreground = foreground };
			peakMemoryBlock = new TextBlock { Text = "", FontSize = fontSize, Foreground = foreground, Margin = new Thickness(5, 0, 0, 0) };
			sp.Children.Add(currentMemoryBlock);
			sp.Children.Add(new TextBlock { Text = " kb", FontSize = fontSize, Foreground = foreground });
			sp.Children.Add(peakMemoryBlock);
			sp.RenderTransform = new CompositeTransform { Rotation = 90, TranslateX = 480, TranslateY = 425, CenterX = 0, CenterY = 0 };
			popup.Child = sp;
			popup.IsOpen = true;
		}

		private static void StartTimer(TimeSpan timespan)
		{
			timer = new DispatcherTimer();
			timer.Interval = timespan;
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		static void timer_Tick(object sender, EventArgs e)
		{
			if (forceGc)
				GC.Collect();

			UpdateCurrentMemoryUsage();
			UpdatePeakMemoryUsage();
		}

		private static void UpdatePeakMemoryUsage()
		{
			if (alreadyFailedPeak)
				return;

			long peak = GetPeakMemoryUsage();
			if (peak >= MAX_MEMORY)
			{
				alreadyFailedPeak = true;
				Checkpoint("*MEMORY USAGE FAIL*");
				peakMemoryBlock.Text = "FAIL!";
				peakMemoryBlock.Foreground = new SolidColorBrush(Colors.Red);
				//if (Debugger.IsAttached)
				//    Debug.Assert(false, "Peak memory condition violated");
			}
		}

		private static void UpdateCurrentMemoryUsage()
		{
			long mem = GetCurrentMemoryUsage();
			currentMemoryBlock.Text = string.Format("{0:N}", mem / 1024);
			int safetyBand = GetSafetyBand(mem);
			if (safetyBand != lastSafetyBand)
			{
				currentMemoryBlock.Foreground = GetBrushForSafetyBand(safetyBand);
				lastSafetyBand = safetyBand;
			}
		}

		private static Brush GetBrushForSafetyBand(int safetyBand)
		{
			switch (safetyBand)
			{
				case 0:
					return new SolidColorBrush(Colors.Green);

				case 1:
					return new SolidColorBrush(Colors.Orange);

				default:
					return new SolidColorBrush(Colors.Red);
			}
		}

		private static int GetSafetyBand(long mem)
		{
			double percent = (double)mem / (double)MAX_MEMORY;
			if (percent <= 0.75)
				return 0;

			if (percent <= 0.90)
				return 1;

			return 2;
		}

		private static void StopTimer()
		{
			timer.Stop();
			timer = null;
		}

		private static void HidePopup()
		{
			popup.IsOpen = false;
			popup = null;
		}
	}

	/// <summary>
	/// Holds checkpoint information for diagnosing memory usage
	/// </summary>
	public class MemoryCheckpoint
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="text">Text for the checkpoint</param>
		/// <param name="memoryUsage">Memory usage at the time of the checkpoint</param>
		internal MemoryCheckpoint(string text, long memoryUsage)
		{
			Text = text;
			MemoryUsage = memoryUsage;
		}

		/// <summary>
		/// The text associated with this checkpoint
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// The memory usage at the time of the checkpoint
		/// </summary>
		public long MemoryUsage { get; private set; }
	}
}

#endif