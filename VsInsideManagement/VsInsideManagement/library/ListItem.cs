namespace VsInsideManagement.library
{
    public class ListItem
    {
        public int Id { get; }
        public string Title { get; }
        public ListItem AddInfo { get; }

        public ListItem(int id, string title, ListItem add)
        {
            Id = id;
            Title = title;
            AddInfo = add;
        }
        
        public ListItem(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}