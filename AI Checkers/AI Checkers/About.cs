﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AICheckers
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_TextChanged(object sender, EventArgs e)
        {
            String text = "Jana je car.";
            textBox1.Text = text;
        }
    }
}
