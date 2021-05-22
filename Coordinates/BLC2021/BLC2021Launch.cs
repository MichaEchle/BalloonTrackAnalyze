using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLC2021
{
    public partial class BLC2021Launch : Form
    {
        public BLC2021Launch()
        {
            InitializeComponent();
        }

        private void btTaskSheet1_Click(object sender, EventArgs e)
        {
            BLC2021TaskSheet1 blc2021TaskSheet1 = new BLC2021TaskSheet1(rbBatchMode.Checked);
            blc2021TaskSheet1.Show();
        }
    }
}
