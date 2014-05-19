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

using System;
using FastSharpIDE.Common;
using FastSharpIDE.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FastSharpIDE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Execute(editor.SelectedText);
        }

        private void ExecuteAll_Click(object sender, RoutedEventArgs e)
        {
            Execute(editor.Text);
        }

        private void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    String text;
                    if (Keyboard.IsKeyDown(Key.LeftShift) ||
                        Keyboard.IsKeyDown(Key.RightShift))
                        text = editor.Text;
                    else
                        text = editor.SelectedText;

                    Execute(text);
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = SimpleIoc.Default.GetInstance<MainViewModel>();

            _vm.Text =
@"var x = 10;
x == 10";

            _vm.PropertyChanged += _vm_PropertyChanged;

            _vm.Load();
        }

        #region Interaction with the view model
        private void Execute(string text)
        {
            _vm.ExecuteCommand.Execute(text);
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Member<MainViewModel>.Name(m => m.ExecutionResult))
            {
                if (_vm.ExecutionResult == null)
                    return;

                switch (_vm.ExecutionResult.Type)
                {
                    case ExecutionResultType.Success:
                        results.Foreground = Brushes.Black;
                        break;

                    case ExecutionResultType.Error:
                        results.Foreground = Brushes.DarkRed;
                        break;
                }
            }
        }
        #endregion
    }
}
