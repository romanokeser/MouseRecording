using MouseRecording.Utils;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace MouseRecording
{
	public partial class MainWindow : Window
	{
		private HeatMap heatmap;

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

		private void MouseMoveHandler(object sender, MouseEventArgs e)
		{
			int xCoord = (int)e.GetPosition(null).X;
			int yCoord = (int)e.GetPosition(null).Y;

			DatabaseHelper.InsertMouseCoordinates(xCoord, yCoord);

			// Add the mouse coordinates to the heatmap
			heatmap.AddHeatPoint(xCoord, yCoord, 255); // You can adjust the intensity as needed
		}

		private void UpdateHeatMap()
		{
			heatmap.Render();
		}

		private void StartRecBtn_Click(object sender, RoutedEventArgs e)
		{
			MouseRecHelper.StartTracking(MouseMoveHandler);
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			MouseRecHelper.StopTracking();
		}

		private void generateHeatmapBtn_Click(object sender, RoutedEventArgs e)
		{
			UpdateHeatMap();
			ExportHeatmapAsImage(heatmap, "heatmap_image.jpg");
			heatmap.RenderAndExport("heatmap_image.jpg");

		}

		private void ExportHeatmapAsImage(FrameworkElement element, string filePath)
		{
			// Create a RenderTargetBitmap to render the heatmap
			RenderTargetBitmap rtb = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(element);

			// Create a JPEG encoder
			BitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));

			// Save the image to file
			using (var stream = File.Create(filePath))
			{
				encoder.Save(stream);
			}

			MessageBox.Show($"Heatmap image saved to:\n{filePath}", "Heatmap Saved", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

}