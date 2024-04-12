using MouseRecording.Utils;
using System;
using System.Collections.Generic;
using System.Data;
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


		public void Render()
		{
			double widthHeight;
			var mouseCoordinates = DatabaseHelper.GetMouseCoordinates();
			heatMapVisuals.Clear();
			DrawingVisual drawingVisual = new DrawingVisual();

			int screenWidth = 1920;
			int screenHeight = 1080;
			int numCellsX = 20;
			int numCellsY = 20;
			int threshold = 20;

			int[,] cellCounts = CountMouseCoordinates(mouseCoordinates, screenWidth, screenHeight, numCellsX, numCellsY);


			using (DrawingContext dc = drawingVisual.RenderOpen())
			{
				RenderOptions.SetEdgeMode(drawingVisual, EdgeMode.Aliased);
				RenderOptions.SetBitmapScalingMode(drawingVisual, BitmapScalingMode.LowQuality);

				foreach (DataRow row in mouseCoordinates.Rows)
				{
					int x = Convert.ToInt32(row["x_coord"]);
					int y = Convert.ToInt32(row["y_coord"]);
					byte intensity = 15; //intensity value for each point

					widthHeight = intensity / 5;

					// Based on the result of ColorClusters, decide the color of the rectangle
					SolidColorBrush brush = GetRectangleBrush(x, y, cellCounts, numCellsX, numCellsY);

					dc.DrawRectangle(brush, null, new Rect(x - widthHeight / 2, y - widthHeight / 2, widthHeight, widthHeight));
				}
			}

			// Create a RenderTargetBitmap to hold the visual content (optional, if needed for other purposes)
			RenderTargetBitmap rtb = new RenderTargetBitmap(1920, 1080, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(drawingVisual);

			// Create a JpegBitmapEncoder and add the visual content to it
			BitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));

			// Save the encoder's frames into a file stream as a JPG image
			using (var stream = File.Create("heatmap_image1.jpg"))
			{
				encoder.Save(stream);
			}
		}

		private int[,] CountMouseCoordinates(DataTable mouseCoordinates, int screenWidth, int screenHeight, int numCellsX, int numCellsY)
		{
			double cellWidth = screenWidth / numCellsX;
			double cellHeight = screenHeight / numCellsY;

			int[,] cellCounts = new int[numCellsX, numCellsY];

			// Count the points in each cell
			foreach (DataRow row in mouseCoordinates.Rows)
			{
				int x = Convert.ToInt32(row["x_coord"]);
				int y = Convert.ToInt32(row["y_coord"]);

				int cellX = (int)(x / cellWidth);
				int cellY = (int)(y / cellHeight);

				cellCounts[cellX, cellY]++;
			}

			return cellCounts;
		}

		private SolidColorBrush GetRectangleBrush(int x, int y, int[,] cellCounts, int numCellsX, int numCellsY)
		{
			// Determine the cell coordinates
			double cellWidth = (double)1920 / numCellsX;
			double cellHeight = (double)1080 / numCellsY;
			int cellX = (int)(x / cellWidth);
			int cellY = (int)(y / cellHeight);

			// Check if the cell count exceeds the threshold
			if (cellCounts[cellX, cellY] > 20)
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

