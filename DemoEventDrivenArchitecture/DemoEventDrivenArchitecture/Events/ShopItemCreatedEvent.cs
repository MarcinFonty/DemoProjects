using DemoEventDrivenArchitecture.Entities;

namespace DemoEventDrivenArchitecture.Events
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
