using MouseRecording.Utils;
using System;
using System.Data;
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
		private HeatMap heatmap;
		private DispatcherTimer mouseTimer;

		public MainWindow()
		{
			InitializeComponent();
			InitializeHeatMap();
			DatabaseHelper.InitializeDatabase();
		}

		private void InitializeHeatMap()
		{
			heatmap = new HeatMap();
			Grid.SetRow(heatmap, 1);
			grid.Children.Add(heatmap);
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		[StructLayout(LayoutKind.Sequential)]
		internal struct Win32Point
		{
			public int X;
			public int Y;
		};

		private void StartMouseTimer()
		{
			mouseTimer = new DispatcherTimer();
			mouseTimer.Interval = TimeSpan.FromMilliseconds(3); // Set interval to 3 milliseconds
			mouseTimer.Tick += MouseTimer_Tick;
			mouseTimer.Start();
		}

		private void StopMouseTimer()
		{
			mouseTimer?.Stop();
		}

		private void MouseTimer_Tick(object sender, EventArgs e)
		{
			Win32Point point = new Win32Point();
			GetCursorPos(ref point);

			int xCoord = point.X;
			int yCoord = point.Y;

			DatabaseHelper.InsertMouseCoordinates(xCoord, yCoord);

			// Add the mouse coordinates to the heatmap
			heatmap.AddHeatPoint(xCoord, yCoord, 255); // You can adjust the intensity as needed
		}

		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			StartMouseTimer();
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			StopMouseTimer();
		}

		private void UpdateHeatMap()
		{
			heatmap.Render();
		}

		private void generateHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
			UpdateHeatMap();
		}
	}
}
