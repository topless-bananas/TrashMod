using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace TrashMod
{
    public class GuiDialogDumpster : GuiDialogBlockEntity
    {
        public override string ToggleKeyCombinationCode => "dumpstergui";

        public GuiDialogDumpster(string DialogTitle, InventoryBase Inventory, BlockPos BlockEntityPosition, ICoreClientAPI capi)
            : base(DialogTitle, Inventory, BlockEntityPosition, capi)
        {
            if (IsDuplicate) return;

            this.capi = capi;
            SetupDialog();
        }

        public void SetupDialog()
        {
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(EnumDialogArea.CenterMiddle);

            ElementBounds titleBounds = ElementBounds.Fixed(20, 20, 200, 30);
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 10, 40, 3, 2);
            ElementBounds emptyButtonBounds = ElementBounds.Fixed(100, 200, 120, 30);

            ClearComposers();

            SingleComposer = capi.Gui.CreateCompo("dumpster" + BlockEntityPosition, dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    .AddStaticText(Lang.Get("trashmod:block-dumpster"), CairoFont.WhiteDetailText(), titleBounds)
                    .AddItemSlotGrid(Inventory, SendInvPacket, 4, slotBounds, "slotGrid")
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
            capi.Logger.Audit("[TrashMod] Dumpster Empty button has been clicked by client. Sending packet to {0}.", capi.World.BlockAccessor.GetBlockEntity(BlockEntityPosition).Block.Code);
            capi.Network.SendBlockEntityPacket(BlockEntityPosition, 1004);
            return true;
        }

        public override void OnGuiOpened()
        {
            base.OnGuiOpened();
        }

        public override void OnGuiClosed()
        {
            SingleComposer.GetSlotGrid("slotGrid").OnGuiClosed(capi);
            base.OnGuiClosed();
        }

        public override void Dispose()
        {
            base.Dispose();
            TryClose();
        }
    }
}