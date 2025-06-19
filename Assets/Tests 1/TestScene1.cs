using System.Collections;

using UnityEngine;

public class TestScene1 : MonoBehaviour
{
    private GameObject userObject;

    void Start()
    {
        userObject = GameObject.FindWithTag("Player");

        if (userObject == null)
        {
            Debug.LogWarning("플레이어 오브젝트를 찾을 수 없습니다. 'Player' 태그가 붙은 오브젝트가 있어야 합니다.");
        }
        else
        {
            Debug.Log($"플레이어 오브젝트를 찾았습니다: {userObject.name}");
        }
    }

    void Update()
    {
        if (userObject == null) return;

        float move = Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
        userObject.transform.Translate(move, 0, 0);
    }
}
