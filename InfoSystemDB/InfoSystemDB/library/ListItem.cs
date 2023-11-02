namespace InfoSystemDB
{
    public class ListItem
    {
        public int Id { get; }
        public string Title { get; }

        public ListItem(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}