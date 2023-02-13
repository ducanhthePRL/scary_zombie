public class PopupInventoryOpener : BaseOpenPanel
{
    protected override void Open()
    {
        PanelManager.Show<PopupInventory>();
    }
}
