using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using u8 = System.UInt32;
using u16 = System.UInt32;
using u32 = System.UInt32;

namespace GCard_UI
{
    public partial class Generator : Form
    {
        public Generator()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Items.AddRange(Enum.GetNames(typeof(CompressionMethod)));
            comboBox2.Items.AddRange(Enum.GetNames(typeof(StringEncoding)));
            comboBox1.SelectedIndex = (int)CompressionMethod.Smart;
            comboBox2.SelectedIndex = 0;
        }

        public Bitmap img;
        public byte[] saved_bytes;
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap _img = OpenBitmap();
            if (_img != null)
            {
                button1.Text = "Select image [Selected]";
                img = _img;
                pictureBox2.Image = img;
            }
        }

        public static Bitmap OpenBitmap()
        {
            var opf = new OpenFileDialog();
            opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";

            if (opf.ShowDialog() == DialogResult.OK)
            {
                Bitmap img = new Bitmap(opf.FileName);
                return img;

            }
            return null;
        }

        public static byte[] GenerateCompressedV2(Bitmap img)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)CompressionMethod.CompressedV2);
            Color pixel = img.GetPixel(0, 0);
            Color8b c = new Color8b(pixel);
            int lastVal = c.value;
            for (int i = 0; i < img.Width * img.Height; i++)
            {
                int x = i / img.Height;
                int y = i % img.Height;
                pixel = img.GetPixel(x, y);
                c = new Color8b(pixel);
                if (lastVal != c.value)
                {
                    bytes.Add(Convert.ToByte(lastVal));
                    bytes.Add(Convert.ToByte((i - 1) >> 16));
                    bytes.Add(Convert.ToByte((i - 1) >> 8 & 0xFF));
                    bytes.Add(Convert.ToByte((i - 1) & 0xFF));
                    lastVal = c.value;
                }
            }
            int _i = img.Width - 1;
            int _j = img.Height - 1;
            bytes.Add(Convert.ToByte(lastVal));
            int val = _j * img.Width + _i;
            bytes.Add(Convert.ToByte(val >> 16));
            bytes.Add(Convert.ToByte(val >> 8 & 0xFF));
            bytes.Add(Convert.ToByte(val & 0xFF));
            return bytes.ToArray();
        }

        public static byte[] GenerateCompressed_2BpP(Bitmap img)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)CompressionMethod.Compressed2BpP);
            Color pixel = img.GetPixel(0, 0);
            Color8b c = new Color8b(pixel);
            int lastVal = c.value;
            bool size_exceeded = false;
            for (int i = 0; i < img.Width * img.Height; i++)
            {
                int x = i / img.Height;
                int y = i % img.Height;
                pixel = img.GetPixel(x, y);
                c = new Color8b(pixel);
                if (lastVal != c.value)
                {
                    bytes.Add(Convert.ToByte(lastVal));
                    if (i > 65535)
                    {
                        size_exceeded = true;
                    }
                    bytes.Add(Convert.ToByte((i - 1) >> 8 & 0xFF));
                    bytes.Add(Convert.ToByte((i - 1) & 0xFF));
                    lastVal = c.value;
                }
            }
            int _i = img.Width - 1;
            int _j = img.Height - 1;
            bytes.Add(Convert.ToByte(lastVal));
            int val = _j * img.Width + _i;
            if (val > 65535)
            {
                size_exceeded = true;
            }
            bytes.Add(Convert.ToByte(val >> 8 & 0xFF));
            bytes.Add(Convert.ToByte(val & 0xFF));
            if (size_exceeded)
                MessageBox.Show("Image may be displayed incorrectly. Check preview", "Warning: 2B size exceeded");
            return bytes.ToArray();
        }

        public static byte[] GenerateCompressedSmart(Bitmap img)
        {
            if (img == null) return null;
            Bitmap bmpr = (Bitmap)img.Clone();
            bmpr.RotateFlip(RotateFlipType.Rotate90FlipNone);
            List<byte[]> bytesc = new List<byte[]>();
            int normal_size = img.Height * img.Width;

            {
                bytesc.Add(GenerateCompressedV2(img));
                byte[] bs = GenerateCompressedV2(bmpr);
                bs[0] |= 128;
                bytesc.Add(bs);
            }

            if (CountColors(img) <= 64)
            {
                bytesc.Add(GenerateCompressed64Color(img));
                byte[] bs = GenerateCompressed64Color(bmpr);
                bs[0] |= 128;
                bytesc.Add(bs);
            }

            if (normal_size < 65536)
            {
                bytesc.Add(GenerateCompressed_2BpP(img));
                byte[] bs = GenerateCompressed_2BpP(bmpr);
                bs[0] |= 128;
                bytesc.Add(bs);
            }

            byte[] min_tab = null;
            foreach (var item in bytesc)
            {
                if (min_tab == null || min_tab.Length > item.Length)
                {
                    min_tab = item;
                }
            }
            if (min_tab.Length > normal_size)
            {
                return GenerateNormal(img);
            }
            return min_tab;
        }

        public static int CountColors(Bitmap img)
        {
            Color pixel = img.GetPixel(0, 0);
            Color8b c = new Color8b(pixel);
            int lastVal = c.value;
            List<byte> colors = new List<byte>();
            byte color;

            for (int i = 0; i < img.Width * img.Height; i++)
            {
                int x = i / img.Height;
                int y = i % img.Height;
                pixel = img.GetPixel(x, y);
                c = new Color8b(pixel);
                if (lastVal != c.value)
                {
                    color = Convert.ToByte(lastVal);
                    if (!colors.Contains(color))
                    {
                        colors.Add(color);
                    }
                    lastVal = c.value;
                }
            }
            color = Convert.ToByte(lastVal);
            if (!colors.Contains(color))
            {
                colors.Add(color);
            }
            return colors.Count;
        }

        public static byte[] GenerateCompressed64Color(Bitmap img)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)CompressionMethod.Compressed64Color);
            Color pixel = img.GetPixel(0, 0);
            Color8b c = new Color8b(pixel);
            int lastVal = c.value;
            List<byte> colors = new List<byte>();
            bool color_exceeded = false;

            for (int i = 0; i < img.Width * img.Height; i++)
            {
                int x = i / img.Height;
                int y = i % img.Height;
                pixel = img.GetPixel(x, y);
                c = new Color8b(pixel);
                if (lastVal != c.value)
                {
                    int colorIndex = 0;
                    if (colors.Count < 64)
                    {
                        byte color = Convert.ToByte(lastVal);
                        if (!colors.Contains(color))
                        {
                            colors.Add(color);
                        }
                        colorIndex = colors.IndexOf(color);

                    }
                    else color_exceeded = true;
                    int value = (i - 1) & 0x03FFFF | colorIndex << 18;
                    bytes.Add(Convert.ToByte(value >> 16));
                    bytes.Add(Convert.ToByte(value >> 8 & 0xFF));
                    bytes.Add(Convert.ToByte(value & 0xFF));
                    lastVal = c.value;
                }
            }
            int _i = img.Width - 1;
            int _j = img.Height - 1;
            int _colorIndex = 0;
            if (colors.Count < 64)
            {
                byte color = Convert.ToByte(lastVal);
                if (!colors.Contains(color))
                {
                    colors.Add(color);
                }
                _colorIndex = colors.IndexOf(color);
            }
            else color_exceeded = true;
            int val = _j * img.Width + _i & 0x03FFFF | _colorIndex << 18;
            bytes.Add(Convert.ToByte(val >> 16));
            bytes.Add(Convert.ToByte(val >> 8 & 0xFF));
            bytes.Add(Convert.ToByte(val & 0xFF));
            int length = bytes.Count + 3;
            bytes.Insert(1, Convert.ToByte(length >> 16));
            bytes.Insert(2, Convert.ToByte(length >> 8 & 0xFF));
            bytes.Insert(3, Convert.ToByte(length & 0xFF));
            bytes.AddRange(colors);
            if (color_exceeded)
            {
                MessageBox.Show("Image may be displayed incorrectly. Check preview", "Warning: 64 colors exceeded");
            }
            return bytes.ToArray();
        }

        public static byte[] GenerateNormal(Bitmap img)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)CompressionMethod.Normal);
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    Color pixel = img.GetPixel(x, y);
                    Color8b c = new Color8b(pixel);
                    bytes.Add(Convert.ToByte(c.value));

                }
            }
            return bytes.ToArray();
        }

        public enum StringEncoding
        {
            CPP_Array,
            HEX,
            Hex0x,
            DEC,
        }
        public enum CompressionMethod
        {
            Normal,
            CompressedV2,
            Compressed2BpP,
            Compressed64Color,
            Smart,
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (img == null) return;
            byte[] bytes = null;
            CompressionMethod method = (CompressionMethod)comboBox1.SelectedIndex;
            StringEncoding encoding = (StringEncoding)comboBox2.SelectedIndex;
            if (method == CompressionMethod.Normal)
            {
                if (checkBox1.Checked)
                {
                    Bitmap bmpr = (Bitmap)img.Clone();
                    bmpr.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    bytes = GenerateNormal(bmpr);
                    bytes[0] |= 128;
                }
                else
                {
                    bytes = GenerateNormal(img);
                }
            }
            else if (method == CompressionMethod.CompressedV2)
            {
                if (checkBox1.Checked)
                {
                    Bitmap bmpr = (Bitmap)img.Clone();
                    bmpr.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    bytes = GenerateCompressedV2(bmpr);
                    bytes[0] |= 128;
                }
                else
                {
                    bytes = GenerateCompressedV2(img);
                }
            }
            else if (method == CompressionMethod.Compressed2BpP)
            {
                if (checkBox1.Checked)
                {
                    Bitmap bmpr = (Bitmap)img.Clone();
                    bmpr.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    bytes = GenerateCompressed_2BpP(bmpr);
                    bytes[0] |= 128;
                }
                else
                {
                    bytes = GenerateCompressed_2BpP(img);
                }
            }
            else if (method == CompressionMethod.Smart)
            {
                bytes = GenerateCompressedSmart(img);
            }
            else if (method == CompressionMethod.Compressed64Color)
            {
                if (checkBox1.Checked)
                {
                    Bitmap bmpr = (Bitmap)img.Clone();
                    bmpr.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    bytes = GenerateCompressed64Color(bmpr);
                    bytes[0] |= 128;
                }
                else
                {
                    bytes = GenerateCompressed64Color(img);
                }
            }

            if (bytes.Length > 65536)
            {
                if (MessageBox.Show($"Image size is huge. ({bytes.Length})", "Continue?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }

            method = (CompressionMethod)(bytes[0] & 0x7F);
            label1.Text = $"Length: {bytes.Length}, Used Compression: {method}";
            if (bytes[0] >> 7 == 1) label1.Text += " (Rotated)";

            saved_bytes = bytes;
            textBox1.Text = ConvertToString(bytes, encoding);
        }

        public static string ConvertToString(byte[] bytes, StringEncoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            int step = bytes.Length / 100;
            if (encoding == StringEncoding.CPP_Array)
            {
                sb.Append("uint8_t array[] = {");
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    sb.Append("0x" + b.ToString("X") + ", ");
                }
                sb.Append("};");
            }
            else if (encoding == StringEncoding.Hex0x)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    sb.Append("0x" + b.ToString("X") + " ");
                }
            }
            else if (encoding == StringEncoding.HEX)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    sb.Append(b.ToString("X") + " ");
                }
            }
            else if (encoding == StringEncoding.DEC)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    sb.Append(b.ToString() + " ");
                }
            }
            return sb.ToString();
        }

        public static Image GeneratePreview(byte[] bytes, int _w, int _h)
        {
            if (bytes == null) return null;
            u8 img_asset = 1;
            u16 w = (uint)_w;
            u16 h = (uint)_h;
            Bitmap bmp = new Bitmap(_w, _h);
            u8 x = 0;
            u8 y = 0;
            u8 Canvas = 0;

            u8 ReadAsset(u8 asset,u32 index)
            {
                if (index >= bytes.Length) return 0;
                return bytes[index];
            }
            u16 Read2Asset(u8 asset,u32 index)
            {
                if (index + 1 >= bytes.Length) return 0;
                return (uint)(bytes[index] << 8 | bytes[index + 1]);
            }
            u32 Read3Asset(u8 asset,u32 index)
            {
                if (index + 2 >= bytes.Length) return 0;
                return (uint)(bytes[index] << 16 | bytes[index + 1] << 8 | bytes[index + 2]);
            }

            void DrawPoint(u8 c, u16 __x, u16 __y, u8 color){
                bmp.SetPixel((int)__x, (int)__y, (new Color8b((byte)color)).color);
            }

            u8 compressed = ReadAsset(img_asset, 0);

            bool rotated = false;
            if (compressed >> 7 == 1)
            {
                compressed = compressed & 0x7F;
                rotated = true;
            }

            if (compressed == 0)
            {
                for (u32 i = 0; i < w * h; i++)
                {
                    u8 b = ReadAsset(img_asset, i + 1);
                    u16 _x = i / h;
                    u16 _y = i % h;
                    if (rotated)
                    {
                        _y = h - (i / w) - 1;
                        _x = i % w;
                    }
                    //&Canvas here
                    DrawPoint(Canvas, _x + x, _y + y, b);
                }
            }
            else if (compressed == 1)
            {
                u8 tmp_color;
                u32 tmp_next;
                u32 asset_index = 1;

                tmp_color = ReadAsset(img_asset, asset_index);
                asset_index += 1;
                tmp_next = Read3Asset(img_asset, asset_index);
                asset_index += 3;

                for (u32 i = 0; i < w * h; i++)
                {
                    if (i > tmp_next)
                    {
                        tmp_color = ReadAsset(img_asset, asset_index);
                        asset_index += 1;
                        tmp_next = Read3Asset(img_asset, asset_index);
                        asset_index += 3;
                    }

                    u16 _x = i / h;
                    u16 _y = i % h;
                    if (rotated)
                    {
                        _y = h - (i / w) - 1;
                        _x = i % w;
                    }
                    //&Canvas here
                    DrawPoint(Canvas, _x + x, _y + y, tmp_color);
                }
            }
            else if (compressed == 2)
            {
                u8 tmp_color;
                u16 tmp_next;
                u32 asset_index = 1;

                tmp_color = ReadAsset(img_asset, asset_index);
                asset_index += 1;
                tmp_next = Read2Asset(img_asset, asset_index);
                asset_index += 2;

                for (u32 i = 0; i < w * h; i++)
                {
                    if (i > tmp_next)
                    {
                        tmp_color = ReadAsset(img_asset, asset_index);
                        asset_index += 1;
                        tmp_next = Read2Asset(img_asset, asset_index);
                        asset_index += 2;
                    }
                    u16 _x = i / h;
                    u16 _y = i % h;
                    if (rotated)
                    {
                        _y = h - (i / w) - 1;
                        _x = i % w;
                    }
                    //&Canvas here
                    DrawPoint(Canvas, _x + x, _y + y, tmp_color);
                }
            }
            else if (compressed == 3)
            {
                u32 tmp_color;
                u32 tmp_next;
                u32 asset_index = 1;

                u32 colors_loc = Read3Asset(img_asset, asset_index);
                asset_index += 3;

                tmp_next = Read3Asset(img_asset, asset_index);
                tmp_color = tmp_next >> 18;
                tmp_next = tmp_next & 0x03FFFF;
                asset_index += 3;
                u32 length = w * h;

                u8 read_color = 0;
                read_color = ReadAsset(img_asset, colors_loc + tmp_color);
                for (u32 i = 0; i < length; i++)
                {
                    if (i > tmp_next)
                    {
                        tmp_next = Read3Asset(img_asset, asset_index);
                        tmp_color = tmp_next >> 18;
                        tmp_next = tmp_next & 0x03FFFF;
                        read_color = ReadAsset(img_asset, colors_loc + tmp_color);
                        asset_index += 3;
                    }

                    u16 _x = i / h;
                    u16 _y = i % h;
                    if (rotated)
                    {
                        _y = h - (i / w) - 1;
                        _x = i % w;
                    }
                    //&Canvas here
                    DrawPoint(Canvas, _x + x, _y + y, read_color);
                }
            }
            return bmp;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0) return;
            Clipboard.SetText(textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (saved_bytes == null || img == null) return;
            tabControl1.SelectedIndex = 1;
            pictureBox1.Image = GeneratePreview(saved_bytes, img.Width, img.Height);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (saved_bytes == null || img == null) return;
            pictureBox1.Image = GeneratePreview(saved_bytes, img.Width, img.Height);
        }

        public bool isUpload = false;
        private void button7_Click(object sender, EventArgs e)
        {
            if (saved_bytes == null) return;
            isUpload = true;
            this.Close();
        }
    }
}
