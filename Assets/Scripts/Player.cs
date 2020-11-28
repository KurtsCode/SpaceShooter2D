using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float powerUpActiveTime = 5f;
    private float powerUpDuration = 5f;

    private float speedBoostActiveTime = 5f;
 
    // Player movement values
    [SerializeField]
    private float _speed = 5.0f;
    private float _boostSpeed = 1f;
    private float _thrusterSpeed = 3.0f;
    
    [SerializeField]
    private float _totalSpeed;

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
    private GameObject outOfAmmoSprite;

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
    private bool crActive_TS = false;

    [SerializeField]
    private bool _homeShotActive = false;
    private bool crActive_HS = false;

    [SerializeField]
    private bool _speedBoostActive = false;
    private bool crActive_SB = false;

    [SerializeField]
    private bool _shieldActive = false;
    private bool crActive_SH = false;


    // Shield powerup values.

    [SerializeField]
    private GameObject playerShield;

    [SerializeField]
    private Slider powerUpMeter;

    [SerializeField]
    private Slider speedBoostMeter;

    [SerializeField]
    private Image powerMeterFill;

    private SpriteRenderer _shieldColor;

    private Color _shieldColorBase;

    [SerializeField]
    private int _shieldHealth;

    // Boolean that controls whether the shield lasts permanently or for a limited time.
    private bool _shieldLimit = false;


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

    [SerializeField]
    private GameObject _thrusterSprite;

    [SerializeField]
    private GameObject _thrusterSpriteBig;

    [SerializeField]
    private GameObject _thrusterSpriteSmall;


    private ShakeWithAnim _camShake;

    // Closest Enemy detection
    [SerializeField]
    private Transform _enemyContainer;

    public Transform _bestTarget;

    private Transform _lastBestTarget;

    private float dSqrToTarget;

    private Vector3 directionToTarget;

    /* Boolean that affects the tracking behavior of the homing shots.
    *  ENABLED: The shots will change their trajectory mid flight to a new valid target if one is detected.
    *  DISABLED: The shots will choose an inital target and will only travel towards that location. 
    *            If the target was destroyed before reaching the shot reaches it the shot will continue flying along its current trajectory.
    */
    public bool strictTracking = false;

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

        //_shieldColorBase = _shieldColor.color;
        _shieldColorBase = new Color(1f,1f,1f,.5f);


        // Player control checks and values.
        _hasAmmo = true;
        _sprintActive = false;

        // Establish the starting values for the thruster meter.
        currentFuel = maxFuel;
        thrusterMeter.maxValue = maxFuel;
        thrusterMeter.value = maxFuel;

        //StartCoroutine("GetClosestEnemyCR");

        // Establish the starting values for the powerup meter.
        powerUpMeter.maxValue = powerUpDuration;
        powerUpMeter.value = 0;
        powerUpMeter.gameObject.SetActive(false);

        //Establish the starting values for the speedboost meter.
        speedBoostMeter.maxValue = powerUpDuration;
        speedBoostMeter.value = 0;
        speedBoostMeter.gameObject.SetActive(false);

        _bestTarget = null;
        _lastBestTarget = null; 


        if (_spawnManager == null)
        {
           // Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
           // Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
           // Debug.LogError("AudioSource on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }


    }

    // Update is called once per frame
    void Update()
    {  

        GetClosestEnemy();
        ActivateThruster();
        CalculateMovement();
      

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _hasAmmo == true)
        {
            FireLaser();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _hasAmmo == false)
        {
            StartCoroutine(EmptyAmmoIndicator());
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
            Instantiate(_homingLaserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
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
            // This block activates if the player activates their thrusters while still having thruster fuel.

            burnFuel = StartCoroutine(UseFuel(0.01f, KeyCode.LeftShift));

            
            _thrusterSprite.SetActive(false);
            _thrusterSpriteBig.SetActive(true);

            if (regen != null)
            {
                StopCoroutine(regen);
            }   

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && _thrustActive == true)
        {
            // This block activates if the player deactivates their thrusters while still having fuel left.
            regen = StartCoroutine(RegenFuel());
            

            _thrusterSprite.SetActive(true);
            _thrusterSpriteBig.SetActive(false);

        }

    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

       
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
       
        // If the player burns all of their fuel, start the coroutine to completely refill their fuel after a delay. Additionally it will activate the smaller thruster sprite until the fuel refills.
        if(currentFuel <= 0)
        {
            //Debug.Log("Thrust deactivated");
            _thrustActive = false;
            _thrusterSprite.SetActive(false);
            _thrusterSpriteSmall.SetActive(true);
            _thrusterSpriteBig.SetActive(false);

            if (_compFillActive == false)
            {
                compFill = StartCoroutine(CompRefillFuel());
            }


            if (burnFuel != null)
            {
                StopCoroutine(burnFuel);
            }

        }

        if (currentFuel >= maxFuel)
        {
            //Debug.Log("Thrust reactivated");
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
            _totalSpeed = _speed * _thrusterSpeed * _boostSpeed;

            if(_totalSpeed > 30f)
            {
                _totalSpeed = 30f;
            }

            transform.Translate(direction * _totalSpeed * Time.deltaTime);          
        }
        else
        {
            transform.Translate(direction * (_speed * _boostSpeed) * Time.deltaTime);
            _totalSpeed = _speed * _boostSpeed;
        }
        
        
        // Sets vertical limit to how high on the screen the player character can move. If the playe tries to move higher than the limit their vertical movement is stopped.
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0); 

        // Sets horizontal limits to the player character. If they exceed these bounds, send the player to the opposite side of the screen.
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
        //Debug.Log("Current Lives: " + _lives);
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

        //Debug.Log("Active ID " + activeID);

        if (activeID == 0)
        {
            if (_leftEngine.activeSelf == false)
            {
                //Debug.Log("Active left engine");
                _leftEngine.SetActive(true);
            }
            else
            {
                //Debug.Log("Left engine already on, Active right engine");
                _rightEngine.SetActive(true);
            }

        }

        if (activeID == 1)
        {
            if (_rightEngine.activeSelf == false)
            {
                //Debug.Log("Active right engine");
                _rightEngine.SetActive(true);
            }
            else
            {
                //Debug.Log("Right engine already on, Active left engine");
                _leftEngine.SetActive(true);
            }

        }

    }

    public void ShieldColor(int _shieldLevel)
    {
        
        if(_shieldLevel == 1)
        {
            _shieldColor.color = new Color(1f, 0f, 0f, 0.5f);
        }

        if(_shieldLevel == 2)
        {
            //_shieldColor.color = Color.yellow;
            _shieldColor.color = new Color(1, 0.92f, 0.016f, 0.5f);
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
        //Debug.Log("Current Score: " + _score);
    }

    public void DecreaseAmmo()
    {
        _ammo -= 1;
        _uiManager.UpdateAmmo(_ammo);

        if(_ammo == 0)
        {
            _hasAmmo = false;
        }

        //Debug.Log("Current Score: " + _ammo);
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


    void GetClosestEnemy()
    {

        float _closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform potentialTarget in _enemyContainer)
        {
            if (potentialTarget.GetComponent<Enemy>().exploded != true)
            {
                directionToTarget = potentialTarget.position - currentPosition;
                dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < _closestDistanceSqr)
                {
                    _closestDistanceSqr = dSqrToTarget;
                    _bestTarget = potentialTarget;
                    dSqrToTarget = Mathf.Infinity;
                    Debug.Log("New closest updated" + _bestTarget);
                    Debug.Log("New distance" + dSqrToTarget);
                }

                if (potentialTarget == null)
                {
                    Debug.LogError("Potential Target is null");
                }
            }
            
        }

        
        //Debug.Log("Number of iterations: " + i);
        
        if(_bestTarget != null)
        {
            if (_bestTarget.GetComponent<Enemy>().exploded == true)
            {
                //Debug.Log("Return an existing object");
                _bestTarget = null;
                _closestDistanceSqr = Mathf.Infinity;
                dSqrToTarget = Mathf.Infinity;
                directionToTarget = transform.position;

                Debug.Log("no target, enemy destroyed by laser");
                Debug.Log("Last Best =" + _bestTarget);

            }
        }
        
        if(_bestTarget == null)
        {
            Debug.Log("no target, Player");
        } 

    }



    public void TripleShotActive()
    {
        powerMeterFill.color = Color.green;
        powerUpMeter.gameObject.SetActive(true);

        powerUpActiveTime = powerUpDuration + Time.deltaTime;
        
        if (_homeShotActive == true)
        {
            _homeShotActive = false;
        }

        if(crActive_HS == true)
        {
            StopCoroutine(HomingShotPowerDownRoutine());
            crActive_HS = false;

        }

        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void HomingShotActive()
    {
        powerMeterFill.color = Color.blue;
        powerUpMeter.gameObject.SetActive(true);

        powerUpActiveTime = powerUpDuration + Time.deltaTime;
        
        if (_tripleShotActive == true)
        {
            _tripleShotActive = false;
        }

        if (crActive_TS == true)
        {
            StopCoroutine(HomingShotPowerDownRoutine());
            crActive_TS = false;

        }

        _homeShotActive = true;
        StartCoroutine(HomingShotPowerDownRoutine());
    }

    public void SpeedBoostActive()
    {
        speedBoostActiveTime = powerUpDuration + Time.deltaTime;
        speedBoostMeter.gameObject.SetActive(true);
        _speedBoostActive = true;
        _boostSpeed = 3.0f;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void ShieldActive()
    {
        _shieldActive = true;
        _shieldHealth = 3;
        ShieldColor(_shieldHealth);
        playerShield.SetActive(true);

        if (_shieldLimit)
        {
            StartCoroutine(ShieldPowerDownRoutine());
        }
        
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        crActive_TS = true;

        while (powerUpActiveTime > 0)
        {
            powerUpActiveTime -= Time.deltaTime;

            powerUpMeter.value = powerUpActiveTime;
            //Debug.Log("Power Up Time: " + powerUpTime);
            //Debug.Log("Power Up Delta Time: " + Time.deltaTime);
            yield return null;
        }

        powerUpMeter.gameObject.SetActive(false);
        _tripleShotActive = false;
        crActive_TS = false;
    }

    IEnumerator HomingShotPowerDownRoutine()
    {
        crActive_HS = true;

        while (powerUpActiveTime > 0)
        {
            powerUpActiveTime -= Time.deltaTime;

            powerUpMeter.value = powerUpActiveTime;
            //Debug.Log("Power Up Time: " + powerUpTime);
            //Debug.Log("Power Up Delta Time: " + Time.deltaTime);
            yield return null;
        }

        powerUpMeter.gameObject.SetActive(false);
        _homeShotActive = false;
        crActive_HS = false;
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        while(speedBoostActiveTime > 0)
        {
            speedBoostActiveTime -= Time.deltaTime;

            speedBoostMeter.value = speedBoostActiveTime;

            yield return null;
        }

        speedBoostMeter.gameObject.SetActive(false);
        _speedBoostActive = false;
        _boostSpeed = 1f;
    }

    IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _shieldActive = false;
        playerShield.SetActive(false);
    }

    IEnumerator UseFuel(float delay, KeyCode code)
    {
       // Debug.Log("UseFuel Started");
        while (Input.GetKey(code) && _thrustActive)
        {
            //Debug.Log("Burning fuel");
            currentFuel -= 1;
            thrusterMeter.value = currentFuel;
            yield return new WaitForSeconds(delay);
        }

        burnFuel = null;
    }

    IEnumerator RegenFuel()
    {
       // Debug.Log("Regen active");
        yield return new WaitForSeconds(0.5f);

        while (currentFuel < maxFuel && _thrustActive == true)
        {
            //Debug.Log("Regenning");
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
        //Debug.Log("CompFill active");
        yield return new WaitForSeconds(2f);

        while (currentFuel < maxFuel && _thrustActive == false)
        {
            //Debug.Log("Completely Refilling");
            yield return new WaitForSeconds(0.002f);
            currentFuel += 1;
            thrusterMeter.value = currentFuel;
        }

        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }

        _thrusterSprite.SetActive(true);
        _thrusterSpriteSmall.SetActive(false);
        
        // Deletes regen once the meter hits the max.
        compFill = null;
    }

    IEnumerator EmptyAmmoIndicator()
    {
        outOfAmmoSprite.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        outOfAmmoSprite.SetActive(false);
    }
}
