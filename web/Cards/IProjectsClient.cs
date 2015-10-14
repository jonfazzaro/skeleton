namespace Skeleton.Cards {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProjectsClient {
        Task<IEnumerable<string>> GetProjectNames();
        Task<Dictionary<string, string>> GetProjectPriorityFieldNames(IEnumerable<string> projectName);
    }
}
