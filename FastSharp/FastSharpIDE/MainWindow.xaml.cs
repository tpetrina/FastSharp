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

namespace FastSharpIDE
{
    using System;
    using System.Windows;
    using Roslyn.Scripting.CSharp;
    using Roslyn.Scripting;
    using System.Windows.Media;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ScriptEngine _engine;
        private Session _session;

        public MainWindow()
        {
            InitializeComponent();
            editor.Text = @"var x = 10;
x == 10";

            _engine = new ScriptEngine();
            _session = _engine.CreateSession();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Execute(editor.SelectedText);
        }

        private void ExecuteAll_Click(object sender, RoutedEventArgs e)
        {
            Execute(editor.Text);
        }

        private void Execute(string text)
        {
            try
            {
                var o = _session.Execute(text);

                results.Foreground = Brushes.Black;

                if (o == null)
                    results.Text = "** no results from the execution **";
                else
                    results.Text = o.ToString();
            }
            catch (Exception e)
            {
                results.Foreground = Brushes.Red;
                results.Text = e.ToString();
            }
        }

        private void editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.IsKeyDown(Key.LeftCtrl))
                Execute(editor.Text);
        }
    }
}
