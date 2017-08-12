using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
  public GameObject startOption;
  public GameObject exitOption;

  public void SetStartOptionSprite(Sprite sprite)
  {
    startOption.GetComponent<Image>().sprite = sprite;
  }

  public void SetExitOptionSprite(Sprite sprite)
  {
    exitOption.GetComponent<Image>().sprite = sprite;
  }

  public void StartGame()
  {
    SceneManager.LoadScene("MainScene");
  }

  public void ExitGame()
  {
    Application.Quit();  
  }
}
