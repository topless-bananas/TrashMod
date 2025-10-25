using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace TrashMod
{
    public class GuiDialogWoodTrashCan : GuiDialogBlockEntity
    {
        private InventoryBase Inventory;
        private BlockPos BlockEntityPosition;

        public override string ToggleKeyCombinationCode => "woodtrashcangui";

        public GuiDialogWoodTrashCan(string dialogTitle, InventoryBase inventory, BlockPos blockPos, ICoreClientAPI capi)
            : base(dialogTitle, inventory, blockPos, capi)
        {
            if (IsDuplicate) return;

            this.Inventory = inventory;
            this.BlockEntityPosition = blockPos;
            SetupDialog();
        }

        public void SetupDialog()
        {
            // Background bounds
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            // Main dialog bounds
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);

            // Title
            ElementBounds titleBounds = ElementBounds.Fixed(20, 20, 200, 30);

            // Slot grid bounds
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0, 40, 3, 2); // 3x2 slots

            // Empty button bounds
            ElementBounds emptyButtonBounds = ElementBounds.Fixed(100, 140, 100, 30);

            ClearComposers();

            SingleComposer = capi.Gui.CreateCompo("woodtrashcan" + BlockEntityPosition, dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    .AddStaticText(Lang.Get("trashmod:block-wood-trashcan"), CairoFont.WhiteDetailText(), titleBounds)
                    .AddItemSlotGrid(Inventory, SendInvPacket, 3, slotBounds, "slotGrid")
                    .AddButton(Lang.Get("trashmod:btn-empty"), OnEmptyTrashClick, emptyButtonBounds)
                .EndChildElements()
                .Compose();
        }

        public void ReloadDialog()
        {
            SingleComposer.ReCompose();
        }

        private void OnTitleBarClose()
        {
            TryClose();
        }

        private void SendInvPacket(object packet)
        {
            capi.Network.SendBlockEntityPacket(BlockEntityPosition.X, BlockEntityPosition.Y, BlockEntityPosition.Z, packet);
        }

        private bool OnEmptyTrashClick()
        {
            capi.Gui.PlaySound("toggleswitch");
            capi.Logger.Audit("[TrashMod] Empty button clicked. Sending packet to {0}.", capi.World.BlockAccessor.GetBlockEntity(BlockEntityPosition)?.Block.Code);
            capi.Network.SendBlockEntityPacket(BlockEntityPosition, 1004);
            return true;
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
        }

        public override void OnGuiClosed()
        {
            SingleComposer.GetSlotGrid("slotGrid")?.OnGuiClosed(capi);
            base.OnGuiClosed();
        }

        public override void Dispose()
        {
            base.Dispose();
            TryClose();
        }
    }
}