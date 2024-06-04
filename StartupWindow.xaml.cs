using MouseRecording.Models;
using MouseRecording.Utils;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
			try
			{
				await Task.Delay(1000); // Delay for 1 second
				SpecsUtility.GatherScreenDimensions();
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while initializing screen dimensions: {ex.Message}");
			}
		}

		private void LogError(Exception ex)
		{
			// Implement logging logic here
			// You can log to a file, Windows Event Log, or any other logging framework
			string logFilePath = "error_log.txt";
			File.AppendAllText(logFilePath, $"{DateTime.Now}: {ex.ToString()}{Environment.NewLine}");
		}

		private void navBtnNewRec_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var createNewRecordingWindow = new CreateNewRecordingWindow();
				createNewRecordingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				createNewRecordingWindow.Show();
				this.Close();
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while navigating to create new recording: {ex.Message}");
			}
		}

		private void navBtnManageRecs_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var listOfRecordings = new ListOfRecordings();
				listOfRecordings.Activate();
				listOfRecordings.Show();
				this.Close();
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while navigating to list of recordingsssssssssssssss: {ex.Message}");
			}
		}
	}
}
