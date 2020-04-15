using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InsertDBTask
{
    public partial class Form1 : Form
    {
        private string folderPath = "";
        private InsertClass cl = new InsertClass();
        private string OutputFolder = "";
        private string InputFolder = "";
        public Form1()
        {
            InitializeComponent();
            folderPath = System.Configuration.ConfigurationManager.AppSettings["folderPath"];
        }
        public void ProcessDirectory(string sourceDirectory,string targetDirectory)
        {
            lblStatus.Visible = true;
            txtStatus.Visible = true;

            // Process the list of files found in the directory.
            var fileEntriesOutput = Directory.GetFiles(targetDirectory).Select(Path.GetFileName).ToList();
            var fileEntriesInput = Directory.GetFiles(sourceDirectory).Select(Path.GetFileName).ToList();

            if (fileEntriesInput != null && fileEntriesInput.Count > 0)
            {
                //var file = fileEntriesInput.Where(m => m.ToLower().Trim() == "hi2processor").FirstOrDefault();
                //fileEntriesInput.Remove(file);
                //fileEntriesInput.Remove(logFile);
                //fileEntriesInput.RemoveAt(0);
                foreach (var item in fileEntriesOutput)
                {
                    var tempItem = fileEntriesInput.Where(m => m.ToLower().Trim() == item.ToLower().Trim()).FirstOrDefault();
                    if (tempItem != null)
                        fileEntriesInput.Remove(tempItem);
                }
            }
            foreach (var item in fileEntriesInput)
            {
                if (item != "hi2processor")
                {
                    ProcessFile(item);
                    txtStatus.Text = item;
                }
                else
                {
                }
            }
            lblStatus.Text = "Hoàn thành!!!";
            txtStatus.Visible = false;
        }

        // Insert logic for processing found files here.
        public void ProcessFile(string path)
        {
            var sourceFileLocation = txtInput.Text + "\\" + path;
            var destinationFileLocation = txtOutput.Text + "\\" + path;
            var m = txtOutput.Text;
            try
            {
                //var fileName = Path.GetFileName(path);
                string[] lines = System.IO.File.ReadAllLines(sourceFileLocation);
                foreach (string line in lines)
                {
                    ProcessLine(line);
                }
            }
            catch
            {

            }
            finally
            {
                if (File.Exists(destinationFileLocation))
                    File.Delete(destinationFileLocation);
                File.Copy(sourceFileLocation, destinationFileLocation);
                //File.Move(sourceFileLocation, txtOutput.Text);
            }
        }

        private void ProcessLine(string line)
        {
            String[] listStr = line.Split(',');
            string cellID = "";
            string CD = "", CR = "", receiver = "", sender = "";
            string ngaygio = listStr[0];
            if (listStr != null && listStr.Length > 0)
            {
                var lastItem = listStr[listStr.Length - 1];
                if (lastItem.ToLower().IndexOf("cell:") >= 0)
                {
                    if (lastItem.Length < 11)
                    {
                        cellID = lastItem.Remove(0, 5).Trim();
                    }
                    else
                        cellID = lastItem.Remove(0, 11).Trim();
                    
                    foreach(var item in listStr)
                    {
                        if (item.Contains("CD:") || item.Contains("CR:"))
                        {
                            if(item.Contains("CD:"))
                            {
                                var itemIndex = item.IndexOf("CD:");
                                CD = item.Remove(0, itemIndex).Replace("CD:", "").Trim();
                            }
                            else
                            {
                                CR = item.Replace("CR:", "").Trim();
                            }
                        }
                        else if(item.Contains("receiver:") || item.Contains("sender:"))
                        {
                            if (item.Contains("receiver:"))
                            {
                                receiver = item.Replace("receiver:", "").Trim();
                            }
                            else
                            {
                                sender = item.Replace("sender:", "").Trim();
                            }
                        }
                    }
                    cl.InsertCELL(ngaygio, CD, CR, sender, receiver, cellID);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblStatus.Visible = false;
            txtStatus.Visible = false;
            lblStatus.Text = "Đang xử lý : ";
            if (Directory.Exists(txtInput.Text) && Directory.Exists(txtOutput.Text))
            {
                // This path is a directory
                ProcessDirectory(txtInput.Text,txtOutput.Text);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtInput.Text) && !String.IsNullOrEmpty(txtOutput.Text))
            {
                timer1.Interval = 3600000;
                timer1.Start();
                btnStart.Enabled = false; btnLocateInput.Enabled = false; btnLocateOutput.Enabled = false;
                txtInput.Enabled = false; txtOutput.Enabled = false; btnStop.Enabled = true;
            }
            else
            {
                MessageBox.Show(this, "Đường dẫn thư mục không được rỗng", "Error Message!");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true; btnLocateInput.Enabled = true; btnLocateOutput.Enabled = true;
            txtInput.Enabled = true; txtOutput.Enabled = true; btnStop.Enabled = false;
            timer1.Stop();
        }

        private void btnLocateInput_Click(object sender, EventArgs e)
        {
            var temp = "";
            FolderBrowserDialog dia = new FolderBrowserDialog();
            dia.RootFolder = Environment.SpecialFolder.Desktop;
            dia.Description = "+++Select Folder+++";
            dia.ShowNewFolderButton = false;
            if (dia.ShowDialog() == DialogResult.OK)
            {
                temp = dia.SelectedPath;
                txtInput.Text = temp;
            }
        }

        private void btnLocateOutput_Click(object sender, EventArgs e)
        {
            var temp = "";
            FolderBrowserDialog dia = new FolderBrowserDialog();
            dia.RootFolder = Environment.SpecialFolder.Desktop;
            dia.Description = "+++Select Folder+++";
            dia.ShowNewFolderButton = false;
            if (dia.ShowDialog() == DialogResult.OK)
            {
                temp = dia.SelectedPath;
                txtOutput.Text = temp;
            }
        }
    }
}
