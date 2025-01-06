using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerWinTxt;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private GameObject winGamePanelUI;

    private void Awake()
    {
        mainMenuBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });

        Hide();
    }

    private void Start()
    {
        GameController.Instance.OnPlayerWin += GameController_OnPlayerWin;
    }

    private void GameController_OnPlayerWin(int playerWin)
    {
        Show();

        playerWinTxt.text = "Player " + playerWin + " Wins!";
    }



    private void Hide()
    {
        winGamePanelUI.SetActive(false);
    }

    private void Show()
    {
        winGamePanelUI.SetActive(true);
    }
}
