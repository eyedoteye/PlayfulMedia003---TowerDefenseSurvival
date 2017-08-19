using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
  public GameObject enemyObject;
  public GameObject enemyTarget;

	void Start()
  {
    IEnumerator spawnEnemies = SpawnEnemies();
    StartCoroutine(spawnEnemies);
  }

  IEnumerator SpawnEnemies()
  {
    for(;;)
    {
      GameObject enemyInstance = Instantiate<GameObject>(
        enemyObject,
        transform.position + enemyObject.transform.position,
        enemyObject.transform.rotation);
      EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();
      enemyController.attackTarget = enemyTarget;
      enemyInstance.SetActive(true);

      yield return new WaitForSeconds(5f);
    }
  }
}
