using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.magnitude > 75.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        HardEnemyController he = other.collider.GetComponent<HardEnemyController>();
        ToughRobotScript te = other.collider.GetComponent<ToughRobotScript>();
        if (e != null)
        {
            e.Fix();
        }

        if (he != null)
        {
            he.Fix();
        }

        if (te != null)
        {
            te.Fix();
        }
        
        Destroy(gameObject);
    }
}
