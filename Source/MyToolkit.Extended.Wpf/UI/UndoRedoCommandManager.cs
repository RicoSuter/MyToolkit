using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyToolkit.Utilities;

namespace MyToolkit.UI
{
    public static class UndoRedoCommandManager
    {
        public static readonly DependencyProperty DisableUndoRedoProperty = DependencyProperty.RegisterAttached(
            "DisableUndoRedo", typeof(bool), typeof(UndoRedoCommandManager), new PropertyMetadata(default(bool), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            obj.Dispatcher.InvokeAsync(() =>
            {
                if ((bool) args.NewValue)
                    Disable((UIElement) obj);
                else
                    Enable((UIElement) obj);
            });
        }

        public static void SetDisableUndoRedo(DependencyObject element, bool value)
        {
            element.SetValue(DisableUndoRedoProperty, value);
        }

        public static bool GetDisableUndoRedo(DependencyObject element)
        {
            return (bool)element.GetValue(DisableUndoRedoProperty);
        }

        public static void Disable(UIElement element)
        {
            if (element is TextBox)
            {
                element.InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.Z, ModifierKeys.Control));
                element.InputBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.Y, ModifierKeys.Control));
            }
            else
            {
                var textBox = element.FindVisualChild<TextBox>();
                if (textBox != null)
                    Disable(textBox);
            }
        }

        public static void Enable(UIElement element)
        {
            foreach (var binding in element.InputBindings.OfType<KeyBinding>()
                .Where(b => b.Modifiers == ModifierKeys.Control && (b.Key == Key.Z || b.Key == Key.Y)).ToArray())
            {
                element.InputBindings.Remove(binding);
            }
        }
    }
}
