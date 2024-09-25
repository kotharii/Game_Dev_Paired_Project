using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SweetBurst/Item")]
public sealed class Item : ScriptableObject
{
    public int value;  // this is the value of one of the item, so if we for instance the vlaue is 3 and we match 3 items then we will get 9 points.

    public Sprite sprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
