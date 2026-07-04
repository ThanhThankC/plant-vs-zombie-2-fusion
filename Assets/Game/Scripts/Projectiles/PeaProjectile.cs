using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private int damage = 20;

    private Vector3 direction = Vector3.right;

    public void Init(Vector3 direction)
    {
        this.direction = direction;
    }

    void Update()
    {
        if (direction == Vector3.left && transform.position.x < GridManager.Instance.GetHousePositionX())
        {
            direction = Vector3.right;
        }

        transform.Translate(direction * speed * Time.deltaTime);
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
