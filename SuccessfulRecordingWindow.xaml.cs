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

		private void reviewHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
			string imagePath = RecordingNameHolder.CurrentRecordingName;
			_heatMap.Render(imagePath);
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Activate();
			startupWindow.Show();
			this.Close();
		}
	}
}