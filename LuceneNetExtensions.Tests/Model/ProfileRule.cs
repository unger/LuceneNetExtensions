namespace LuceneNetExtensions.Tests.Model
{
    public class ProfileRule
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public string[] Species { get; set; }

        public string[] Municipalities { get; set; }

        public string[] Provinces { get; set; }
    }
}
