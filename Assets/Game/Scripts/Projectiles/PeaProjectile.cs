using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private int damage = 20;

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            var zombie = other.GetComponent<ZombieBase>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
