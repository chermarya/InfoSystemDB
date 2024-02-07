namespace VsInsideManagement.library
{
    public class SupplyItem
    {
        public string Date { get; }
        public string Product { get; }
        public int Quantity { get; }

        public SupplyItem(string date, string product, int quantity)
        {
            Date = date;
            Product = product;
            Quantity = quantity;
        }
    }
}