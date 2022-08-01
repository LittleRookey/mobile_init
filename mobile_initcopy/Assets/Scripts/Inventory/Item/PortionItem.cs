namespace Litkey.InventorySystem
{
    /// <summary> ���� ������ - ���� ������ </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            // �ӽ� : ���� �ϳ� ����
            Amount--;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}