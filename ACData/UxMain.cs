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
using System.Threading;

namespace ACData
{
    public partial class UxMain : Form
    {

        public UxMain()
        {
            InitializeComponent();
            this.ListBoxSourceFiles.DragDrop += new
           System.Windows.Forms.DragEventHandler(this.ListBoxSourceFiles_DragDrop);
            this.ListBoxSourceFiles.DragEnter += new
                       System.Windows.Forms.DragEventHandler(this.ListBoxSourceFiles_DragEnter);
        }

        private void UxMain_FormClosing()
        {
            Application.Exit();
        }

        private void ButtonSourcePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog sourceFolderBrowserDialog = new FolderBrowserDialog();
            sourceFolderBrowserDialog.ShowNewFolderButton = false;
            DialogResult result = sourceFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ListBoxSourceFiles.Items.Clear();

                string folderName = sourceFolderBrowserDialog.SelectedPath;
                foreach (string file in Directory.GetFiles(folderName).Select(Path.GetFileName))
                    ListBoxSourceFiles.Items.Add(file);

            }
        }

        private void ButtonDestination_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog sourceFolderBrowserDialog = new FolderBrowserDialog();
            sourceFolderBrowserDialog.ShowNewFolderButton = true;

            DialogResult result = sourceFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {

                TextBoxDestinationPath.Text = sourceFolderBrowserDialog.SelectedPath;

            }
        }

        private void ListBoxSourceFiles_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
        private void ListBoxSourceFiles_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            int i = 0;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                if (Directory.Exists(files[i]))
                {
                    foreach (string subfiles in Directory.GetFiles(files[i], "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".json") || s.EndsWith(".sql")))
                    ListBoxSourceFiles.Items.Add(System.IO.Path.GetFullPath(subfiles).ToString());
                }

                if (Path.GetExtension(file) == ".json" || Path.GetExtension(file) == ".sql")
                    ListBoxSourceFiles.Items.Add(System.IO.Path.GetFullPath(file).ToString());
            }

        }

        private void ListBoxSourceFiles_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog sourceFolderBrowserDialog = new FolderBrowserDialog();


            DialogResult result = sourceFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ListBoxSourceFiles.Items.Clear();

                string folderName = sourceFolderBrowserDialog.SelectedPath;
                foreach (string file in Directory.GetFiles(folderName,"*.*",SearchOption.AllDirectories).Where(s => s.EndsWith(".json") || s.EndsWith(".sql")))
                    ListBoxSourceFiles.Items.Add(file);

            }
        }

        private void ButtonProcess_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(TextBoxDestinationPath.Text))
            {
                int i = 0;
                foreach (string filename in ListBoxSourceFiles.Items)
                {
                    i++;
                    Program.Process(filename, new List<string>() { "*.json", "*.sql" }, (fi) => Program.Convert(fi));
                    // Dest path is from TextBoxDestinationPath.Text

                }
                MessageBox.Show($"Convert Completed! {i} files converted.");
            }
            else
                MessageBox.Show("Invalid Destination Directory");
        }

        private void ButtonClearFiles_Click(object sender, EventArgs e)
        {
            ListBoxSourceFiles.Items.Clear();
        }
    }
}
