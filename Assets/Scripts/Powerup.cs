using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;

    [SerializeField]
    private int powerupID;

    [SerializeField]
    private AudioClip _clip;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        Debug.Log("Collected Triple Shot");
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        Debug.Log("Collected Speed boost");
                        break;
                    case 2:
                        player.ShieldActive();
                        Debug.Log("Collected Shield");
                        break;
                    case 3:
                        player.RefillAmmo();
                        Debug.Log("Replenished Ammo");
                        break;
                    case 4:
                        player.RefillHealth();
                        Debug.Log("Replenished Ammo");
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}
