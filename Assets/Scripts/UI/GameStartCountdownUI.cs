using System.Collections;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private GameObject gameStartCountdownPanel;
    [SerializeField] private Animator gameStartCountdownAnimator;
    [SerializeField] private TextMeshProUGUI gameStartCountdownTxt;

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
            StartCoroutine(CountdownToStartCoroutine());
        }
    }

    private IEnumerator CountdownToStartCoroutine()
    {
        Show();
        gameStartCountdownTxt.text = "3";
        gameStartCountdownAnimator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
        gameStartCountdownTxt.text = "2";
        gameStartCountdownAnimator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
        gameStartCountdownTxt.text = "1";
        gameStartCountdownAnimator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1f);
        print("Start Game");
        

        Hide();
        GameController.Instance.ChangeGameState(GameController.GameState.GamePlaying);

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
