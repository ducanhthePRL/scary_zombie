using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay : BaseOpenPanel
{
    protected override void Open()
    {
        Observer.Instance.Notify(ObserverKey.Replay, null);
    }
}
