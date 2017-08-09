using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUDController : MonoBehaviour
{
  public PlayerController playerController;
  public Sprite[] healthSprites;

  private Image image; 

	void Start()
  {
    image = GetComponent<Image>();	
	}
	
	void Update()
  {
    int healthSpriteIndex = playerController.health - 1;
    if(healthSpriteIndex < 0)
      healthSpriteIndex = 0;
    else if(healthSpriteIndex > healthSprites.Length)
      healthSpriteIndex = healthSprites.Length - 1;

    image.sprite = healthSprites[healthSpriteIndex];		
	}
}
