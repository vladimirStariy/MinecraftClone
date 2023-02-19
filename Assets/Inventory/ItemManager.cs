using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public List<Item> Items;

    public Sprite[] ItemSprites;

    private void Awake() 
    {
        if(!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }

        GenerateItems();
    }

    private void GenerateItems()
    {
        Items = new List<Item>();

        Items.Add(new Item("Dick1", ItemSprites[0]));

        //test dick craft
        var dickRecipe = new Item[,]
        {
            { Items[0] }
        };
    }
}
