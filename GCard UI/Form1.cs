using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GCard_UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            argument1.SubCommand = Actions.commands;
            argument1.RedrawArg();

            for (int i = 0; i < 128; i++)
            {
                listBox1.Items.Add(i);
            }
            listBox1.SelectedIndex = 0;
            try
            {
                var lines = File.ReadAllLines("saved_cmds.dat");
                foreach (var item in lines)
                {
                    listBox4.Items.Add(item);
                }
            }
            catch
            {

            }
        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.RtsEnable = true;
                    serialPort1.DtrEnable = true;
                    serialPort1.Open();
                    button1.Text = "Close";
                }
                else
                {
                    serialPort1.Close();
                    button1.Text = "Open";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string cmd_txt = "";
            cmd_txt = argument1.GetCommandRec();
            cmd_txt = ResolveInsertAfter(cmd_txt);
            while (cmd_txt.Contains("  "))
                cmd_txt = cmd_txt.Replace("  ", " ");

            if (textBox1.Text != cmd_txt)
            {
                textBox1.Text = cmd_txt;
            }

            if (serialPort1.IsOpen)
            {
                button1.Text = "Close";
                button1.BackColor = Color.Green;
            }
            else
            {
                button1.Text = "Open";
                button1.BackColor = Color.Transparent;
            }

            try
            {
                int i = 0;
                string a = "";
                string b = "";
                while (TextBuffer.Count > 4000)
                {
                    TextBuffer.RemoveAt(0);
                }
                foreach (var item in TextBuffer)
                {
                    a += Convert.ToInt32(item) + " ";
                    b += Convert.ToChar(item);
                }
                textBox2.Text = a;
                textBox3.Text = b;
            }
            catch
            {

            }

        }

        public string ResolveInsertAfter(string inp)
        {
            inp = inp.Replace("{::selectedObjIndex}", listBox1.SelectedIndex.ToString());
            return inp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendCommand(textBox1.Text,true);
        }

        public enum ObjectType
        {
            ObjType_RECTANGLE = 1, ObjType_TEXT = 2, ObjType_CIRCLE = 3, ObjType_LINE = 4,
            NULL = -1
        }

        public struct VObject
        {
            public ObjectType type;
            public int x, y;
            public Color8b color;
            public bool visible;
        }
        public VObject[] objects = new VObject[128];

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ReadBuffer.Clear();
                serialPort1.Write(new byte[] { 8, 0, 255, 1 }, 0, 4);
                int length = (ConsumeRBF() << 8) | ConsumeRBF();
                byte[] buff = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    buff[i] = Convert.ToByte(ConsumeRBF());
                }
                for (int i = 0; i < 128; i++)
                {
                    objects[i].type = ObjectType.NULL;
                }
                for (int i = 0; i + 6 < length; i += 9)
                {
                    objects[buff[i]].type = (ObjectType)buff[i + 1];
                    objects[buff[i]].x = (buff[i + 2] << 8) | (buff[i + 3] & 0x00FF);
                    objects[buff[i]].x = (buff[i + 4] << 8) | (buff[i + 5] & 0x00FF);
                    objects[buff[i]].visible = Convert.ToBoolean(buff[i + 6]);
                    objects[buff[i]].color = new Color8b(buff[i + 7]);
                }
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                for (int i = 0; i < 128; i++)
                {
                    var obj = objects[i];
                    if (objects[i].type != ObjectType.NULL)
                    {
                        listBox1.Items.Add($"{i} - {obj.type}");
                        listBox2.Items.Add($"{i} - {obj.type} - {(obj.visible ? "visible" : "hiidden")} - color: {obj.color.value}");
                        //listView1.Items.Add(new ListViewItem() { Text= $"{obj.color.value}",BackColor = obj.color.color });
                    }
                    else
                    {
                        listBox1.Items.Add(i);
                    }

                }
            }
            catch
            {

            }
            listBox1.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            TextBuffer.Clear();
        }

        List<byte> ReadBuffer = new List<byte>();
        List<byte> TextBuffer = new List<byte>();
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            AddByteRBF();
        }

        public void AddByteRBF()
        {
            try
            {
                int toread = serialPort1.BytesToRead;
                byte[] b = new byte[toread];
                if (toread > 2000) toread = 2000;
                serialPort1.Read(b, 0, toread);
                serialPort1.DiscardInBuffer();
                AddByteRBF(b);
            }
            catch { }
        }

        public void AddByteRBF(byte[] b)
        {
            ReadBuffer.AddRange(b);
            TextBuffer.AddRange(b);
        }

        public byte ConsumeRBF()
        {
            for (int i = 0; i < 10; i++)
            {
                if (ReadBuffer.Count < 1)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    break;
                }

            }

            byte b = ReadBuffer[0];
            ReadBuffer.RemoveAt(0);
            return b;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendCommand("4 3");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var ci = new Code_input(0);
            ci.ShowDialog();
            string value = ci.parsedCode(false, true);
            if (value == null) return;

            if (value.Length > 0)
            {
                List<byte> bytes = new List<byte>();
                foreach (var item in value.Split(' '))
                {
                    try
                    {
                        bytes.Add(Convert.ToByte(item));
                    }
                    catch
                    {

                    }
                }
                ReadBuffer.Clear();
                serialPort1.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Generator gen = new Generator();
            gen.ShowDialog();
            if (gen.isUpload)
            {
                ((ComboBox)argument1.Controls[0].Controls[0]).SelectedIndex = 10;
                var ci = new Code_input("c" + Generator.ConvertToString(gen.saved_bytes, Generator.StringEncoding.HEX));
                argument1.next_arg.next_arg.dvalue = ci.code;
                argument1.next_arg.next_arg.value = Actions.ValueToString(ActionSize.Code, ci);
            }
        }

        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = (string)listBox3.SelectedItem;
            text = text.Split(':')[1];
            SendCommand(text);
        }

        public void SendCommand(string text, bool addToLog=false)
        {
            try
            {
                while (text.Contains("  "))
                    text = text.Replace("  ", " ");

                if (addToLog)
                {
                    listBox3.Items.Insert(0, $"{((ComboBox)argument1.Controls[0].Controls[0]).SelectedItem}: {textBox1.Text.Trim()}");
                }

                List<byte> bytes = new List<byte>();
                foreach (var item in text.Trim().Split(' '))
                {
                    bytes.Add(Convert.ToByte(item));
                }
                ReadBuffer.Clear();
                progressBar1.Minimum = 0;
                progressBar1.Maximum = bytes.Count();
                for (int i = 0; i < bytes.Count(); i += 512)
                {
                    serialPort1.Write(bytes.ToArray(), i, Math.Min(bytes.Count() - i, 512));
                    progressBar1.Value = i;
                }

                int status = ConsumeRBF();
                if ((ActionStatus)status != ActionStatus.STATUS_OK)
                {
                    MessageBox.Show(Enum.GetNames(typeof(ActionStatus))[status]);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error while sending: " + ex);
            }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox4.Items.Add(listBox3.SelectedItem);
            List<string> lines = new List<string>();
            foreach (var item in listBox4.Items)
            {
                lines.Add((string)item);
            }
            File.WriteAllLines("saved_cmds.dat", lines);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(listBox3.SelectedIndex < 0)
            {
                e.Cancel = true;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string text = (string)listBox4.SelectedItem;
            text = text.Split(':')[1];
            SendCommand(text);
        }

        private void contextCmdSaved_Opening(object sender, CancelEventArgs e)
        {
            if (listBox4.SelectedIndex < 0)
            {
                e.Cancel = true;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = (string)listBox3.SelectedItem;
            text = text.Split(':')[1];
            Clipboard.SetText(text);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = (string)listBox4.SelectedItem;
            text = text.Split(':')[1];
            Clipboard.SetText(text);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox4.Items.RemoveAt(listBox4.SelectedIndex);
            List<string> lines = new List<string>();
            foreach (var item in listBox4.Items)
            {
                lines.Add((string)item);
            }
            File.WriteAllLines("saved_cmds.dat", lines);
        }
    }
}
