using System.Diagnostics;
using System.Windows.Controls;

namespace FastSharpIDE.Controls
{
    public class LinkButton : Button
    {
        public LinkButton()
        {
            Click += LinkButton_Click;
        }

        void LinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(Content as string);
        }
    }
}
