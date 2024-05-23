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

		private DispatcherTimer _mouseTimer;
		private readonly List<(int, int)> _recordedCoordinates = new();

		public CreateNewRecordingWindow()
		{
			InitializeComponent();
			DatabaseHelper.InitializeDatabase();
		}

		/// <summary>
		/// Initializes the mouse recording timer.
		/// </summary>
		private void StartMouseTimer()
		{
			_mouseTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(3) // Set interval to 3 milliseconds
			};
			_mouseTimer.Tick += MouseTimer_Tick;
			_mouseTimer.Start();
		}

		/// <summary>
		/// Stops the mouse recording timer and saves the recorded coordinates to the database.
		/// </summary>
		public void StopMouseTimer()
		{
			_mouseTimer?.Stop();
			string recordName = recordNameTextbox.Text;
			DatabaseHelper.InsertMouseCoordinates(recordName, _recordedCoordinates);
			RecordingNameHolder.CurrentRecordingName = recordName;
		}

		/// <summary>
		/// Event handler for the mouse timer tick event. Records the current cursor position.
		/// </summary>
		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			Win32Point point = new Win32Point();
			GetCursorPos(ref point);
			_recordedCoordinates.Add((point.X, point.Y));
		}

		/// <summary>
		/// Event handler for the Start Recording button click event. Starts recording mouse coordinates.
		/// </summary>
		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			_recordedCoordinates.Clear(); // Clear previously recorded coordinates
			StartMouseTimer();

			var recordingFinalWindow = new ActiveRecWindow(this);
			recordingFinalWindow.Show();
			recordingFinalWindow.Activate();
			this.Close();
		}

		/// <summary>
		/// Event handler for the Back button click event. Navigates back to the startup window.
		/// </summary>
		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Show();
			startupWindow.Activate();
			this.Close();
		}
	}
}
