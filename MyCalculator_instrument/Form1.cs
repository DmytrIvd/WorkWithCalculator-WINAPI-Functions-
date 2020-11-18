using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCalculator_instrument
{

    public partial class Form1 : Form
    {
        

    
    //Path of calculator app 
    //PLEASE CHANGE IT TO YOUR CALCULATOR URI ADDRESS
        const string CalculatorPath= @"C:\Users\User\source\repos\MyCalculator_instrument\MyCalculator_instrument\Calculator\calc.exe";


        public Process CalculatorProcess { get; private set; }

        /*Calculator pointers*/
        public IntPtr EditCalculatorHandle { get; private set; }
        public IntPtr CalculatorWindowHandle { get; private set; }

        #region DLLImport
        [DllImport("USER32.DLL", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
        string lpWindowName);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #region SendMessages
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);
        #endregion
        #endregion




        public Form1()
        {
            InitializeComponent();
        }
        #region EventHandlers
        private void GetFromCalculator_ButtonClick(object sender, EventArgs e)
        {

            // v CheckIfCalculatorClosed();
            if (EditCalculatorHandle == IntPtr.Zero||CalculatorWindowHandle==IntPtr.Zero) LoadCalculator();
            //Дістаємо кількість символів в калькуляторі
            if(IsCalculatorClosed()){
                ReloadCalculator();
            }
            Int32 titleSize = SendMessage((int)EditCalculatorHandle, WM_Values.WM_GETTEXTLENGTH, 0, 0).ToInt32();

          
          
            //Резервуємо місце під символи
            StringBuilder title = new StringBuilder(titleSize + 1);
            //Дістаємо уже число
            SendMessage(EditCalculatorHandle, (uint)WM_Values.WM_GETTEXT, title.Capacity, title);
            textBox1.Text = title.ToString();
           

          
         
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCalculator();
        }
        private void SetToCalculator_ButtonClick(object sender, EventArgs e)
        {
            if (EditCalculatorHandle == IntPtr.Zero || CalculatorWindowHandle == IntPtr.Zero) LoadCalculator();
            if (IsCalculatorClosed())
            {
                ReloadCalculator();
            }
            //Just sending text to calculator field
            SendMessage(EditCalculatorHandle, WM_Values.WM_SETTEXT, 0, textBox1.Text);
        }


        #endregion
        #region CalculatorOperators
        private IntPtr FindCalculator(){
            return FindWindow("SciCalc", "Calculator");
        }


        private void ReloadCalculator()
        {
            CalculatorWindowHandle = IntPtr.Zero;
            EditCalculatorHandle = IntPtr.Zero;
            //CalculatorProcess.Kill();
            LoadCalculator();
        }
        private void LoadCalculator()
        {
            // Verify that Calculator is a running process.

            CalculatorWindowHandle = FindCalculator();
            if (CalculatorWindowHandle == IntPtr.Zero)
            {
                 CalculatorProcess = Process.Start(new ProcessStartInfo(CalculatorPath));

                CalculatorWindowHandle = CalculatorProcess.MainWindowHandle;
               
                //  MessageBox.Show("Calculator is not running. Please start him, his file location is in  admin folder.");

            }
            else{
                CalculatorProcess = Process.GetProcessesByName("Calc").First();
            }

            // Make Calculator the foreground application 
            //SetForegroundWindow(calculatorHandle);

            EditCalculatorHandle = FindWindowEx(CalculatorWindowHandle, IntPtr.Zero, "Edit", "");
        }
       private bool IsCalculatorClosed(){
            return FindCalculator()==IntPtr.Zero;
       }
        #endregion
    }
}
