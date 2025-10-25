using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;

namespace TrashMod
{
    public class TrashMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            // Register TrashCan
            api.RegisterBlockClass("BlockTrashCan", typeof(BlockTrashCan));
            api.RegisterBlockEntityClass("BlockEntityTrashCan", typeof(BlockEntityTrashCan));

            // Register Dumpster
            api.RegisterBlockClass("BlockDumpster", typeof(BlockDumpster));
            api.RegisterBlockEntityClass("BlockEntityDumpster", typeof(BlockEntityDumpster));

            // Register TrashBin
            api.RegisterBlockClass("BlockTrashBin", typeof(BlockTrashBin));
            api.RegisterBlockEntityClass("BlockEntityTrashBin", typeof(BlockEntityTrashBin));

            api.Logger.Notification("[TrashMod] TrashMod initialized on " + api.Side);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Logger.Notification("[TrashMod] TrashMod server-side initialized.");
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            api.Logger.Notification("[TrashMod] TrashMod client-side initialized.");
        }
    }
}

