using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverScreenController : MonoBehaviour
{
  private Animator animator;

  private int gameover_animatorProperty_hash = Animator.StringToHash("gameover");

  private void Awake()
  {
    animator = GetComponent<Animator>();
  }

  private void Start()
  {
    animator.SetTrigger(gameover_animatorProperty_hash);
    Debug.Log("awake");
  }

  void Update()
  {
		
	}
}
