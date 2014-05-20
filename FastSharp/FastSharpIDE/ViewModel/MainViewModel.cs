// Copyright 2014 Toni Petrina
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastSharpIDE.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Roslyn interaction
        private ScriptEngine _engine;
        private Session _session;
        private CancellationTokenSource _cancellationToken;

        public ReadOnlyArray<CommonDiagnostic> BuildErrors { get; set; }
        #endregion

        #region Bindable properties

        public TextDocument Document
        {
            get { return Get<TextDocument>(); }
            set
            {
                Set(value);

                if (value != null)
                {
                    var observable = Observable.FromEvent<EventHandler, string>(
                        h =>
                        {
                            EventHandler handler = (sender, e) =>
                                h(value.Text);
                            return handler;
                        },
                        h => value.TextChanged += h,
                        h => value.TextChanged -= h)
                        .Throttle(TimeSpan.FromMilliseconds(750));
                    observable.Subscribe(SourceChanged);
                }
            }
        }

        public string Text
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public ExecutionResultViewModel ExecutionResult
        {
            get { return Get<ExecutionResultViewModel>(); }
            set { Set(value); }
        }

        public StatusViewModel Status
        {
            get { return Get<StatusViewModel>(); }
            set { Set(value); }
        }

        public bool IsExecuting
        {
            get { return Get<bool>(); }
            set { Set(value); }
        }
        #endregion

        public RelayCommand<string> ExecuteCommand { get; set; }
        public RelayCommand CancelExecutionCommand { get; set; }

        public MainViewModel()
        {
            Document = new TextDocument();

            ExecuteCommand = new RelayCommand<string>(Execute);
            CancelExecutionCommand = new RelayCommand(CancelExecution);
        }

        private void CancelExecution()
        {
            if (_cancellationToken != null)
                _cancellationToken.Cancel(true);
        }

        private async void Execute(string code)
        {
            await ExecuteInternalAsync(code);
        }

        private async Task ExecuteInternalAsync(string code)
        {
            if (IsExecuting)
                return;
            IsExecuting = true;

            ExecutionResult = new ExecutionResultViewModel();

            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    ExecutionResult = new ExecutionResultViewModel
                    {
                        Message = "Nothing to execute",
                        Type = ExecutionResultType.Warning
                    };
                    Status.SetReady();
                    return;
                }

                Status.SetInfo("Executing...");

                _cancellationToken = new CancellationTokenSource();
                var o = await Task.Run(() => _session.Execute(code), _cancellationToken.Token);
                _cancellationToken = null;

                Status.SetInfo("Executed");
                var message = o == null ? "** no results from the execution **" : o.ToString();

                ExecutionResult = new ExecutionResultViewModel
                {
                    Message = message,
                    Type = ExecutionResultType.Success
                };
            }
            catch (OperationCanceledException ex)
            {
                _cancellationToken = null;
                ExecutionResult = new ExecutionResultViewModel
                {
                    Message = "...",
                    Type = ExecutionResultType.Warning
                };
                Status.SetStatus("Execution stopped", StatusType.Error);
            }
            catch (Exception e)
            {
                _cancellationToken = null;
                ExecutionResult = new ExecutionResultViewModel
                {
                    Message = e.ToString(),
                    Type = ExecutionResultType.Error
                };
                Status.SetStatus("Failed", StatusType.Error);
            }

            IsExecuting = false;
        }

        public void Load()
        {
            _engine = new ScriptEngine();
            _session = _engine.CreateSession();
            Status = new StatusViewModel();
        }

        private void SourceChanged(string code)
        {
            try
            {
                var result = _session.CompileSubmission<object>(code);
                Status.SetReady();
            }
            catch (CompilationErrorException ex)
            {
                BuildErrors = ex.Diagnostics;
                if (BuildErrors.Any())
                {
                    var error = BuildErrors[0];
                    Status.SetStatus(error.ToString(), StatusType.Error);
                }
            }
        }
    }
}