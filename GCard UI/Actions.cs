using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GCard_UI
{
    class Actions
    {
        public static SubCommand commands = new SubCommand()
        {
            value = 0,
            name = "Primary command",
            subCommands = new SubCommand[]
            {
                new SubCommand(){ name="Init",value=1,final=true,subCommands=new SubCommand[]
                {
                    new SubCommand(){ name="resolution",subCommands=SubCommand.GenerateFromStringInc1(new string[]{"RES_ZX","RES_CGA","RES_QVGA","RES_EGA","RES_VGA","RES_SVGA","RES_XGA","RES_HD","RES_MAX"})},
                    new SubCommand(){ name="tiles",subCommands=SubCommand.GenerateFromStringInc1(new string[]{"FORM_8BIT","FORM_4BIT","FORM_MONO","FORM_TILE8","FORM_TILE12","FORM_TILE16","FORM_TILE24","FORM_TILE32","FORM_TILE48","FORM_TILE64","FORM_MTEXT8","FORM_MTEXT16","FORM_TEXT8","FORM_TEXT16","FORM_RLE","FORM_MAX"})},
                    new SubCommand(){ name="board_size",type = ActionType.SubCommand,size=ActionSize.U32,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="RES_ZX",value=256*192},
                        new SubCommand(){ name="RES_CGA",value=256*192},
                        new SubCommand(){ name="RES_QVGA",value=320 * 240},
                        new SubCommand(){ name="RES_EGA",value=512 * 400},
                        new SubCommand(){ name="RES_VGA",value=640 * 480},
                        new SubCommand(){ name="RES_SVGA",value=800 * 600},
                        new SubCommand(){ name="RES_XGA",value=1024 * 768},
                        new SubCommand(){ name="RES_HD",value=1280 * 960},
                    } },
                } },
                new SubCommand(){ name="Write",value=2,subCommands=new SubCommand[]{ } },
                new SubCommand(){ name="Clear",value=3,final=true,subCommands=new SubCommand[]
                {
                    new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                } },
                new SubCommand(){ name="AutoInit",value=4,final=true,subCommands=new SubCommand[]
                {
                     new SubCommand(){ name="resolution",subCommands=SubCommand.GenerateFromStringInc1(new string[]{"RES_ZX","RES_CGA","RES_QVGA","RES_EGA","RES_VGA","RES_SVGA","RES_XGA","RES_HD","RES_MAX"})},
                } },
                new SubCommand(){ name="Fill",value=5,final=true,subCommands=new SubCommand[]
                {
                    new SubCommand(){ name="location",subCommands=SubCommand.GenerateFromStringInc1(new string[]{ "Default"})},
                    new SubCommand(){ name="offset",size=ActionSize.U32,type=ActionType.Input},
                    new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                    new SubCommand(){ name="code_asset",size=ActionSize.U8,type=ActionType.Input},
                } },
                new SubCommand(){ name="DrawFeature",value=6,subCommands=new SubCommand[]
                {
                    new SubCommand(){name="rectangle",value=0, final=true,subCommands=new SubCommand[]{
                        new SubCommand(){ name="rect",type=ActionType.Input,size=ActionSize.Rect},
                        new SubCommand(){ name="color",type=ActionType.Input,size=ActionSize.Color},
                    } },
                    new SubCommand(){name="point", value=1,final=true,subCommands=new SubCommand[]{
                        new SubCommand(){ name="location",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){ name="color",type=ActionType.Input,size=ActionSize.Color},
                    } },
                    new SubCommand(){name="points", value=0|64,final=true,subCommands=new SubCommand[]{

                    } },
                    new SubCommand(){name="line",value=2, final=true,subCommands=new SubCommand[]{
                        new SubCommand(){ name="location1",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){ name="location2",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){ name="color",type=ActionType.Input,size=ActionSize.Color},
                    } },
                    new SubCommand(){name="circle",value=3, final=true,subCommands=new SubCommand[]{
                        new SubCommand(){ name="location",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){ name="radius",type=ActionType.Input,size=ActionSize.U16},
                        new SubCommand(){ name="color",type=ActionType.Input,size=ActionSize.Color},
                        new SubCommand(){ name="mask",type=ActionType.Input,size=ActionSize.U8},
                    } },
                    new SubCommand(){name="circle",value=3|128, final=true,subCommands=new SubCommand[]{
                        new SubCommand(){ name="location",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){ name="radius",type=ActionType.Input,size=ActionSize.U16},
                        new SubCommand(){ name="color",type=ActionType.Input,size=ActionSize.Color},
                        new SubCommand(){ name="mask",type=ActionType.Input,size=ActionSize.U8},
                    } },

                } },
                new SubCommand(){ name="Text",value=7,subCommands=new SubCommand[]
                {
                    new SubCommand(){ name="normal",value=0,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="location",size=ActionSize.Location ,type=ActionType.Input},
                        new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="font height",size=ActionSize.U8,def_val=8,type=ActionType.Input},
                        new SubCommand(){ name="scale X",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="cale Y",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="text",size=ActionSize.String,type=ActionType.Input},
                    } },
                    new SubCommand(){ name="background",value=0|64,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="location",size=ActionSize.Location ,type=ActionType.Input},
                        new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="bgcolor",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="font height",size=ActionSize.U8,def_val=8,type=ActionType.Input},
                        new SubCommand(){ name="scale X",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="cale Y",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="text",size=ActionSize.String,type=ActionType.Input},
                    } },
                    new SubCommand(){ name="cursored",value=1,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="font height",size=ActionSize.U8,def_val=8,type=ActionType.Input},
                        new SubCommand(){ name="scale X",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="cale Y",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="text",size=ActionSize.String,type=ActionType.Input},
                    } },
                    new SubCommand(){ name="cursored bg",value=1|64,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="color",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="bgcolor",size=ActionSize.Color,type=ActionType.Input},
                        new SubCommand(){ name="font height",size=ActionSize.U8,def_val=8,type=ActionType.Input},
                        new SubCommand(){ name="scale X",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="cale Y",size=ActionSize.U8,def_val=1,type=ActionType.Input},
                        new SubCommand(){ name="text",size=ActionSize.String,type=ActionType.Input},
                    } },
                    new SubCommand(){ name="set cursor",value=128|0,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="location",size=ActionSize.Location,type=ActionType.Input},
                    } },
                } },
                new SubCommand(){ name="Objects",value=8,insert_after="{::selectedObjIndex}",subCommands=new SubCommand[]
                {
                    new SubCommand(){name="Create",value=0,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="rectangle",value=1},
                        new SubCommand(){ name="text",value=2},
                        new SubCommand(){ name="circle",value=3},
                        new SubCommand(){ name="line",value=4},
                        new SubCommand(){ name="blob",value=5},
                    } },
                    new SubCommand(){name="Delete",value=1,final=true,type=ActionType.Hidden,subCommands=new SubCommand[]{

                    } },
                    new SubCommand(){name="Move",value=2,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Relative",type=ActionType.Input,size=ActionSize.Bool},
                        new SubCommand(){name="Location",type=ActionType.Input,size=ActionSize.Location},
                    } },
                    new SubCommand(){name="Change Color",final=true,value=3,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Color",type=ActionType.Input,size=ActionSize.Color},
                    } },
                    new SubCommand(){name="Visibility",final=true,value=4,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Visible",type=ActionType.Input,size=ActionSize.Bool},
                    } },
                    new SubCommand(){name="Resize",value=5,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Relative",type=ActionType.Input,size=ActionSize.Bool},
                        new SubCommand(){name="Size",type=ActionType.Input,size=ActionSize.Size},
                    } },
                    new SubCommand(){name="Change Radius",final=true,value=6,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Relative",type=ActionType.Input,size=ActionSize.Bool},
                        new SubCommand(){name="Radius",type=ActionType.Input,size=ActionSize.U16},
                    } },
                    new SubCommand(){name="Change Mask & Fill",final=true,value=7,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Mask",type=ActionType.Input,size=ActionSize.U8},
                        new SubCommand(){name="Fill",type=ActionType.Input,size=ActionSize.U8},
                    } },
                    new SubCommand(){name="Move Line",final=true,value=8,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Relative",type=ActionType.Input,size=ActionSize.Bool},
                        new SubCommand(){name="Loc1",type=ActionType.Input,size=ActionSize.Location},
                        new SubCommand(){name="Loc2",type=ActionType.Input,size=ActionSize.Location},
                    } },
                    new SubCommand(){name="Set Text",final=true,value=9,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="text_asset",size=ActionSize.U8,type=ActionType.Input},
                    } },
                    new SubCommand(){name="Set Text Scale",final=true,value=10,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="sX",type=ActionType.Input,size=ActionSize.U16},
                        new SubCommand(){name="sY",type=ActionType.Input,size=ActionSize.U16},
                        new SubCommand(){name="fH",type=ActionType.Input,size=ActionSize.U16},

                    } },
                    new SubCommand(){name="Set Code",final=true,value=11,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="code_asset",size=ActionSize.U8,type=ActionType.Input},
                    } },
                    new SubCommand(){name="List All",value=255,final=true,subCommands=new SubCommand[]
                    {
                        new SubCommand(){name="Detailed Info",type=ActionType.Input,size=ActionSize.Bool},
                    } },
                    new SubCommand(){name="Set BlobData",final=true,value=12,subCommands=new SubCommand[]
                    {
                        new SubCommand(){ name="code_asset",size=ActionSize.U8,type=ActionType.Input},
                    } }

                } },
                new SubCommand(){ name="Sound",value=9,subCommands=new SubCommand[]{ } },
                new SubCommand(){ name="SoundStop",value=9|128,subCommands=new SubCommand[]{ } },
                new SubCommand(){ name="Set Asset",value=10,final=true,subCommands=new SubCommand[]
                {
                    new SubCommand(){ name="asset",size=ActionSize.U8,type=ActionType.Input},
                    new SubCommand(){name="data",size=ActionSize.Code,type=ActionType.Input},
                } },
                new SubCommand(){ name="Redraw",value=11,final=true,subCommands=new SubCommand[]
                {
                    
                } },
                new SubCommand(){ name="Test",value=254,subCommands=new SubCommand[]{ } },
            }
        };

        public static string ValueToString(ActionSize size, object _value)
        {
            try
            {
                if (typeof(decimal) == _value.GetType())
                {
                    return ValueToString(size, Convert.ToInt32(Math.Floor((decimal)_value)));
                }
                if (typeof(int) == _value.GetType())
                {
                    int value = (int)_value;
                    if (size == ActionSize.U8) return (value & 0xFF).ToString();
                    if (size == ActionSize.U16) return ((value & 0xFF00) >> 8).ToString() + " " + (value & 0xFF).ToString();
                    if (size == ActionSize.U32) return ((value & 0xFF000000) >> 24).ToString() + " " + ((value & 0xFF0000) >> 16).ToString() + " " + ((value & 0xFF00) >> 8).ToString() + " " + (value & 0xFF).ToString();
                }

                if (size == ActionSize.Color)
                {
                    Color8b value = (Color8b)_value;
                    return value.value.ToString();
                }

                if (size == ActionSize.Location)
                {
                    Point value = (Point)_value;
                    return $"{ValueToString(ActionSize.U16, value.X)} {ValueToString(ActionSize.U16, value.Y)}";
                }
                if (size == ActionSize.Size)
                {
                    Size value = (Size)_value;
                    return $"{ValueToString(ActionSize.U16, value.Width)} {ValueToString(ActionSize.U16, value.Height)}";
                }
                if (size == ActionSize.Rect)
                {
                    Rectangle value = (Rectangle)_value;
                    return $"{ValueToString(ActionSize.Location, value.Location)} {ValueToString(ActionSize.Size, value.Size)}";
                }
                if (size == ActionSize.String)
                {
                    string value = (string)_value;
                    value = value.Replace("\\n", "\n");
                    string numbers = "";
                    for (int i = 1; i < 256; i++)
                    {
                        numbers += Convert.ToChar(i);
                    }
                    value = value.Replace("\\{1-255}", numbers);

                    string output = ValueToString(ActionSize.U16, value.Length) + " ";
                    foreach (var item in value.ToCharArray())
                    {
                        output += Convert.ToInt32(item) + " ";
                    }
                    return output;
                }
                if (size == ActionSize.Bool)
                {
                    bool value = (bool)_value;
                    return value ? "1" : "0";
                }
                if (size == ActionSize.Code)
                {
                    var ci = (Code_input)_value;
                    string value = ci.parsedCode();
                    

                    return value;
                }
            }
            catch
            {
                MessageBox.Show("parsing error");
            }

            return "";
        }
    }
    public class SubCommand
    {
        public SubCommand[] subCommands;
        public string name;
        public int value = -1;
        public bool final = false;
        public object def_val = null;
        public ActionType type = ActionType.SubCommand;
        public ActionSize size = ActionSize.U8;
        public string insert_after;

        public string[] ToStringSubcommands()
        {
            if (subCommands == null) return new string[] { };
            List<string> data = new List<string>();
            foreach (var item in subCommands)
            {
                data.Add($"{item.name} = {item.value}");
            }
            return data.ToArray();
        }

        public static SubCommand[] GenerateFromStringInc1(string[] arr)
        {
            SubCommand[] subs = new SubCommand[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                subs[i] = new SubCommand() { name = arr[i], value = i };
            }
            return subs;
        }
    }

    public enum ActionType
    {
        SubCommand, Input, Hidden
    }
    public enum ActionSize
    {
        U8, U16, Location, Color, U32, Rect, Size, String, Bool, Code
    }

    public enum ActionStatus
    {
        STATUS_OK = 0,
        STATUS_NOT_FOUND = 1,
        STATUS_UNKNOWN_ERROR = 2,
        STATUS_UNTERMINATED = 3,
        STATUS_WRONG_TYPE = 4,
        STATUS_WRONG_VALUE = 5,
    }

    public class Color8b
    {
        public Color color
        {
            get
            {
                int r = _color.R / 32;
                int g = _color.G / 32;
                int b = _color.B / 64;
                return Color.FromArgb(r * 32, g * 32, b * 64);
            }
            set
            {
                _color = value;
            }
        }
        public int value
        {
            get
            {
                int r = _color.R / 32;
                int g = _color.G / 32;
                int b = _color.B / 64;
                return (r << 5) | (g << 2) | b;
            }
            set
            {
                int r = (value & 0b11100000) >> 5;
                int g = (value & 0b00011100) >> 2;
                int b = value & 0b00000011;
                _color = Color.FromArgb(r * 32, g * 32, b * 64);

            }
        }
        Color _color;
        public Color8b(Color c)
        {
            color = c;
        }
        public Color8b(byte c)
        {
            value = c;
        }
    }
}
