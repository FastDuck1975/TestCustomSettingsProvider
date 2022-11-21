namespace TestCustomSettingsProvider
{
    internal class BranchComparer : IComparer<Branch>
    {
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
