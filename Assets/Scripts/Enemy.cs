using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float _speed;

    [SerializeField]
    private int _points = 10;

    [SerializeField]
    public GameObject _laserPrefab;

    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private float _fireRate = 3.0f;

    [SerializeField]
    private int movementType;

    public bool exploded = false; 

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    private float _canFire;

    private bool _laserActive;

    [SerializeField]
    private float newPos;

    [SerializeField]
    private int angleCheck;


    // Start is called before the first frame update
    void Start()
    {
        float newPos = Random.Range(-8f, 8f);

        _speed = Random.Range(4.0f, 5.0f);

        //transform.position = new Vector3(newPos, 7.0f, 0);

        _player = GameObject.Find("Player").GetComponent<Player>();

        _audioSource = GetComponent<AudioSource>();

        _laserActive = true;

        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if(_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL.");
        }



    }

    // Update is called once per frame
    void Update()
    {
        switch (movementType)
        {
            case 0:
                CalcLineMovement();
                break;
            case 1:
                CalcDiagonalMovement(0);
                break;
            case 2:
                CalcDiagonalMovement(1);
                break;
        }
        //CalculateMovement();

        if (Time.time > _canFire && _laserActive == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

        }
        
    }

    void CalcLineMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            float newPos = Random.Range(-8f, 8f);

            // Respawn at the top with a new random x position.
            transform.position = new Vector3(newPos, 7.0f, 0);
        }
    }




    void CalcDiagonalMovement(int angle)
    {
        angleCheck = angle;

        if (angle == 0)
        {
            // Move diagonally from right to left.
            transform.Translate(new Vector3(-0.75f, -1f, 0) * _speed * Time.deltaTime);
        }
        else if (angle == 1)
        {
            // Move diagonally from left to right.
            transform.Translate(new Vector3(0.75f, -1f, 0) * _speed * Time.deltaTime);
        }
       

        if (transform.position.y < -5.0f)
        {

            movementType = Random.Range(1, 3);

            if (movementType == 1)
            {
                newPos = 8.0f;
            }
            else if (movementType == 2)
            {
                newPos = -8.0f;
            }

            // Respawn at the top with a new random x position.
            transform.position = new Vector3(newPos, Random.Range(5.0f, 8.0f), 0);
        }
    }


    void CalcCurveMovement()
    {

    }

    void CalcSerpentMovement()
    {

    }


    public void SetMovementType(int moveID)
    {
        movementType = moveID;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: " + other.transform.name);

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if(player != null) 
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");

            _speed = 0;
            _audioSource.Play();
            _laserActive = false;

            exploded = true;

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.4f);
        }


        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(_points);
            }

            _anim.SetTrigger("OnEnemyDeath");

            _speed = 0;
            _audioSource.Play();
            _laserActive = false;

            exploded = true;

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject,2.4f);    
        }   
    }

}
