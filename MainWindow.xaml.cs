using MouseRecording.Utils;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MouseRecording
{
	public partial class MainWindow : Window
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

		private HeatMap _heatmap;
		private DispatcherTimer _mouseTimer;
		private readonly List<(int, int)> _recordedCoordinates = [];

		public MainWindow()
		{
			InitializeHeatMap();
			DatabaseHelper.InitializeDatabase();
		}

		private void InitializeHeatMap()
		{
			_heatmap = new HeatMap();
			Grid.SetRow(_heatmap, 1);
		}
		

		private void StartMouseTimer()
		{
			_mouseTimer = new DispatcherTimer();
			_mouseTimer.Interval = TimeSpan.FromMilliseconds(3); // Set interval to 3 milliseconds
			_mouseTimer.Tick += MouseTimer_Tick;
			_mouseTimer.Start();
		}

		private void StopMouseTimer()
		{
			_mouseTimer?.Stop();
			string recordName = recordNameTextbox.Text;
			DatabaseHelper.InsertMouseCoordinates(recordName, _recordedCoordinates);
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			Win32Point point = new Win32Point();
			GetCursorPos(ref point);

			int xCoord = point.X;
			int yCoord = point.Y;

			_recordedCoordinates.Add((xCoord, yCoord));

			// Add the mouse coordinates to the heatmap
			_heatmap.AddHeatPoint(xCoord, yCoord, 255);
		}

		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			_recordedCoordinates.Clear(); // Clear previously recorded coordinates
			StartMouseTimer();
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			StopMouseTimer();
		}

		private void UpdateHeatMap()
		{
			_heatmap.Render();
		}

		private void generateHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
			UpdateHeatMap();
		}

		private void openImageBtn_Click(object sender, RoutedEventArgs e)
		{
			string imagePath = "heatmap_image1.jpg";
			try
			{
				Process.Start("explorer.exe", imagePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while trying to open the image file: {ex.Message}");
				// Handle the exception as needed
			}
		}

		private void clearRecsBtn_Click(object sender, RoutedEventArgs e)
		{
			DatabaseHelper.WipeAllData();
		}

		private void openPopupBtn_Click(object sender, RoutedEventArgs e)
		{
			var notificationWindow = new NotificationWindow();
			notificationWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			notificationWindow.Show();
		}
	}
}
