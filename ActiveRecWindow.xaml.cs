using System;
using System.Windows;
using System.Windows.Threading;

namespace MouseRecording
{
	public partial class ActiveRecWindow : Window
	{
		private CreateNewRecordingWindow _createNewRecordingWindow;
		private DispatcherTimer _toggleImageTimer;
		private bool _isImageVisible;

		public ActiveRecWindow(CreateNewRecordingWindow createNewRecordingWindow)
		{
			InitializeComponent();
			_createNewRecordingWindow = createNewRecordingWindow;

			_toggleImageTimer = new DispatcherTimer();
			_toggleImageTimer.Interval = TimeSpan.FromSeconds(0.3);
			_toggleImageTimer.Tick += ToggleImageVisibility;
			_toggleImageTimer.Start();
		}

		private void ToggleImageVisibility(object sender, EventArgs e)
		{
			if (_isImageVisible)
			{
				recImage.Visibility = Visibility.Hidden;
			}
			else
			{
				recImage.Visibility = Visibility.Visible;
			}
			_isImageVisible = !_isImageVisible;
		}

		private void stopRecordingBtn_Click(object sender, RoutedEventArgs e)
		{
			_toggleImageTimer.Stop(); // Stop the timer when stopping the recording

			_createNewRecordingWindow.StopMouseTimer();

			var successfulRecordingWindow = new SuccessfulRecordingWindow(_createNewRecordingWindow);
			successfulRecordingWindow.Activate();
			successfulRecordingWindow.Show();
			this.Close();
		}
	}
}
