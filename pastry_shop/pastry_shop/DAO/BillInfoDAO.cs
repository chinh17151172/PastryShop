using pastry_shop.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DAO
{
    public class BillInfoDAO
    {
        private static BillInfoDAO instance;
        public static BillInfoDAO Instance { get => instance == null ? instance = new BillInfoDAO() : instance; private set => instance = value; }

        private BillInfoDAO() { }

        public void DeleteBillInfoByFoodID(int id)
        {
            DataProvider.Instance.ExecuteQuery("Delete dbo.BillInfo WHERE idFood = " + id);
        }


        public List<BillInfo> GetListBillInfo(int id) // id cua Bill
        {
            List<BillInfo> listbillInfo = new List<BillInfo>();

            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.BillInfo WHERE idBill = " + id);

            foreach (DataRow item in data.Rows)
            {
                BillInfo info = new BillInfo(item);
                listbillInfo.Add(info);
            }

            return listbillInfo;
        }

        public void InsertBillInfo(int ibBill, int idFood, int count)
        {
            DataProvider.Instance.ExecuteQuery("exec USP_InsertBillInfo @idBill , @idFood , @count", new object[] { ibBill, idFood,count});
        }

    }
}
