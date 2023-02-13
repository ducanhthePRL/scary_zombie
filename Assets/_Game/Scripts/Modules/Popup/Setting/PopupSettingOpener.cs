public class PopupSettingOpener : BaseOpenPanel
{
    protected override void Open()
    {
        PanelManager.Show<PopupSetting>();
    }
}
