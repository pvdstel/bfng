using Avalonia;
using Avalonia.Markup.Xaml;

namespace bfng.dbgui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
