namespace InfoSystemDB
{
    public class ListItem
    {
        public int Id { get; }
        public string Title { get; }
        public string AddInfo { get; }

        public ListItem(int id, string title, string add)
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