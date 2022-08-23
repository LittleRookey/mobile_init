namespace Litkey.InventorySystem
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItemData PortionItemData { get; private set; }

        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) 
        {
            PortionItemData = data;
        }

        public bool Use()
        {
            // 임시 : 개수 하나 감소
            Amount -= 1;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}