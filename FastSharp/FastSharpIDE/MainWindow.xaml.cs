namespace FastSharpIDE
{
    using System;
    using System.Linq;
    using System.Windows;
    using Roslyn.Scripting.CSharp;
    using Roslyn.Scripting;
    using System.Windows.Media;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScriptEngine _engine;
        private Session _session;

        public MainWindow()
        {
            InitializeComponent();
            editor.Text = @"var x = 10;
x == 10";

            _engine = new ScriptEngine();
            _session = Session.Create();
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
                var o = _engine.Execute(text, _session);

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

        private void editor_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && Keyboard.IsKeyDown(Key.LeftCtrl))
                Execute(editor.Text);
        }
    }
}
