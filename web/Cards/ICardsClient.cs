namespace Skeleton.Cards {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICardsClient {
        Task<IEnumerable<string>> GetProjectNames();
        Task<IEnumerable<Card>> GetCards(string projectName);
        Task UpdateCards(IEnumerable<Card> cards);
    }
}