namespace TestCustomSettingsProvider
{
    [Serializable]
    internal class Branch
    {
        public Branch(string id)
        {
            ID = id;
        }
        public string ID { get; set; }
        public string? Name { get; set; }
        public string? StreetName { get; set; }
        public string? BuildingNumber { get; set; }
    }
}
