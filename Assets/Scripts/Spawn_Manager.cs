using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{

    [SerializeField]
    public GameObject[] _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    public GameObject[] powerups;

    private Vector3 spawnPos;

    private int[] powerUpIds = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6};
    private int[] enemyIds = { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 };

    // Experimental array to also randomly decide the movement pattern of spawned items. Will not be used on first version of this game.
    private int[] spawnPosIds = {0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4};

    private bool _stopSpawning = false;

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

            int randomEnemyType = enemyIds[Random.Range(0, enemyIds.Length)];
            int randomSpawnType = spawnPosIds[Random.Range(0, spawnPosIds.Length)];


            switch (randomSpawnType)
            {
                case 0:
                    // Spawn a regular enemy that moves in a straight line.
                    spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

                    GameObject newEnemy = Instantiate(_enemyPrefab[0], spawnPos, Quaternion.identity);

                    Enemy enemyCtrl = newEnemy.transform.GetComponent<Enemy>();

                    enemyCtrl.SetMovementType(randomSpawnType);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    StoreEnemyTransform(newEnemy);
                    break;
                case 1:
                    // Spawn a regular enemy that moves in at an angle from right to left.
                    spawnPos = new Vector3(8.0f, Random.Range(5.0f, 8.0f), 0);

                    newEnemy = Instantiate(_enemyPrefab[0], spawnPos, Quaternion.identity);

                    enemyCtrl = newEnemy.transform.GetComponent<Enemy>();

                    enemyCtrl.SetMovementType(randomSpawnType);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    StoreEnemyTransform(newEnemy);

                    break;
                case 2:
                    // Spawn a regular enemy that moves in at an angle from left to right.
                    spawnPos = new Vector3(-8.0f, Random.Range(5.0f, 8.0f), 0);

                    newEnemy = Instantiate(_enemyPrefab[0], spawnPos, Quaternion.identity);

                    enemyCtrl = newEnemy.transform.GetComponent<Enemy>();

                    enemyCtrl.SetMovementType(randomSpawnType);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    StoreEnemyTransform(newEnemy);
                    break;
                case 3:
                    // Spawn a regular enemy that moves in a serpentine motion.
                    spawnPos = new Vector3(Random.Range(-5.0f, 5.0f), 7.0f, 0);

                    newEnemy = Instantiate(_enemyPrefab[0], spawnPos, Quaternion.identity);

                    enemyCtrl = newEnemy.transform.GetComponent<Enemy>();

                    enemyCtrl.SetMovementType(randomSpawnType);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    StoreEnemyTransform(newEnemy);
                    break;
                case 4:
                    // Spawn an cluster enemy that moves in a straight line.
                    spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

                    newEnemy = Instantiate(_enemyPrefab[1], spawnPos, Quaternion.identity);

                    enemyCtrl = newEnemy.transform.GetComponent<Enemy>();

                    enemyCtrl.SetMovementType(0);

                    newEnemy.transform.parent = _enemyContainer.transform;

                    StoreEnemyTransform(newEnemy);
                    break;

            }

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
        Debug.Log("Spawning power-up");

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-8.0f, 8.0f), 7.0f, 0);

            //int randomPowerUp = Random.Range(0,6);

            int randomPowerUp = powerUpIds[Random.Range(18, powerUpIds.Length)];

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
