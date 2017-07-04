using Skeleton.Web.Cards;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Skeleton.Web.Api
{
    [RoutePrefix("api/projects/{projectName}/{areaName}/cards")]
    public class CardsController : ApiController
    {
        private readonly ICardsClient _client;

        public CardsController(ICardsClient client)
        {
            _client = client;
        }

        [Route("")]
        public async Task<IEnumerable<Card>> Get(string projectName, string areaName = null, int depth = 0)
        {
            return await _client.GetCards(projectName, areaName, depth);
        }

        [Route("")]
        public async Task Put(IEnumerable<Card> cards)
        {
            await _client.UpdateCards(cards);
        }
    }
}