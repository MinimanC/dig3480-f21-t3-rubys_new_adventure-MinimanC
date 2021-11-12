using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeAmmo(4);
            Destroy(gameObject);

            controller.PlaySound(collectedClip);
            controller.healEffect.Play();
        }
    }
}
