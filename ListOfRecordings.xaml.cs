using MouseRecording.Models;
using MouseRecording.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace MouseRecording
{
	/// <summary>
	/// Interaction logic for ListOfRecordings.xaml
	/// </summary>
	public partial class ListOfRecordings : Window
	{
		public ObservableCollection<RecordModel> Records { get; set; }

		public ListOfRecordings()
		{
			InitializeComponent();
			DataContext = this; // Set the DataContext to this instance of ListOfRecordings
			Records = new ObservableCollection<RecordModel>(RetrieveRecordsFromDatabase());
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			var startupWindow = new StartupWindow();
			startupWindow.Show();
			this.Close();
		}

		private List<RecordModel> RetrieveRecordsFromDatabase()
		{
			List<RecordModel> records = new List<RecordModel>();

			DataTable dataTable = DatabaseHelper.GetMouseCoordinates();

			foreach (DataRow row in dataTable.Rows)
			{
				RecordModel record = new RecordModel();
				record.Name = row["record_name"].ToString();
				records.Add(record);
			}
			return records;
		}
	}
}
