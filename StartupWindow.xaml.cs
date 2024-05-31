using MouseRecording.Utils;
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
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {

        public StartupWindow()
        {
			InitializeComponent();
            InitializeScreenDimensionsAsync();
        }

		private async void InitializeScreenDimensionsAsync()
		{
			await Task.Delay(1000); // Delay for 1 second
			SpecsUtility.GatherScreenDimensions();
            labelTest.Content = SpecsUtility.ScreenWidth.ToString() + SpecsUtility.ScreenHeight.ToString();
		}

		private void navBtnNewRec_Click(object sender, RoutedEventArgs e)
		{
            var createNewRecordingWindow = new CreateNewRecordingWindow();
            createNewRecordingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            createNewRecordingWindow.Show();
            this.Close();
		}

		private void navBtnManageRecs_Click(object sender, RoutedEventArgs e)
		{
            var listOfRecordings = new ListOfRecordings();
            listOfRecordings.Activate();
            listOfRecordings.Show();
            this.Close();
        }
    }
}
