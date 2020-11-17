using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
 
    [SerializeField]
    private float _speed = 3.5f;
    private float _boostSpeed = 15.0f;
    private float _speedMultiplier = 2;

    [SerializeField]
    public GameObject _laserPrefab;

    [SerializeField]
    public GameObject _tripleShotPrefab;

    [SerializeField]
    private float _fireRate = 0.5f;

    private float _canFire = -1f;

    [SerializeField]
    private bool _tripleShotActive = false;

    [SerializeField]
    private bool _speedBoostActive = false;

    [SerializeField]
    private bool _shieldActive = false;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private int _score;

    private Spawn_Manager _spawnManager;
    private UIManager _uiManager;

    [SerializeField]
    private GameObject playerShield;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private AudioClip _explosionSoundClip;

    private AudioSource _audioSource;

    private bool _sprintActive;

    [SerializeField]
    private int _shieldHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        // Snap the player object to the origin point 0,0,0 when the game starts.
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _sprintActive = false;

        if(_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }


    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();


        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

       

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
  
        if (_tripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();

    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _sprintActive = true;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            _sprintActive = false;
        }


        if (_sprintActive == true)
        {
            transform.Translate(direction * _boostSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0); 

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void Damage()
    {
        if (_shieldActive == true)
        {
            _shieldHealth -= 1;

            ShieldColor(_shieldHealth);

            if (_shieldHealth == 0)
            {
                _shieldActive = false;
                playerShield.SetActive(false);
                return;
            }

            return;
        }
        
        _lives--;
        Debug.Log("Current Lives: " + _lives);
        _uiManager.UpdateLives(_lives);

        ActiveDamage();

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();

            Destroy(this.gameObject);
        }
    }

    public void ActiveDamage()
    {
        int activeID = Random.Range(0, 2);

        Debug.Log("Active ID " + activeID);

        if (activeID == 0)
        {
            if (_leftEngine.activeSelf == false)
            {
                Debug.Log("Active left engine");
                _leftEngine.SetActive(true);
            }
            else
            {
                Debug.Log("Left engine already on, Active right engine");
                _rightEngine.SetActive(true);
            }

        }

        if (activeID == 1)
        {
            if (_rightEngine.activeSelf == false)
            {
                Debug.Log("Active right engine");
                _rightEngine.SetActive(true);
            }
            else
            {
                Debug.Log("Right engine already on, Active left engine");
                _leftEngine.SetActive(true);
            }

        }

    }

    public void ShieldColor(int _shieldLevel)
    {
        SpriteRenderer _shieldColor = playerShield.GetComponent<SpriteRenderer>();
        
        if(_shieldLevel == 1)
        {
            _shieldColor.color = new Color(1, 0, 0, 1);
        }

        if(_shieldLevel == 2)
        {
            _shieldColor.color = Color.yellow;
        }
    }


    public void AddScore(int points)
    {
        _score = _score + points;
        _uiManager.AddScore(_score);
        Debug.Log("Current Score: " + _score);
    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        _speedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldActive()
    {
        _shieldActive = true;
        _shieldHealth = 3;
        playerShield.SetActive(true);
        //StartCoroutine(ShieldPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _tripleShotActive = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _speedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _shieldActive = false;
        playerShield.SetActive(false);
    }
}
