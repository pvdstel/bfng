using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace bfng.dbgui.Views
{
    public class DebugCommands : UserControl
    {
        public DebugCommands()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
