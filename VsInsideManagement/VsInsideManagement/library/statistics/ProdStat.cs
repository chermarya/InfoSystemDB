namespace VsInsideManagement.library.statistics
{
    public class ProdStat
    {
        public string Title { get; }
        public int Quantity { get; }

        public ProdStat(string title, int quan)
        {
            Title = title;
            Quantity = quan;
        }
    }
}