using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Csharp.Apk_Reader;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SharpAdbClient;

namespace new_Apk_installer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        string apkSelected;
        string apkSelectedFileName;
        ApkInfo file_info, file_info2;
        bool fileIsSelected = false;
        
        /* ******* Impelement of OpenFile Dialog Function ******** */
        public bool openFileDialogeFunc()
        {
            ofdApk.DefaultExt = "*.apk";
            ofdApk.AddExtension = true;
            ofdApk.Multiselect = true;
            ofdApk.Filter = "Android Application Files|*.apk";
            ofdApk.Title = "Select Your APK File";
            ofdApk.FileName = "Choose Files...";
            bool retFlag = ofdApk.ShowDialog() == DialogResult.OK;
            return retFlag;
        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            rtbInfo.Text = "";
            bool flag = openFileDialogeFunc();
            if (flag)
            {
                fileIsSelected = true;
                foreach (string file in ofdApk.FileNames)
                {
                    apkSelected = ofdApk.FileName;
                    apkSelectedFileName = ofdApk.SafeFileName;
                    var readApk = new Apk_Reader();

                    file_info = readApk.Get_File_Info(file);
                    panel1.Visible = true;
                    lblAppInfo.Visible = true;
                    lblAppInfo.Text = "APK Name: " + file_info.apkName + "\n" + "APK Version: " + file_info.versionName + "\n";
                }

                /*string[] apkSelected2 = ofdApk.FileNames;
                int i = 0;
                foreach (string str in apkSelected2)
                {
                    i++;
                    long lengthApk = new System.IO.FileInfo(str).Length;
                    string retSize = humanReadableConvert(lengthApk);
                    rtbInfo.Text = rtbInfo.Text + "\nSuccesful Select[" + i + "]" + ":\n" +
                        "\t>> Apk Path: " + str + "\n" +
                        "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
                }*/
            }

        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            AdbServer server = new AdbServer();
            var result = server.StartServer("adb.exe", restartServerIfNewer: false);
            var devices = AdbClient.Instance.GetDevices();
            int i = 1;
            rtbInfo.Text = "";
            foreach (var device in devices)
            {

                rtbInfo.Text = rtbInfo.Text + "\n" + "Device " + i + ":\n" +
                    "\t>> Phone Name: " + device.Name + "\n" +
                    "\t>> Phone Model: " + device.Model + "\n" +
                    "\t>> Phone Product: " + device.Product + "\n" +
                    "\t>> Phone Serial: " + device.Serial;
                i++;
                rtbInfo.Text = rtbInfo.Text + "\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
            }
            
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            rtbInfo.Text = "";
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (!fileIsSelected)
            {
                rtbInfo.Text = "";
                rtbInfo.Text = "Please Select a Apk File First";
            }
            else
            {
                rtbInfo.Text = "";
                int i = 0;
                foreach (string file in ofdApk.FileNames)
                {
                    apkSelected = ofdApk.FileNames[i];
                    apkSelectedFileName = ofdApk.SafeFileNames[i];
                    i += 1;
                    var readApk2 = new Apk_Reader();

                    file_info2 = readApk2.Get_File_Info(file);

                    if (apkSelectedFileName != file_info2.apkName + " " + file_info2.versionName + ".apk")
                    {
                        string tempStr = Path.GetDirectoryName(apkSelected);

                        if (File.Exists(tempStr + "\\" + file_info2.apkName + " " + file_info2.versionName + ".apk"))
                        {
                            rtbInfo.Text = "A File With This Apk Name Exist IN Your Computer, Please Rename That File And Try Agian.";
                            rtbInfo.Text = rtbInfo.Text + "\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
                        }
                        else
                        {
                            if (!File.Exists(apkSelected))
                            {
                                rtbInfo.Text = "You Already Changed The Apk Name.";
                                rtbInfo.Text = rtbInfo.Text + "\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
                            }
                            else
                            {
                                FileSystem.RenameFile(apkSelected, file_info2.apkName + " " + file_info2.versionName + ".apk");
                                string apkPathPrint = Path.GetDirectoryName(apkSelected);
                                rtbInfo.Text = rtbInfo.Text + "\n" + "Successful:";
                                rtbInfo.Text = rtbInfo.Text + "\n\t>> Apk Path: " + apkPathPrint + "\\" + file_info2.apkName + " " + file_info2.versionName + ".apk";  
                                rtbInfo.Text = rtbInfo.Text + "\n\t>> APK Renamed From \"" + apkSelectedFileName + "\" To \"" + file_info2.apkName + " " + file_info2.versionName + ".apk\"";

                                long length = new System.IO.FileInfo(tempStr + "\\" + file_info2.apkName + " " + file_info2.versionName + ".apk").Length;
                                string ffSize = humanReadableConvert(length);
                                rtbInfo.Text = rtbInfo.Text + "\n\t>> APK Size: " + ffSize + "\n"; //length.ToString() + " Bytes\n";
                                rtbInfo.Text = rtbInfo.Text + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
                            }

                        }

                    }
                    else
                    {
                        rtbInfo.Text = rtbInfo.Text + "\n" + apkSelectedFileName + " >> The Original APK File Name IS The Same AS The Previous One, No Need TO Change The APK Name.";
                        rtbInfo.Text = rtbInfo.Text + "\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ";
                    }

                }
            }
       
        }

        /* ******* Impelement of humanReadableConvert Function ******** */
        public string humanReadableConvert(double fSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            //double len = new FileInfo(filename).Length;
            int order = 0;
            while (fSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fSize = fSize / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string resultSize = String.Format("{0:0.##} {1}", fSize, sizes[order]);

            return resultSize;
        }
    }
}
