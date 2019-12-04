using System.Collections.Generic;

namespace Louis.Model
{
    public class FilmResult
    {
        public List<Item> data { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public List<string> directors { get; set; }
        public List<string> casts { get; set; }
        public string title { get; set; }
        public string rate { get; set; }
        public string cover { get; set; }
        public string url { get; set; }
        public string star { get; set; }
        public int cover_x { get; set; }
        public int cover_y { get; set; }
    }
}