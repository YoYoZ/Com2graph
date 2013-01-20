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
namespace Reader
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
            string[] temp = SerialPort.GetPortNames();
            for (int i = 0; i < temp.Length; i++)
            {
                comboBox1.Items.Add(temp[i].ToString());
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            Form1.speed = Convert.ToInt32(textBox1.Text);
            if (!String.IsNullOrWhiteSpace(comboBox1.Items[comboBox1.SelectedIndex].ToString()))
            {
                this.Visible = false;
                new Form1(comboBox1.Items[comboBox1.SelectedIndex].ToString()).Visible = true;
            }
        }
    }
}
