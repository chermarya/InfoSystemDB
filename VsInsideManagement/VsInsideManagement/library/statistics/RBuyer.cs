namespace VsInsideManagement.library.statistics
{
    public class RBuyer
    {
        public string Buyer { get; }
        public int OrderCount { get; }
        public int ProductCount { get; }
        public int TotalSum { get; }

        public RBuyer(string buyer, int order, int product, int sum)
        {
            Buyer = buyer;
            OrderCount = order;
            ProductCount = product;
            TotalSum = sum;
        }
    }
}