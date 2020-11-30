using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour
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


    /* Boolean that affects the tracking behavior of the homing shots.
     * ENABLED: The shots will change their trajectory mid flight to a new valid target if one is detected.
     * DISABLED: The shots will choose an inital target and will only travel towards that location. 
     *           If the target was destroyed before reaching the shot reaches it the shot will continue flying along its current trajectory.
    */
    private bool _strictTracking;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _strictTracking = _player.GetComponent<Player>().strictTracking;

        _laserRigidbody = GetComponent<Rigidbody2D>();

        if (_laserTarget != null)
        {
            _laserTargetPos = _laserTarget.transform;
        }

        _laserTarget = _player.GetComponent<Player>()._bestTarget;
    }

    // Update is called once per frame
    void Update()
    {

        if (_strictTracking == true)
        {
            _laserTarget = _player.GetComponent<Player>()._bestTarget;
        }

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
        }

        if (_laserTarget == null)
        {
            Debug.Log("no target, laser");
            _laserRigidbody.AddForce(transform.up * 0.45f, ForceMode2D.Impulse);
        }

        BoundCheck();

    }



    void BoundCheck()
    {
        //Debug.Log("Check Bound");
        if (transform.position.y > 8.0f || transform.position.y < -8.0)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            //Debug.Log("Bound Broken");
            Destroy(gameObject);
        }

        if (transform.position.x > 11.3f || transform.position.x < -11.3f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            //Debug.Log("Bound Broken");
            Destroy(gameObject);
        }
    }


}
