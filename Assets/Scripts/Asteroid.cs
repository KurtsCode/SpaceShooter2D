﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 19.0f;

    [SerializeField]
    private GameObject _explosionAnim;

    private Player _player;

    private Spawn_Manager _spawnManager;

    private ShakeWithAnim _camShake;


    // Start is called before the first frame update
    void Start()
    {

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _camShake = GameObject.Find("Main Camera").GetComponent<ShakeWithAnim>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.transform.name);

        if (other.tag == "Laser")
        {
            _camShake.ActiveAnim();

            Instantiate(_explosionAnim, transform.position, Quaternion.identity);
            Destroy(other.gameObject);

            

            _spawnManager.StartSpawning();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 0.25f);
            
            
        }
    }
}
