using Vintagestory.API.Common;

[assembly: ModInfo("TrashMod", "trashmod", Version = "1.0.0", Authors = new[] { "Fjre" })]

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