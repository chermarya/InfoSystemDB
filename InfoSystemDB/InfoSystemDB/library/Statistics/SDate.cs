using System;

namespace InfoSystemDB
{
    public class SDate
    {
        public DateTime Date { get; }
        
        public int Product { get; }
        public int Quantity { get; }

        public SDate(DateTime date, int prod, int quan)
        {
            Date = date;
            Product = prod;
            Quantity = quan;
        }
    }
}