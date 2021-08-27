using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DTO
{
    public class Bill
    {
        public Bill(int id, DateTime? dataCheckIn, DateTime? dataCheckOut, int status,int discount = 0)
        {
            this.ID = id;
            this.DataCheckIn = dataCheckIn;
            this.dataCheckOut = dataCheckOut;
            this.Status = status;
            this.Discount = discount;
        }

        public Bill(DataRow row)
        {
            this.ID = (int)row["id"];
            this.DataCheckIn = (DateTime?)row["DateCheckIn"];

            var dataCheckOutTemp = row["DateCheckOut"];
            if (dataCheckOutTemp.ToString() != "")
            {
                this.DataCheckOut = (DateTime?)dataCheckOutTemp;
            }
            this.Status = (int)row["status"];
            this.Discount = (int)row["discount"];
        }

        private int status;
        public int Status { get => status; set => status = value; }

        private DateTime? dataCheckOut;
        public DateTime? DataCheckOut { get => dataCheckOut; set => dataCheckOut = value; }

        private DateTime? dataCheckIn;
        public DateTime? DataCheckIn { get => dataCheckIn; set => dataCheckIn = value; }

        private int iD;
        public int ID { get => iD; set => iD = value; }

        private int discount;
        public int Discount { get => discount; set => discount = value; }
    }
}
