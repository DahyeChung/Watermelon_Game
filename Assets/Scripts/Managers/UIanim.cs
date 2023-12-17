using UnityEngine;

public class UIanim : MonoBehaviour
{
    [SerializeField]
    GameObject gameOver;
    [SerializeField]
    GameObject starScale;
    [SerializeField]
    GameObject backGroundMove;


    [SerializeField]
    private float speed;

    public void ScaleAnim(GameObject obj, float scaleSize)
    {
        LeanTween.cancel(obj);
        gameObject.transform.localScale = Vector2.one;
        LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 2f).setEasePunch();
        //LeanTween.scale(obj, new Vector2(scaleSize, scaleSize), 2f).setEase(LeanTweenType.easeOutElastic);
    }

    public void GameOverUI()
    {
        LeanTween.moveLocal(gameOver, new Vector3(0f, -12f, 0f), 0.4f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.moveLocal(backGroundMove, new Vector3(0f, 2f, 0f), 0.4f).setEase(LeanTweenType.easeOutBounce);
        LeanTween.scale(starScale, new Vector3(100f, 100f, 100f), 0.2f).setEase(LeanTweenType.easeOutBounce);
    }

}
