using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;

    void Start()
    {
        playButton.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneLoader.Scene.S04RoomScene));
    }
}