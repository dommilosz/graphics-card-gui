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
        }

        public bool isText = false;

        private void button1_Click(object sender, EventArgs e)
        {
            isText = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isText = false;
            this.Close();
        }
    }
}
