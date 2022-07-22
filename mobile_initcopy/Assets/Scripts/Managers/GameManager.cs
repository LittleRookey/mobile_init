using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;


    // Start is called before the first frame update
    void Start()
    {
        _player.gameObject.SetActive(true);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
