using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using System.Collections.Generic;
using System;

namespace TrashMod
{
    public class TrashMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterBlockClass("BlockTrashCan", typeof(BlockTrashCan));
            api.RegisterBlockEntityClass("BlockEntityTrashCan", typeof(BlockEntityTrashCan));

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





