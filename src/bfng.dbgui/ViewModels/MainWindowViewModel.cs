﻿using bfng.dbgui.Models;
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
        readonly Debugger debugger = new Debugger();
        readonly Lazy<List<Symbol>> programSymbols = new Lazy<List<Symbol>>(() => ProgramToSymbols(Program.BefungeProgram));

        public MainWindowViewModel()
        {
            StartDebuggingCommand = ReactiveCommand.Create(() => debugger.StartDebugging(Program.BefungeProgram));

            IObservable<bool> debugging = debugger.WhenAnyValue(d => d.IsDebugging);
            IObservable<bool> canGoAhead = debugger.WhenAnyValue(d => d.IsProgramRunning, d => d.IsExecuting, (d, e) => d && !e);
            IObservable<bool> canRewind = debugger.WhenAnyValue(d => d.IsDebugging, d => d.IsExecuting, d => d.HistoryCount, (d, e, h) => d && !e && h > 0);
            IObservable<bool> canBreak = debugger.WhenAnyValue(d => d.IsProgramRunning, d => d.IsExecuting, (d, e) => d && e);

            StopDebuggingCommand = ReactiveCommand.Create(() => debugger.StopDebugging(), debugging);
            StepDebuggerCommand = ReactiveCommand.Create(() => debugger.Step(), canGoAhead);
            ContinueDebuggerCommand = ReactiveCommand.Create(() => debugger.Continue(), canGoAhead);
            SkipDebuggerCommand = ReactiveCommand.Create(() => debugger.Skip(), canGoAhead);
            RewindDebuggerCommand = ReactiveCommand.Create(() => debugger.Rewind(), canRewind);
            BreakDebuggerCommand = ReactiveCommand.Create(() => debugger.Break(), canBreak);
            AutoStepDebuggerCommand = ReactiveCommand.Create(() => debugger.AutoStep(), canGoAhead);

            debugger.WhenAnyValue(d => d.IsDebugging)
                .ToProperty(this, x => x.IsDebugging, out _isDebugging);
            debugger.WhenAnyValue(d => d.IsExecuting)
                .ToProperty(this, x => x.IsExecuting, out _isExecuting);
            debugger.WhenAnyValue(d => d.CurrentState, c => (c?.ExecutionContext.InstructionPointer).GetValueOrDefault())
                .ToProperty(this, x => x.InstructionPointer, out _instructionPointer);
            debugger.WhenAnyValue(d => d.CurrentState, c => c?.Output.ToString() ?? string.Empty)
                .ToProperty(this, x => x.OutputString, out _outputString);
            debugger.WhenAnyValue(d => d.CurrentState, c => c?.ExecutionContext.Stack)
                .ToProperty(this, x => x.StackValues, out _stackValues);
            debugger.WhenAnyValue(d => d.CurrentState, c => c != null ? ExecutionContextToSymbols(c.ExecutionContext) : programSymbols.Value)
                .ToProperty(this, x => x.Symbols, out _symbols);
            debugger.WhenAnyValue(d => d.CurrentState, c => c?.ExecutionContext.Program ?? Program.BefungeProgram)
                .ToProperty(this, x => x.InstructionProgram, out _instructionProgram);
            debugger.WhenAnyValue(d => d.HistoryCount)
                .ToProperty(this, x => x.HistoryCount, out _historyCount);
            debugger.WhenAnyValue(d => d.CurrentState, c => (c?.Round).GetValueOrDefault())
                .ToProperty(this, x => x.Round, out _round);
        }

        private static List<Symbol> ProgramToSymbols(InstructionProgram program)
        {
            List<Symbol> result = new List<Symbol>();
            for (int j = 0; j < program.Height; ++j)
            {
                for (int i = 0; i < program.Width; ++i)
                {
                    result.Add(new Symbol(program, i, j));
                }
            }
            return result;
        }

        private static List<Symbol> ExecutionContextToSymbols(ExecutionContext executionContext)
        {
            List<Symbol> result = new List<Symbol>();
            for (int j = 0; j < executionContext.Program.Height; ++j)
            {
                for (int i = 0; i < executionContext.Program.Width; ++i)
                {
                    result.Add(new Symbol(executionContext, i, j));
                }
            }
            return result;
        }

        public ICommand StartDebuggingCommand { get; }
        public ICommand StopDebuggingCommand { get; }
        public ICommand StepDebuggerCommand { get; }
        public ICommand RewindDebuggerCommand { get; }
        public ICommand ContinueDebuggerCommand { get; }
        public ICommand SkipDebuggerCommand { get; }
        public ICommand BreakDebuggerCommand { get; }
        public ICommand AutoStepDebuggerCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _isDebugging;
        public bool IsDebugging => _isDebugging.Value;

        private readonly ObservableAsPropertyHelper<bool> _isExecuting;
        public bool IsExecuting => _isExecuting.Value;

        private readonly ObservableAsPropertyHelper<InstructionPointer> _instructionPointer;
        public InstructionPointer InstructionPointer => _instructionPointer.Value;

        private readonly ObservableAsPropertyHelper<string> _outputString;
        public string OutputString => _outputString.Value;

        private readonly ObservableAsPropertyHelper<Stack<int>> _stackValues;
        public Stack<int> StackValues => _stackValues.Value;

        private readonly ObservableAsPropertyHelper<List<Symbol>> _symbols;
        public List<Symbol> Symbols => _symbols.Value;

        private readonly ObservableAsPropertyHelper<InstructionProgram> _instructionProgram;
        public InstructionProgram InstructionProgram => _instructionProgram.Value;

        private readonly ObservableAsPropertyHelper<int> _historyCount;
        public int HistoryCount => _historyCount.Value;

        private readonly ObservableAsPropertyHelper<ulong> _round;
        public ulong Round => _round.Value;

        public string BefungeFilePath => Program.BefungeProgramPath;
    }
}
