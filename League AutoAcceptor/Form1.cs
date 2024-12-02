using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
namespace League_AutoAcceptor
{
    public partial class Form1 : Form
    {
        
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public static bool IsGameRunning()
        {
            Process[] pname = Process.GetProcessesByName("LeagueClientUx");
            if (pname.Length != 0)
            {
                return true;


            }
            else
            {
                return false;
            }
        }
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static (int X, int Y, int Width, int Height) GetWindowDimensions(string processName)
        {
            var process = Process.GetProcessesByName(processName);
            if (process.Length == 0)
                return (0, 0, 0, 0);

            IntPtr windowHandle = process[0].MainWindowHandle;

            if (GetWindowRect(windowHandle, out RECT rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                return (rect.Left, rect.Top, width, height);
            }
            else
            {
                throw new InvalidOperationException("Could not retrieve window dimensions.");
            }
        }
        public static (int width, int height, int X, int Y) GetWindowInfo()
        {
            try
            {
                var WindowInfo = GetWindowDimensions("LeagueClientUx");
                return (WindowInfo.Width, WindowInfo.Height, WindowInfo.X, WindowInfo.Y);
            }
            catch
            (Exception e)
            {
                MessageBox.Show(e.Message);
                return (0, 0, 0, 0);
            }
        }
        public static Color GetPixelColor(int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }
        public Form1()
        {

            Color clr = Color.FromArgb(255, 35, 64, 112);
            InitializeComponent();
            this.BackColor = clr;
        }
        private void PrintXY()
        {
            MessageBox.Show("X: " + GetWindowInfo().X.ToString() + "Y: " + GetWindowInfo().Y.ToString());
        }
        private void PrintCalculatedXY()
        {
            int x = GetWindowInfo().X + (GetWindowInfo().width / 2) + 10;
            int y = GetWindowInfo().Y + ((GetWindowInfo().height / 3) * 2);
            MessageBox.Show("X: " + x.ToString() + "Y: " + y.ToString());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
            label4.ForeColor = Color.White;
            label5.ForeColor = Color.White;
            label6.ForeColor = Color.White;
            label7.ForeColor = Color.White;
            label8.ForeColor = Color.White;
            label9.ForeColor = Color.White;
            label10.ForeColor = Color.White;
            label11.ForeColor = Color.White;

            if (IsAdministrator())
            {
                label4.ForeColor = Color.Lime;
                label4.Text = "Administrator!";
            }
            else
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Not Administrator!";
            }
            if (IsGameRunning())
            {
                label2.ForeColor = Color.Lime;
                label2.Text = "Found client!";
            }
            else
            {
                label2.ForeColor = Color.Red;
                label2.Text = "Client not Found!";
            }
            if (GetWindowInfo().width != 0)
            {

                if (GetWindowInfo().height != 0)
                {
                    label6.ForeColor = Color.Lime;
                    label7.ForeColor = Color.Lime;
                    label6.Text = GetWindowInfo().width + "px";
                    label7.Text = GetWindowInfo().height + "px";
                }
                else
                {
                    label6.ForeColor = Color.Red;
                    label7.ForeColor = Color.Red;
                    label6.Text = "Error";
                    label7.Text = "Error";
                }
            }
            Thread backgroundThread = new Thread(new ThreadStart(BackgroundProcess));
            backgroundThread.IsBackground = true;
            backgroundThread.Start();



        }
        private void BackgroundProcess()
        {
            label11.ForeColor = Color.White;
            label8.ForeColor = Color.Red;
            label8.Text = "Not Found";
            int x = GetWindowInfo().X + (GetWindowInfo().width / 2);
            int y = GetWindowInfo().Y + ((GetWindowInfo().height / 10) * 8);
            POINT cursorPos;
            while (true)
            {

                Color color = GetPixelColor(x, y);
                label11.Text = color.ToString();
                Color targetColor = Color.FromArgb(255, 30, 37, 42);

                Thread.Sleep(1);
                if (color.ToArgb() == targetColor.ToArgb())
                {

                    GetCursorPos(out cursorPos);
                    label11.ForeColor = Color.Lime;
                    SetCursorPos(x, y);
                    Thread.Sleep(5);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    Thread.Sleep(5);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(5);
                    SetCursorPos(cursorPos.X, cursorPos.Y);
                    label8.ForeColor = Color.Lime;
                    label8.Text = "Found!";
                    if (checkBox1.Checked)
                    {
                        BackgroundProcess();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(
                    "Do you want to continue?",
                    "Confirmation",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                        if (result == DialogResult.OK)
                        {
                            BackgroundProcess();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            Environment.Exit(0);
                        }
                    }


                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

      
    }
}
