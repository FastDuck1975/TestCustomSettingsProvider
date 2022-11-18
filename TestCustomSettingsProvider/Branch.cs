namespace TestCustomSettingsProvider
{
    internal class Branch : IComparer<Branch>
    {
        public Branch(string id)
        {
            ID = id;
        }
        public string ID { get; set; }
        public string? Name { get; set; }
        public string? StreetName { get; set; }
        public string? BuildingNumber { get; set; }

        public int Compare(Branch? x, Branch? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }
            else
            {
                return x.ID.CompareTo(y.ID);
            }
        }
    }
}
