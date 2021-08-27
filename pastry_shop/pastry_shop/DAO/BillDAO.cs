using pastry_shop.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DAO
{
     public class BillDAO // lấy ra cái Bill từ idTable
    {
        private static BillDAO instance;

        public static BillDAO Instance { get => instance == null ? instance = new BillDAO() : instance; private set => instance = value; }

        private BillDAO() { }

        public int GetUncheckBillByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Bill WHERE idTable = " + id + "   AND status = 0");

            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.ID;
            }
            return -1; // khong co id nao het
        }

        public  void InsertBill(int id)
        {
            DataProvider.Instance.ExecuteNonQuery("exec USP_InsertBill @idTable",new object[] { id});
        }

        public DataTable GetBillListByDate(DateTime checkIn,DateTime checkOut)
        {
           return  DataProvider.Instance.ExecuteQuery("exec USP_GetListBillByDate @chechIn , @chechOut",new object[] {checkIn,checkOut});
        }

        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar(" SELECT MAX(id) FROM dbo.Bill");
            }
            catch 
            {

                return 1;
            }
        }

        public void CheckOut(int id,int discount,float totalPrice)
        {
            string query = " UPDATE dbo.Bill SET DateCheckOut=GETDATE(), status = 1 ,"+"discount = " + discount + ",totalPrice = " + totalPrice + " WHERE id = " + id;
            DataProvider.Instance.ExecuteNonQuery(query);
        }

    }
}
