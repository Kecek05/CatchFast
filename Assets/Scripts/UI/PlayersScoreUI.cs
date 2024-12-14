using TMPro;
using Unity.Netcode;
using UnityEngine;
public class PlayersScoreUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;


    private void Start()
    {
        GameController.Instance.OnPlayer1ScoreChanged += GameController_OnPlayer1ScoreChanged; ;
        GameController.Instance.OnPlayer2ScoreChanged += GameController_OnPlayer2ScoreChanged; ;
    }

    private void GameController_OnPlayer1ScoreChanged(int scorePlayer1)
    {
        if(IsHost)
            player1ScoreText.text = scorePlayer1.ToString();
        else
            player2ScoreText.text = scorePlayer1.ToString();
    }

    private void GameController_OnPlayer2ScoreChanged(int scorePlayer2)
    {
        if (IsHost)
            player2ScoreText.text = scorePlayer2.ToString();
        else
            player1ScoreText.text = scorePlayer2.ToString();
    }
}
