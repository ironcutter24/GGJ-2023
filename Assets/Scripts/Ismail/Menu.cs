using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private Vector3 originalScale;
    [SerializeField]
    private GameObject ScaleText;
    [SerializeField]
    private string NextScene;
    private Vector3 scaleTo;
    Sequence mySequence;
    private void Start()
    {
        originalScale = ScaleText.transform.localScale;
        scaleTo = originalScale * 1.1f;
        mySequence = DOTween.Sequence();
        mySequence.Append(ScaleText.transform.DOScale(scaleTo, 1.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(int.MaxValue, LoopType.Yoyo));
    }
    void OnScale()
    {
        ScaleText.transform.DOScale(scaleTo, 1.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(NextScene);
     
        mySequence.Kill();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
