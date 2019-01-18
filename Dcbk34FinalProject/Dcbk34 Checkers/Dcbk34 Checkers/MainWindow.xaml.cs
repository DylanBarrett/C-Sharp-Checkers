using System.Windows;

namespace Dcbk34_Checkers
{
    public partial class MainWindow : Window
    {
        CheckerBoard checkerboard;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Checkers_Loaded(object sender, RoutedEventArgs e)
        {
            checkerboard = new CheckerBoard(Checkers);
        }
    }
}