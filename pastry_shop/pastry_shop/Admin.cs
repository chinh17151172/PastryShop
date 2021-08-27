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
    public partial class fAdmin : Form
    {
        BindingSource foodlist = new BindingSource();
        BindingSource accountlist = new BindingSource();

        public Account loginAccount;

        public fAdmin()
        {
            InitializeComponent();
            Load();
            // loadaccountlist();
        }

        //void loadaccountlist()
        //{
        //    string query = "SELECT * FROM dbo.Account ";
        //    dtgvAccount.DataSource = DataProvider.Instance.ExecuteQuery(query);
        //}

        //void loadfoodlist()
        //{
        //    string query = "select * from food";
        //    dtgvFood.DataSource = DataProvider.Instance.ExecuteQuery(query);
        //}

        #region Methods

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);


            return listFood;
        }

        void Load()
        {
            dtgvFood.DataSource = foodlist;
            dtgvAccount.DataSource = accountlist;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadAccount();
            AddFoodBinding();
            LoadCategoryIntoComboBox(cbFoodCategory);
            AddAccountBinding();
        }

        void AddAccountBinding()
        {
            tbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "username",true,DataSourceUpdateMode.Never));// 2 statement cuối là không cho chuyển đổi ngược về  từ textbox sang datagridview
            tbDisplayName.DataBindings.Add(new Binding("Text",dtgvAccount.DataSource,"displayname",true, DataSourceUpdateMode.Never));
            nmAccount.DataBindings.Add(new Binding("Value",dtgvAccount.DataSource,"type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountlist.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void LoadListFood()
        {
            foodlist.DataSource = FoodDAO.Instance.GetListFood();
        }

        void AddFoodBinding()
        {
            tbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            tbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "price", true, DataSourceUpdateMode.Never));
        }

        void LoadCategoryIntoComboBox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void AddAccount(string username, string displayname,int type)
        {
            if(AccountDAO.Instance.InsertAccount(username,displayname,type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }
            LoadAccount();
        }

        void EditAccount(string username, string displayname, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(username, displayname, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }
            LoadAccount();
        }

        void DeleteAccount(string username)
        {
            if (loginAccount.UserName.Equals(username))
            {
                MessageBox.Show("Vui lòng đừng xóa tên bạn chứ");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(username))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }
            LoadAccount();
        }

        void ResetPass(string username)
        {

            if (AccountDAO.Instance.ResetPassword(username))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công ");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại ");
            }
        }

        private void tbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategotyID"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }
        #endregion

        #region Events

        private void btAddAccount_Click(object sender, EventArgs e)
        {
            string username = tbUserName.Text;
            string displayname = tbDisplayName.Text;
            int type = (int)nmAccount.Value;

            AddAccount(username,displayname,type);
        }

        private void btDeleteAccount_Click(object sender, EventArgs e)
        {
            string username = tbUserName.Text;

            DeleteAccount(username);
        }

        private void btEditAccount_Click(object sender, EventArgs e)
        {
            string username = tbUserName.Text;
            string displayname = tbDisplayName.Text;
            int type = (int)nmAccount.Value;

            EditAccount(username, displayname, type);
        }

        private void btRessetPassword_Click(object sender, EventArgs e)
        {
            string username = tbUserName.Text;

            ResetPass(username);
        }

        private void btShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void btSearchFoodName_Click(object sender, EventArgs e)
        {
            foodlist.DataSource = SearchFoodByName(tbSearchFoodName.Text);

        }

        private void btAddFood_Click(object sender, EventArgs e)
        {
            string name = tbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm thức ăn");
            }


        }

        private void btEditFood_Click(object sender, EventArgs e)
        {
            string name = tbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(tbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }

        }

        private void btDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(tbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa thức ăn");
            }
        }

        private void btViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }


        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }





        #endregion

       
    }
}
