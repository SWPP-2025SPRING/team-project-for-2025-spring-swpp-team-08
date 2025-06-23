using UnityEngine;

public class EndingCreditScript : MonoBehaviour
{
    public RectTransform creditText;
    public float scrollSpeed = 30f;
    public float endY;
    public System.Action onCreditEnd;

    private bool _isRolling = false;

    public void StartRoll()
    {
        _isRolling = true;
    }

    void Update()
    {
        if (!_isRolling) return;

        creditText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditText.anchoredPosition.y >= endY)
        {
            _isRolling = false;
            onCreditEnd?.Invoke();
        }
    }
}
