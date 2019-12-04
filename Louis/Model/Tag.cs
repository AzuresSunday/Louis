namespace Louis.Model
{
    public class Tag
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Tag(int id, string key, string value)
        {
            this.Id = id;
            this.Key = key;
            this.Value = value;
        }
    }
}