using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button allConnectedButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("CLIENT");
            NetworkManager.Singleton.StartClient();
            Hide();
        });

        allConnectedButton.onClick.AddListener(() =>
        {
            Debug.Log("ALL CONNECTED");
            Loader.LoadNetwork(Loader.Scene.Game);
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
