﻿using bfng.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace bfng.Debugging
{
    public class Debugger : INotifyPropertyChanged
    {
        #region Fields

        private Stack<DebuggerState> _history;
        private DebuggerEnvironment _debuggerEnvironment;
        private DebuggerState _debuggerState;
        #endregion

        #region Constructor

        public Debugger()
        {
            _debuggerEnvironment = new DebuggerEnvironment(this);
        }
        #endregion

        #region Properties

        public DebuggerState CurrentState {
            get => _debuggerState;
            private set
            {
                _debuggerState = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsDebugging));
            }
        }

        public bool IsDebugging => (CurrentState?.ExecutionContext.IsRunning).GetValueOrDefault();
        #endregion

        public void StartDebugging(InstructionProgram program)
        {
            _history = new Stack<DebuggerState>();
            CurrentState = new DebuggerState(program);
        }

        public void StopDebugging()
        {
            _history = null;
            CurrentState = null;
        }

        public void Step()
        {
            if (!IsDebugging) return;

            _history.Push(CurrentState);

            DebuggerState state = new DebuggerState(CurrentState);
            _debuggerEnvironment.DebuggerState = state;

            Instruction currentInstruction = state.ExecutionContext.GetCurrentInstruction();
            currentInstruction(state.ExecutionContext, _debuggerEnvironment);
            state.ExecutionContext.AdvanceInstructionPointer();

            CurrentState = state;
        }

        public void Rewind()
        {
            if (!IsDebugging) return;
            if (_history.Count == 0) return;

            CurrentState = _history.Pop();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string member = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
        }
        #endregion
    }
}