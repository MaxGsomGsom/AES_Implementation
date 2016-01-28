using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CryptLab3AES
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int keySize = 4, blockSize = 4;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1key.Text.Length<keySize*4)
            {
                MessageBox.Show("Key too short");
                return;
            }


            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
            if (f.FileName == "") return;

            byte[] key = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(textBox1key.Text));

            AES algo = new AES(key, keySize, blockSize);

            byte[] data = File.ReadAllBytes(f.FileName);

            byte[] result = algo.Encode(data);

            File.WriteAllBytes(Path.GetFileNameWithoutExtension(f.FileName) + "_encoded" + Path.GetExtension(f.FileName), result);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) keySize = 4;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) keySize = 6;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked) keySize = 8;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked) blockSize = 4;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked) blockSize = 6;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked) blockSize = 6;
        }

        private void button1decode_Click(object sender, EventArgs e)
        {
            if (textBox1key.Text.Length < keySize * 4)
            {
                MessageBox.Show("Key too short");
                return;
            }

            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
            if (f.FileName == "") return;

            byte[] key = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(textBox1key.Text));

            AES algo = new AES(key, keySize, blockSize);

            byte[] data = File.ReadAllBytes(f.FileName);

            byte[] result = algo.Decode(data);

            File.WriteAllBytes(Path.GetFileNameWithoutExtension(f.FileName) + "_decoded" + Path.GetExtension(f.FileName), result);

        }
    }
}
