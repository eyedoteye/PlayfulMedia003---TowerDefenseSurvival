using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
  public GameObject enemyObject;
  public GameObject enemyTarget;

  public float delayBetweenWaves;
  public float[] waveEnemyDelay;
  public int[] waveEnemyCount;

  private int currentWaveEnemyCount;
  private int currentWave;
  private int maxWave;

	void Start()
  {
    IEnumerator spawnEnemies = SpawnEnemies();
    currentWave = 0;
    currentWaveEnemyCount = 0;

    if(waveEnemyDelay.Length != waveEnemyCount.Length)
    {
      Debug.LogError(
        "waveEnemyDelay.Length:(" + waveEnemyDelay.Length +
        ") != waveEnemyCount.Length (" + waveEnemyCount.Length + ")");
    }
    else
    {
      StartCoroutine(spawnEnemies);
    }
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
      currentWaveEnemyCount++;

      if(currentWave != waveEnemyDelay.Length - 1 && currentWaveEnemyCount == waveEnemyCount[currentWave])
      {
        currentWaveEnemyCount = 0;
        currentWave++;
        yield return new WaitForSeconds(delayBetweenWaves);
      }
      else
        yield return new WaitForSeconds(waveEnemyDelay[currentWave]);
    }
  }
}
