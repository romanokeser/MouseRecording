using System;
using System.Windows;
using System.Windows.Threading;

namespace MouseRecording
{
	public partial class ActiveRecWindow : Window
	{
		private CreateNewRecordingWindow _createNewRecordingWindow;
		private DispatcherTimer _beepTimer;
		private bool _isImageVisible;
		private double _imageTurnedOffTime = 500; 
		private double _imageTurnedOnTime = 1500;
		public ActiveRecWindow(CreateNewRecordingWindow createNewRecordingWindow)
		{
			InitializeComponent();
			_createNewRecordingWindow = createNewRecordingWindow;

			// Initialize the DispatcherTimer
			_beepTimer = new DispatcherTimer();
			_beepTimer.Interval = TimeSpan.FromMilliseconds(2); // 1 second total interval (0.7s visible + 0.3s hidden)
			_beepTimer.Tick += OnBeepTimerTick;
			_beepTimer.Start();

			_isImageVisible = true;
		}

		private void OnBeepTimerTick(object sender, EventArgs e)
		{
			if (_isImageVisible)
			{
				recImage.Opacity = 0; // Hide the image
				_beepTimer.Interval = TimeSpan.FromMilliseconds(_imageTurnedOffTime); // Set the interval to 0.3 seconds
			}
			else
			{
				recImage.Opacity = 1; // Show the image
				_beepTimer.Interval = TimeSpan.FromMilliseconds(_imageTurnedOnTime); // Set the interval to 0.7 seconds
			}

			_isImageVisible = !_isImageVisible; // Toggle the visibility flag
		}

		private void stopRecordingBtn_Click(object sender, RoutedEventArgs e)
		{
			_createNewRecordingWindow.StopMouseTimer();

			var successfulRecordingWindow = new SuccessfulRecordingWindow(_createNewRecordingWindow);
			successfulRecordingWindow.Activate();
			successfulRecordingWindow.Show();
			this.Close();
		}
	}
}
