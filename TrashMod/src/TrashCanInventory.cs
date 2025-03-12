using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace TrashMod
{
    public class TrashCanInventory : InventoryBase, ISlotProvider
    {
        private ItemSlot[] slots;
        
        public string InventoryClassName => "trashcan";
        public ItemSlot[] Slots => slots;

        public TrashCanInventory(int size, ICoreAPI api) : base("trashcan", api)
        {
            slots = GenEmptySlots(size);
        }

        public override int Count => slots.Length;

        public override ItemSlot this[int slotId]
        {
            get { return slots[slotId]; }
            set { slots[slotId] = value; }
        }

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            slots = SlotsFromTreeAttributes(tree, slots);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }

        protected override ItemSlot NewSlot(int i)
        {
            return new ItemSlotTrashCan(this);
        }

        public override bool CanPlayerAccess(IPlayer player, EntityPos position)
        {
            return base.CanPlayerAccess(player, position);
        }

        public void EmptyTrash()
        {
            Api?.Event.EnqueueMainThreadTask(() => {
                foreach (var slot in slots)
                {
                    slot.Itemstack = null;
                    slot.MarkDirty();
                }
            }, "EmptyTrash");
        }
    }
}