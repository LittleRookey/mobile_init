namespace Litkey.InventorySystem
{
    /// <summary> ���� ������ - ���� ������ </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItemData PortionItemData { get; private set; }

        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) 
        {
            PortionItemData = data;
        }

        public bool Use()
        {
            // �ӽ� : ���� �ϳ� ����
            Amount -= 1;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}