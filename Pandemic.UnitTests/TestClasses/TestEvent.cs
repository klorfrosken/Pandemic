using Pandemic.Cards.EventCards;
using Pandemic.Cards;
using Pandemic.Game;


namespace Pandemic.UnitTests.TestClasses
{
    public class TestEvent : EventCard
    {
        public TestEvent() : base("testEvent", "event for testing") { }

        public override void Play(Role roleWithCard)
        {
            roleWithCard.Hand.Remove(this);
            throw new TestException();
        }
    }
}
