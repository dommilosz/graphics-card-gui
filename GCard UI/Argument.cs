using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GCard_UI.Actions;

namespace GCard_UI
{
    public partial class Argument : UserControl
    {
        SubCommand _subCommand;
        public Argument next_arg;
        [Category("Data")]
        public SubCommand SubCommand
        {
            get => _subCommand;
            set => _subCommand = value;
        }

        public string value = "";
        public string dvalue = "";

        public Argument()
        {
            InitializeComponent();

            RedrawArg();
        }

        public void RedrawArg()
        {
            panel1.Controls.Clear();
            if (_subCommand == null) return;

            if (SubCommand.type == ActionType.SubCommand)
            {
                label1.Text = _subCommand.name;
                ComboBox cb = new ComboBox();
                cb.Items.AddRange(_subCommand.ToStringSubcommands());
                panel1.Controls.Add(cb);
                cb.Show();
                cb.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                cb.SelectedIndex = 0;
            }
            else if (SubCommand.type == ActionType.Input)
            {
                label1.Text = _subCommand.name;
                if (SubCommand.size == ActionSize.U8)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = 0;
                    nud.Maximum = 255;
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();
                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, nud.Value);
                    };
                    value = ValueToString(SubCommand.size, nud.Value);
                    if (SubCommand.def_val != null)
                    {
                        nud.Value = (int)SubCommand.def_val;
                    }
                }
                else if (SubCommand.size == ActionSize.U16)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = 0;
                    nud.Maximum = (decimal)Math.Pow(2, 16);
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();
                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, nud.Value);
                    };
                    value = ValueToString(SubCommand.size, nud.Value);
                    if (SubCommand.def_val != null)
                    {
                        nud.Value = (int)SubCommand.def_val;
                    }
                }
                else if (SubCommand.size == ActionSize.U32)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = 0;
                    nud.Maximum = (decimal)Math.Pow(2, 32);
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();
                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, nud.Value);
                    };
                    value = ValueToString(SubCommand.size, nud.Value);
                    if (SubCommand.def_val != null)
                    {
                        nud.Value = (int)SubCommand.def_val;
                    }
                }
                else if (SubCommand.size == ActionSize.Color)
                {
                    var but = new Button();
                    but.BackColor = Color.Black;
                    but.Click += (object sender, EventArgs e) =>
                    {
                        var cd = new ColorDialog();
                        cd.Color = but.BackColor;
                        if (cd.ShowDialog() == DialogResult.OK)
                        {
                            var c8 = new Color8b(cd.Color);
                            value = c8.value.ToString();
                            but.BackColor = c8.color;
                        }
                    };
                    panel1.Controls.Add(but);
                    but.Show();
                    value = "0";
                }
                else if (SubCommand.size == ActionSize.Location)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = short.MinValue;
                    nud.Maximum = short.MaxValue;
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();

                    var nud2 = new NumericUpDown();
                    nud2.Minimum = short.MinValue;
                    nud2.Maximum = short.MaxValue;
                    nud2.DecimalPlaces = 0;
                    nud2.Location = new Point(0, 30);
                    panel1.Controls.Add(nud2);
                    nud2.Show();

                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Point(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                    };
                    nud2.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Point(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                    };
                    value = ValueToString(SubCommand.size, new Point(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                }
                else if (SubCommand.size == ActionSize.Size)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = short.MinValue;
                    nud.Maximum = short.MaxValue;
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();

                    var nud2 = new NumericUpDown();
                    nud2.Minimum = short.MinValue;
                    nud2.Maximum = short.MaxValue;
                    nud2.DecimalPlaces = 0;
                    nud2.Location = new Point(0, 30);
                    panel1.Controls.Add(nud2);
                    nud2.Show();

                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Size(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                    };
                    nud2.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Size(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                    };
                    value = ValueToString(SubCommand.size, new Size(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value)));
                }
                else if (SubCommand.size == ActionSize.Rect)
                {
                    var nud = new NumericUpDown();
                    nud.Minimum = short.MinValue;
                    nud.Maximum = short.MaxValue;
                    nud.DecimalPlaces = 0;
                    panel1.Controls.Add(nud);
                    nud.Show();

                    var nud2 = new NumericUpDown();
                    nud2.Minimum = short.MinValue;
                    nud2.Maximum = short.MaxValue;
                    nud2.DecimalPlaces = 0;
                    nud2.Location = new Point(0, 30);
                    panel1.Controls.Add(nud2);
                    nud2.Show();

                    var nud3 = new NumericUpDown();
                    nud3.Minimum = short.MinValue;
                    nud3.Maximum = short.MaxValue;
                    nud3.DecimalPlaces = 0;
                    nud3.Location = new Point(0, 60);
                    panel1.Controls.Add(nud3);
                    nud3.Show();

                    var nud4 = new NumericUpDown();
                    nud4.Minimum = short.MinValue;
                    nud4.Maximum = short.MaxValue;
                    nud4.DecimalPlaces = 0;
                    nud4.Location = new Point(0, 90);
                    panel1.Controls.Add(nud4);
                    nud4.Show();

                    nud.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Rectangle(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value), Convert.ToInt32(nud3.Value), Convert.ToInt32(nud4.Value)));
                    };
                    nud2.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Rectangle(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value), Convert.ToInt32(nud3.Value), Convert.ToInt32(nud4.Value)));
                    };
                    nud3.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Rectangle(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value), Convert.ToInt32(nud3.Value), Convert.ToInt32(nud4.Value)));
                    };
                    nud4.ValueChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, new Rectangle(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value), Convert.ToInt32(nud3.Value), Convert.ToInt32(nud4.Value)));
                    };
                    value = ValueToString(SubCommand.size, new Rectangle(Convert.ToInt32(nud.Value), Convert.ToInt32(nud2.Value), Convert.ToInt32(nud3.Value), Convert.ToInt32(nud4.Value)));
                }
                else if (SubCommand.size == ActionSize.String)
                {
                    var txt = new TextBox();
                    panel1.Controls.Add(txt);
                    txt.Show();
                    txt.TextChanged += (s, ev) =>
                    {
                        value = ValueToString(SubCommand.size, txt.Text);
                    };
                    value = ValueToString(SubCommand.size, txt.Text);
                }else if(SubCommand.size == ActionSize.Bool)
                {
                    ComboBox cb = new ComboBox();
                    cb.Items.AddRange(new string[] { "false","true"});
                    panel1.Controls.Add(cb);
                    cb.Show();
                    cb.SelectedIndexChanged += (s,e)=>
                    {
                        value = ValueToString(SubCommand.size, Convert.ToBoolean(cb.SelectedIndex));
                    };
                    cb.SelectedIndex = 0;
                    value = ValueToString(SubCommand.size, Convert.ToBoolean(cb.SelectedIndex));
                }
                else if (SubCommand.size == ActionSize.Code)
                {
                    Button b = new Button();
                    b.Text = "Set Code";
                    panel1.Controls.Add(b);
                    b.Show();
                    b.Click += (s, e) =>
                    {
                        var ci = new Code_input(dvalue);
                        ci.ShowDialog();
                        dvalue = ci.code;
                        value = ValueToString(SubCommand.size, ci);
                    };
                    value = "";
                }
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteNextRecursive();
            if (this.SubCommand.type == ActionType.SubCommand)
            {
                var sub = SubCommand.subCommands[((ComboBox)sender).SelectedIndex];
                if (sub != null)
                {
                    value = ValueToString(sub.size, sub.value);
                    if (sub.subCommands != null && sub.subCommands.Length >= 1)
                    {
                        DeleteNextRecursive();
                        if (sub.final)
                        {
                            AddFinal(sub.subCommands);
                        }
                        else if (sub.type != ActionType.Hidden)
                        {
                            next_arg = new Argument();
                            this.Parent.Controls.Add(next_arg);
                            next_arg.SubCommand = sub;
                            next_arg.Show();
                            next_arg.Location = new Point(this.Location.X + 200, this.Location.Y);
                        }
                    }
                }

                DrawRecursive();
            }
        }

        public void DeleteNextRecursive()
        {
            if (next_arg != null)
            {
                next_arg.DeleteNextRecursive();
                next_arg.Dispose();
                next_arg = null;
            }
        }

        public void DrawRecursive()
        {
            if (next_arg != null)
            {
                next_arg.DrawRecursive();
                next_arg.RedrawArg();
            }
        }

        public void AddFinal(SubCommand[] subs, int index = 0)
        {
            if (index >= subs.Length) return;
            if (subs[index].type == ActionType.Hidden) return;
            next_arg = new Argument();
            this.Parent.Controls.Add(next_arg);
            next_arg.SubCommand = subs[index];
            next_arg.Show();
            next_arg.Location = new Point(this.Location.X + 150, this.Location.Y);
            next_arg.AddFinal(subs, index + 1);
        }

        public string GetCommandRec(string already_done = "")
        {
            if (next_arg != null)
            {
                already_done = next_arg.GetCommandRec(already_done);
            }
            return SubCommand.insert_after + " " + value + " " + already_done;
        }
    }
}
