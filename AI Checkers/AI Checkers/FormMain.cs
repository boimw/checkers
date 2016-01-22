using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace AICheckers
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
    }
}
