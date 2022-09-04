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
    public partial class PokeForm : Form
    {
        public PokeForm(string poker)
        {
            InitializeComponent();
            this.lblPoke.Text = poker;
        }

        private void PokeForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("poke iniated");
        }
    }
}
