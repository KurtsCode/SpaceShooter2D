using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [SerializeField]
    public GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    public GameObject[] powerups;
    
    private bool _stopSpawning = false;


    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }


    IEnumerator SpawnEnemyRoutine()
    {
        Debug.Log("Spawning enemy");

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false) 
        {
            Vector3 spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);

            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(5.0f);

        }
            
    }
             


    IEnumerator SpawnPowerupRoutine()
    {
        Debug.Log("Spawning power-up");

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

            int randomPowerUp = Random.Range(0, 3);

            Instantiate(powerups[randomPowerUp], spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(3, 8));
        }

        
    }

    public void OnPlayerDeath()
    {
        Debug.Log("Spawning Stopped");
        _stopSpawning = true;
    }

}
