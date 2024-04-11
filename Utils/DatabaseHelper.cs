using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MouseRecording.Utils
{
	public class DatabaseHelper
	{
		private const string DatabaseFileName = "mouse_coordinates.db";

		public static void InitializeDatabase()
		{
			if (!File.Exists(DatabaseFileName))
			{
				SQLiteConnection.CreateFile(DatabaseFileName);

				using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
				{
					connection.Open();

					using (var command = new SQLiteCommand(connection))
					{
						command.CommandText = @"
                        CREATE TABLE mouse_coordinates (
                            x_coord INTEGER,
                            y_coord INTEGER
                        )";
						command.ExecuteNonQuery();
					}
				}
			}
		}

		public static void InsertMouseCoordinates(int xCoord, int yCoord)
		{
			using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "INSERT INTO mouse_coordinates VALUES (@xCoord, @yCoord)";
					command.Parameters.AddWithValue("@xCoord", xCoord);
					command.Parameters.AddWithValue("@yCoord", yCoord);

					command.ExecuteNonQuery();
				}
			}
		}

		public static DataTable GetMouseCoordinates()
		{
			var dataTable = new DataTable();

			using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "SELECT * FROM mouse_coordinates";

					using (var adapter = new SQLiteDataAdapter(command))
					{
						adapter.Fill(dataTable);
					}
				}
			}

			return dataTable;
		}
	}
}
