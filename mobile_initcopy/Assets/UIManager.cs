using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabInventory = default;
    [SerializeField] private Button _buttonOpenClose = default;
    
    // Start is called before the first frame update
    void Start()
    {
        //var entities = ResourcesExt.LoadDataEntities("InventoryFolder");
        //var inventory = new InventoryOpenCloseObject(_prefabInventory, entities, "First Inventory");
        //_buttonOpenClose.onClick.AddListener(() => inventory.OpenClose());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
