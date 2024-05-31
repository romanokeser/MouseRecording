using MouseRecording.Utils;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace MouseRecording
{
	/// <summary>
	/// Interaction logic for SuccessfulRecordingWindow.xaml
	/// </summary>
	public partial class SuccessfulRecordingWindow : Window
	{
		private CreateNewRecordingWindow _createNewRecordingWindow;
		private HeatMap _heatMap;
		public SuccessfulRecordingWindow(CreateNewRecordingWindow createNewRecordingWindow)
		{
			_createNewRecordingWindow = createNewRecordingWindow;
			_heatMap = new HeatMap();

			InitializeComponent();
		}

		private async void reviewHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
			loadingScreen.Visibility = Visibility.Visible; // Show loading screen

			try
			{
				string imagePath = RecordingNameHolder.CurrentRecordingName;
				await Task.Run(() => _heatMap.Render(imagePath));
			}
			catch (Exception ex)
			{
				System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
			}
			finally
			{
				loadingScreen.Visibility = Visibility.Collapsed; // Hide loading screen
			}
		}

		private void backBtn2_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Activate();
			startupWindow.Show();
			this.Close();
		}
	}
}