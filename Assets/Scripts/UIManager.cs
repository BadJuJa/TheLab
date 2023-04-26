using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviourSingleton<UIManager> {

    public enum UI_STATE {
        Main,
        Gameplay,
        Pause,
        Gameover,
    }

    public UI_STATE state { get; private set; }

    #region GO

    [SerializeField]
    private GameObject pause;

    [SerializeField]
    private GameObject gameplay;

    [SerializeField]
    private GameObject gameOver;

    [SerializeField]
    private BankContainer bank;

    [SerializeField]
    private List<TextMeshProUGUI> crystalScreens;

    #endregion GO

    private List<GameObject> all_ui;

    private void Start()
    {
        state = UI_STATE.Gameplay;
        all_ui = new() { gameplay, pause, gameOver };

        bank.OnCrystalsChanged += UpdateCrystals;
        GameManager.Instance.OnStartGame += () => ChangeCurrentUi(0);
    }

    public void ChangeCurrentUi(int index)
    {
        all_ui[(int)state].SetActive(false);

        if (index == 1 || index == 2) Time.timeScale = 0;
        else Time.timeScale = 1;

        all_ui[index].SetActive(true);
        state = (UI_STATE)index;
    }

    public void ResetGame()
    {
        GameManager.Instance.ResetGame();
        FaderAnimatorController.Instance.FadeIn();
        ChangeCurrentUi(0);
    }

    private void UpdateCrystals(int value)
    {
        if (crystalScreens == null) return;
        foreach (TextMeshProUGUI text in crystalScreens)
        {
            text.SetText(value.ToString());
        }
    }

    public void ExitToMain()
    {
        SceneManager.LoadScene(0);
    }
}