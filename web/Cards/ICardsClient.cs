using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skeleton.Web.Cards
{
    public interface ICardsClient
    {
        Task<IEnumerable<Card>> GetCards(string projectName, string areaName = null, int depth = 0);
        Task UpdateCards(IEnumerable<Card> cards);
    }
}