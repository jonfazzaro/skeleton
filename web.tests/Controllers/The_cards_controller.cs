namespace Skeleton.Web.Tests.Controllers
{
    using Api;
    using Cards;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [TestFixture]
    public class The_cards_controller
    {
        private static Mock<ICardsClient> _client;
        private static CardsController _controller;

        [Test]
        public void is_an_api_controller()
        {
            Arrange();
            Assert.IsInstanceOf(typeof(ApiController), _controller);
        }

        [TestFixture]
        public class when_getting_cards
        {

            [Test]
            public async Task gets_cards_from_the_client()
            {
                Arrange();
                var expectedCards = new List<Card> { new Card(), new Card() };
                ArrangeGetToReturn(expectedCards);
                var cards = await _controller.Get("ProjectObject");
                Assert.AreEqual(expectedCards, cards);
            }

            [TestFixture]
            public class with_a_specified_depth
            {

                [Test]
                public async Task passes_the_depth_arg()
                {
                    Arrange();
                    ArrangeGetToReturn(new List<Card> { new Card(), new Card() });
                    var cards = await _controller.Get("ProjectObject", depth: 6);
                    _client.Verify(c => c.GetCards("ProjectObject", null, 6));
                }
            }

            [TestFixture]
            public class with_a_specified_area
            {

                [Test]
                public async Task passes_the_area_arg()
                {
                    Arrange();
                    ArrangeGetToReturn(new List<Card> { new Card(), new Card() });
                    var cards = await _controller.Get("ProjectObject", "Waddle");
                    _client.Verify(c => c.GetCards("ProjectObject", "Waddle", 0));
                }
            }
        }

        [TestFixture]
        public class when_putting_cards
        {

            [Test]
            public async Task updates_cards_using_the_client()
            {
                Arrange();
                var cards = new List<Card> { new Card(), new Card() };
                ArrangeVerifiableUpdate(cards);
                await _controller.Put(cards);
                _client.Verify();
            }
        }


        private static void Arrange()
        {
            _client = new Mock<ICardsClient>();
            _controller = new CardsController(_client.Object);
        }

        private static void ArrangeGetToReturn(IEnumerable<Card> expectedCards)
        {
            _client.Setup(c => c.GetCards("ProjectObject", null, 0))
                .Returns(Task.FromResult(expectedCards.AsEnumerable()))
                .Verifiable();
        }

        private static void ArrangeVerifiableUpdate(IEnumerable<Card> expectedCards)
        {
            _client.Setup(c => c.UpdateCards(expectedCards))
                .Returns(Task.FromResult(string.Empty)).Verifiable();
        }
    }
}
