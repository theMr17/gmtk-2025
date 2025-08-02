using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public static SceneLoader Instance { get; private set; }

  public Animator transitionAnimator;
  public float transitionDuration = 1f;

  public enum Scene
  {
    S04RoomScene,
    CorridorScene,
    EntranceHallScene,
  }

  private void Awake()
  {
    Instance = this;
  }

  public void LoadScene(Scene targetScene, bool useTransition = true)
  {
    if (useTransition && transitionAnimator != null)
    {
      StartCoroutine(LoadSceneWithTransition(targetScene));
    }
    else
    {
      SceneManager.LoadScene(targetScene.ToString());
    }
  }

  IEnumerator LoadSceneWithTransition(Scene targetScene)
  {
    transitionAnimator.SetTrigger("Start");
    yield return new WaitForSeconds(transitionDuration);
    SceneManager.LoadScene(targetScene.ToString());
  }
}