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
using Microsoft.Win32;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        string sWFX32;

        public Form1()
        {
            InitializeComponent();
        }

        string GetWFX32()
        {
            string strWFX32;
            strWFX32 = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\CCHWinFx", "NetIniLocation", "none").ToString();
            if (strWFX32 == "none")
                strWFX32 = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\CCHWinFx", "Dir", "none").ToString();
            strWFX32 = strWFX32.TrimEnd('\0');
            if (strWFX32 == "none")
            {
                MessageBox.Show("Please Run Workstation Setup before running this program");
                Application.Exit();
            }
            return strWFX32;
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            string sPath;

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                sPath = folderBrowserDialog1.SelectedPath;
                textBox1.Text = sPath;
                lSelected.Text = sPath;
                sWFX32 = sPath;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sWFX32 = lSelected.Text = textBox1.Text;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = lSelected.Text = sWFX32 = GetWFX32();
            fillYears();
        }

        private void fillYears()
        {
            string syear;
            for (int i = 0; i < 21; i++)
            {
                if (i < 10)
                    syear = 0 + i.ToString();
                else
                    syear = i.ToString();
                cbYear.Items.Add(syear);
            }
        }

        private void MoveSubdirectory(string Path, string destination)
        {
            string ActiveItem;
            //For each file in directory, copy file to new destination
            foreach (string f in Directory.GetFiles(Path))
            {
                ActiveItem = f.Substring(f.LastIndexOf('\\'));
                ActiveItem = destination + ActiveItem;
                //If file doesn't exist in destination, move the file
                if (!File.Exists(ActiveItem))
                {
                    File.Move(f, ActiveItem);
                }
                //If file DOES exist, move file under dated file name
                else
                {
                    ActiveItem = ActiveItem + DateTime.Now.Month + "-" + DateTime.Now.Day;
                    if (File.Exists(ActiveItem))
                    {
                        File.Copy(f, ActiveItem, true);
                        File.Delete(ActiveItem);
                    }
                    else
                        File.Move(f, ActiveItem);
                }
            }
            //Check for subdirectories.  If subdirectory does not exist in source move entire directory, else recursively call to search subdirectory
            foreach (string g in Directory.GetDirectories(Path))
            {
                ActiveItem = g.Substring(g.LastIndexOf('\\'));
                ActiveItem = destination + ActiveItem;
                if (!Directory.Exists(ActiveItem))
                {
                    Directory.Move(g, ActiveItem);
                }
                else
                {
                    MoveSubdirectory(g, ActiveItem);
                }
            }
            if (!checkBox1.Checked)
                Directory.Delete(Path);
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            string TenYear = sWFX32 + "\\10Year";
            string Year, TenClient, TenInstall, TenUser, Working, ActiveItem;
            if (!Directory.Exists(TenYear))
                Directory.CreateDirectory(TenYear);
            if (cbYear.SelectedIndex == -1)
                return;
            Year = cbYear.Items[cbYear.SelectedIndex].ToString();
            TenYear = TenYear + "\\20" + Year;
            if (!Directory.Exists(TenYear))
                Directory.CreateDirectory(TenYear);
            TenClient = TenYear + "\\Client";
            TenUser = TenYear + "\\User";
            TenInstall = TenYear + "\\Install";
            //Start Move of Client Return files to 10Year;
            Working = sWFX32 + "\\Client";
            if (!Directory.Exists(TenClient))
                Directory.CreateDirectory(TenClient);
            if (Directory.Exists(Working))
            {

                foreach (string d in Directory.GetDirectories(Working))
                {
                    ActiveItem = d.Substring(d.LastIndexOf('\\') + 1);
                    if (ActiveItem.StartsWith(Year))
                    {
                        ActiveItem = TenClient + "\\" + ActiveItem;
                        //If destination directory exists, copy all files from source folder
                        if (Directory.Exists(ActiveItem))
                        {
                            string destination = ActiveItem;
                            MoveSubdirectory(d, destination);
                        }
                        else
                        {
                            Directory.Move(d, ActiveItem);
                        }
                    }
                }
                foreach (string f in Directory.GetFiles(Working, "*.C00"))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenClient + ActiveItem;
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
            }
            //Start Copy of Permission key from Install
            Working = sWFX32 + "\\Install\\" + Year;
            if (!Directory.Exists(TenInstall))
                Directory.CreateDirectory(TenInstall);
            if (Directory.Exists(Working))
            {
                TenInstall = TenInstall + "\\" + Year;
                if (!Directory.Exists(TenInstall))
                    Directory.CreateDirectory(TenInstall);
                foreach (string f in Directory.GetFiles(Working))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenInstall + ActiveItem;
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
            }
            //Start Copy of User
            Working = sWFX32 + "\\user";
            if (!Directory.Exists(TenUser))
                Directory.CreateDirectory(TenUser);
            if (Directory.Exists(Working))
                foreach (string f in Directory.GetFiles(Working, "*.usr"))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenUser + ActiveItem;
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
            //Start Copy of ctx.ini
            ActiveItem = sWFX32 + "\\ctx.ini";
            if (File.Exists(ActiveItem))
            {
                if (!File.Exists(TenYear + "\\ctx.ini"))
                    File.Copy(ActiveItem, TenYear + "\\ctx.ini");
            //Remove products from CTX.ini
            //TODO break this into smaller relevant methods for each section
                DialogResult result = MessageBox.Show("Scan CTX.ini for entries for Year " + Year, "CTX.ini Removal", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string input, output = "";
                    FileStream file = new FileStream(ActiveItem, FileMode.Open, FileAccess.ReadWrite);
                    using (StreamReader reader = new StreamReader(file, Encoding.ASCII))
                    {

                        input = reader.ReadToEnd();
                        reader.Close();
                    }
                    int index, endindex;
                    //prepare to read input line by line
                    string line;
                    index = 0;
                    endindex = input.IndexOf('\r', index);
                    while (endindex > 0 && index < input.Length)
                    {

                        line = input.Substring(index, (endindex - index));
                        switch (line)
                        {
                            //remove entry from Current Release Number version list
                            case "[Current Release Number]":
                                {
                                    output = output + line + "\r\n";
                                    //Loop through lines until reaching a line that starts a new section
                                    while (true)
                                    {
                                        index = endindex + 2;
                                        endindex = input.IndexOf('\r', index);
                                        line = input.Substring(index, (endindex - index));
                                        if (line.StartsWith("["))
                                            break;
                                        //If line contains the specified year skip that line
                                        if (!line.Contains(Year + "Version="))
                                        {
                                            output = output + line + "\r\n";
                                        }
                                    }
                                    break;
                                }
                            //Remove entry for list of Installed Tax Years
                            case "[TaxYear]":
                                {
                                    output = output + line + "\r\n";
                                    //Loop through lines until reaching a line that starts a new section;
                                    while (true)
                                    {
                                        index = endindex + 2;
                                        endindex = input.IndexOf('\r', index);
                                        line = input.Substring(index, (endindex - index));
                                        if (line.StartsWith("["))
                                            break;
                                        if (line.StartsWith("TaxYears="))
                                        {
                                            //check for entry with comma before and after
                                            line = line.Replace(Year + ",", "");
                                            line = line.Replace("," + Year, "");
                                        }
                                        output = output + line + "\r\n";
                                    }
                                    break;
                                }
                            //Remove paths for tax products
                            case "[paths]":
                                {
                                    output = output + line + "\r\n";
                                    //Loop through lines until reaching a line that starts a new section;
                                    while (true)
                                    {
                                        index = endindex + 2;
                                        endindex = input.IndexOf('\r', index);
                                        line = input.Substring(index, (endindex - index));
                                        if (line.StartsWith("["))
                                            break;
                                        //If line starts with specified year to remove, skip
                                        if (!line.StartsWith(Year))
                                        {
                                            output = output + line + "\r\n";
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    //Line does not require special processing, Add line to ouput string, move to next line.
                                    output = output + line + "\r\n";
                                    index = endindex + 2;
                                    endindex = input.IndexOf('\r', index);
                                    break;
                                }
                        }
                    }
                    //Remove Icon for Tax Year
                    line = "[" + Year + " Tax Preparation]";
                    if (output.Contains(line))
                    {
                        index = output.IndexOf(line);
                        endindex = output.IndexOf('[', index + 1);
                        string beginning = output.Substring(0, index);
                        string ending = output.Substring(endindex);
                        output = beginning + ending;
                    }
                    //Check if FX Direct is installed
                    line = Year + "WDIRECT=I";
                    if (output.Contains(line))
                    {
                        index = output.IndexOf(line);
                        endindex = output.IndexOf("\r\n", index + 1);
                        string beginning = output.Substring(0, index);
                        string ending = output.Substring(endindex + 1);
                        output = beginning + ending;
                    }
                    //if checked, do not make permanent changes to ctx.ini, write to test file ctx.init
                    if (checkBox1.Checked)
                        ActiveItem = ActiveItem + "t";
                    //write output to file
                    using (StreamWriter writer = new StreamWriter(ActiveItem))
                    {
                        writer.Write(output);
                        writer.Close();
                    }
                    file.Close();
                }
            }
            //Remove application files
            if (!checkBox1.Checked)
            {
                try
                {
                    ActiveItem = sWFX32 + "\\ctx" + Year + ".ini";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\CtxTp" + Year + ".exe";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\Tax" + Year + ".exe";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\IJS.BKD";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\IJS.BKX";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\IJS.dat";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                    ActiveItem = sWFX32 + "\\IJS.idx";
                    if (File.Exists(ActiveItem))
                        File.Delete(ActiveItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                ActiveItem = sWFX32 + "\\ctx" + Year + ".ini";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ctxyear.ini", true);
                ActiveItem = sWFX32 + "\\CtxTp" + Year + ".exe";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ctxtp.exe", true);
                ActiveItem = sWFX32 + "\\Tax" + Year + ".exe";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\tax.exe", true);
                ActiveItem = sWFX32 + "\\IJS.BKD";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ijs.bkd", true);
                ActiveItem = sWFX32 + "\\IJS.BKX";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ijs.bkx", true);
                ActiveItem = sWFX32 + "\\IJS.dat";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ijs.dat", true);
                ActiveItem = sWFX32 + "\\IJS.idx";
                if (File.Exists(ActiveItem))
                    //File.Delete(ActiveItem);
                    File.Copy(ActiveItem, TenYear + "\\ijs.idx", true);
            }
            //Prompt to run rebuild;
            if (!checkBox1.Checked)
            {
                ActiveItem = sWFX32 + "\\rebuild.exe";
                MessageBox.Show("Please run a System Rebuild \r\nUnder High-Level Options check \"Synchonize Client Links\"\r\nUnser Low-Level Options select \"Synchronize Product Types\"");
                if (File.Exists(ActiveItem))
                    System.Diagnostics.Process.Start(sWFX32 + "\\rebuild.exe");
            }
            else
                MessageBox.Show("Test ctx.ini written to \"ctx.iniT\".\nApplication files are copied, and not deleted.\nClient folders will need to be moved back to original location in WFX32\\Client.");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = checkBox1.Checked;
        }
    }
}
