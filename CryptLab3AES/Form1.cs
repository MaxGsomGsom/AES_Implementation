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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
            if (f.FileName == "") return;

            byte[] key = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(textBox1key.Text));

            AES algo = new AES(key, 6, 6);

            byte[] data = File.ReadAllBytes(f.FileName);

            byte[] result = algo.Encode(data);

            File.WriteAllBytes(Path.GetFileNameWithoutExtension(f.FileName) + "_encoded" + Path.GetExtension(f.FileName), result);
        }

        private void button1decode_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();
            if (f.FileName == "") return;

            byte[] key = Encoding.Convert(Encoding.Unicode, Encoding.Default, Encoding.Unicode.GetBytes(textBox1key.Text));

            AES algo = new AES(key, 6, 6);

            byte[] data = File.ReadAllBytes(f.FileName);

            byte[] result = algo.Decode(data);

            File.WriteAllBytes(Path.GetFileNameWithoutExtension(f.FileName) + "_decoded" + Path.GetExtension(f.FileName), result);

        }
    }
}
