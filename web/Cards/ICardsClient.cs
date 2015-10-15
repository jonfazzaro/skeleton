namespace Skeleton.Cards {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICardsClient {
        Task<IEnumerable<Card>> GetCards(string projectName, int depth = 0);
        Task UpdateCards(IEnumerable<Card> cards);
    }
}