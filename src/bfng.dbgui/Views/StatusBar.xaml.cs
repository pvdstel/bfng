using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace bfng.dbgui.Views
{
    public class StatusBar : UserControl
    {
        public StatusBar()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
