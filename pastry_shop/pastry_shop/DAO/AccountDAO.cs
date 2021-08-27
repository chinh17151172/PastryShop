using pastry_shop.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace pastry_shop.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance; // Ctrl + R + E

        public static AccountDAO Instance
        {
            get => instance == null ? instance = new AccountDAO() : instance;
            private set => instance = value;
        }
        private AccountDAO() { }

        public bool LogIn(string userName, string password)
        {
            //byte[] temp = ASCIIEncoding.ASCII.GetBytes(password);
            //byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            //var list = hasData.ToString();
            //list.Reverse();


            string query = "USP_LogIn @userName , @password";
            //string query = " select* from dbo.account where username = @username and password = @password ";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, password /*list*/});
            return result.Rows.Count > 0; // mặc định là returrn false      
        }

        public bool UdateAccount(string userName, string displayName, string pass, string newpass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newPassword ",new object[] { userName,displayName,pass,newpass});
            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("SELECT UserName,DisplayName,Type FROM dbo.Account");
        }


        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from  account where userName = '" + userName + "'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }

        public bool InsertAccount(string name, string displayname,int  type)
        {
            string query = string.Format("INSERT dbo.Account ( UserName, DisplayName, Type) VALUES(   N'{0}', N'{1}', {2}  )", name, displayname, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateAccount( string name, string displayname, int type)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{1}', Type = {2} WHERE username  = N'{0}'", name, displayname, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteAccount(string name)
        {
           // BillInfoDAO.Instance.DeleteBillInfoByFoodID(id);



            string query = string.Format("Delete Account Where username = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ResetPassword(string name)
        {
            string query = string.Format("Update Account Set password  = N'0' Where username = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

    }
}
