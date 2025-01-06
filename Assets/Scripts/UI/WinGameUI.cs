using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinGameUI : NetworkBehaviour
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

        if(playerWin == 1 && IsServer)
        {
            // server win and in server
            playerWinTxt.text = "YOU WIN!";
        } else if (playerWin == 1 && !IsServer)
        {
            // server win  and not in server
            playerWinTxt.text = "YOU LOSE!";
        } else if (playerWin == 2 && IsServer)
        {
            //client win and in server
            playerWinTxt.text = "YOU LOSE!";
        } else if (playerWin == 2 && !IsServer)
        {
            //client win and not in server
            playerWinTxt.text = "YOU WIN!";
        }

        
        Show();
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
