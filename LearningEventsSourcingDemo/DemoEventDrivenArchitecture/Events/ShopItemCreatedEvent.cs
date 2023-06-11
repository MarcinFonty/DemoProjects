using LearningEventsSourcingDemo.Entities;

namespace LearningEventsSourcingDemo.Events
{
    public class ShopItemCreatedEvent
    {
        public ShopItemCreatedEvent(ShopItem shopItem)
        {
            ShopItem = shopItem;
        }

        public ShopItem ShopItem { get; }
    }
}
