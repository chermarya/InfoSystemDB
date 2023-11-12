namespace InfoSystemDB
{
    public class Order
    {
        public int ID { get; }
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
        public string Invoice { get; }
        public string Status { get; }
        public string Note { get; }
        public string Shop { get; }
        public string Manager { get; }

        public Order(int id, string date, string nick, string name, string phone, string products, string address,
            string discount, int sum, int prepay, int amount, string invoice, string status, string note, string shop,
            string manager)
        {
            ID = id;
            Date = date;
            Nick = nick;
            Name = name;
            Phone = phone;
            Products = products;
            Address = address;
            Discount = discount;
            Sum = sum;
            Prepay = prepay;
            Amount_due = amount;
            Invoice = invoice;
            Status = status;
            Note = note;
            Shop = shop;
            Manager = manager;
        }
    }
}