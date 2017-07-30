using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  public GameObject navigationTarget;

	void Awake()
  {
		
	}
	
	void Update()
  {
    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    agent.destination = navigationTarget.transform.position;
  }
}
