using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHintOpener : BaseOpenPanel
{
    protected override void Open()
    {
        PanelManager.Show<PopupHint>();
    }
}
