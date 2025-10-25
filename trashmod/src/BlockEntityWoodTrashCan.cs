using System;
using trashmod;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace TrashMod
{
    public class BlockEntityWoodTrashCan : BlockEntityOpenableContainer
    {
        private InventoryGeneric inventory;
        public override InventoryBase Inventory => inventory;
        public ItemSlot TrashSlot => Inventory[0];
        public override string InventoryClassName => "wood-trashcan";
        private GuiDialogTrashCan ClientDialog;

        public BlockEntityWoodTrashCan()
        {
            inventory = new InventoryGeneric(6, null, this.Api);
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

                TrashSlot.Itemstack = null;
                TrashSlot.MarkDirty();
                Api.Logger.Audit("[TrashMod] TrashSlot has been emptied.");
            }
            Api.Logger.Audit("[TrashMod] Wood TrashCan inventory emptied.");
        }

        public override bool OnPlayerRightClick(IPlayer byPlayer, BlockSelection blockSel)
        {
            if (blockSel.SelectionBoxIndex == 1) return false;

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
            ClientDialog?.ReloadDialog();
        }

        public void ToggleTrashCanGui(IPlayer player)
        {
            if (Api.Side != EnumAppSide.Client) return;

            if (ClientDialog == null)
            {
                ICoreClientAPI capi = Api as ICoreClientAPI;
                ClientDialog = new GuiDialogTrashCan(Lang.Get("trashmod:block-wood-trashcan"), Inventory, Pos, capi);
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