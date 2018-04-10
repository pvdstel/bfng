using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media.Imaging;
using bfng.dbgui.ViewModels;
using bfng.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace bfng.dbgui.Views
{
    public partial class Code : UserControl
    {
        public const int BlockSize = 20;

        public static readonly DirectProperty<Code, IEnumerable<SourceStatementViewModel>> SourceStatementsProperty =
            AvaloniaProperty.RegisterDirect<Code, IEnumerable<SourceStatementViewModel>>(
                nameof(SourceStatements), c => c.SourceStatements, (c, v) => c.SourceStatements = v);

        public static readonly DirectProperty<Code, InstructionProgram> InstructionProgamProperty =
            AvaloniaProperty.RegisterDirect<Code, InstructionProgram>(
                nameof(InstructionProgram), c => c.InstructionProgram, (c, v) => c.InstructionProgram = v);

        private IEnumerable<SourceStatementViewModel> _sourceStatements = Enumerable.Empty<SourceStatementViewModel>();
        private InstructionProgram _instructionProgram;

        private Canvas _statementGrid;

        public Code()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _statementGrid = new Canvas();

            ScrollViewer scroll = new ScrollViewer()
            {
                Content = _statementGrid,
                HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
            };

            Border root = new Border
            {
                Child = scroll,
                Classes = new Classes("box")
            };
            this.Content = root;
        }

        public IEnumerable<SourceStatementViewModel> SourceStatements
        {
            get => _sourceStatements;
            set
            {
                _sourceStatements = value;
                RenderGrid();
            }
        }

        public InstructionProgram InstructionProgram
        {
            get => _instructionProgram;
            set
            {
                _instructionProgram = value;
                RenderGrid();
            }
        }

        public void RenderGrid()
        {

        }
    }
}
