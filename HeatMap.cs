using MouseRecording.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MouseRecording
{
	public class HeatMap : FrameworkElement
	{
		private const int GradientNumber = 256; // Number of color gradients

		private VisualCollection heatMapVisuals;
		private List<HeatPoint> heatPoints = new List<HeatPoint>(); // Points of the heat map
		private readonly RadialGradientBrush[] brushes; // Monochrome brushes used for heat map drawing


		public HeatMap()
		{

			heatMapVisuals = new VisualCollection(this);
			brushes = new RadialGradientBrush[GradientNumber];

			for (int i = 0; i < brushes.Length; i++)
			{
				brushes[i] = new RadialGradientBrush(Color.FromArgb((byte)i, 255, 255, 255), Color.FromArgb(0, 255, 255, 255));
			}
		}

		protected override int VisualChildrenCount => heatMapVisuals.Count;

		protected override Visual GetVisualChild(int index)
		{
			if (index < 0 || index >= heatMapVisuals.Count)
				throw new ArgumentOutOfRangeException(nameof(index));

			return heatMapVisuals[index];
		}

		public void AddHeatPoint(int x, int y, byte intensity)
		{
			heatPoints.Add(new HeatPoint(x, y, intensity));
		}

		public void Clear()
		{
			heatPoints.Clear();
		}

		public void Render(string currentRecordingName)
		{
			double widthHeight;
			heatMapVisuals.Clear();
			DrawingVisual drawingVisual = new DrawingVisual();

			int numCellsX = 20;
			int numCellsY = 20;
			int threshold = 20;

			DataTable mouseCoordinates = DatabaseHelper.GetMouseCoordinates(currentRecordingName);

			List<(int, int)> recordedCoordinates = new List<(int, int)>();

			using (DrawingContext dc = drawingVisual.RenderOpen())
			{
				RenderOptions.SetEdgeMode(drawingVisual, EdgeMode.Aliased);
				RenderOptions.SetBitmapScalingMode(drawingVisual, BitmapScalingMode.LowQuality);

				foreach (DataRow row in mouseCoordinates.Rows)
				{
					byte[] coordinatesBytes = (byte[])row["coordinates"];
					recordedCoordinates = DeserializeCoordinatesList(coordinatesBytes);

					foreach (var (x, y) in recordedCoordinates)
					{
						byte intensity = 15; // intensity value for each point
						widthHeight = intensity / 5;

						SolidColorBrush brush = GetRectangleBrush(x, y, coordinatesBytes, numCellsX, numCellsY, threshold);

						dc.DrawRectangle(brush, null, new Rect(x - widthHeight / 2, y - widthHeight / 2, widthHeight, widthHeight));
					}
					StorePointCountFromRectangle(recordedCoordinates, numCellsX, numCellsY);
				}
			}

			RenderTargetBitmap rtb = new RenderTargetBitmap(1920, 1080, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(drawingVisual);

			BitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));

			string imagePath = $"{currentRecordingName}.jpg";
			using (var stream = File.Create(imagePath))
			{
				encoder.Save(stream);
			}

			OpenImageFile(imagePath);

			string filePath = currentRecordingName + ".txt";
			int[,] counts = StorePointCountFromRectangle(recordedCoordinates, numCellsX, numCellsY);
			WritePointCountsToFile(counts, filePath);

			// Call the Python script with the file path as an argument
			string pythonScriptPath = @"D:\MouseRecording\render.py"; // Update this path
			string pythonInterpreter = @"python";

			System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo
			{
				FileName = pythonInterpreter,
				Arguments = $"{pythonScriptPath} {filePath}",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
			{
				using (System.IO.StreamReader reader = process.StandardOutput)
				{
					string result = reader.ReadToEnd();
					Console.Write(result);
				}
			}
		}




		private int[,] StorePointCountFromRectangle(List<(int, int)> points, int numCellsX, int numCellsY)
		{
			int[,] pointCounts = new int[numCellsX, numCellsY];

			foreach (var (x, y) in points)
			{
				// Determine which cell the point belongs to
				int cellX = (int)Math.Floor((double)x / (1920 / numCellsX));
				int cellY = (int)Math.Floor((double)y / (1080 / numCellsY));

				// Increment the count for the corresponding cell
				pointCounts[cellX, cellY]++;
			}

			return pointCounts;
		}

		private void WritePointCountsToFile(int[,] pointCounts, string filePath)
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

		private List<(int, int)> DeserializeCoordinatesList(byte[] coordinatesBytes)
		{
			List<(int, int)> recordedCoordinates = new List<(int, int)>();

			using (MemoryStream ms = new MemoryStream(coordinatesBytes))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					// Read the number of coordinates from the stream
					int count = reader.ReadInt32();

					// Read each coordinate pair from the stream
					for (int i = 0; i < count; i++)
					{
						int x = reader.ReadInt32();
						int y = reader.ReadInt32();
						recordedCoordinates.Add((x, y));
					}
				}
			}

			return recordedCoordinates;
		}


		private void OpenImageFile(string filePath)
		{
			try
			{
				Process.Start(filePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while trying to open the image file: {ex.Message}");
			}
		}

		private int[,] CountMouseCoordinates(byte[] allCoordinatesBlob, int numCellsX, int numCellsY)
		{
			double cellWidth = (double)1920 / numCellsX;
			double cellHeight = (double)1080 / numCellsY;

			int[,] cellCounts = new int[numCellsX, numCellsY];

			List<(int, int)> recordedCoordinates = DeserializeCoordinatesList(allCoordinatesBlob);

			foreach (var (x, y) in recordedCoordinates)
			{
				int cellX = (int)(x / cellWidth);
				int cellY = (int)(y / cellHeight);

				cellCounts[cellX, cellY]++;
			}

			return cellCounts;
		}

		private SolidColorBrush GetRectangleBrush(int x, int y, byte[] allCoordinatesBlob, int numCellsX, int numCellsY, int threshold)
		{
			double cellWidth = (double)1920 / numCellsX;
			double cellHeight = (double)1080 / numCellsY;
			int cellX = (int)(x / cellWidth);
			int cellY = (int)(y / cellHeight);

			int[,] cellCounts = CountMouseCoordinates(allCoordinatesBlob, numCellsX, numCellsY);

			// Check if the cell count exceeds the threshold
			if (cellCounts[cellX, cellY] > threshold)
			{
				return Brushes.Red;
			}
			else
			{
				return Brushes.White;
			}
		}

	}
}

public struct HeatPoint
{
	public int X;
	public int Y;
	public byte Intensity;

	public HeatPoint(int x, int y, byte intensity)
	{
		X = x;
		Y = y;
		Intensity = intensity;
	}
}

