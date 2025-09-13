using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.GameContent;
using Vintagestory.API.Config;
using trashmod;

namespace TrashMod
{
    public class BlockDumpster : Block
    {
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }
    }

    public class BlockEntityDumpster : BlockEntityOpenableContainer
    {
        private InventoryGeneric inventory;
        public override InventoryBase Inventory
        {
            get { return inventory; }
        }
        public ItemSlot TrashSlot
        {
            get { return Inventory[0]; }
        }
        public override string InventoryClassName => "dumpster";
        private GuiDialogDumpster ClientDialog;

        public BlockEntityDumpster()
        {
            inventory = new InventoryGeneric(12, null, this.Api);
            inventory.SlotModified += OnSlotModified;
        }

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);
            inventory.LateInitialize(Block.FirstCodePart() + "-" + Pos.X + "/" + Pos.Y + "/" + Pos.Z, api);
        }

        private void OnSlotModified(int slotid)
        {
            if (Api.Side == EnumAppSide.Server)
            {
                MarkDirty();
            }
        }

        public void EmptyTrash()
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (!Inventory[i].Empty)
                {
                    Inventory[i].Itemstack = null;
                    Inventory[i].MarkDirty();
                }
            }
            Api.Logger.Audit("[TrashMod] Inventory has been emptied.");
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel.SelectionBoxIndex == 1) return false;

            if (Api.Side == EnumAppSide.Client)
                ToggleDumpsterGui(byPlayer);
            return true;
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            base.ToTreeAttributes(tree);
            ITreeAttribute invtree = new TreeAttribute();
            Inventory.ToTreeAttributes(invtree);
            tree["inventory"] = invtree;
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving)
        {
            base.FromTreeAttributes(tree, worldForResolving);

            Inventory.FromTreeAttributes(tree.GetTreeAttribute("inventory"));

            if (ClientDialog != null)
                ClientDialog?.ReloadDialog();
        }

        public override void OnReceivedClientPacket(IPlayer fromPlayer, int packetId, byte[] data)
        {
            base.OnReceivedClientPacket(fromPlayer, packetId, data);

            if (packetId == 1004)
            {
                Api?.Logger?.Audit("[TrashMod] Dumpster received GUI packet from player. {0} clicked the Empty button.", fromPlayer.PlayerName);
                EmptyTrash();
                MarkDirty();
            }
        }

        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            base.OnReceivedServerPacket(packetid, data);

            if (packetid == (int)EnumBlockEntityPacketId.Close)
            {
                (Api.World as IClientWorldAccessor).Player.InventoryManager.CloseInventory(Inventory);
                ClientDialog?.TryClose();
                ClientDialog?.Dispose();
                ClientDialog = null;
            }
        }

        public void ToggleDumpsterGui(IPlayer player)
        {
            if (Api.Side != EnumAppSide.Client) return;

            if (ClientDialog == null)
            {
                ICoreClientAPI capi = Api as ICoreClientAPI;
                ClientDialog = new GuiDialogDumpster(Lang.Get("trashmod:block-dumpster"), Inventory, this.Pos, capi);
                ClientDialog.OnClosed += () =>
                {
                    ClientDialog = null;
                    capi.Network.SendBlockEntityPacket(Pos, (int)EnumBlockEntityPacketId.Close, null);
                    capi.Network.SendPacketClient(Inventory.Close(player));
                };
                ClientDialog.OpenSound = AssetLocation.Create("sounds/block/barrelopen");
                ClientDialog.CloseSound = AssetLocation.Create("sounds/block/barrelclose");
                ClientDialog.TryOpen();
                capi.Network.SendPacketClient(Inventory.Open(player));
                capi.Network.SendBlockEntityPacket(Pos, (int)EnumBlockEntityPacketId.Open, null);
                MarkDirty();
            }
            else
            {
                ClientDialog.TryClose();
            }
        }

        public override void OnBlockBroken(IPlayer byPlayer = null)
        {
            base.OnBlockBroken(byPlayer);
            ClientDialog?.TryClose();
            ClientDialog?.Dispose();
            ClientDialog = null;
        }
    }
}
