using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour
{

    private GameObject _laserTarget = null;
    private Transform _laserTargetPos = null;
    private Collider2D _targetCollider = null;

    private Rigidbody2D _laserRigidbody;

    [SerializeField]
    private float _speed = 20.0f;

    private Vector3 _dir;

    private float _rotationSpeed = 360f;


    // Start is called before the first frame update
    void Start()
    {

        _laserRigidbody = GetComponent<Rigidbody2D>();

        _laserTarget = GetClosestEnemy();

        if (_laserTarget != null)
        {
            _laserTargetPos = _laserTarget.transform;
            _targetCollider = _laserTarget.GetComponent<Collider2D>();
        }

    }

    void FixedUpdate()
    {

        if (_targetCollider != null)
        {
            Debug.Log("Homing Active");

            _dir = (_laserTargetPos.position - transform.position).normalized;

            float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_dir), Time.deltaTime * _rotationSpeed);

            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);

            _laserRigidbody.velocity = new Vector3(_dir.x * 20, _dir.y * 20, 0);
        }
        else
        {
            Debug.Log("Retrieving new target");
            _laserTarget = GetClosestEnemy();


            if (_laserTarget != null)
            {
                _laserTargetPos = _laserTarget.transform;
                _targetCollider = _laserTarget.GetComponent<Collider2D>();
            }


            Debug.Log("MOVE UP STARTED 1");

            _laserRigidbody.AddForce(transform.up * 0.45f, ForceMode2D.Impulse);
            
            /// Alternative movement methods.///

            //_laserRigidbody.velocity = new Vector3(0, 20, 0);

            //transform.Translate(Vector3.up * _speed * Time.deltaTime);

        }

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


    GameObject GetClosestEnemy()
    {
        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject _player = GameObject.FindGameObjectWithTag("Player");

        GameObject _bestTarget = null;

        if (_enemies == null)
        {
            Debug.Log("No viable targets");
            return _bestTarget;
        }

        if (_player == null)
        {
            return _bestTarget;
        }

        float _closestDistanceSqr = Mathf.Infinity;

        Vector3 currentPosition = _player.transform.position;

        foreach (GameObject potentialTarget in _enemies)
        {
            if (potentialTarget != null)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < _closestDistanceSqr)
                {
                    _closestDistanceSqr = dSqrToTarget;
                    _bestTarget = potentialTarget;
                    Debug.Log("New closest updated");
                }
            }

            if (potentialTarget == null)
            {
                continue;
            }
        }
        return _bestTarget;
    }
}

