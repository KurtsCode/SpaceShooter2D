using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShot : MonoBehaviour
{
    [SerializeField]
    private Transform _laserTarget = null;

    [SerializeField]
    private Transform _enemyContainer;

    private Transform _laserTargetPos = null;

    [SerializeField]
    private Collider2D _targetCollider = null;

    private Rigidbody2D _laserRigidbody;

    [SerializeField]
    private float _speed = 20.0f;

    private Vector3 _dir;

    private float _rotationSpeed = 360f;

    private bool _gettingEnemy;


    // Take time difference between when the coroutine starts and ends to see how much time passes between the coroutines.


    // Start is called before the first frame update
    void Start()
    {

        _laserRigidbody = GetComponent<Rigidbody2D>();

        _enemyContainer = GameObject.Find("Enemy_Container").transform;

        Debug.Log("Retrieving new target 1");
        StartCoroutine("GetClosestEnemy");
        _gettingEnemy = true;

        if (_laserTarget != null)
        {
            _laserTargetPos = _laserTarget.transform;
        }

    }

    void Update()
    {

        if (_laserTarget != null)
        {
            


            if (_laserTarget.GetComponent<Enemy>().exploded != true)
            {

                _laserTargetPos = _laserTarget.transform;

                Debug.Log("Homing Active");

                _dir = (_laserTargetPos.position - transform.position).normalized;

                float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_dir), Time.deltaTime * _rotationSpeed);

                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);

                _laserRigidbody.velocity = new Vector3(_dir.x * 20, _dir.y * 20, 0);
            }
            else
            {
                if (_gettingEnemy == false)
                {
                    Debug.Log("Retrieving new target 2");
                    StartCoroutine("GetClosestEnemy");
                    _gettingEnemy = true;
                }
            }
        }
        else
        {

            if (_gettingEnemy == false)
            {
                Debug.Log("Retrieving new target 3");
                StartCoroutine("GetClosestEnemy");
                _gettingEnemy = true;
            }
        }

        Debug.Log("MOVE UP STARTED 1");

        _laserRigidbody.AddForce(transform.up * 0.45f, ForceMode2D.Impulse);

        BoundCheck();

    }

    void BoundCheck()
    {
        Debug.Log("Check Bound");
        if (transform.position.y > 8.0f || transform.position.y < -8.0)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Debug.Log("Bound Broken");
            Destroy(gameObject);
        }

        if (transform.position.x > 11.3f || transform.position.x < -11.3f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Debug.Log("Bound Broken");
            Destroy(gameObject);
        }
    }


    IEnumerator GetClosestEnemy()
    {
        Debug.Log("Coroutine Started");
        //GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        Transform _bestTarget = null;


        while(_bestTarget == null)
        {

            // float _closestDistanceSqr = Mathf.Infinity;

            float _closestDistanceSqr = 1000.0f;
            Vector3 currentPosition = _player.transform.position;

            int i = 0;
            foreach (Transform potentialTarget in _enemyContainer)
            {
                i++;
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < _closestDistanceSqr)
                {
                    _closestDistanceSqr = dSqrToTarget;
                    _bestTarget = potentialTarget;
                    //Debug.Log("New closest updated" + potentialTarget);
                }


                if (potentialTarget == null)
                {
                    //continue;
                    //Debug.LogError("Potential Target is null");
                }
            }

            //Debug.Log("Number of iterations: " + i);

            if (_bestTarget.GetComponent<Collider2D>() == null)
            {
                //Debug.Log("Return an existing object");
                _bestTarget = null;
            }
           
            yield return null;
        }

        _laserTarget = _bestTarget;
        _laserTarget.GetComponent<SpriteRenderer>().material.color = Color.red;
        _gettingEnemy = false;

        Debug.Log("Coroutine end");
    }

    
}

