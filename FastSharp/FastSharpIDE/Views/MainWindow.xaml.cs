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

using FastSharpIDE.Common;
using FastSharpIDE.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.IO;
using System.IO.IsolatedStorage;
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
        private const string TempFileName = "temp";

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

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = SimpleIoc.Default.GetInstance<MainViewModel>();

            using (var isoStore = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (isoStore.FileExists("temp"))
                {
                    using (var fileStream = isoStore.OpenFile(TempFileName, FileMode.OpenOrCreate, FileAccess.Read))
                    using (var reader = new StreamReader(fileStream))
                    {
                        var text = await reader.ReadToEndAsync();
                        if (text.EndsWith(Environment.NewLine))
                            text = text.Substring(0, text.Length - Environment.NewLine.Length);
                        _vm.Text = text;
                    }
                }
                else
                {
                    _vm.Text =
@"// Sample code
// Run to see what happens
var x = 10;
x == 10";
                }
            }
            editor.CaretOffset = editor.Text.Length;

            _vm.Load();
        }

        #region Interaction with the view model
        private void Execute(string text)
        {
            _vm.ExecuteCommand.Execute(text);
        }

        #endregion

        private async void MainWindow_Deactivated(object sender, EventArgs e)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForAssembly())
            using (var fileStream = isoStore.OpenFile("temp", FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteLineAsync(_vm.Text);
            }
        }
    }
}
