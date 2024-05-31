using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;

namespace MouseRecording.Utils
{
	public class MouseRecHelper
	{
		private const int WH_MOUSE_LL = 14;
		private const int WM_MOUSEMOVE = 0x0200;

		private static IntPtr hookId = IntPtr.Zero;
		private static LowLevelMouseProc mouseProc;

		public static event EventHandler<System.Windows.Input.MouseEventArgs> MouseMove;

		public static void StartTracking(Action<object, System.Windows.Input.MouseEventArgs> mouseMoveHandler)
		{
			mouseProc = HookCallback;
			hookId = SetHook(mouseProc);

			MouseMove += new EventHandler<System.Windows.Input.MouseEventArgs>(mouseMoveHandler);
		}

		public static void StopTracking()
		{
			UnhookWindowsHookEx(hookId);
			MouseMove = null;
		}

		private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr SetHook(LowLevelMouseProc proc)
		{
			using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
			using (var curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEMOVE)
			{
				MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

				// Get the mouse position relative to the screen
				System.Windows.Point screenPosition = new System.Windows.Point(hookStruct.pt.x, hookStruct.pt.y);

				var mouseEventArgs = new System.Windows.Input.MouseEventArgs(Mouse.PrimaryDevice, 0)
				{
					RoutedEvent = UIElement.MouseMoveEvent
				};

				// Use GetPosition to get X and Y coordinates
				mouseEventArgs.GetPosition(null);

				MouseMove?.Invoke(null, mouseEventArgs);
			}

			return CallNextHookEx(hookId, nCode, wParam, lParam);
		}



		[StructLayout(LayoutKind.Sequential)]
		private struct MSLLHOOKSTRUCT
		{
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			public int x;
			public int y;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
	}
}
