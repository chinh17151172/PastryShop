﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pastry_shop.DTO
{
    public class Food
    {
        public Food(int id,string name,int categoryID,float price)
        {
            this.ID = id;
            this.Name = name;
            this.CategotyID = categoryID;
            this.Price = price;
        }

        public Food(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name_food"].ToString();
            this.CategotyID = (int)row["idCategory"];
            this.Price = (float)Convert.ToDouble(row["price"].ToString());
        }

        private float price;
        public float Price { get => price; set => price = value; }

        private int categotyID;
        public int CategotyID { get => categotyID; set => categotyID = value; }

        private string name;
        public string Name { get => name; set => name = value; }

        private int iD;
        public int ID { get => iD; set => iD = value; }
        
    }
}
