﻿using bfng.Debugging;
using bfng.Runtime;
using ReactiveUI;
using System;
using System.Collections.Generic;
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

            debugger.WhenAnyValue(d => d.IsDebugging)
                .ToProperty(this, x => x.IsDebugging, out _isDebugging);
            debugger.WhenAnyValue(d => d.CurrentState.ExecutionContext.InstructionPointer)
                .ToProperty(this, x => x.InstructionPointer, out _instructionPointer);
            debugger.WhenAnyValue(d => d.CurrentState.Output, (StringBuilder v) => v.ToString())
                .ToProperty(this, x => x.OutputString, out _outputString);
        }

        public ICommand StartDebuggingCommand { get; }
        public ICommand StopDebuggingCommand { get; }
        public ICommand StepDebuggerCommand { get; }
        public ICommand RewindDebuggerCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _isDebugging;
        public bool IsDebugging => _isDebugging.Value;

        private readonly ObservableAsPropertyHelper<InstructionPointer> _instructionPointer;
        public InstructionPointer InstructionPointer => _instructionPointer.Value;

        private readonly ObservableAsPropertyHelper<string> _outputString;
        public string OutputString => _outputString.Value;

        public string BefungeFilePath => Program.BefungeProgramPath;
    }
}