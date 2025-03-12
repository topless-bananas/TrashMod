using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Client;
using TrashMod;

namespace TrashMod
{
    public class BlockTrashCan : Block
    {
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
        {
            BlockEntityTrashCan be = world.BlockAccessor.GetBlockEntity(blockSel.Position) as BlockEntityTrashCan;
            if (be != null)
            {
                return true;
            }
            return false;
        }

        public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos)
        {
            base.OnBlockRemoved(world, pos);
            BlockEntityTrashCan be = world.BlockAccessor.GetBlockEntity(pos) as BlockEntityTrashCan;
            be?.EmptyTrash();
        }
    }
}
