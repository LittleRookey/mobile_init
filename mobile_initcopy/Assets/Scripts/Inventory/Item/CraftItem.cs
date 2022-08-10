

namespace Litkey.InventorySystem
{
    /// <summary> ���� ������ - ���� ������ </summary>
    public class CraftItem : CountableItem
    {
        public CraftItem(CraftItemData data, int amount = 1) : base(data, amount) { }

        public bool Use()
        {
            // �ӽ� : ���� �ϳ� ����
            //Amount--;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new CraftItem(CountableData as CraftItemData, amount);
        }
    }
}
