using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeWithAnim : MonoBehaviour
{

    public Animator camAnim;

    public void ActiveAnim()
    {
        camAnim.SetTrigger("Shake");
    }
}
