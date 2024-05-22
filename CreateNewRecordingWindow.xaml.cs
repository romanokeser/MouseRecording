using MouseRecording.Utils;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace MouseRecording
{
	/// <summary>
	/// Interaction logic for CreateNewRecordingWindow.xaml
	/// </summary>
	public partial class CreateNewRecordingWindow : Window
	{

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public int X;
			public int Y;
		};

		//private HeatMap _heatmap;
		private DispatcherTimer _mouseTimer;
		private readonly List<(int, int)> _recordedCoordinates = [];

		public CreateNewRecordingWindow()
		{
			InitializeComponent();
			InitializeHeatMap();
			DatabaseHelper.InitializeDatabase();
		}

		private void InitializeHeatMap()
		{
			HeatMap _heatmap = new HeatMap();
		}

		private void StartMouseTimer()
		{
			_mouseTimer = new DispatcherTimer();
			_mouseTimer.Interval = TimeSpan.FromMilliseconds(3); // Set interval to 3 milliseconds
			_mouseTimer.Tick += MouseTimer_Tick;
			_mouseTimer.Start();
		}

		public void StopMouseTimer()
		{
			_mouseTimer?.Stop();
			string recordName = recordNameTextbox.Text;
			DatabaseHelper.InsertMouseCoordinates(recordName, _recordedCoordinates);

			RecordingNameHolder.CurrentRecordingName = recordName;
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			Win32Point point = new Win32Point();
			GetCursorPos(ref point);

			int xCoord = point.X;
			int yCoord = point.Y;

			_recordedCoordinates.Add((xCoord, yCoord));

		}

		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			_recordedCoordinates.Clear(); // Clear previously recorded coordinates
			StartMouseTimer();

			var recordingFinalWindow = new ActiveRecWindow(this);
			recordingFinalWindow.Show();
			recordingFinalWindow.Activate();
			this.Close();
		}
		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Show();
			startupWindow.Activate();
			this.Close();
		}
	}
}