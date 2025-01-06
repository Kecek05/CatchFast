using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private GameObject gameStartCountdownPanel;
    [SerializeField] private Animator gameStartCountdownAnimator;


    private void Start()
    {
        GameController.Instance.OnGameStateChanged += GameController_OnGameStateChanged;

        Hide();
    }

    private void GameController_OnGameStateChanged()
    {
        if(GameController.Instance.IsInCountdownToStartState())
        {
            //Start Countdown
            CountdownToStart();
        }
    }


    private void CountdownToStart()
    {
        Show();

    }


    private void Hide()
    {
        gameStartCountdownPanel.SetActive(false);
    }

    private void Show()
    {
        gameStartCountdownPanel.SetActive(true);
    }
}
