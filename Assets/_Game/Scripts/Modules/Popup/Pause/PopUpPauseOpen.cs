public class PopUpPauseOpen : BaseOpenPanel
{
    protected override void Open()
    {
        PanelManager.Show<PopUpPause>();
    }
}
