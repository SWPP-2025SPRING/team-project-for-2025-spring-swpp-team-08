using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void Awake()
    {
        var text = GetComponent<TMP_Text>();

        text.text = $"v{Application.version}";
    }
}
