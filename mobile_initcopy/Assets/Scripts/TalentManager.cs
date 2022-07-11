using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentManager : MonoBehaviour
{
    public Talent[] talents;

    private void Awake()
    {
        talents = Resources.LoadAll<Talent>("Talents");

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void InitTalent()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
