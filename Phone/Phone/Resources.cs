using System.Windows;
using System.Windows.Media;

namespace MyToolkit.Phone
{
	public static class Resources
	{
		public static Visibility PhoneLightThemeVisibility
		{
			get { return (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"]; }
		}

		#region Colors

		public static Color PhoneBackgroundColor
		{
			get { return (Color)Application.Current.Resources["PhoneBackgroundColor"]; }
		}

		public static Color PhoneContrastForegroundColor
		{
			get { return (Color)Application.Current.Resources["PhoneContrastForegroundColor"]; }
		}

		public static Color PhoneForegroundColor
		{
			get { return (Color)Application.Current.Resources["PhoneForegroundColor"]; }
		}

		public static Color PhoneInactiveColor
		{
			get { return (Color)Application.Current.Resources["PhoneInactiveColor"]; }
		}

		public static Color PhoneDisabledColor
		{
			get { return (Color)Application.Current.Resources["PhoneDisabledColor"]; }
		}

		public static Color PhoneSubtleColor
		{
			get { return (Color)Application.Current.Resources["PhoneSubtleColor"]; }
		}

		public static Color PhoneContrastBackgroundColor
		{
			get { return (Color)Application.Current.Resources["PhoneContrastBackgroundColor"]; }
		}

		public static Color PhoneTextBoxColor
		{
			get { return (Color)Application.Current.Resources["PhoneTextBoxColor"]; }
		}

		public static Color PhoneBorderColor
		{
			get { return (Color)Application.Current.Resources["PhoneBorderColor"]; }
		}

		public static Color PhoneTextSelectionColor
		{
			get { return (Color)Application.Current.Resources["PhoneTextSelectionColor"]; }
		}

		public static Color PhoneAccentColor
		{
			get { return (Color)Application.Current.Resources["PhoneAccentColor"]; }
		}

		#endregion

		#region Brushes

		public static Brush PhoneAccentBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneAccentBrush"]; }
		}

		public static Brush PhoneBackgroundBrush 
		{
			get { return (Brush)Application.Current.Resources["PhoneBackgroundBrush"]; }
		}

		public static Brush PhoneContrastForegroundBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneContrastForegroundBrush"]; }
		}

		public static Brush PhoneForegroundBrush 
		{
			get { return (Brush)Application.Current.Resources["PhoneForegroundBrush"]; }
		}

		public static Brush PhoneInactiveBrush 
		{
			get { return (Brush)Application.Current.Resources["PhoneInactiveBrush"]; }
		}

		public static Brush PhoneDisabledBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneDisabledBrush"]; }
		}

		public static Brush PhoneContrastBackgroundBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneContrastBackgroundBrush"]; }
		}

		public static Brush PhoneTextBoxBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneTextBoxBrush"]; }
		}

		public static Brush PhoneBorderBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneBorderBrush"]; }
		}

		public static Brush PhoneTextSelectionBrush
		{
			get { return (Brush)Application.Current.Resources["PhoneTextSelectionBrush"]; }
		}

		public static Brush TransparentBrush
		{
			get { return (Brush)Application.Current.Resources["TransparentBrush"]; }
		}

		#endregion

		#region Fonts

		public static FontFamily PhoneFontFamilyNormal
		{
			get { return (FontFamily)Application.Current.Resources["PhoneFontFamilyNormal"]; }
		}

		public static FontFamily PhoneFontFamilyLight
		{
			get { return (FontFamily)Application.Current.Resources["PhoneFontFamilyLight"]; }
		}

		public static FontFamily PhoneFontFamilySemiLight
		{
			get { return (FontFamily)Application.Current.Resources["PhoneFontFamilySemiLight"]; }
		}

		public static FontFamily PhoneFontFamilySemiBold
		{
			get { return (FontFamily)Application.Current.Resources["PhoneFontFamilySemiBold"]; }
		}

		#endregion

		#region Sizes

		public static double PhoneFontSizeSmall
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeSmall"]; }
		}

		public static double PhoneFontSizeNormal
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeNormal"]; }
		}

		public static double PhoneFontSizeMedium
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeMedium"]; }
		}

		public static double PhoneFontSizeMediumLarge
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeMediumLarge"]; }
		}

		public static double PhoneFontSizeLarge
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeLarge"]; }
		}

		public static double PhoneFontSizeExtraLarge
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeExtraLarge"]; }
		}

		public static double PhoneFontSizeExtraExtraLarge
		{
			get { return (double)Application.Current.Resources["PhoneFontSizeExtraExtraLarge"]; }
		}

		#endregion

		#region Styles

		public static Style PhoneTextNormalStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextNormalStyle"]; }
		}

		public static Style PhoneTextTitle1Style
		{
			get { return (Style)Application.Current.Resources["PhoneTextTitle1Style"]; }
		}

		public static Style PhoneTextSubtleStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextSubtleStyle"]; }
		}

		public static Style PhoneTextTitle2Style
		{
			get { return (Style)Application.Current.Resources["PhoneTextTitle2Style"]; }
		}

		public static Style PhoneTextTitle3Style
		{
			get { return (Style)Application.Current.Resources["PhoneTextTitle3Style"]; }
		}

		public static Style PhoneTextExtraLargeStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextExtraLargeStyle"]; }
		}

		public static Style PhoneTextGroupHeaderStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextGroupHeaderStyle"]; }
		}

		public static Style PhoneTextLargeStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextLargeStyle"]; }
		}

		public static Style PhoneTextSmallStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextSmallStyle"]; }
		}

		public static Style PhoneTextContrastStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextContrastStyle"]; }
		}

		public static Style PhoneTextAccentStyle
		{
			get { return (Style)Application.Current.Resources["PhoneTextAccentStyle"]; }
		}

		#endregion
	}
}
