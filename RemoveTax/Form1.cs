using System;
using System.IO;
using System.Text;
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

        //Retrieve location of WFX32 folder from registry
        private string GetWFX32()
        {
            string strWFX32;
            strWFX32 = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\CCHWinFx", "NetIniLocation", "none").ToString();
            if (strWFX32 == "none")
                strWFX32 = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\CCHWinFx", "Dir", "none").ToString();
            strWFX32 = strWFX32.TrimEnd('\0');
            if (strWFX32 == "none")
            {
                //MessageBox.Show("Please Run Workstation Setup before running this program");
                //Application.Exit();
            }
            return strWFX32;
        }

        //Create a dialog box to specify a different folder location
        private void btBrowse_Click(object sender, EventArgs e)
        {
            string sPath;

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                sPath = folderBrowserDialog1.SelectedPath;
                txtPath.Text = sPath;
                lSelected.Text = sPath;
                sWFX32 = sPath;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //Only update values if Enter key is pressed from text entry
            if (e.KeyCode == Keys.Enter)
            {
                sWFX32 = lSelected.Text = txtPath.Text;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //get WFX32 location from directory, and then set it to display
            txtPath.Text = lSelected.Text = sWFX32 = GetWFX32();
            fillYears();
        }

        //Fill entries in cbYear combo box for many years
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

        //Moves files in a specified directory, this is used to aviod file conflicts with old archivals, and conflicts with file.move
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
                    ActiveItem = ActiveItem + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
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
            //Only delete the source files if not Testing
            if (!checkTesting.Checked)
                Directory.Delete(Path);
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            string TenYear = sWFX32 + "\\10Year";
            string Year, TenClient, TenInstall, TenUser;
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
                CopyClients(Year, TenClient);
            //Start Copy of Permission key from Install
                CopyInstall(Year, TenInstall);
            //Start Copy of User
                CopyUser(TenUser);
            //Start edit of ctx.ini
                EditCTXini(TenYear, Year, sWFX32 + "\\ctx.ini");
            //Remove application files
                RemoveApplication(TenYear, Year);
            //Prompt to run rebuild;
            if (!checkTesting.Checked)
            {
                string RebuildExe = sWFX32 + "\\rebuild.exe";
                MessageBox.Show("Please run a System Rebuild \r\nUnder High-Level Options check \"Synchonize Client Links\"\r\nUnser Low-Level Options select \"Synchronize Product Types\"");
                if (File.Exists(RebuildExe))
                    System.Diagnostics.Process.Start(RebuildExe);
            }
            else
                //I could include something to copy the moved clients back... TODO
                MessageBox.Show(TestingLabel.Text);

        }

        private void CopyClients(string Year, string TenClient)
        {
            string Working = sWFX32 + "\\Client";
            string ActiveItem;
            if (!Directory.Exists(TenClient))
                Directory.CreateDirectory(TenClient);
            //only proceed if WFX32\Client folder exists
            if (Directory.Exists(Working))
            {
                string destination;
                foreach (string d in Directory.GetDirectories(Working))
                {
                    //get folder name from full path to folder
                    ActiveItem = d.Substring(d.LastIndexOf('\\') + 1);
                    if (ActiveItem.StartsWith(Year))
                    {
                        destination = TenClient + "\\" + ActiveItem;
                        //If destination directory exists, copy all files from source folder using MoveSubDirecotry method
                        if (Directory.Exists(destination))
                        {
                            MoveSubdirectory(d, destination);
                        }
                        else
                        {
                            Directory.Move(d, destination);
                        }
                    }
                }
                //Get all files in Client folder with extension ".C00"
                foreach (string f in Directory.GetFiles(Working, "*.C00"))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenClient + ActiveItem;
                    //do not overwrite files from previous archival attempts
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
            }
        }

        private void CopyInstall(string Year, string TenInstall)
        {
            string Working = sWFX32 + "\\Install";
            string ActiveItem;
            if (!Directory.Exists(TenInstall))
                Directory.CreateDirectory(TenInstall);
            if (File.Exists(Working + "\\PermInfo.dat"))
                File.Copy(Working + "\\PermInfo.dat", TenInstall + "\\PermInfo.dat", true);
            Working = Working + "\\" + Year;
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
        }

        private void CopyUser(string TenUser)
        {
            string Working = sWFX32 + "\\user";
            string ActiveItem;
            if (!Directory.Exists(TenUser))
                Directory.CreateDirectory(TenUser);
            if (Directory.Exists(Working))
            {
                foreach (string f in Directory.GetFiles(Working, "*.usr"))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenUser + ActiveItem;
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
                foreach (string f in Directory.GetFiles(Working, "*.uex"))
                {
                    ActiveItem = f.Substring(f.LastIndexOf('\\'));
                    ActiveItem = TenUser + ActiveItem;
                    if (!File.Exists(ActiveItem))
                        File.Copy(f, ActiveItem);
                }
            }
        }

        private void EditCTXini(string TenYear, string Year, string ActiveItem)
        {
            if (File.Exists(ActiveItem))
            {
                //COPY CTX.INI BEFORE ANY EDITS
                if (!File.Exists(TenYear + "\\ctx.ini"))
                    File.Copy(ActiveItem, TenYear + "\\ctx.ini");
                
                //Prompt if ctx.ini should be changed
                DialogResult result = MessageBox.Show("Scan CTX.ini for entries for Year " + Year, "CTX.ini Removal", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    //Remove products from CTX.ini
                    string input, output = string.Empty;
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
                        //Begin reading ctx.ini file line by line for processing within sections.
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
                    //Begin re-reading ctx.ini to remove entire section for removed year product entires.
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
                    //if testing is checked, do not make permanent changes to ctx.ini, write to test file ctx.init
                    if (checkTesting.Checked)
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
        }

        private void RemoveApplication(string TenYear, string Year)
        {
            string ActiveItem;
            if (!checkTesting.Checked)
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
                    File.Copy(ActiveItem, TenYear + "\\ctxyear.ini", true);
                ActiveItem = sWFX32 + "\\CtxTp" + Year + ".exe";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\ctxtp.exe", true);
                ActiveItem = sWFX32 + "\\Tax" + Year + ".exe";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\tax.exe", true);
                ActiveItem = sWFX32 + "\\IJS.BKD";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\ijs.bkd", true);
                ActiveItem = sWFX32 + "\\IJS.BKX";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\ijs.bkx", true);
                ActiveItem = sWFX32 + "\\IJS.dat";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\ijs.dat", true);
                ActiveItem = sWFX32 + "\\IJS.idx";
                if (File.Exists(ActiveItem))
                    File.Copy(ActiveItem, TenYear + "\\ijs.idx", true);
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TestingLabel.Visible = checkTesting.Checked;
        }
    }
}
