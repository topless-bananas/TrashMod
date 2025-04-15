using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace trashmod
{
    public class GuiDialogTrashCan : GuiDialogBlockEntity
    {
        // private BlockEntityTrashCan trashCan;
        public override string ToggleKeyCombinationCode => "trashcangui";
        public GuiDialogTrashCan(string DialogTitle, InventoryBase Inventory, BlockPos BlockEntityPosition, ICoreClientAPI capi) : base(DialogTitle, Inventory, BlockEntityPosition, capi)
        {
            if (IsDuplicate) return;

            this.capi = capi;
            SetupDialog();

            // this.trashCan = trashCan;
        }

        public void SetupDialog()
        {
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            bgBounds.BothSizing = ElementSizing.FitToChildren;

            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog
                .WithAlignment(EnumDialogArea.CenterMiddle);

            ElementBounds titleBounds = ElementBounds.Fixed(20, 20, 200, 30);
            ElementBounds slotBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 130, 60, 1, 1);
            ElementBounds emptyButtonBounds = ElementBounds.Fixed(100, 120, 100, 30);

            ClearComposers();

            SingleComposer = capi.Gui.CreateCompo("trashcan" + BlockEntityPosition, dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar(DialogTitle, OnTitleBarClose)
                .BeginChildElements(bgBounds)
                    .AddStaticText(Lang.Get("trashmod:block-trashcan"), CairoFont.WhiteDetailText(), titleBounds)
                    .AddItemSlotGrid(Inventory, SendInvPacket, 1, slotBounds, "slotGrid")
                    .AddButton("Empty", OnEmptyTrashClick, emptyButtonBounds)
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
            capi.Logger.Audit("[TrashMod] Empty button has been clicked by client. Sending packet to {0}.", capi.World.BlockAccessor.GetBlockEntity(BlockEntityPosition).Block.Code);
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
