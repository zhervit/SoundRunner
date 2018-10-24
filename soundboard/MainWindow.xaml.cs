using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Button = System.Windows.Controls.Button;
using FlowDirection = System.Windows.FlowDirection;

namespace soundboard
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			InitComponent();
			GetDevices();
		}

		private void GetDevices()
		{
			string dev = "";
			for (int n = -1; n < WaveOut.DeviceCount; n++)
			{
				var caps = WaveOut.GetCapabilities(n);
				txtDevices.Items.Add($"{n}: {caps.ProductName}");
			}

			
		}

	
	

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			var sr = SoundRunner.Instance;
			sr.PlaySound();
		}

		

		private void OnButtonStopClick(object sender, EventArgs args)
		{
			var sr = SoundRunner.Instance;
			sr.Stop();
		}

		

		private void InitComponent()
		{
			var flowPanel = new FlowLayoutPanel();
			flowPanel.FlowDirection = (System.Windows.Forms.FlowDirection)FlowDirection.LeftToRight;
			flowPanel.Margin = new Padding(10);


			_hookID = SetHook(_proc);



			//var buttonPlay = new Button();
			//buttonPlay.Text = "Play";
			//buttonPlay.Click += OnButtonPlayClick;
			//flowPanel.Controls.Add(btnPlay);

			//var buttonStop = new Button();
			//buttonStop.Text = "Stop";
			//buttonStop.Click += OnButtonStopClick;
			//flowPanel.Controls.Add(buttonStop);

			//this.Controls.Add(flowPanel);

			//this.FormClosing += OnButtonStopClick;
		}

		

		private int deviceNumber = -2;
		private void txtDevices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var selectedDevice = txtDevices.SelectedItems;
			var s = selectedDevice[0].ToString().Substring(0, 1);
			Int32.TryParse(s, out deviceNumber);
			SoundRunner.Instance.deviceNumber = this.deviceNumber;
		}

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 0x0100;

		private static LowLevelKeyboardProc _proc = HookCallback;

		private static IntPtr _hookID = IntPtr.Zero;


	//	public static void Main()
		//{
		//	_hookID = SetHook(_proc);
		//	Application.Run();
		//	UnhookWindowsHookEx(_hookID);
		//}


		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}


		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				Task.Run(() => RunSound(vkCode));
				//Console.WriteLine((Keys)vkCode);
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		private static string pathToFile= "01.mp3";
		private static void RunSound(int key)
		{
			switch (key)
			{
				case 96:
					//outputDevice?.Stop();
					break;
				case 97:
					pathToFile = "01.mp3";
					break;
				case 98:
					pathToFile = "02.mp3";
					break;
				case 99:
					pathToFile = "03.mp3";
					break;
				case 100:
					//pathToFile = "04.mp3";
					break;
				case 101:
					pathToFile = "05.m4a";
					break;
				case 102:
					pathToFile = "06.m4a";
					break;
				case 103:
					pathToFile = "07.m4a";
					break;
				case 104:
					pathToFile = "08.m4a";
					break;
				case 105:
					pathToFile = "09.m4a";
					break;
				default:
					return;
			}

			var s = SoundRunner.Instance;
			s.PlaySound(pathToFile);
		}


		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,IntPtr wParam, IntPtr lParam);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UnhookWindowsHookEx(_hookID);
		}
	}
}
