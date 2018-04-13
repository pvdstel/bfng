using bfng.Parsing;
using bfng.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace bfng.Debugging
{
    public class Debugger : INotifyPropertyChanged
    {
        public const int DEFAULT_MAX_HISTORY_SIZE = 250;

        #region Fields

        private DropOutStack<DebuggerState> _history;
        private DebuggerEnvironment _debuggerEnvironment;
        private DebuggerState _debuggerState;
        private bool _isExecuting;

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
            }
        }

        public bool IsDebugging => CurrentState != null;
        public bool IsProgramRunning => (CurrentState?.ExecutionContext.IsRunning).GetValueOrDefault();

        public bool IsExecuting
        {
            get { return _isExecuting; }
            private set
            {
                _isExecuting = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public void StartDebugging(InstructionProgram program, int maxHistorySize = DEFAULT_MAX_HISTORY_SIZE)
        {
            _history = new DropOutStack<DebuggerState>(maxHistorySize);
            CurrentState = new DebuggerState(program);
        }

        public void StopDebugging()
        {
            Break();
            _history = null;
            CurrentState = null;
        }

        public void Step()
        {
            if (!IsDebugging) return;
            if (IsExecuting) return;

            _history.Push(CurrentState);

            DebuggerState state = new DebuggerState(CurrentState);
            _debuggerEnvironment.DebuggerState = state;

            Instruction currentInstruction = state.ExecutionContext.GetCurrentInstruction();
            currentInstruction(state.ExecutionContext, _debuggerEnvironment);
            state.ExecutionContext.AdvanceInstructionPointer();

            CurrentState = state;
        }

        public async void Skip()
        {
            if (IsExecuting) return;
            IsExecuting = true;
            _currentLongRunning = new CancellationTokenSource();

            _history.Push(CurrentState);

            DebuggerState state = new DebuggerState(CurrentState);
            _debuggerEnvironment.DebuggerState = state;

            await Task.Run(() =>
            {
                while (state.ExecutionContext.IsRunning)
                {
                    Instruction currentInstruction = state.ExecutionContext.GetCurrentInstruction();
                    currentInstruction(state.ExecutionContext, _debuggerEnvironment);
                    state.ExecutionContext.AdvanceInstructionPointer();

                    if (_currentLongRunning.IsCancellationRequested) break;
                }
            });

            CurrentState = state;

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string member = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
        }
        #endregion
    }
}
