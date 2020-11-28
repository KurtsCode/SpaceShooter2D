using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _ammoText;

    [SerializeField]
    private Image _LivesImg;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    private GameManager _gameManager;

    private bool restartActive;

    private bool outOfAmmo;

    private int ammo;

    // Start is called before the first frame update
    void Start()
    {
        restartActive = false;
        
        _ammoText.text = "Ammo: " + 15;
        _scoreText.text = "SCORE: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int playerScore)
    {
        _scoreText.text = "SCORE: " + playerScore.ToString();
    }

    public void UpdateAmmo(int ammoCount)
    {
        _ammoText.text = "Ammo: " + ammoCount.ToString();

        ammo = ammoCount;

        if (ammoCount == 0)
        {
            outOfAmmo = true;
            _ammoText.color = Color.red;
            StartCoroutine(AmmoFlickerRoutine());
        }
        else
        {
            outOfAmmo = false;
            _ammoText.color = Color.white;
            StopCoroutine(AmmoFlickerRoutine());
        }
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }

        if(currentLives < 0)
        {
            Debug.LogError("Life total went into the negative");
        }
    }


    public void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);

        _gameManager.GameOver();

        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {

        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
            
            
            /* Alternative method involving activating and deactivating the game over message to casue the flicker effect.
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            */
        }   
    }

    IEnumerator AmmoFlickerRoutine()
    {

        while (outOfAmmo)
        {
            _ammoText.text = "Ammo: " + ammo.ToString();
            yield return new WaitForSeconds(0.25f);
            _ammoText.text = "";
            yield return new WaitForSeconds(0.25f);


            /* Alternative method involving activating and deactivating the game over message to casue the flicker effect.
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            */
        }
    }
}
