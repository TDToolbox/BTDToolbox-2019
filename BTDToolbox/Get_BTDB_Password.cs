﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTDToolbox
{
    public partial class Get_BTDB_Password : Form
    {
        //new refactoring variables
        public string projName { get; set; }
        public string destPath { get; set; }
        public bool isExtracting { get; set; }
        public bool launch { get; set; }

        public Get_BTDB_Password()
        {
            InitializeComponent();
            this.AcceptButton = CreateProject_Button;
            
        }
        public void GetPass()
        {
            string password = Password_TextBox.Text.ToString();
            if (password.Length < 3)
            {
                ConsoleHandler.appendLog("The password you entered was too short...");
                MessageBox.Show("The password you entered was too short...");
            }
            else
            {
                ConsoleHandler.appendLog("You entered the password:  " + password);
                var zip = new ZipForm();
                zip.jetFile_Game = "BTDB";
                zip.password = password;
                zip.projName = projName;                
                zip.Show();
                if (isExtracting == true)
                {
                    zip.Extract();
                }
                else
                {
                    zip.destPath = destPath;
                    zip.launch = launch;
                    zip.Compile();
                }
                this.Close();
            }
        }

        private void CreateProject_Button_Click(object sender, EventArgs e)
        {
            GetPass();
        }
    }
}