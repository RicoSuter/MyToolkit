//-----------------------------------------------------------------------
// <copyright file="PivotExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Controls
{
    /// <summary>Attached properties for (Windows Phone's) Pivot control. </summary>
    public class PivotExtensions
    {
        public static readonly DependencyProperty DisableAutoMarginProperty = DependencyProperty.RegisterAttached(
            "DisableAutoMargin", typeof (bool), typeof (PivotExtensions), new PropertyMetadata(default(bool), OnDisableAutoMarginChanged));

        /// <summary>Sets a value indicating whether the automatic margin on a Windows Phone Pivot control should be disabled. </summary>
        /// <remarks>When this is set to <c>true</c>, the Pivot's template cannot be changed. 
        /// Use this property only on Windows Phone Pivot controls. </remarks>
        /// <param name="element">The element. </param>
        /// <param name="value">The value. </param>
        public static void SetDisableAutoMargin(DependencyObject element, bool value)
        {
            element.SetValue(DisableAutoMarginProperty, value);
        }

        /// <summary>Gets a value indicating whether the automatic margin on a Windows Phone Pivot control should be disabled. </summary>
        /// <param name="element">The element. </param>
        public static bool GetDisableAutoMargin(DependencyObject element)
        {
            return (bool) element.GetValue(DisableAutoMarginProperty);
        }

        private static void OnDisableAutoMarginChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var control = (Control)dependencyObject;
            control.Template = (ControlTemplate) XamlReader.Load(
@"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                   TargetType=""Pivot"">
    <Grid Background=""{TemplateBinding Background}"" HorizontalAlignment=""{TemplateBinding HorizontalAlignment}"" 
          VerticalAlignment=""{TemplateBinding VerticalAlignment}"">
        <Grid.RowDefinitions>
            <RowDefinition Height=""Auto""/>
            <RowDefinition Height=""*""/>
        </Grid.RowDefinitions>
        <VisualStateManager.VisualStateGroups>
            <VisualStateGroup x:Name=""Orientation"">
                <VisualState x:Name=""Portrait"">
                    <Storyboard>
                        <ObjectAnimationUsingKeyFrames Storyboard.TargetProperty=""Margin"" Storyboard.TargetName=""TitleContentControl"">
                            <DiscreteObjectKeyFrame KeyTime=""0"" Value=""20,0,0,0""/>
                        </ObjectAnimationUsingKeyFrames>
                    </Storyboard>
                </VisualState>
                <VisualState x:Name=""Landscape"">
                    <Storyboard>
                        <ObjectAnimationUsingKeyFrames Storyboard.TargetProperty=""Margin"" Storyboard.TargetName=""TitleContentControl"">
                            <DiscreteObjectKeyFrame KeyTime=""0"" Value=""20,0,0,0""/>
                        </ObjectAnimationUsingKeyFrames>
                    </Storyboard>
                </VisualState>
            </VisualStateGroup>
        </VisualStateManager.VisualStateGroups>
        <ContentControl x:Name=""TitleContentControl"" ContentTemplate=""{TemplateBinding TitleTemplate}"" Content=""{TemplateBinding Title}"" 
                        Style=""{StaticResource PivotTitleContentControlStyle}""/>
        <ScrollViewer x:Name=""ScrollViewer"" HorizontalSnapPointsAlignment=""Center"" HorizontalSnapPointsType=""MandatorySingle"" 
                      HorizontalScrollBarVisibility=""Hidden"" Margin=""{TemplateBinding Padding}"" Grid.Row=""1"" 
                      Template=""{StaticResource ScrollViewerScrollBarlessTemplate}"" VerticalSnapPointsType=""None"" 
                      VerticalScrollBarVisibility=""Disabled"" VerticalScrollMode=""Disabled"" 
                      VerticalContentAlignment=""Stretch"" ZoomMode=""Disabled"">
            <PivotPanel x:Name=""Panel"" VerticalAlignment=""Stretch"">
                <PivotHeaderPanel x:Name=""Header"">
                    <PivotHeaderPanel.RenderTransform>
                        <CompositeTransform x:Name=""HeaderTranslateTransform"" TranslateX=""0""/>
                    </PivotHeaderPanel.RenderTransform>
                </PivotHeaderPanel>
                <ItemsPresenter x:Name=""PivotItemPresenter"">
                    <ItemsPresenter.RenderTransform>
                        <TranslateTransform x:Name=""ItemsPresenterTranslateTransform"" X=""0""/>
                    </ItemsPresenter.RenderTransform>
                </ItemsPresenter>
            </PivotPanel>
        </ScrollViewer>
    </Grid>
</ControlTemplate>");
        }
    }
}
