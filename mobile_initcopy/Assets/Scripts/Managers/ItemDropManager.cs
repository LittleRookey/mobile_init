using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Litkey;
using Litkey.Utility;
using Litkey.InventorySystem;
using DG.Tweening;


public class ItemDropManager : MonoBehaviour
{
    private class ItemWithCount
    {
        public Item item;
        public Vector2Int count;
        public ItemWithCount(Item item, Vector2Int count)
        {
            this.item = item;
            this.count = count;
        }
    }

    public static ItemDropManager Instance;

    public int smallParticlesNum = 5;
    public float magnetSpeed = 3f;
    //public float dropRadius = 2f;
    [SerializeField] private ItemViewer itemViewer;
    [SerializeField] private DropItem dropItemTemplate;

    [SerializeField] private PortionItemData coinData;

    WeightedRandomPicker<Item> randomPicker;

    public static UnityAction<SA_Unit> SetLootTableOnSpawn;
    
    // Manages drop visual items 
    public static UnityAction<SA_Unit> DropItems; 

    
    [Header("Item Data Library")]
    [SerializeField] WeaponItemData[] weaponItemData;
    [SerializeField] ArmorItemData[] armorItemData;
    WeaponItem[] weaponData;
    ArmorItem[] armorData;

    [Header("Item paths")]
    [SerializeField] private string weaponPath = "ScriptableObjects/Items/Equipments";
    [SerializeField] private string armorPath = "ScriptableObjects/Items/Equipments";
    [SerializeField] private string lootTablePath = "ScriptableObjects/LootTable";

    [SerializeField] List<ItemWithCount> droppedItems = new List<ItemWithCount>();
    List<DropItem> dropItemVisuals = new List<DropItem>();

    public Dictionary<string, LootTable> lootLibrary; // Library of all lootTable
    public Dictionary<string, LootTable> lootTable; // loottable that stores the spawned enemy's loottable

    private string coinString = "±ÝÈ­";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        lootLibrary = new Dictionary<string, LootTable>();
        lootTable = new Dictionary<string, LootTable>();

    }
    private void Start()
    {
        PoolManager.WarmPool(dropItemTemplate.gameObject, 10);

        randomPicker = new WeightedRandomPicker<Item>();

        // load armor item datas
        weaponItemData = Resources.LoadAll<WeaponItemData>(weaponPath);
        armorItemData = Resources.LoadAll<ArmorItemData>(armorPath);

        weaponData = new WeaponItem[weaponItemData.Length];
        armorData = new ArmorItem[armorItemData.Length];

        // Load armor items
        for (int i = 0; i < weaponItemData.Length; i++)
        {
            WeaponItem wItem = new WeaponItem(weaponItemData[i]);
            randomPicker.Add(wItem, wItem.Data.Weight);
            weaponData[i] = wItem;
        }

        for (int i = 0; i < armorItemData.Length; i++)
        {
            ArmorItem wItem = new ArmorItem(armorItemData[i]);
            randomPicker.Add(wItem, wItem.Data.Weight);
            armorData[i] = wItem;
        }
        
        // Set loot table for each enemies

        //ShowDictData(randomPicker.GetNormalizedItemDictReadonly());
        SaveLootTables2Library();
        
        ShowDictData(lootLibrary);
    }

    void SaveLootTables2Library()
    {
        LootTable[] loots = Resources.LoadAll<LootTable>(lootTablePath);
        foreach(LootTable lt in loots)
        {
            LootTable nl;
            if(!lootLibrary.TryGetValue(lt._lootID, out nl))
            {
                //for (int i = 0; i < )
                //if ()
                lootLibrary[lt._lootID] = lt;
            } 
        }
    }


    void ShowDictData(System.Collections.ObjectModel.ReadOnlyDictionary<Item, double> d)
    {
        string ret = "";
        foreach(KeyValuePair<Item, double> kv in d)
        {
            ret += kv.Key.Data.Name + ": " + kv.Value * 100 + "%\n";
        }
        Debug.Log(ret);
    }


    void ShowDictData(Dictionary<string, LootTable> d)
    {
        string ret = "";
        foreach (KeyValuePair<string, LootTable> kv in d)
        {
            ret += kv.Key + ":: " + kv.Value+ "%\n";
        }
        Debug.Log(ret);
    }


    LootTable GetLootTableByEnemy(SA_Unit sa)
    {
        SetLootTable(sa);
        return lootTable[sa._Name];
    }


    // 1. On Enemy spawn, check whether enemy is in the dictionary, 
    // 2. if enemy is not in the dict, add loottable based on its name and rarity of enemy
    // 3. if enemy exists in dict, pass
    void SetLootTable(SA_Unit sa)
    {
        LootTable lt;
        if (!lootTable.TryGetValue(sa._Name, out lt))
        {
            lootTable[sa._Name] = lootLibrary[sa._Name];
        }
    }

    // Iterate over LootTable Items and check probability for each item
    // return all items that passed 
    List<ItemWithCount> LootItems(LootTable loot)
    {
        LootTable.ItemDrop[] _lootTable = loot.GetLootTableInfo();
        droppedItems.Clear();

        for (int i = 0; i < _lootTable.Length; i++)
        {
            if (ProbabilityCheck.GetThisChanceResult_Percentage(_lootTable[i].dropRate))
            {
                //Debug.Log("###########################");
                droppedItems.Add(new ItemWithCount(_lootTable[i].item.CreateItem(), _lootTable[i].dropCount));
            }
        }
        return droppedItems;
    }


    void DropVisualItems(List<ItemWithCount> items, Vector3 pos)
    {
        if (items.Count <= 0) return;
        dropItemVisuals.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            DropItem di = PoolManager.SpawnObject(dropItemTemplate.gameObject).GetComponent<DropItem>();
            Debug.Log(items[i].item.Data.Name);
            di.transform.position = pos;
            di.SetDropItem(items[i].item.Data.IconSprite, 
                            items[i].item.Data.Name, 
                            Random.Range(items[i].count.x, items[i].count.y), 
                            items[i].item.Data.rarity, 
                            items[i].item.Data);
            dropItemVisuals.Add(di);
        }


    }

    // OnEnamyDead
    // 1. LootTable lt = GetLootTableByEnemy()
    // 2. List<Item> lootedItems = LootItems(lt);
    // 3. Drop Gold 
    // 4. DropVisualItems(lootedItems);
    public void DropItemsOnEnemyDead(SA_Unit enemy)
    {
        LootTable lt = GetLootTableByEnemy(enemy);
        List<ItemWithCount> temp = LootItems(lt);
        DropVisualItems(temp, enemy.transform.position);
    }



    public void DropCoin(SA_Unit sa)
    {
        DropItem coin = PoolManager.SpawnObject(dropItemTemplate.gameObject).GetComponent<DropItem>();
        coin.SetDropItem(coinData.IconSprite, coinString, sa.dropGold, true);
        coin.transform.position = sa.transform.position;
    }


    public void MoveToPlayer(BounceDue bd)
    {
        SA_Player player = StatManager.Instance._player;

        DOTween.To(() => bd.transform.position, x => bd.transform.position = x, player.transform.position, 1f);
        //bd.transform.position = Vector3.Lerp(bd.transform.position, player.transform.position, magnetSpeed * Time.deltaTime);
        if ((player.transform.position - bd.transform.position).sqrMagnitude <= 0.1f)
        {
            PoolManager.ReleaseObject(bd.gameObject);
        }
    }

    private void OnEnable()
    {
        DropItems += DropCoin;
        DropItems += DropItemsOnEnemyDead;
        Actions.OnCoinDrop += MoveToPlayer;

    }

    private void OnDisable()
    {
        DropItems -= DropItemsOnEnemyDead;
        DropItems -= DropCoin;
        Actions.OnCoinDrop -= MoveToPlayer;
    }


}
