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
    /// Interaction logic for CreateNewRecordingWindow.xaml
    /// </summary>
    public partial class CreateNewRecordingWindow : Window
    {
        public CreateNewRecordingWindow()
        {
            InitializeComponent();
        }

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Show();
			startupWindow.Activate();
			this.Close();
		}

		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			var recordingFinalWindow = new RecordingFinalWindow();
			recordingFinalWindow.Show();
			recordingFinalWindow.Activate();
			this.Close();
		}
	}
}
