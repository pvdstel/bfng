using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using bfng.dbgui.ViewModels;
using bfng.Parsing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using System;
using SixLabors.Fonts;
using System.Threading.Tasks;

namespace bfng.dbgui.Views
{
    public partial class Code : UserControl
    {
        #region Constants

        public const int BlockSize = 20;
        public const int CoordinateOffset = 20;
        public readonly Rgba32 GridLineColor = new Rgba32(192, 192, 192);
        private readonly Font CoordinateFont = SystemFonts.CreateFont("Arial", 12);
        #endregion

        #region Avalonia properties

        public static readonly DirectProperty<Code, IEnumerable<SourceStatementViewModel>> SourceStatementsProperty =
            AvaloniaProperty.RegisterDirect<Code, IEnumerable<SourceStatementViewModel>>(
                nameof(SourceStatements), c => c.SourceStatements, (c, v) => c.SourceStatements = v);

        public static readonly DirectProperty<Code, InstructionProgram> InstructionProgamProperty =
            AvaloniaProperty.RegisterDirect<Code, InstructionProgram>(
                nameof(InstructionProgram), c => c.InstructionProgram, (c, v) => c.InstructionProgram = v);
        #endregion

        #region Fields

        private IEnumerable<SourceStatementViewModel> _sourceStatements = Enumerable.Empty<SourceStatementViewModel>();
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
        }
        #endregion

        #region Properties

        public IEnumerable<SourceStatementViewModel> SourceStatements
        {
            get => _sourceStatements;
            set
            {
                _sourceStatements = value;
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
        #endregion

        private int ProgramToBlock(int n) => n * BlockSize + CoordinateOffset;

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
            await Task.Yield();

            int w = _codeMapSize.Item1,
                h = _codeMapSize.Item2;

            using (MemoryStream ms = new MemoryStream())
            {
                using (SixLabors.ImageSharp.Image<Rgba32> code = new SixLabors.ImageSharp.Image<Rgba32>(w, h))
                {
                    RenderGrid(code);
                    code.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                }

                ms.Seek(0, SeekOrigin.Begin);
                Bitmap b = new Bitmap(ms);
                _codeMap.Source = b;
            }
        }

        private void RenderGrid(SixLabors.ImageSharp.Image<Rgba32> image)
        {
            int w = _codeMapSize.Item1,
                h = _codeMapSize.Item2;

            var cxs = Enumerable.Range(0, InstructionProgram.Width).Select(ProgramToBlock).ToList();
            var cys = Enumerable.Range(0, InstructionProgram.Height).Select(ProgramToBlock).ToList();
            var gxs = cxs.Skip(1).Take(InstructionProgram.Width - 1);
            var gys = cys.Skip(1).Take(InstructionProgram.Height - 1);

            for (int i = 0; i < cxs.Count; ++i)
                image.Mutate(t => t.DrawText(i.ToString(), CoordinateFont, GridLineColor, new PointF(cxs[i], 1)));

            for (int i = 0; i < cys.Count; ++i)
                image.Mutate(t => t.DrawText(i.ToString(), CoordinateFont, GridLineColor, new PointF(4, cys[i])));

            foreach (int x in gxs)
                for (int y = CoordinateOffset; y < h; ++y) image[x, y] = GridLineColor;

            foreach (int y in gys) 
                for (int x = CoordinateOffset; x < w; ++x) image[x, y] = GridLineColor;
        }
    }
}
