using bfng.Parsing;
using bfng.Runtime;
using bfng.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace bfng.Debugging
{
    public class Debugger : INotifyPropertyChanged
    {
        public const int DEFAULT_MAX_HISTORY_SIZE = 250;
        public const int AUTO_STEP_DELAY = 250;

        #region Fields

        private DropOutStack<DebuggerState> _history;
        private DebuggerEnvironment _debuggerEnvironment;
        private DebuggerState _debuggerState;
        private bool _isExecuting;
        private HashSet<InstructionPointer> _breakpoints = new HashSet<InstructionPointer>();

        private CancellationTokenSource _currentLongRunning;
        #endregion

        #region Constructor

        public Debugger()
        {
            _debuggerEnvironment = new DebuggerEnvironment(this);
        }
        #endregion

        #region Properties

        public DebuggerState CurrentState
        {
            get => _debuggerState;
            private set
            {
                _debuggerState = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsDebugging));
                NotifyPropertyChanged(nameof(IsProgramRunning));
                NotifyPropertyChanged(nameof(HistoryCount));
            }
        }

        public bool IsDebugging => CurrentState != null;
        public bool IsProgramRunning => (CurrentState?.ExecutionContext.IsRunning).GetValueOrDefault();
        public int HistoryCount => (_history?.Count).GetValueOrDefault();

        public bool IsExecuting
        {
            get { return _isExecuting; }
            private set
            {
                _isExecuting = value;
                NotifyPropertyChanged();
            }
        }

        public ISet<InstructionPointer> Breakpoints
        {
            get => new HashSet<InstructionPointer>(_breakpoints);
        }
        #endregion

        #region Methods

        public void StartDebugging(InstructionProgram program, int maxHistorySize = DEFAULT_MAX_HISTORY_SIZE)
        {
            _history = new DropOutStack<DebuggerState>(maxHistorySize);
            CurrentState = new DebuggerState(program);
        }

        public async void StopDebugging()
        {
            Break();
            await Task.Yield();
            _history = null;
            CurrentState = null;
        }

        private void DoNext(DebuggerState state)
        {
            Instruction currentInstruction = state.ExecutionContext.GetCurrentInstruction();
            currentInstruction(state.ExecutionContext, _debuggerEnvironment);
            state.ExecutionContext.AdvanceInstructionPointer();
            state.Round++;
        }

        private void StepInternal()
        {
            _history.Push(CurrentState);

            DebuggerState state = new DebuggerState(CurrentState);
            _debuggerEnvironment.DebuggerState = state;

            DoNext(state);

            CurrentState = state;
        }

        public void Step()
        {
            if (!IsDebugging) return;
            if (IsExecuting) return;

            StepInternal();
        }

        public async void Continue()
        {
            if (IsExecuting) return;
            IsExecuting = true;
            _currentLongRunning = new CancellationTokenSource();

            DebuggerState state = CurrentState;

            await Task.Run(() =>
            {
                while (state.ExecutionContext.IsRunning)
                {
                    _history.Push(state);
                    state = new DebuggerState(state);
                    _debuggerEnvironment.DebuggerState = state;

                    DoNext(state);

                    if (_breakpoints.Contains(state.ExecutionContext.InstructionPointer)) break;
                    if (_currentLongRunning.IsCancellationRequested) break;
                }
            });

            CurrentState = state;
            IsExecuting = false;
        }

        public async void Skip()
        {
            if (IsExecuting) return;
            IsExecuting = true;
            _currentLongRunning = new CancellationTokenSource();

            DebuggerState state = new DebuggerState(CurrentState);

            await Task.Run(() =>
            {
                _history.Push(CurrentState);
                _debuggerEnvironment.DebuggerState = state;
                while (state.ExecutionContext.IsRunning)
                {
                    DoNext(state);

                    if (_breakpoints.Contains(state.ExecutionContext.InstructionPointer)) break;
                    if (_currentLongRunning.IsCancellationRequested) break;
                }
            });

            CurrentState = state;
            IsExecuting = false;
        }

        public async void AutoStep(int delay = AUTO_STEP_DELAY)
        {
            if (IsExecuting) return;
            IsExecuting = true;
            _currentLongRunning = new CancellationTokenSource();

            await Task.Yield();

            while (!_currentLongRunning.IsCancellationRequested)
            {
                StepInternal();
                if (_breakpoints.Contains(_debuggerState.ExecutionContext.InstructionPointer)) break;
                await Task.Delay(delay);
            }

            IsExecuting = false;
        }

        public void Break()
        {
            if (!IsDebugging) return;
            if (!IsExecuting) return;
            if (_currentLongRunning == null) return;

            _currentLongRunning.Cancel();
        }

        public void Rewind()
        {
            if (!IsDebugging) return;
            if (IsExecuting) return;
            if (_history.Count == 0) return;

            CurrentState = _history.Pop();
        }

        public bool ToggleBreakpoint(InstructionPointer position)
        {
            if (_breakpoints.Contains(position))
            {
                _breakpoints.Remove(position);
                NotifyPropertyChanged(nameof(Breakpoints));
                return false;
            }
            else
            {
                _breakpoints.Add(position);
                NotifyPropertyChanged(nameof(Breakpoints));
                return true;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string member = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
        }
        #endregion
    }
}
