using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderAnimatorController : MonoBehaviourSingleton<FaderAnimatorController> {
    public UIManager.UI_STATE state;

    private Animator faderAnimator;

    private void Awake()
    {
        faderAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnResetGame += FadeIn;
        if (state == UIManager.UI_STATE.Main) SwipeManager.Instance.ClickEvent += (v) => FadeOut();
    }

    public void FadeIn()
    {
        if (faderAnimator.GetBool("IsFading")) return;
        faderAnimator.SetTrigger("Fade_In");
    }

    public void FadeOut()
    {
        if (faderAnimator.GetBool("IsFading")) return;
        faderAnimator.SetTrigger("Fade_Out");
    }

    public void FadeTo()
    {
        if (state == UIManager.UI_STATE.Main)
        {
            SceneManager.LoadScene(1);
        }
        else if (state == UIManager.UI_STATE.Gameplay)
        {
            UIManager.Instance.ResetGame();
        }
    }

    public void IsFading(int value)
    {
        faderAnimator.SetBool("IsFading", value != 0);
    }
}