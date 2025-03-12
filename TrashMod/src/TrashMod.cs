using Vintagestory.API.Common;

namespace TrashMod
{
    public class TrashMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterBlockClass("BlockTrashCan", typeof(BlockTrashCan));
            api.RegisterBlockEntityClass("BlockEntityTrashCan", typeof(BlockEntityTrashCan));
        }
    }
}
