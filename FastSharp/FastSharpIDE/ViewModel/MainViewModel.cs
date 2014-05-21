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

using System.Collections.ObjectModel;
using System.Linq;
using FastSharp.Engine.Core;
using FastSharp.Engine.RoslynOld;
using FastSharpIDE.Common;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastSharpIDE.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Scripting integration
        private IScriptingEngine _scriptingEngine;
        private CancellationTokenSource _cancellationToken;

        public ObservableCollection<CompilerError> BuildErrors { get; set; }

        public ConsoleOutStream ConsoleOutput { get; set; }
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
                    var textChanges = Observable.FromEvent<EventHandler, string>(
                        h =>
                        {
                            EventHandler handler = (sender, e) =>
                                h(value.Text);
                            return handler;
                        },
                        h => value.TextChanged += h,
                        h => value.TextChanged -= h);

                    value.TextChanged += (sender, args) => BuildErrors.Clear();

                    // update errors
                    textChanges
                        .Throttle(TimeSpan.FromMilliseconds(250))
                        .Subscribe(SourceChanged);
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
            BuildErrors = new ObservableCollection<CompilerError>();
            Document = new TextDocument();

            ExecuteCommand = new RelayCommand<string>(Execute);
            CancelExecutionCommand = new RelayCommand(CancelExecution);

            Console.SetOut(ConsoleOutput = new ConsoleOutStream());
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
                ConsoleOutput.Output.Clear();

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
                var result = await _scriptingEngine.ExecuteAsync(code);

                if (result.CompilerErrors.Any())
                {
                    BuildErrors.Clear();
                    BuildErrors.AddRange(result.CompilerErrors);
                    Status.SetStatus(result.CompilerErrors.First().Text, StatusType.Error);
                }
                else
                {
                    var message = ConsoleOutput.Output.ToString();
                    message += result.ReturnValue == null ? "** no results from the execution **" : result.ReturnValue.ToString();

                    Status.SetInfo("Executed");
                    ExecutionResult = new ExecutionResultViewModel
                    {
                        Message = message,
                        Type = ExecutionResultType.Success
                    };
                }
            }
            catch (OperationCanceledException)
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
            finally
            {
                IsExecuting = false;
                _cancellationToken = null;
            }
        }

        public void Load()
        {
            Status = new StatusViewModel();

            _scriptingEngine = new RoslynScriptingEngine();
        }

        private void SourceChanged(string code)
        {
            var result = _scriptingEngine.Compile(code);
            BuildErrors.Clear();
            if (result.CompilerErrors.Any())
            {
                BuildErrors.AddRange(result.CompilerErrors);
                Status.SetStatus(result.CompilerErrors.First().Text, StatusType.Error);
            }
            else
            {
                Status.SetReady();
            }
        }
    }
}