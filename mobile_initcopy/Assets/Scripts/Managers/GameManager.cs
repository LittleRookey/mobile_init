using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    public Dictionary<string, SA_Unit> enemyLibrary;

    [SerializeField] private string enemyPath = "SPUM/SPUM_Units";
    void SaveEnemies()
    {
        SA_Unit[] enemies = Resources.LoadAll<SA_Unit>(enemyPath);

        foreach(SA_Unit sa in enemies)
        {
            SA_Unit cop;
            if (!enemyLibrary.TryGetValue(sa.ID.ToString(), out cop))
            {
                enemyLibrary[sa.ID.ToString()] = sa;
            }
        }

    }

    private void Awake()
    {
        enemyLibrary = new Dictionary<string, SA_Unit>();
        SaveEnemies();
    }

    // Start is called before the first frame update
    void Start()
    {
        _player.gameObject.SetActive(true);    
    }

    

}
