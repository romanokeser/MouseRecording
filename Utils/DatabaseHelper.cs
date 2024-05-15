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
						    record_name TEXT,
						    coordinates BLOB
						)";
						command.ExecuteNonQuery();
					}
				}
			}
		}

		public static void InsertMouseCoordinates(string recordName, List<(int, int)> coordinatesList)
		{
			using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					// Serialize coordinates into a single byte array
					byte[] coordinatesBytes = SerializeCoordinatesList(coordinatesList);

					command.CommandText = "INSERT INTO mouse_coordinates (record_name, coordinates) VALUES (@recordName, @coordinates)";
					command.Parameters.AddWithValue("@recordName", recordName);
					command.Parameters.AddWithValue("@coordinates", coordinatesBytes);

					command.ExecuteNonQuery();
				}
			}
		}


		// Method to serialize coordinates into byte array
		private static byte[] SerializeCoordinatesList(List<(int, int)> coordinatesList)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					// Write the number of coordinates to the stream
					writer.Write(coordinatesList.Count);

					// Write each coordinate pair to the stream
					foreach (var (x, y) in coordinatesList)
					{
						writer.Write(x);
						writer.Write(y);
					}
				}
				// Return the byte array containing serialized coordinates
				return ms.ToArray();
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

		public static DataTable GetMouseCoordinates(string recordName)
		{
			var dataTable = new DataTable();

			using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					// Modify the query to select mouse coordinates for a specific recording
					command.CommandText = "SELECT * FROM mouse_coordinates WHERE record_name = @recordName";
					command.Parameters.AddWithValue("@recordName", recordName);

					using (var adapter = new SQLiteDataAdapter(command))
					{
						adapter.Fill(dataTable);
					}
				}
			}

			return dataTable;
		}


		public static DataTable WipeAllData()
		{
			var dataTable = new DataTable();

			using (var connection = new SQLiteConnection($"Data Source={DatabaseFileName};Version=3;"))
			{
				connection.Open();

				using (var command = new SQLiteCommand(connection))
				{
					command.CommandText = "DELETE FROM mouse_coordinates";
					command.ExecuteNonQuery();
				}
			}

			return dataTable;
		}

	}
}
