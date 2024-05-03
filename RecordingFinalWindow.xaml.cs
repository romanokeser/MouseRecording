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
    /// Interaction logic for RecordingFinalWindow.xaml
    /// </summary>
    public partial class RecordingFinalWindow : Window
    {
        public RecordingFinalWindow()
        {
            InitializeComponent();
        }

		private void stopRecordingBtn_Click(object sender, RoutedEventArgs e)
		{
            var succesfulRecordinWindow = new SuccessfulRecordingWindow();
            succesfulRecordinWindow.Activate();
            succesfulRecordinWindow.Show();
            this.Close();
		}
	}
}
