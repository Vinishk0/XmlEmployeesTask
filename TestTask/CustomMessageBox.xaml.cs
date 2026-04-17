using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace TestTask
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message, string title, MessageBoxImage image)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;

            switch (image)
            {
                case MessageBoxImage.Error:
                    IconText.Text = "❌";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")); 
                    break;
                case MessageBoxImage.Warning:
                    IconText.Text = "⚠️";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")); 
                    break;
                case MessageBoxImage.Information:
                default:
                    IconText.Text = "✨";
                    IconText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")); 
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public static void Show(string message, string title, MessageBoxImage image = MessageBoxImage.Information)
        {
            var msgBox = new CustomMessageBox(message, title, image);
            msgBox.ShowDialog();
        }
    }
}