using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MouseRecording
{
    /// <summary>
    /// Interaction logic for SuccessfulRecordingWindow.xaml
    /// </summary>
    public partial class SuccessfulRecordingWindow : Window
    {
        public SuccessfulRecordingWindow()
        {
            InitializeComponent();
        }

		private void reviewHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
            //open the image! todo
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            var initialWindow = new StartupWindow();
            initialWindow.Activate();
            initialWindow.Show();
            this.Close();
        }
    }
}
