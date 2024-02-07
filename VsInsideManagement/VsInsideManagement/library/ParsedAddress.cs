namespace VsInsideManagement.library
{
    public class ParsedAddress
    {
        public string Office { get; }
        public string Address { get; }
        public string Schedule { get; }

        public ParsedAddress(string office, string address, string schedule)
        {
            Office = office;
            Address = address;
            Schedule = schedule;
        }
    }
}