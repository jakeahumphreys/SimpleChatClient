using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClient
{
    public partial class Form2 : Form
    {
        public Form2(string poker)
        {
            InitializeComponent();
            this.lblPoke.Text = poker;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Console.WriteLine("poke iniated");
        }
    }
}
