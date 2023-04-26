using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {

    public delegate void StartGameDelegate();

    public event StartGameDelegate OnStartGame;

    public delegate void ResetGameDelegate();

    public event ResetGameDelegate OnResetGame;

    [SerializeField]
    private BankContainer bank;

    public bool gameStarted { get; private set; } = false;

    public void AddCoin()
    {
        bank.Add(1);
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;
        OnStartGame?.Invoke();
    }

    public void ResetGame()
    {
        if (!gameStarted) return;
        gameStarted = false;
        OnResetGame?.Invoke();
    }

    public void GameOver()
    {
        UIManager.Instance.ChangeCurrentUi(2);
    }
}