using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pastry_shop
{
    public partial class fBackground : Form
    {
        public fBackground()
        {
            InitializeComponent();
        }

        private void btLogIn_Click(object sender, EventArgs e)
        {
            fLogIn t = new fLogIn();
          
            t.ShowDialog();

        }
    }
}
