namespace Skeleton.Web.Models
{
    public interface IViewModel
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        string Title { get; set; }
    }
}