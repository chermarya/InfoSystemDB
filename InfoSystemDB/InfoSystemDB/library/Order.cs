using System;
using System.Windows;

namespace InfoSystemDB
{
    public class Order
    {
        private string _status;
        public int mode = 0;

        public string ID { get; }
        public string Date { get; }
        public string Nick { get; }
        public string Name { get; }
        public string Phone { get; }
        public string Products { get; }
        public string Address { get; }
        public string Discount { get; }
        public int Sum { get; }
        public int Prepay { get; }
        public int Amount_due { get; }
        public string Invoice { get; set; }

        public string FullStatus { get; set; }
        public DateTime? StatusDate { get; set; }
        
        public string Status
        {
            get { return _status; }

            set
            {
                if (value.Contains("відгружен") && mode == 1)
                {
                    MessageBox.Show(value);
                }
                else
                    _status = value;
            }
        }

        public string Note { get; }
        public string Shop { get; }
        public string Manager { get; }

        public Order(int m, string id, string date, string nick, string name, string phone, string products,
            string address, string discount, int sum, int prepay, int amount, string invoice, string status, 
            string note, string shop, string manager)
        {
            mode = m;
            ID = id;
            Date = date;
            Nick = nick;
            Name = name;
            Phone = phone;
            Address = address;
            Discount = discount;
            Sum = sum;
            Prepay = prepay;
            Amount_due = amount;
            Invoice = invoice;

            FullStatus = status;
            if (status.Contains("відгружен"))
            {
                string[] stat = status.Split(' ');
                Status = stat[0];
                StatusDate = DateTime.Parse(stat[1]);
                FullStatus = stat[0] + "\n" + stat[1];
            }
            else
            {
                FullStatus = status;
                Status = status;
                //StatusDate = null;
            }

            Note = note;
            Shop = shop;
            Manager = manager;

            string[] prod = products.Split(',');
            string prodStr = "";
            foreach (var i in prod)
            {
                prodStr += "⦿ " + i + "\n";
            }

            Products = prodStr;
        }
    }
}