using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DAO
{
    public class DataProvider// lớp này có nhiệm vụ là tạo kết nối với csdl và đưa dữ liệu ra
    {
        private static DataProvider instance;

        public static DataProvider Instance // tất cả các chương trình đều có thể truy cập class này
        {
            get => instance == null ? instance = new DataProvider() : instance;
            private set => instance = value;
        }

        private DataProvider() { } // hàm dựng để đảm bảo bên ngoài không tác động vào được

        private string connectionString = @"Data Source=.\KID;Initial Catalog=PASTRY_SHOP;Integrated Security=True;Connect Timeout=3"; // thêm @ để lấy đường dẫn tuyệt đối

        // ExecuteQuery trả ra dòng kết quả
        public DataTable ExecuteQuery(string query, object[] parameter = null) // thực thi câu query có dữ liệu trả ra là DataTable
        {
            DataTable data = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString)) // kết nối Client với Server
                                                                                   // khi kết thúc khối lệnh (connection.Close()) thì dữ liệu ở connection sẽ tự được giải phóng
            {

                connection.Open();
                SqlCommand command = new SqlCommand(query, connection); // Execute câu truy vấn query thực thi cho cái connection

                //command.Parameters.AddWithValue("@userName", id);
                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command); // trung gian thực hiện truy vấn lấy dữ liệu

                adapter.Fill(data);

                connection.Close();
            }

            return data;
        }

        // ExecuteNonQuery trả ra số dòng thực thi (số dòng thực thi đó là insert, delete, update)
        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                data = command.ExecuteNonQuery();

                connection.Close();
            }

            return data;
        }


        
        public object ExecuteScalar(string query, object[] parameter = null)
        {
            object data = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))

            {

                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                if (parameter != null)
                {
                    string[] listPara = query.Split(' ');
                    int i = 0;
                    foreach (string item in listPara)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameter[i]);
                            i++;
                        }
                    }
                }

                data = command.ExecuteScalar();

                connection.Close();
            }

            return data;
        }
    }

    
}
