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
            if (!isText)
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
                else
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
            if (isText)
            {
                return Actions.ValueToString(ActionSize.String, value);
            }
            return "";
        }
        public Code_input()
        {
            InitializeComponent();
            textBox1.Text = code;
        }
        public Code_input(string value)
        {
            InitializeComponent();
            code = value;
            if (code.Length >= 1)
            {
                if (code[0] == 'c')
                {
                    isText = false;
                }
                if (code[0] == 't')
                {
                    isText = true;
                }
                textBox1.Text = code.Substring(1);
            }
            else
            {
                var stoc = new SelectTextOrCode();
                stoc.ShowDialog();
                isText = stoc.isText;
                textBox1.Text = code;
            }
            button3.Text = isText ? "Change To CODE" : "Change To TEXT";
        }

        public Code_input(bool text)
        {
            InitializeComponent();
            code = "";
            if (code.Length >= 1)
            {
                if (code[0] == 'c')
                {
                    isText = false;
                }
                if (code[0] == 't')
                {
                    isText = true;
                }
                textBox1.Text = code.Substring(1);
            }
            else
            {
                isText = text;
                textBox1.Text = code;
            }
            button3.Text = isText ? "Change To CODE" : "Change To TEXT";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            code = (isText?"t":"c")+textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool isText = false;
        private void button3_Click(object sender, EventArgs e)
        {
            isText = !isText;
            button3.Text = isText ? "Change To CODE" : "Change To TEXT";

            string a = "";

            if (isText)
            {
                foreach (var item in textBox1.Text.Split(' '))
                {
                    try
                    {
                        a += (char)(Convert.ToInt32(item));
                    }
                    catch { }
                }
            }
            if (!isText)
            {
                try
                {
                    foreach (var item in textBox1.Text)
                    {
                        a += (int)item + " ";
                    }
                }
                catch { }
            }
            textBox1.Text = a;
        }

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
    }
}
