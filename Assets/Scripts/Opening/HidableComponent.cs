
using System.Collections;

using UnityEngine;

public class HidableComponent: MonoBehaviour
{
    public void HideObj()
    {
        SetObjVisible(false);
    }

    public void UnHideObj()
    {

        SetObjVisible(true);
    }

    private void SetObjVisible(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(var objRenderer in renderers)
        {
            objRenderer.enabled = visible;
        }
    }
}
