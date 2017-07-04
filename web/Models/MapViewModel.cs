namespace Skeleton.Web.Models
{
    public class MapViewModel : IViewModel
    {
        public string BaseUrl { get; set; }

        public int Depth { get; set; }

        public string ProjectName { get; set; }
        public string AreaName { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Title { get; set; }
    }
}