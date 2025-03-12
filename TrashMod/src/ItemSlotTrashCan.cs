using Vintagestory.API.Common;

namespace TrashMod
{
    public class ItemSlotTrashCan : ItemSlot
    {
        public ItemSlotTrashCan(InventoryBase inventory) : base(inventory)
        {
        }

        public override bool CanTake()
        {
            return false;
        }

        public override bool CanHold(ItemSlot sourceSlot)
        {
            return true;
        }

        public override void OnItemSlotModified(ItemStack stack)
        {
            base.OnItemSlotModified(stack);
        }
    }
}