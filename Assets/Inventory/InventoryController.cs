using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public Slot[,] MainSlots {get; private set;}

    public Slot[,] AdditionalSlots {get; private set;}

    public GameObject slotPref;

    [SerializeField]
    private Transform mainSlotGrid;
    [SerializeField]
    private Transform additionalSlotGrid;

    public void Init()
    {
        InitTestInventory();
    }

    private void InitTestInventory()
    {
        MainSlots = new Slot[1, 9];
        AdditionalSlots = new Slot[3, 9];

        CreateSlotPrefabs();

        MainSlots[0, 0].SetItem(new SlotItem(ItemManager.Instance.Items[0], 10));
    }

    private void CreateSlotPrefabs()
    {
        for(int i = 0; i < MainSlots.GetLength(1); i++)
        {
            var slot = Instantiate(slotPref, mainSlotGrid, false);
            MainSlots[0, i] = slot.AddComponent<Slot>();
        }
        for(int i = 0; i < AdditionalSlots.GetLength(0); i++)
        {
            for(int k = 0; k < AdditionalSlots.GetLength(1); k++)
            {
                var slot = Instantiate(slotPref, additionalSlotGrid, false);
                AdditionalSlots[i, k] = slot.AddComponent<Slot>();
            }
        }
    }
}
