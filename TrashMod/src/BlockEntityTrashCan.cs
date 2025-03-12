using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace TrashMod
{
    public class BlockEntityTrashCan : BlockEntity
    {
        private TrashCanInventory inventory;

        public BlockEntityTrashCan()
        {
            inventory = new TrashCanInventory(1, null);
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            inventory.LateInitialize("trashcan-" + Pos, api);
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);
            inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invTree = new TreeAttribute();
            inventory.ToTreeAttributes(invTree);
            tree["inventory"] = invTree;
        }

        public void EmptyTrash()
        {
            inventory[0].Itemstack = null;
            MarkDirty();
        }
    }
}