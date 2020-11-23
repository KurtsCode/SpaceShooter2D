using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
 
    // Player movement values
    [SerializeField]
    private float _speed = 3.5f;
    private float _boostSpeed = 15.0f;
    private float _speedMultiplier = 2;

    private bool _sprintActive;


    // Imported prefabs
    [SerializeField]
    public GameObject _laserPrefab;

    [SerializeField]
    public GameObject _homingLaserPrefab;

    [SerializeField]
    public GameObject _tripleShotPrefab;


    // Player attack values
    [SerializeField]
    private float _fireRate = 0.5f;

    private float _canFire = -1f;

    [SerializeField]
    private int _ammo = 15;

    private bool _hasAmmo;

    private int _maxAmmo = 15;

    [SerializeField]
    private int _score;

    
    // Game Controller values
    private Spawn_Manager _spawnManager;
    private UIManager _uiManager;


    // Player damage values
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;



    // Audio controller values.
    [SerializeField]
    private AudioClip _laserSoundClip;

    [SerializeField]
    private AudioClip _explosionSoundClip;

    private AudioSource _audioSource;


    // Power up related values.

    [SerializeField]
    private bool _tripleShotActive = false;

    [SerializeField]
    private bool _homeShotActive = false;

    [SerializeField]
    private bool _speedBoostActive = false;

    [SerializeField]
    private bool _shieldActive = false;

   
    // Shield powerup values.

    [SerializeField]
    private GameObject playerShield;

    private SpriteRenderer _shieldColor;

    private Color _shieldColorBase;

    [SerializeField]
    private int _shieldHealth;


    // Thruster values
    [SerializeField]
    private Slider thrusterMeter;

    private int maxFuel = 100;
    private int currentFuel;

    private Coroutine regen;
    private Coroutine compFill;
    private Coroutine burnFuel;
    private bool _thrustActive = true;
    private bool _compFillActive = false;

    private ShakeWithAnim _camShake;


    // Start is called before the first frame update
    void Start()
    {
        // Snap the player object to the origin point 0,0,0 when the game starts.
        transform.position = new Vector3(0, 0, 0);

        // Find the prefabs for the Spawn and UI managers.
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<Spawn_Manager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _camShake = GameObject.Find("Main Camera").GetComponent<ShakeWithAnim>();

        // Access attached audio source component.
        _audioSource = GetComponent<AudioSource>();

        // Access the Sprite Render fo the shield sprite that is chileded to the player.
        _shieldColor = playerShield.GetComponent<SpriteRenderer>();

        _shieldColorBase = _shieldColor.color;


        // Player control checks and values.
        _hasAmmo = true;
        _sprintActive = false;

        // Establish the starting balues for the thruster meter.
        currentFuel = maxFuel;
        thrusterMeter.maxValue = maxFuel;
        thrusterMeter.value = maxFuel;

        if (_spawnManager == null)
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


        ActivateThruster();
        CalculateMovement();


        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _hasAmmo == true)
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
        else if (_homeShotActive == true)
        {
            Instantiate(_homingLaserPrefab, transform.position, Quaternion.identity);
        }
        else 
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        DecreaseAmmo();

        _audioSource.Play();

    }

    // Function that controls the amount of time a player can use the sprint function.
    void ActivateThruster()
    {

        if(currentFuel <= 0)
        {
            currentFuel = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrustActive == true)
        {
            burnFuel = StartCoroutine(UseFuel(0.01f, KeyCode.LeftShift));

            if (regen != null)
            {
                StopCoroutine(regen);
            }

            

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && _thrustActive == true)
        {
            
            regen = StartCoroutine(RegenFuel());
             
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && _thrustActive == false)
        {
            Debug.Log("Thruster Comp Fill started");
            
            if (_compFillActive == false)
            {
                compFill = StartCoroutine(CompRefillFuel());
            }
            

            if (burnFuel != null)
            {
                StopCoroutine(burnFuel);
            }

            
        }

    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
       

        if(currentFuel <= 0)
        {
            Debug.Log("Thrust deactivated");
            _thrustActive = false;
        }

        if(currentFuel >= maxFuel)
        {
            Debug.Log("Thrust reactivated");
            _thrustActive = true;
            _compFillActive = false;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && _thrustActive == true)
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

        _camShake.ActiveAnim();

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
        
        if(_shieldLevel == 1)
        {
            _shieldColor.color = new Color(1, 0, 0, 1);
        }

        if(_shieldLevel == 2)
        {
            _shieldColor.color = Color.yellow;
        }

        if(_shieldLevel >= 3)
        {
            _shieldColor.color = _shieldColorBase;
        }
    }


    public void AddScore(int points)
    {
        _score += points;
        _uiManager.AddScore(_score);
        Debug.Log("Current Score: " + _score);
    }

    public void DecreaseAmmo()
    {
        _ammo -= 1;
        _uiManager.UpdateAmmo(_ammo);

        if(_ammo == 0)
        {
            _hasAmmo = false;
        }

        Debug.Log("Current Score: " + _ammo);
    }

    public void RefillAmmo()
    {
        _ammo = _maxAmmo;
        _uiManager.UpdateAmmo(_ammo);
        _hasAmmo = true;
    }

    public void RefillHealth()
    {
        if (_lives < 3)
        {
            _lives += 1;
            _uiManager.UpdateLives(_lives);

            if (_leftEngine.activeSelf == true)
            {
                Debug.Log("Active left engine");
                _leftEngine.SetActive(false);
            }
            else
            {
                Debug.Log("Left engine already on, Active right engine");
                _rightEngine.SetActive(false);
            }
        }
  
    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void HomingShotActive()
    {
        _homeShotActive = true;
        StartCoroutine(HomingShotPowerDownRoutine());
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
        ShieldColor(_shieldHealth);
        playerShield.SetActive(true);
        //StartCoroutine(ShieldPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _tripleShotActive = false;
    }

    IEnumerator HomingShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _homeShotActive = false;
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

    IEnumerator UseFuel(float delay, KeyCode code)
    {
        Debug.Log("UseFuel Started");
        while (Input.GetKey(code) && _thrustActive)
        {
            Debug.Log("Burning fuel");
            currentFuel -= 1;
            thrusterMeter.value = currentFuel;
            yield return new WaitForSeconds(delay);
        }

        burnFuel = null;
    }

    IEnumerator RegenFuel()
    {
        Debug.Log("Regen active");
        yield return new WaitForSeconds(0.5f);

        while (currentFuel < maxFuel && _thrustActive == true)
        {
            Debug.Log("Regenning");
            yield return new WaitForSeconds(0.02f);
            currentFuel += 1;
            thrusterMeter.value = currentFuel;
        }

        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }

        // Deletes regen once the meter hits the max.
        regen = null;
    }

    IEnumerator CompRefillFuel()
    {
        _compFillActive = true;
        Debug.Log("CompFill active");
        yield return new WaitForSeconds(2f);

        while (currentFuel < maxFuel && _thrustActive == false)
        {
            Debug.Log("Completely Refilling");
            yield return new WaitForSeconds(0.002f);
            currentFuel += 1;
            thrusterMeter.value = currentFuel;
        }

        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }

        // Deletes regen once the meter hits the max.
        compFill = null;
    }
}
