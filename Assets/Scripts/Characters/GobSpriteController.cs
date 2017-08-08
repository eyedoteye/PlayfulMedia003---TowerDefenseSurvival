using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobSpriteController : MonoBehaviour
{
  public Sprite[] sprites;
  public SpriteRenderer spriteRenderer;

  private void Start()
  {
    int spriteChoice = (int)(Random.value * sprites.Length);
    spriteRenderer.sprite = sprites[spriteChoice];
  }

  private void OnTriggerEnter(Collider other)
  {
    PlayerController playerController = other.GetComponent<PlayerController>();
    if(playerController != null)
    {
      playerController.gobs++;
      Destroy(gameObject);
    }
  }
}
