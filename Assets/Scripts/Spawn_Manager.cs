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

    private int[] powerUpIds = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5};

    private bool _stopSpawning = false;
    private int selectedID;

    public List<GameObject> enemyList = new List<GameObject>();
    public List<Transform> enemyPosList = new List<Transform>();



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

            StoreEnemyTransform(newEnemy);


            yield return new WaitForSeconds(5.0f);

        }
            
    }
             
    public void StoreEnemyTransform(GameObject enemy)
    {
        Debug.Log("Enemy Position Stored");
        enemyList.Add(enemy);
        enemyPosList.Add(enemy.transform);
    }


    IEnumerator SpawnPowerupRoutine()
    {
        selectedID = powerUpIds[Random.Range(0, powerUpIds.Length)];

        Debug.Log("Spawning power-up");

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

            //int randomPowerUp = Random.Range(0,6);

            int randomPowerUp = powerUpIds[Random.Range(0, powerUpIds.Length)];

            Debug.Log("Power Up Spawned: " + randomPowerUp);

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
