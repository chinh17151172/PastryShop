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
    public partial class fAccountProfile : Form
    {

        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount); }
        }

        public fAccountProfile(Account acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }

        void ChangeAccount(Account acc)
        {
            tbUserName.Text = LoginAccount.UserName;
            tbDisplayName.Text = LoginAccount.DisplayName;
        }


        void UpdateAccount()
        {
            string displayName = tbDisplayName.Text;
            string password = tbPassword.Text;
            string newpass = tbNewPassword.Text;
            string reenterPass = tbReEnterPassword.Text;
            string userName = tbUserName.Text;

            if (!newpass.Equals(reenterPass))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu đúng với mật khẩu mới !");
            }
            else
            {
                if (AccountDAO.Instance.UdateAccount(userName, displayName, password, newpass))
                {
                    MessageBox.Show("Cập nhật thành công");
                    if (updateAccount1 != null)
                        updateAccount1(this,new AccountEvent(AccountDAO.Instance.GetAccountByUserName(userName)));
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đúng mật khẩu");
                }
            }

        }

        private event EventHandler<AccountEvent>updateAccount1;
        public event EventHandler<AccountEvent> UpdateAccount1
        {
            add { updateAccount1 += value; }
            remove { updateAccount1 -= value; }
        }


        private void btExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            UpdateAccount();
        }

        private void fAccountProfile_Load(object sender, EventArgs e)
        {
            tbUserName.TabIndex = 0;
            tbDisplayName.TabIndex = 1;
            tbPassword.TabIndex = 2;
            tbNewPassword.TabIndex = 3;
            tbReEnterPassword.TabIndex = 4;
            btUpdate.TabIndex = 5;
            btExit.TabIndex = 6;
        }
    }

    public class AccountEvent : EventArgs
    {
        private Account acc;
        public Account Acc { get => acc; set => acc = value; }

        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }

}
