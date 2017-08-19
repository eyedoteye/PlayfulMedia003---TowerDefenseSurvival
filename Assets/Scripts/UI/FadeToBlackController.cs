using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class
FadeToBlackController : MonoBehaviour
{
	public void
  GoToGameoverScene()
  {
    SceneManager.LoadScene("GameoverScene");
	}
}
