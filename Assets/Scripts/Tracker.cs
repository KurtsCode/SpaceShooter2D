using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{

    [SerializeField]
    private Transform _laserTarget = null;


    private Transform _laserTargetPos = null;

    [SerializeField]
    private Collider2D _targetCollider = null;

    private Rigidbody2D _laserRigidbody;

    [SerializeField]
    private float _speed = 20.0f;

    private Vector3 _dir;

    private float _rotationSpeed = 360f;

    GameObject _player;

    private Vector3 startPos;




    // Start is called before the first frame update
    void Start()
    {

        startPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _laserTarget = _player.GetComponent<Player>()._bestTarget;


        if(_laserTarget != null && _laserTarget.GetComponent<Enemy>().exploded != true)
        {
            transform.position = _laserTarget.position;
        }
        else if(_laserTarget.GetComponent<Enemy>().exploded == true)
        {
            _laserTarget = _player.GetComponent<Player>()._bestTarget;
            Debug.Log("No target, tracker");
            transform.position = startPos;
            
        }
        
        if(_laserTarget == null)
        {
            Debug.Log("No target");
        }

    }
}
