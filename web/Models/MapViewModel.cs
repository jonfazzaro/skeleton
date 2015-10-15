namespace Skeleton.Web.Models {
    public class MapViewModel : IViewModel {
        public string ProjectName { get; set; }
        public string BaseUrl { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Title { get; set; }
        public int Depth {
            get;
            set;
        }
    }
}