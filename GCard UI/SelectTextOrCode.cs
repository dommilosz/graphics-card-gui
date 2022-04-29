using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCard_UI
{
    public partial class SelectTextOrCode : Form
    {
        public SelectTextOrCode()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        public int type = 0;

        private void button1_Click_1(object sender, EventArgs e)
        {
            type = comboBox1.SelectedIndex;
            this.Close(); 
        }
    }
}
