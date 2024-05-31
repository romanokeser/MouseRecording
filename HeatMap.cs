using MouseRecording.Utils;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MouseRecording
{
	public class HeatMap : FrameworkElement
	{
		public HeatMap() { }

		/// <summary>
		/// Renders the heatmap based on the mouse coordinates recorded in the specified recording.
		/// </summary>
		/// <param name="currentRecordingName">The name of the current recording.</param>
		public void Render(string currentRecordingName)
		{
			int numCellsX = 20;
			int numCellsY = 20;

			try
			{
				DataTable mouseCoordinates = DatabaseHelper.GetMouseCoordinates(currentRecordingName);
				List<(int, int)> recordedCoordinates = ExtractCoordinates(mouseCoordinates);

				string filePath = $"{currentRecordingName}.txt";
				int[,] counts = CalculatePointCounts(recordedCoordinates, numCellsX, numCellsY);
				WritePointCountsToFile(counts, filePath);

				ExecutePythonScript(filePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during rendering: {ex.Message}");
			}
		}

		/// <summary>
		/// Extracts coordinates from the given DataTable.
		/// </summary>
		/// <param name="mouseCoordinates">DataTable containing mouse coordinates.</param>
		/// <returns>List of tuples representing the coordinates.</returns>
		private List<(int, int)> ExtractCoordinates(DataTable mouseCoordinates)
		{
			var recordedCoordinates = new List<(int, int)>();

			foreach (DataRow row in mouseCoordinates.Rows)
			{
				byte[] coordinatesBytes = (byte[])row["coordinates"];
				recordedCoordinates.AddRange(DeserializeCoordinatesList(coordinatesBytes));
			}

			return recordedCoordinates;
		}

		/// <summary>
		/// Calculates the point counts for each cell in the heatmap grid.
		/// </summary>
		/// <param name="points">List of points representing mouse coordinates.</param>
		/// <param numCellsX">Number of cells along the X axis.</param>
		/// <param numCellsY">Number of cells along the Y axis.</param>
		/// <returns>2D array representing the counts of points in each cell.</returns>
		private int[,] CalculatePointCounts(List<(int, int)> points, int numCellsX, int numCellsY)
		{
			int[,] pointCounts = new int[numCellsX, numCellsY];
			int screenWidth = SpecsUtility.ScreenWidth;
			int screenHeight = SpecsUtility.ScreenHeight;
			foreach (var (x, y) in points)
			{
				if (IsPointInsideBounds(x, y, screenWidth, screenHeight))
				{
					int cellX = (int)Math.Floor((double)x / (screenWidth / numCellsX));
					int cellY = (int)Math.Floor((double)y / (screenHeight / numCellsY));

					if (cellX >= 0 && cellX < numCellsX && cellY >= 0 && cellY < numCellsY) // Additional boundary check
					{
						pointCounts[cellX, cellY]++;
					}
				}
			}

			return pointCounts;
		}

		/// <summary>
		/// Checks if the given point is inside the 1920x1080 bounds.
		/// </summary>
		/// <returns>True if the point is inside the bounds, otherwise false.</returns>
		private bool IsPointInsideBounds(int x, int y, int screenWidth, int screenHeight)
		{
			return x >= 0 && x < screenWidth && y >= 0 && y < screenHeight;
		}

		/// <summary>
		/// Writes the point counts to a file.
		/// </summary>
		/// <param name="pointCounts">2D array of point counts.</param>
		/// <param name="filePath">Path to the output file.</param>
		private void WritePointCountsToFile(int[,] pointCounts, string filePath)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(filePath))
				{
					int numRows = pointCounts.GetLength(0);
					int numCols = pointCounts.GetLength(1);

					for (int i = 0; i < numRows; i++)
					{
						for (int j = 0; j < numCols; j++)
						{
							writer.Write(pointCounts[i, j] + " ");
						}
						writer.WriteLine();
					}
				}
			}
			catch (IOException ioEx)
			{
				Console.WriteLine($"Error writing to file: {ioEx.Message}");
			}
		}

		/// <summary>
		/// Executes a Python script to render the heatmap.
		/// </summary>
		/// <param name="filePath">Path to the file containing point counts.</param>
		private void ExecutePythonScript(string filePath)
		{
			string pythonScriptPath = @"D:\MouseRecording\render.py"; // Update this path
			string pythonInterpreter = @"python"; // Use "python" or "python3" depending on your setup

			var startInfo = new ProcessStartInfo
			{
				FileName = pythonInterpreter,
				Arguments = $"\"{pythonScriptPath}\" \"{filePath}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			try
			{
				using (Process process = Process.Start(startInfo))
				{
					if (process != null)
					{
						string result = process.StandardOutput.ReadToEnd();
						Console.Write(result);

						string errorResult = process.StandardError.ReadToEnd();
						if (!string.IsNullOrEmpty(errorResult))
						{
							Console.WriteLine("Errors:");
							Console.Write(errorResult);
						}

						process.WaitForExit();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error executing Python script: {ex.Message}");
			}
		}

		/// <summary>
		/// Deserializes a list of coordinates from a byte array.
		/// </summary>
		/// <param name="coordinatesBytes">Byte array containing serialized coordinates.</param>
		/// <returns>List of tuples representing the coordinates.</returns>
		private List<(int, int)> DeserializeCoordinatesList(byte[] coordinatesBytes)
		{
			var recordedCoordinates = new List<(int, int)>();

			try
			{
				using (var ms = new MemoryStream(coordinatesBytes))
				using (var reader = new BinaryReader(ms))
				{
					int count = reader.ReadInt32();

					for (int i = 0; i < count; i++)
					{
						int x = reader.ReadInt32();
						int y = reader.ReadInt32();
						recordedCoordinates.Add((x, y));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deserializing coordinates: {ex.Message}");
			}

			return recordedCoordinates;
		}
	}
}
