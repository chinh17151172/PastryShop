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
using System.Globalization;
using System.Threading;

namespace pastry_shop
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();
            LoadCategory();
        }

        #region Methods

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += "(" + LoginAccount.DisplayName + ")";
        }

        void LoadCategory()
        {
            List<Category> category = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = category;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> foods = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = foods;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();

            List<Table> tableList = TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button bt = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                bt.Text = item.Name + Environment.NewLine + "(" + item.Status + ")";
                bt.Click += Bt_Click;
                bt.Tag = item;

                if (item.Status == "Trống")
                {
                    bt.BackColor = Color.Aquamarine;
                }
                else
                {
                    bt.BackColor = Color.Lime;
                }

                flpTable.Controls.Add(bt);
            }
        }

        void ShowBill(int id)
        {
            lvBill.Items.Clear();
            List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;

                lvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi_VN"); // Chuyển đổi tổng tiền từ en-US(England) thành vi_VN(VN)
                                                            // Thread.CurrentThread.CurrentCulture = culture;

            tbTotalPrice.Text = totalPrice.ToString("c", culture);
            //  LoadTable();
        }

        #endregion

        #region Events

        private void danhMụcNướcUốngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fDrink f = new fDrink();
            f.ShowDialog();
        }

        private void danhMụcThứcĂnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fFoodCategory f = new fFoodCategory();
            f.ShowDialog();
        }

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btCheckOut_Click(this, new EventArgs());
        }

        private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btAddFood_Click(this, new EventArgs());
        }

        public void Bt_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood += F_InsertFood;
            f.DeleteFood += F_DeleteFood;
            f.UpdateFood += F_UpdateFood;
            f.ShowDialog();
        }

        private void F_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lvBill.Tag != null)
                ShowBill((lvBill.Tag as Table).ID);
        }

        private void F_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lvBill.Tag != null)
                ShowBill((lvBill.Tag as Table).ID);
            LoadTable();
        }

        private void F_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lvBill.Tag != null)
                ShowBill((lvBill.Tag as Table).ID);
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount1 += F_UpdateAccount1;
            f.ShowDialog();
        }
        //
        private void F_UpdateAccount1(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }
        //
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        private void btAddFood_Click(object sender, EventArgs e)
        {
            Table table = lvBill.Tag as Table;

            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1) //khong co bill nao
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);// update lai
            LoadTable();
        }


        private void btCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillByTableID(table.ID);
            int discount = (int)nmDiscount.Value;

            double totalPrice = Convert.ToDouble(tbTotalPrice.Text.Split(',')[0].Replace(".", ""));
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if (idBill != 1)
            {
                if (discount != 0)
                {
                    if (MessageBox.Show(string.Format("Bạn có chắc muốn thanh toán hóa đơn cho bàn {0}\n Tổng tiền - (Tổng tiền/100) x Giảm giá \n = {1} - ({1} /100)   x {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {

                        BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                        ShowBill(table.ID);

                        LoadTable();

                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format("Bạn có chắc muốn thanh toán hóa đơn cho bàn {0} ", table.Name), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {

                        BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                        ShowBill(table.ID);

                        LoadTable();

                    }
                }
            }
        }




        #endregion

      
    }
}
