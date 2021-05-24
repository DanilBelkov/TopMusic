using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetTopMusicForm
{
   
    public partial class Form1 : Form
    {

        private string _trackFilePuth = "";
        public Form1()
        {
            InitializeComponent();
            DirectoryInfo di = new DirectoryInfo(@"Tracks\TracksFile\");
            di.Create();
        }

        private void B_Start_Click(object sender, EventArgs e)
        {
            if (_trackFilePuth != "")
            {
                FindTopMusic.morgenBlock = checkMorgenBlock.Checked;
                FindTopMusic.WebPuth = Combo_B_Sites.SelectedItem.ToString();
                FindTopMusic.ParsingToList();

                richTextBox1.Text = "";
                foreach (Track t in FindTopMusic.listTracks)
                {
                    richTextBox1.Text += t.ToString() + "\n";
                }

                FindTopMusic.SaveToFile();

                FindTopMusic.DownloadTracks(_trackFilePuth).GetAwaiter();
            }
        }

        private void B_ChooseDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                _trackFilePuth = FBD.SelectedPath + "\\";
            }
        }

        private void ToolStripComboBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
