using System.Collections;
using UnityEngine;

public class OpeningSequence : MonoBehaviour
{
    public GameObject roomObj;
    public GameObject realPlayerObj;
    public GameObject fakePlayerBodyObj;
    public GameObject fakePlayerBallObj;
    public ImageUIFadeInFadeOut stage1Screen;
    public ImageUIFadeInFadeOut stage2Screen;
    public ImageUIFadeInFadeOut stage3Screen;

    private float _subtitleDuration = 3f;

    public IEnumerator PlaySequence()
    {
        SetObjVisible(realPlayerObj, false);
        fakePlayerBodyObj.SetActive(true);

        // 플레이어 첨에는 서있음

        // yield return new WaitForSeconds(1f);
        //
        // yield return PrintPlayerLine("합격이다!!!");
        // yield return PrintSubtitle("당신은 수년간 고대하고 고대하던\n서울대학교에 합격하였습니다.");
        // yield return PrintSubtitle("이제 학업에서 해방인 줄 알았죠.");
        // yield return PrintSubtitle("그러나 몰랐습니다. 앞으로 닥칠 미래를...");
        //
        // yield return GameManager.Instance.InitiateTransition();
         roomObj.SetActive(false);
        //
        // fakePlayerBodyObj.GetComponent<Animator>().SetTrigger("StandupToIdleTrig");
        //
        // fakePlayerBallObj.SetActive(true);
        // yield return PrintSubtitle("당신은 구체로 형상화된 대학생이라는 틀에 갇히게 되었습니다.");
        // StartCoroutine(MoveOverTime(fakePlayerBallObj, fakePlayerBodyObj.transform.position, 4f));
        //
        // yield return PrintSubtitle("앞으로 열심히 구르며 학업을 이어가야 하죠.");
        //
        // StartCoroutine(stage1Screen.FadeInOut());
        // yield return PrintSubtitle("때로는 학업의 길을 따라 요리조리 움직이기도 하고");
        //
        // StartCoroutine(stage2Screen.FadeInOut());
        // yield return PrintSubtitle("때로는 문제를 부수고 앞으로 나아가야 하죠.");
        //
        // StartCoroutine(stage3Screen.FadeInOut());
        // yield return PrintSubtitle("예측하기 어려운 이슈들을 감수하고\n도전하는 경험도 하게 될 것입니다.");
        //
        // yield return PrintSubtitle("돌아가고 싶어도 늦었습니다.");
        // yield return PrintSubtitle("이제 당신은 구를 일만 남았습니다.");

        SetObjVisible(realPlayerObj, true);
        fakePlayerBodyObj.SetActive(false);
        fakePlayerBallObj.SetActive(false);

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PrintPlayerLine(string content)
    {
        GetPlayManager().UpdatePlayerLineSubtitle(content, _subtitleDuration);
        yield return new WaitForSeconds(_subtitleDuration);
    }

    private IEnumerator PrintSubtitle(string content)
    {
        GetPlayManager().UpdateStorySubtitle(content, _subtitleDuration);
        yield return new WaitForSeconds(_subtitleDuration);
    }

    private void PlayAnimation()
    {
    }

    private void SetObjVisible(GameObject obj, bool visible)
    {
        Renderer[] objRenders = obj.GetComponentsInChildren<Renderer>();
        foreach(var objRender in objRenders)
        {
            objRender.enabled = visible;
        }
    }

    private IEnumerator MoveOverTime(GameObject obj, Vector3 targetPosition, float duration)
    {
        Transform objTransform = obj.transform;
        Vector3 start = objTransform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            objTransform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        objTransform.position = targetPosition;
    }

    private PlayManager GetPlayManager()
    {
        return GameManager.Instance.playManager;
    }
}
