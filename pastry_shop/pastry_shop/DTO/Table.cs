using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DTO
{
    public class Table
    {
        public Table(int id, string name, string status)
        {
            this.ID = id;
            this.Name = name;
            this.Status = status;
        }

        public Table(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name_table"].ToString();
            this.Status = row["status_table"].ToString();
        }

        private string status;
        public string Status
        {
            get => status;
            set => status = value;
        }

        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        private int iD;
        public int ID
        {
            get => iD;
            set => iD = value;
        }

    }
}
