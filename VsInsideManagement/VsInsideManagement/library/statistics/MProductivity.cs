namespace VsInsideManagement.library.statistics
{
    public class MProductivity
    {
        public string Manager { get; }
        public int OrderCount { get; }
        public int TotalSum { get; }
        public int ProductCount { get; }

        public MProductivity(string manager, int order, int sum, int product)
        {
            Manager = manager;
            OrderCount = order;
            TotalSum = sum;
            ProductCount = product;
        }
    }
}