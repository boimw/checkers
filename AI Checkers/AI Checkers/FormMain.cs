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
            Warning warning = new Warning();
            warning.ShowDialog();

            if (warning.DialogResult == DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WarningRestart wr = new WarningRestart();
            wr.ShowDialog();
            if (wr.DialogResult == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void aIVsAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            boardPanel2.Enabled = false;
            boardPanel2.AITurn();
        }

        private void howToPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tutorial t = new Tutorial();
            t.ShowDialog();
        }

    }
}
