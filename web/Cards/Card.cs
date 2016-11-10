namespace Skeleton.Web.Cards
{
    public class Card
    {
        public int? FeatureId { get; set; }
        public int Id { get; set; }
        public double? OriginalPriority { get; set; }
        public int? ParentId { get; set; }
        public double? Priority { get; set; }
        public string Project { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}