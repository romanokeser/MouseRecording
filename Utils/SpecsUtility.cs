using System;
using System.Drawing;
using System.Windows.Forms;

namespace MouseRecording.Utils
{
	/// <summary>
	/// Provides information about the screen specifications.
	/// </summary>
	public static class SpecsUtility
	{
		public static int ScreenWidth { get; private set; }
		public static int ScreenHeight { get; private set; }

		/// <summary>
		/// Gathers the screen width and height.
		/// </summary>
		public static void GatherScreenDimensions()
		{
			ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
			ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
		}
	}
}
