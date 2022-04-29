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
    public partial class Code_input : Form
    {
        public string code;
        public string parsedCode(bool sized = true,bool dec = false)
        {
            string value = "";
            if (code.Length > 1)
            {
                value = code.Substring(1);
            }
            else
            {
                return "";
            }
            if (type == 0)
            {
                value = value.Replace("\r", " ");
                value = value.Replace("\n", " ");
                value = value.Replace("\t", " ");
                while (value.Contains("  "))
                {
                    value = value.Replace("  ", " ");
                }
                if (!dec)
                {
                    List<byte> bytes = new List<byte>();

                    foreach (var item in value.Split(' '))
                    {
                        try
                        {
                            int i = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                            if (i > 255) continue;
                            bytes.Add(Convert.ToByte(i));
                        }
                        catch
                        {

                        }
                    }
                    string v2 = "";

                    if (sized)
                    {
                        int length = bytes.Count;
                        v2 = (length >> 8) + " " + (length & 0x00FF) + " ";
                    }
                    foreach (var item in bytes)
                    {
                        v2 += item + " ";
                    }
                    return v2;
                }
                else if (type == 1)
                {
                    string v2 = "";

                    if (sized)
                    {
                        int length = value.Split(' ').Count();
                        v2 = (length >> 8) + " " + (length & 0x00FF) + " ";
                    }
                    v2 += value;
                    return v2;
                }
                

                
            }
            if (type == 1)
            {
                return Actions.ValueToString(ActionSize.String, value);
            }
            else if (type == 2)
            {
                return Actions.ValueToString(ActionSize.Audio, value);
            }
            return "";
        }
        public Code_input()
        {
            InitializeComponent();
            textBox1.Text = code;
            comboBox1.SelectedIndex = 0;
        }
        public Code_input(string value)
        {
            InitializeComponent();
            code = value;
            comboBox1.SelectedIndex = 0;
            if (code.Length >= 1)
            {
                type = Convert.ToInt32(""+code[0]);
                comboBox1.SelectedIndex = type;
                textBox1.Text = code.Substring(1);
            }
            else
            {
                var stoc = new SelectTextOrCode();
                stoc.ShowDialog();
                type = stoc.type;
                comboBox1.SelectedIndex = type;
                textBox1.Text = code;
            }
        }

        public Code_input(int type)
        {
            InitializeComponent();
            this.type = type;
            comboBox1.SelectedIndex = type;
            textBox1.Text = code;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            code = type+textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        int type = 0;

        private void button4_Click(object sender, EventArgs e)
        {
            var opf = new OpenFileDialog();
            opf.Filter = "jpg|*.jpg|png|*.png";
            
            if(opf.ShowDialog() == DialogResult.OK)
            {
                string a = "0 ";
                Bitmap img = new Bitmap(opf.FileName);
                for (int j = 0; j < img.Width; j++)
                {
                    for (int i = 0; i < img.Height; i++)
                    {
                        Color pixel = img.GetPixel(i, j);
                        Color8b c = new Color8b(pixel);
                        a += c.value.ToString("X") + " ";
                        
                    }
                }
                textBox1.Text = a;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var opf = new OpenFileDialog();
            opf.Filter = "jpg|*.jpg|png|*.png";

            if (opf.ShowDialog() == DialogResult.OK)
            {
                string a = "1 ";
                Bitmap img = new Bitmap(opf.FileName);
                Color pixel = img.GetPixel(0, 0);
                Color8b c = new Color8b(pixel);
                int lastVal = c.value;
                for (int j = 0; j < img.Width; j++)
                {
                    for (int i = 0; i < img.Height; i++)
                    {
                        pixel = img.GetPixel(i, j);
                        c = new Color8b(pixel);
                        if (lastVal != c.value)
                        {
                            int __i = i;
                            int __j = j;
                            if (__i < 0)
                            {
                                __i = 0;
                                //__j = j - 1;
                            }
                            a += lastVal.ToString("X") + " " + (((__i) >> 8).ToString("X") + " " + ((__i) & 0xFF).ToString("X")) + " " + ((__j >> 8).ToString("X") + " " + (__j & 0xFF).ToString("X")) + " ";
                            lastVal = c.value;
                        }
                    }
                }
                int _i = img.Width-1;
                int _j = img.Height - 1;
                a += lastVal.ToString("X") + " " + ((_i >> 8).ToString("X") + " " + (_i & 0xFF).ToString("X")) + " " + ((_j >> 8).ToString("X") + " " + (_j & 0xFF).ToString("X")) + " ";
                textBox1.Text = a;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = comboBox1.SelectedIndex;
            textBox1.Text = "";
        }
    }
}
