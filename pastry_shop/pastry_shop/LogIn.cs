using pastry_shop.DAO;
using pastry_shop.DTO;
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
    public partial class fLogIn : Form
    {
        public fLogIn()
        {
            InitializeComponent();
        }

        private void btLogIn_Click(object sender, EventArgs e)
        {
            string userName = tbUserName.Text;
            string password = tbPassword.Text;
            if (LogIn(userName, password))
            {
                Account loginAccount = AccountDAO.Instance.GetAccountByUserName(userName);
                fTableManager t = new fTableManager(loginAccount);
                this.Hide();
                t.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Sai tên tài khoản hoặc mất khẩu, vui lòng kiểm tra lại !");
            }
        }

        bool LogIn(string userName, string password)
        {
            return AccountDAO.Instance.LogIn(userName, password);
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fLogIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("Bạn có thật sự muốn thoát chương trinh ?","Thông báo",MessageBoxButtons.YesNo,MessageBoxIcon.Asterisk,MessageBoxDefaultButton.Button1) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
