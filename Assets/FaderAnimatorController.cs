using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderAnimatorController : MonoBehaviourSingleton<FaderAnimatorController>
{
    public UIManager.UI_STATE state;

    //[SerializeField]
    private Animator faderAnimator;

    private void Awake()
    {
        faderAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        //GameManager.Instance.OnStartGame += FadeIn;
        GameManager.Instance.OnResetGame += FadeIn;
        if (state == UIManager.UI_STATE.Main) SwipeManager.Instance.ClickEvent += (Vector3) => FadeOut();
    }

    public void FadeIn()
    {
        //Time.timeScale = 1f;
        if (faderAnimator.GetBool("IsFading")) return;
        faderAnimator.SetTrigger("Fade_In");
        IsFading(1);
        //faderAnimator.SetBool("IsFading", true);
    }
    public void FadeOut()
    {
        //Time.timeScale = 1f;
        if (faderAnimator.GetBool("IsFading")) return;
        faderAnimator.SetTrigger("Fade_Out");
        IsFading(1);
        //faderAnimator.SetBool("IsFading", true);
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
        IsFading(0);
        //faderAnimator.SetBool("IsFading", false);
    }

    public void IsFading(int value)
    {
        faderAnimator.SetBool("IsFading", value != 0);
    }
}
