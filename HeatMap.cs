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

		private int[,] CalculatePointCounts(List<(int, int)> points, int numCellsX, int numCellsY)
		{
			int[,] pointCounts = new int[numCellsX, numCellsY];

			foreach (var (x, y) in points)
			{
				int cellX = (int)Math.Floor((double)x / (1920 / numCellsX));
				int cellY = (int)Math.Floor((double)y / (1080 / numCellsY));
				pointCounts[cellX, cellY]++;
			}

			return pointCounts;
		}

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
