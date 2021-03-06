﻿using BTDToolbox.Classes;
using BTDToolbox.Extra_Forms;
using BTDToolbox.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTDToolbox
{
    public partial class ThemedForm : Form
    {
        //Low level for toolbar dragging
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;
        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        //for resizing
        int Mx;
        int My;
        int Sw;
        int Sh;
        bool mov;
        public bool canResize = true;
        public bool moveCenterScreen = false;

        private IntPtr cancelThisOnSize;

        //Resize defaults
        int minWidth = 200;
        int minHeight = 100;
        Rectangle resolution = Screen.PrimaryScreen.Bounds;

        public ThemedForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            
            //this.Dock = DockStyle.Fill;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.FormBorderStyle = FormBorderStyle.None;

            TitleLabel.MouseMove += ToolbarDrag;
            titleSeperator.Panel1.MouseMove += ToolbarDrag;
            titleSeperator.Panel2.MouseMove += ToolbarDrag;
            titleSeperator.MouseMove += ToolbarDrag;

            Sizer.MouseDown += SizerMouseDown;
            Sizer.MouseMove += SizerMouseMove;
            Sizer.MouseUp += SizerMouseUp;

            cancelThisOnSize = this.contentPanel.Handle;

            close_button.Click += close_button_Click;
            this.Load += onThemedLoad;
        }

        

        public void onThemedLoad(object sender, EventArgs e)
        {
            titleSeperator.SplitterDistance = 25;
            CheckInBounds();

            if(moveCenterScreen)
                this.Location = new Point(GeneralMethods.GetCenterScreen().X - this.Size.Width / 2 - 75, GeneralMethods.GetCenterScreen().Y - this.Size.Height / 2 - 145);
        }
        private void CheckInBounds()
        {
            //Top left corner
            if (this.Location.X < 0)
            {
                this.Location = new Point(0, this.Location.Y);
            }   
            if (this.Location.Y < 0)
            {
                this.Location = new Point(this.Location.X, 0);
            }
            

            //Bottom right corner
            if (this.Location.X + this.Size.Width > resolution.Width - 10)
            {
                this.Location = new Point(resolution.Width - this.Width - 4, this.Location.Y);
            }
            if (this.Location.Y + this.Size.Height > resolution.Height-105)
            {
                this.Location = new Point(this.Location.X, resolution.Height - this.Height - 91);
            }
            if ((this.Location.X + this.Size.Width > resolution.Width - 10) && (this.Location.Y + this.Size.Height > resolution.Height - 95))
            { 
                this.Location = new Point(resolution.Width - this.Width - 4, resolution.Height - this.Height - 91);
            }
        }
        public virtual void close_button_Click(object sender, EventArgs e)
        {            
            if (!JsonEditorHandler.AreJsonErrors())
                this.Close();
            Main.bg.SendToBack();
        }

        //resizing event methods
        private void SizerMouseDown(object sender, MouseEventArgs e)
        {
            titleSeperator.Hide();
            SendMessage(cancelThisOnSize, 11, 0, 0);
            this.SuspendLayout();
            mov = true;
            My = MousePosition.Y;
            Mx = MousePosition.X;
            Sw = Width;
            Sh = Height;
        }
        private void SizerMouseMove(object sender, MouseEventArgs e)
        {
            if ((MousePosition.X - Mx + Sw) >= minWidth && (MousePosition.Y - My + Sh) >= minHeight)
            {
                if (mov == true)
                {
                    titleSeperator.SplitterDistance = 25;
                    Width = MousePosition.X - Mx + Sw;
                    Height = MousePosition.Y - My + Sh;
                }
            }
            else if ((MousePosition.X - Mx + Sw) >= minWidth && (MousePosition.Y - My + Sh) < minHeight)
            {
                if (mov == true)
                {
                    titleSeperator.SplitterDistance = 25;
                    Width = MousePosition.X - Mx + Sw;
                    Height = minHeight;
                }
            }
            else if ((MousePosition.X - Mx + Sw) < minWidth && (MousePosition.Y - My + Sh) >= minHeight)
            {
                if (mov == true)
                {
                    titleSeperator.SplitterDistance = 25;
                    Height = MousePosition.Y - My + Sh;
                    Width = minWidth;
                }
            }
            else
            {
                titleSeperator.SplitterDistance = 25;
                Height = minHeight;
                Width = minWidth;
            }
        }
        private void SizerMouseUp(object sender, MouseEventArgs e)
        {
            mov = false;
            titleSeperator.Show();
            //Top left corner
            if (this.Location.X < 0)
            {
                int width = resolution.Width - this.Width;
                this.Width = resolution.Width - width;
            }
            if (this.Location.Y < 0)
            {
                int hegiht = resolution.Height - this.Height;
                this.Height = resolution.Height - hegiht;
            }

            //Bottom right corner
            if (this.Location.X + this.Size.Width > resolution.Width - 10)
            {
                int width = resolution.Width - this.Width;
                this.Width = resolution.Width - width - 10;
            }
            if (this.Location.Y + this.Size.Height > resolution.Height - 95)
            {
                int hegiht = resolution.Height - this.Height;
                this.Height = resolution.Height - hegiht - 55;
            }

            if ((this.Location.X + this.Size.Width > resolution.Width - 10) && (this.Location.Y + this.Size.Height > resolution.Height - 95))
            {
                int width = resolution.Width - this.Width;
                this.Width = resolution.Width - width - 10;
                int hegiht = resolution.Height - this.Height;
                this.Height = resolution.Height - hegiht - 55;
            }

            SendMessage(cancelThisOnSize, 11, 1, 0);
            this.ResumeLayout();
            this.Refresh();
        }

        //toolbar drag method
        private void ToolbarDrag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
            CheckInBounds();
        }

        private void ThemedForm_Resize(object sender, EventArgs e)
        {
            if (Main.getInstance() == null || this == null || this.Visible == false)
                return;

            if (Main.getInstance().InvokeRequired)
                Invoke((MethodInvoker)delegate { Main.getInstance().Refresh(); });
            else
                try
                { Main.getInstance().Refresh(); }
                catch (OutOfMemoryException) { }
                
        }

        private void ThemedForm_Shown(object sender, EventArgs e)
        {
            if (!canResize)
                Sizer.Hide();
        }

        private void close_button_MouseDown(object sender, MouseEventArgs e)
        {
            close_button.BackgroundImage = Properties.Resources.new_close_button_click;
        }

        private void close_button_MouseUp(object sender, MouseEventArgs e)
        {
            close_button.BackgroundImage = Properties.Resources.new_close_button_2;
        }
    }
}
