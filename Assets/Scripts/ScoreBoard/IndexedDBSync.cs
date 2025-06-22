using System.Runtime.InteropServices;
using UnityEngine;

public class IndexedDBSync : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SyncFileSystem();
    #endif

    public static void FlushFileSystem()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        SyncFileSystem();
        #endif
    }
}
