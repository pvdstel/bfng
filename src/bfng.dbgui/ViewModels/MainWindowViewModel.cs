using bfng.Debugging;
using bfng.Parsing;
using bfng.Runtime;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace bfng.dbgui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        Debugger debugger = new Debugger();

        public MainWindowViewModel()
        {
            debugger = new Debugger();

            StartDebuggingCommand = ReactiveCommand.Create(() => debugger.StartDebugging(Program.BefungeProgram));

            IObservable<bool> canExecuteDebugActions = debugger.WhenAnyValue(d => d.IsDebugging);
            StopDebuggingCommand = ReactiveCommand.Create(() => debugger.StopDebugging(), canExecuteDebugActions);
            StepDebuggerCommand = ReactiveCommand.Create(() => debugger.Step(), canExecuteDebugActions);
            RewindDebuggerCommand = ReactiveCommand.Create(() => debugger.Rewind(), canExecuteDebugActions);
            RunDebuggerToEndCommand = ReactiveCommand.Create(() => debugger.RunToEnd(), canExecuteDebugActions);

            debugger.WhenAnyValue(d => d.IsDebugging)
                .ToProperty(this, x => x.IsDebugging, out _isDebugging);
            debugger.WhenAnyValue(d => d.CurrentState.ExecutionContext.InstructionPointer)
                .ToProperty(this, x => x.InstructionPointer, out _instructionPointer);
            debugger.WhenAnyValue(d => d.CurrentState.Output, (StringBuilder v) => v.ToString())
                .ToProperty(this, x => x.OutputString, out _outputString);
            debugger.WhenAnyValue(d => d.CurrentState.ExecutionContext.Stack)
                .ToProperty(this, x => x.StackValues, out _stackValues);
            debugger.WhenAnyValue(d => d.CurrentState.ExecutionContext, ProgramToSourceStatements)
                .ToProperty(this, x => x.SourceStatements, out _sourceStatements);
            debugger.WhenAnyValue(d => d.CurrentState.ExecutionContext.Program)
                .ToProperty(this, x => x.InstructionProgram, out _instructionProgram);
        }

        private List<SourceStatementViewModel> ProgramToSourceStatements(ExecutionContext executionContext)
        {
            List<SourceStatementViewModel> result = new List<SourceStatementViewModel>();
            for (int j = 0; j < executionContext.Program.Height; ++j)
            {
                for (int i = 0; i < executionContext.Program.Width; ++i)
                {
                    result.Add(new SourceStatementViewModel(executionContext, i, j));
                }
            }
            return result;
        }

        public ICommand StartDebuggingCommand { get; }
        public ICommand StopDebuggingCommand { get; }
        public ICommand StepDebuggerCommand { get; }
        public ICommand RewindDebuggerCommand { get; }
        public ICommand RunDebuggerToEndCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _isDebugging;
        public bool IsDebugging => _isDebugging.Value;

        private readonly ObservableAsPropertyHelper<InstructionPointer> _instructionPointer;
        public InstructionPointer InstructionPointer => _instructionPointer.Value;

        private readonly ObservableAsPropertyHelper<string> _outputString;
        public string OutputString => _outputString.Value;

        private readonly ObservableAsPropertyHelper<Stack<int>> _stackValues;
        public Stack<int> StackValues => _stackValues.Value;

        private readonly ObservableAsPropertyHelper<List<SourceStatementViewModel>> _sourceStatements;
        public List<SourceStatementViewModel> SourceStatements => _sourceStatements.Value;

        private readonly ObservableAsPropertyHelper<InstructionProgram> _instructionProgram;
        public InstructionProgram InstructionProgram => _instructionProgram.Value;

        public string BefungeFilePath => Program.BefungeProgramPath;
    }
}
