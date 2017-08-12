using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverScreenController : MonoBehaviour
{
  public GameObject optionSelector;

  private Animator animator;
  private int gameover_animatorProperty_hash = Animator.StringToHash("gameover");

  private void Awake()
  {
    animator = GetComponent<Animator>();
    Debug.Log("awake" + optionSelector);
  }

  private void Start()
  {
    animator.SetTrigger(gameover_animatorProperty_hash);
    Debug.Log("start" + optionSelector);
  }

  public void SelectOption(GameObject option)
  {
    Debug.Log("select: " + optionSelector);

    optionSelector.GetComponent<RectTransform>().position =
      option.GetComponent<RectTransform>().position;
  }

  public void RestartGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void EndGame()
  {
    Debug.Log("Game End");
  }
}
