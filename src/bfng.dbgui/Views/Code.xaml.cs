using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using bfng.dbgui.Models;
using bfng.Parsing;
using bfng.Runtime;
using ReactiveUI;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Drawing.Brushes;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace bfng.dbgui.Views
{
    public partial class Code : UserControl
    {
        #region Constants

        public const int BlockSize = 22;
        public const int CoordinateOffset = 20;

        public static readonly Rgba32 SymbolColorOnLight = Rgba32.Black;
        public static readonly Rgba32 SymbolColorOnDark = Rgba32.White;
        public static readonly Rgba32 BackgroundColor = Rgba32.White;
        public static readonly SolidBrush<Rgba32> ActiveColor = new SolidBrush<Rgba32>(Rgba32.Yellow);
        public static readonly SolidBrush<Rgba32> BreakpointColor = new SolidBrush<Rgba32>(new Rgba32(150, 58, 70));
        public static readonly SolidBrush<Rgba32> MutatedColor = new SolidBrush<Rgba32>(Rgba32.Purple);
        public static readonly Rgba32 GridLineColor = new Rgba32(192, 192, 192);

        private static readonly Font SymbolFont = SystemFonts.CreateFont("Courier New", 14);
        private static readonly Font CoordinateFont = SystemFonts.CreateFont("Arial", 12);

        private long _renderCycle = 0;
        private object _renderLock = new object();
        #endregion

        #region Avalonia properties

        public static readonly DirectProperty<Code, IEnumerable<Symbol>> SourceStatementsProperty =
            AvaloniaProperty.RegisterDirect<Code, IEnumerable<Symbol>>(
                nameof(Symbols), c => c.Symbols, (c, v) => c.Symbols = v);

        public static readonly DirectProperty<Code, InstructionProgram> InstructionProgamProperty =
            AvaloniaProperty.RegisterDirect<Code, InstructionProgram>(
                nameof(InstructionProgram), c => c.InstructionProgram, (c, v) => c.InstructionProgram = v);

        public static readonly DirectProperty<Code, ReactiveCommand<InstructionPointer, bool>> ToggleBreakpointCommandProperty =
            AvaloniaProperty.RegisterDirect<Code, ReactiveCommand<InstructionPointer, bool>>(
                nameof(ToggleBreakpointCommand), c => c.ToggleBreakpointCommand, (c, v) => c.ToggleBreakpointCommand = v);
        #endregion

        #region Fields

        private IEnumerable<Symbol> _symbols = Enumerable.Empty<Symbol>();
        private List<Symbol> _internalSymbols = new List<Symbol>();
        private InstructionProgram _instructionProgram;

        private Image _codeMap;

        private Tuple<int, int> _codeMapSize = new Tuple<int, int>(0, 0);
        #endregion

        #region Initialization

        public Code()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _codeMap = this.FindControl<Image>("codeMap");

            Tuple<Avalonia.Point, MouseButton> pd = null;
            _codeMap.PointerPressed += (sender, e) => pd = new Tuple<Avalonia.Point, MouseButton>(e.GetPosition(_codeMap), e.MouseButton);
            _codeMap.PointerReleased += (sender, e) =>
            {
                var pos = e.GetPosition(_codeMap);
                if (pd != null && Math.Abs(pd.Item1.X - pos.X) < 3 && Math.Abs(pd.Item1.Y-  pos.Y) < 3 && pd.Item2 == e.MouseButton)
                {
                    if (pd.Item2 == MouseButton.Left && ToggleBreakpointCommand != null)
                    {
                        var ip = new InstructionPointer(BlockToProgram((int)pd.Item1.X), BlockToProgram((int)pd.Item1.Y));
                        ToggleBreakpointCommand.Execute(ip).Subscribe();
                    }
                }
                pd = null;
            };
        }
        #endregion

        #region Properties

        public IEnumerable<Symbol> Symbols
        {
            get => _symbols;
            set
            {
                _symbols = value;
                _internalSymbols = value?.ToList() ?? new List<Symbol>();
                RenderCode();
            }
        }

        public InstructionProgram InstructionProgram
        {
            get => _instructionProgram;
            set
            {
                _instructionProgram = value;
                UpdateCodeMap();
            }
        }

        public ReactiveCommand<InstructionPointer, bool> ToggleBreakpointCommand
        {
            get;
            set;
        }
        #endregion

        private int ProgramToBlock(int n) => n * BlockSize + CoordinateOffset;
        private int BlockToProgram(int n) => (n - CoordinateOffset) / BlockSize;

        public void UpdateCodeMap()
        {
            _codeMapSize = new Tuple<int, int>(
                (InstructionProgram?.Width).GetValueOrDefault() * BlockSize + CoordinateOffset,
                (InstructionProgram?.Height).GetValueOrDefault() * BlockSize + CoordinateOffset
            );
            _codeMap.Width = _codeMapSize.Item1;
            _codeMap.Height = _codeMapSize.Item2;
            RenderCode();
        }

        public async void RenderCode()
        {
            if (InstructionProgram == null || _codeMap == null) return;

            int w = _codeMapSize.Item1,
                h = _codeMapSize.Item2;
            long renderCycle = System.Threading.Interlocked.Increment(ref _renderCycle);

            using (MemoryStream ms = new MemoryStream())
            {
                await Task.Run(() =>
                {
                    using (SixLabors.ImageSharp.Image<Rgba32> code = new SixLabors.ImageSharp.Image<Rgba32>(w, h))
                    {
                        code.Mutate(t => t.Fill(BackgroundColor));

                        foreach (Symbol symbol in _internalSymbols)
                        {
                            RenderSymbol(symbol, code);
                        }

                        RenderGrid(code);
                        code.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                });

                lock (_renderLock)
                {
                    if (renderCycle == _renderCycle)
                    {
                        Bitmap b = new Bitmap(ms);
                        _codeMap.Source = b;
                    }
                }
            }
        }

        private void RenderSymbol(Symbol symbol, SixLabors.ImageSharp.Image<Rgba32> image)
        {
            Rgba32 symbolColor = SymbolColorOnLight;
            int x = ProgramToBlock(symbol.X),
                y = ProgramToBlock(symbol.Y);
            if (symbol.IsActive)
            {
                image.Mutate(t => t.Fill(ActiveColor, new Rectangle(x, y, BlockSize, BlockSize)));
            } else if (symbol.HasBreakpoint)
            {
                image.Mutate(t => t.Fill(BreakpointColor, new Rectangle(x, y, BlockSize, BlockSize)));
                symbolColor = SymbolColorOnDark;
            }
            if (symbol.IsMutated)
            {
                image.Mutate(t => t.Fill(MutatedColor, new Rectangle(x, y + BlockSize - 3, BlockSize, 3)));
            }
            if (!char.IsWhiteSpace(symbol.Expression))
            {
                image.Mutate(t => t.DrawText(symbol.Expression.ToString(), SymbolFont, symbolColor, new PointF(x + 3, y)));
            }
        }

        private void RenderGrid(SixLabors.ImageSharp.Image<Rgba32> image)
        {
            if (InstructionProgram == null) return;

            int w = _codeMapSize.Item1,
                h = _codeMapSize.Item2;

            var cxs = Enumerable.Range(0, InstructionProgram.Width).Select(ProgramToBlock).ToList();
            var cys = Enumerable.Range(0, InstructionProgram.Height).Select(ProgramToBlock).ToList();
            var gxs = cxs.Skip(1).Take(InstructionProgram.Width - 1);
            var gys = cys.Skip(1).Take(InstructionProgram.Height - 1);

            for (int i = 0; i < cxs.Count; ++i)
                image.Mutate(t => t.DrawText(i.ToString(), CoordinateFont, GridLineColor, new SixLabors.Primitives.Point(cxs[i], 1)));

            for (int i = 0; i < cys.Count; ++i)
                image.Mutate(t => t.DrawText(i.ToString(), CoordinateFont, GridLineColor, new SixLabors.Primitives.Point(4, cys[i])));

            foreach (int x in gxs)
                for (int y = CoordinateOffset; y < h; ++y) image[x, y] = GridLineColor;

            foreach (int y in gys)
                for (int x = CoordinateOffset; x < w; ++x) image[x, y] = GridLineColor;
        }
    }
}
