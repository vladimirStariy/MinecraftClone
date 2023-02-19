public class SlotItem
{
    public Item Item {get; private set;}
    public int Amount {get; set;}
    public SlotItem(Item item, int amount)
    {
        Item = item;
        Amount = amount; 
    }
}
