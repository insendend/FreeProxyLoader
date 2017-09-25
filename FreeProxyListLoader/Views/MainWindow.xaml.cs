using System.Windows;
using System.Windows.Controls;

namespace FreeProxyListLoader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // for rise TextProperty changed
            (sender as TextBox)
                ?.GetBindingExpression(TextBox.TextProperty)
                ?.UpdateSource();
        }
    }
}
