using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellService : MonoBehaviour
{
    [SerializeField] private List<Bell> listBell;
    private void Start()
    {
        foreach(Bell bell in listBell)
        {
            bell.onTrigger = OnTriggerBell;
        }
    }
    private void OnTriggerBell(Bell bell)
    {
        foreach(Bell bell1 in listBell)
        {
            if (bell1 != bell)
            {
                bell1.bellTarget.ThuHut(bell1);
            }
        }
    }
}
