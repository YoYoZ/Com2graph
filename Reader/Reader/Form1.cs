using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
namespace Reader
{

    public partial class Form1 : Form
    {
        public Form1(string comPort)
        {
            InitializeComponent();
            s = new SerialPort(comPort, speed, Parity.None, 8, StopBits.One); //Initializing com port
            chart1.Series[0].Name = "Линейка";
            chart1.Series[0].Color = Color.Red; //Creating chart.
        }
      public static int speed;  
      public static SerialPort s;
      private Thread th;
      private Thread drawer;
      private Bitmap btmFront1;
      private Graphics grFront1;
      private int lenght = 0;
      private int previousLenght = 0; //This all is required to operate with program.
      private int y = 15;
      public static string[] st;
      public static char[] stt = new char[2];
      public static bool working = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            createBitmap();
            th = new Thread(delegate()
                {
                    s.Open();
                    Debug.WriteLine(s.CtsHolding);
                    while (working)
                    {
                        previousLenght = lenght;
                        string LINEW = s.ReadLine();
                        this.Invoke(new Action(() => { this.addText(LINEW); }));    //This is engine that downoloads info from COM port.
                        try
                        {
                            lenght = Convert.ToInt32(LINEW);
                        }
                        catch (Exception exc)           
                        {
                            lenght = previousLenght;
                            this.Invoke(new Action(() => 
                            {
                                this.addText("Exception: " + exc.Message); //Just typing the exception that happened, to debug it.
                                this.addText("Value is: " + LINEW);
                            }));
                        }
                        this.Invoke(new Action(
                            () => { this.drawLine();}));
                    }
                });
            drawer = new Thread(this.drawLine);
            working = true;
            th.Start();
            drawer.Start();
        }

        public void addText(string msg)
        {
            richTextBox1.AppendText(msg);  //Typing the received info.
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            working = false;
            Environment.Exit(0); //Properly closing drawing stream AND program itself.
        }
        private void drawLine()
        {

            lock (new object()) //Lock is required, because different thread's can use this code.
            {
                try
                {
                    createBitmap(); //Creating another bitmap to operate.
                    grFront1.FillRectangle(Brushes.Red, 0, 30, lenght * 15, 30); //Filling the grid.
                    this.Invoke(
                        new Action(() => { this.updateChart(); })); //Updating graph which is created by .NET services.
                }
                catch { } //We don't need crash, do we?

            }
            
        }

        private void updateChart()
        {
            
                chart1.Series[0].Points.Add(new DataPoint((double)y, (double)lenght)); //This adds points to graph to draw.
                y += 1;
            this.Refresh(); //And refreshe's it.
        }

        private void createBitmap()
        {
            lock (new object()) //Yup. Another lock.
            {
                try
                {
                    btmFront1 = new Bitmap(700, 100);           //All this
                    grFront1 = Graphics.FromImage(btmFront1);   //Is code
                    pictureBox1.Image = btmFront1;              //That draws our own graph.
                    for (int x = 0; x <= 700; x++)              //Neither it's effective, nor it's beautifull code, but why not?
                    {
                        for (int y = 0; y <= 100; y++)
                        {
                            grFront1.DrawLine(Pens.Black, x * 15, 1, x * 15, 100); //Yes. Grid.
                            grFront1.DrawLine(Pens.Black, 1, x * 15, 700, x * 15);
                        }
                    };
                }
                catch { };
            }
        }

    }
}
