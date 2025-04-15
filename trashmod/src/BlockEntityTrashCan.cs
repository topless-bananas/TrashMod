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
    public class BlockTrashCan : Block
    {
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
        }
    }

    public class BlockEntityTrashCan : BlockEntityOpenableContainer
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
        public override string InventoryClassName => "trashcan";
        private GuiDialogTrashCan ClientDialog;

        public BlockEntityTrashCan()
        {
            inventory = new InventoryGeneric(1, null, this.Api);
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
        /// <summary>
        /// Nulls the TrashSlot and notifies the Inventory. Be sure to MarkDirty after using!
        /// </summary>
        public void EmptyTrash()
        {
            if (!TrashSlot.Empty)
            {
                TrashSlot.Itemstack = null;
                TrashSlot.MarkDirty();
                Api.Logger.Audit("[TrashMod] TrashSlot has been emptied.");
            }
        }
        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel.SelectionBoxIndex == 1) return false;

            // Setup the GUI for the client
            if (Api.Side == EnumAppSide.Client)
                ToggleTrashCanGui(byPlayer);
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
                Api?.Logger?.Audit("[TrashMod] TrashCan received GUI packed from player. {0} clicked the Empty button.", fromPlayer.PlayerName);
                EmptyTrash();
                MarkDirty();
            }

        }
        public override void OnReceivedServerPacket(int packetid, byte[] data)
        {
            base.OnReceivedServerPacket(packetid, data);

            // Force the GUI closed if the server says so
            if (packetid == (int)EnumBlockEntityPacketId.Close)
            {
                (Api.World as IClientWorldAccessor).Player.InventoryManager.CloseInventory(Inventory);
                ClientDialog?.TryClose();
                ClientDialog?.Dispose();
                ClientDialog = null;
            }
        }
        /// <summary>
        /// Called when a Client right clicks the BlockEntity.
        /// </summary>
        /// <param name="player"></param>
        public void ToggleTrashCanGui(IPlayer player)
        {
            if (Api.Side != EnumAppSide.Client) return;

            // Create a new GUI if one does not exist
            if (ClientDialog == null)
            {
                ICoreClientAPI capi = Api as ICoreClientAPI;
                ClientDialog = new GuiDialogTrashCan(Lang.Get("trashmod:block-trashcan"), Inventory, this.Pos, capi);
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
            
            // We force the GUI closed to stop shenanigans after the Block is broken.
            ClientDialog?.TryClose();
            ClientDialog?.Dispose();
            ClientDialog = null;
        }
    }
}
