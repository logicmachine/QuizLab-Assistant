using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizLabAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void SendInput(int nInputs, Input[] pInputs, int cbsize);

        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        static extern int MapVirtualKey(int wCode, int wMapType);

        private int GWL_EXSTYLE = -20;

        private enum WS_EX : long
        {
            TOOLWINDOW = 0x00000080,
            NOACTIVATE = 0x08000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MouseInput
        {
            public int X;
            public int Y;
            public int Data;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KeyboardInput
        {
            public short VirtualKey;
            public short ScanCode;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HardwareInput
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Input
        {
            public int Type;
            public InputUnion ui;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MouseInput Mouse;
            [FieldOffset(0)]
            public KeyboardInput Keyboard;
            [FieldOffset(0)]
            public HardwareInput Hardware;
        }

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;
        private const int KEYEVENTF_SCANCODE = 0x0008;
        private const int KEYEVENTF_UNICODE = 0x0004;

        private const int MAPVK_VK_TO_VSC = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private static async Task SendKeyToVRChat(Keys key, bool shift)
        {
            IntPtr hwnd = FindWindow("UnityWndClass", "VRChat");
            if (hwnd != IntPtr.Zero) {
                SetForegroundWindow(hwnd);
                if (shift)
                {
                    var shift_inputs = new List<Input>();
                    var input = new Input();
                    input.Type = 1;
                    input.ui.Keyboard.VirtualKey = 0; // 0xA0; // VK_LSHIFT
                    input.ui.Keyboard.ScanCode = (short)MapVirtualKey(0xA1, MAPVK_VK_TO_VSC);
                    Debug.WriteLine($"VK_LSHIFT => {input.ui.Keyboard.ScanCode}");
                    input.ui.Keyboard.Flags = KEYEVENTF_SCANCODE;
                    input.ui.Keyboard.Time = 0;
                    input.ui.Keyboard.ExtraInfo = IntPtr.Zero;
                    shift_inputs.Add(input);
                    SendInput(shift_inputs.Count, shift_inputs.ToArray(), Marshal.SizeOf(input));
                    await Task.Delay(50);
                }
                var inputs = new List<Input>();
                {
                    var input = new Input();
                    input.Type = 1;
                    input.ui.Keyboard.VirtualKey = 0; // (short)key;
                    input.ui.Keyboard.ScanCode = (short)MapVirtualKey((int)key, MAPVK_VK_TO_VSC);
                    Debug.WriteLine($"KEY => {input.ui.Keyboard.ScanCode}");
                    input.ui.Keyboard.Flags = KEYEVENTF_SCANCODE;
                    input.ui.Keyboard.Time = 0;
                    input.ui.Keyboard.ExtraInfo = IntPtr.Zero;
                    inputs.Add(input);
                }
                {
                    var input = new Input();
                    input.Type = 1;
                    input.ui.Keyboard.VirtualKey = 0; // (short)key;
                    input.ui.Keyboard.ScanCode = (short)MapVirtualKey((int)key, MAPVK_VK_TO_VSC);
                    input.ui.Keyboard.Flags = KEYEVENTF_KEYUP | KEYEVENTF_SCANCODE;
                    input.ui.Keyboard.Time = 0;
                    input.ui.Keyboard.ExtraInfo = IntPtr.Zero;
                    inputs.Add(input);
                }
                SendInput(inputs.Count, inputs.ToArray(), Marshal.SizeOf(inputs[0]));
                if (shift)
                {
                    await Task.Delay(50);
                    var shift_inputs = new List<Input>();
                    var input = new Input();
                    input.Type = 1;
                    input.ui.Keyboard.VirtualKey = 0; // 0xA0; // VK_LSHIFT
                    input.ui.Keyboard.ScanCode = (short)MapVirtualKey(0xA1, MAPVK_VK_TO_VSC);
                    input.ui.Keyboard.Flags = KEYEVENTF_KEYUP | KEYEVENTF_SCANCODE;
                    input.ui.Keyboard.Time = 0;
                    input.ui.Keyboard.ExtraInfo = IntPtr.Zero;
                    shift_inputs.Add(input);
                    SendInput(shift_inputs.Count, shift_inputs.ToArray(), Marshal.SizeOf(input));
                }
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.Enter, false);
        }

        private async void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.T, false);
        }

        private async void FailButton_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.Q, false);
        }

        private async void SuccButton_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.E, false);
        }

        private async void P1Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D1, false);
        }

        private async void M1Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D1, true);
        }

        private async void P2Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D2, false);
        }

        private async void M2Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D2, true);
        }

        private async void P3Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D3, false);
        }

        private async void M3Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D3, true);
        }

        private async void P4Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D4, false);
        }

        private async void M4Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D4, true);
        }

        private async void P5Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D5, false);
        }

        private async void M5Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D5, true);
        }

        private async void P6Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D6, false);
        }

        private async void M6Button_Click(object sender, RoutedEventArgs e)
        {
            await SendKeyToVRChat(Keys.D6, true);
        }
    }
}
