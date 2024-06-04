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
	/// Interaction logic for ListOfRecordings.xaml
	/// </summary>
	public partial class ListOfRecordings : Window
	{
		public ObservableCollection<RecordModel> Records { get; set; }
		private string _selectedRecordingName;

		public ListOfRecordings()
		{
			InitializeComponent();
			DataContext = this; // Set the DataContext to this instance of ListOfRecordings
			Records = new ObservableCollection<RecordModel>();

			try
			{
				Records = new ObservableCollection<RecordModel>(RetrieveRecordsFromDatabase());
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while retrieving records: {ex.Message}");
			}
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var startupWindow = new StartupWindow();
				startupWindow.Show();
				this.Close();
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while navigating back: {ex.Message}");
			}
		}

		private List<RecordModel> RetrieveRecordsFromDatabase()
		{
			List<RecordModel> records = new List<RecordModel>();

			try
			{
				DataTable dataTable = DatabaseHelper.GetMouseCoordinates();

				foreach (DataRow row in dataTable.Rows)
				{
					RecordModel record = new RecordModel();
					record.Name = row["record_name"].ToString();
					records.Add(record);
				}
			}
			catch (Exception ex)
			{
				LogError(ex);
				System.Windows.MessageBox.Show($"An error occurred while retrieving records from database: {ex.Message}");
			}

			return records;
		}

		private void listViewRecordings_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Check if an item is selected
			if (listViewRecordings.SelectedItem != null)
			{
				// Get the selected item's name
				_selectedRecordingName = ((RecordModel)listViewRecordings.SelectedItem).Name;
				// Show the review button
				reviewButton.Visibility = Visibility.Visible;
			}
		}

		private async void ReviewButton_Click(object sender, RoutedEventArgs e)
		{
			// Implement the review logic here
			if (!string.IsNullOrEmpty(_selectedRecordingName))
			{
				try
				{
					loadingScreen.Visibility = Visibility.Visible; // Show loading screen
																   // Initialize HeatMap and render
					HeatMap heatMap = new HeatMap();
					await Task.Run(() => heatMap.Render(_selectedRecordingName));
				}
				catch (Exception ex)
				{
					LogError(ex);
					System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
				}
				finally
				{
					loadingScreen.Visibility = Visibility.Collapsed; // Hide loading screen
				}
			}
		}

		private void LogError(Exception ex)
		{
			// Implement logging logic here
			// You can log to a file, Windows Event Log, or any other logging framework
			string logFilePath = "error_log.txt";
			File.AppendAllText(logFilePath, $"{DateTime.Now}: {ex.ToString()}{Environment.NewLine}");
		}
	}
}
