namespace Skeleton.Cards {
    public class Card {
        public int Id { get; set; }
        public string Title { get; set; }
        public double? Priority { get; set; }
        public string Type { get; set; }
        public int? FeatureId { get; set; }
        public double? OriginalPriority { get; set; }
        public string Project { get; set; }
    }
}