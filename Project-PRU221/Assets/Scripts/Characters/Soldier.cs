using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    int health;
    protected int maxHealth = 0;
    bool isDead = false;

    float speed = 1.5f;
    bool isBlocked;
    float keepDistance = 2f;

    public int damage = 0;
    bool inCombat;
    protected float attackRange = 0;
    protected float timeBetweenHit = 0;
    float attackCooldown;

    public int cost = 0;

    Collider2D target;

    [SerializeField]
    HealthBar healthBar;

    public enum Generation
    {
        Gen1,
        Gen2
    }

    public abstract Generation GetGeneration();

    private void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.layer == LayerMask.NameToLayer("P1"))
        {
            isBlocked = Physics2D.OverlapCapsule(
                transform.position + transform.right * keepDistance / 2,
                new Vector2(keepDistance, 1f),
                CapsuleDirection2D.Horizontal,
                0
            );
            inCombat = Physics2D.OverlapCapsule(
                transform.position + transform.right * attackRange / 2,
                new Vector2(attackRange, 1f),
                CapsuleDirection2D.Horizontal,
                0,
                LayerMask.NameToLayer("P2")
            );
        }
        else if (gameObject.layer == LayerMask.NameToLayer("P2"))
        {
            isBlocked = Physics2D.OverlapCapsule(
                transform.position + transform.right * -keepDistance / 2,
                new Vector2(keepDistance, 1f),
                CapsuleDirection2D.Horizontal,
                0
            );
            inCombat = Physics2D.OverlapCapsule(
                transform.position + transform.right * -attackRange / 2,
                new Vector2(attackRange, 1f),
                CapsuleDirection2D.Horizontal,
                0,
                LayerMask.NameToLayer("P1")
            );
        }

        // Lord forgives my sins for those nested if-else!
        if (isBlocked || isDead)
        {
            if (inCombat && !isDead)
            {
                if (timeBetweenHit <= attackCooldown)
                {
                    if (gameObject.layer == LayerMask.NameToLayer("P1"))
                    {
                        target = Physics2D.OverlapCapsule(
                            transform.position + transform.right * attackRange / 2,
                            new Vector2(attackRange, 1f),
                            CapsuleDirection2D.Horizontal,
                            0,
                            LayerMask.NameToLayer("P2")
                        );
                        target.gameObject.GetComponent<Soldier>().UpdateHealth(-damage);
                    }
                    else if (gameObject.layer == LayerMask.NameToLayer("P2"))
                    {
                        target = Physics2D.OverlapCapsule(
                            transform.position + transform.right * attackRange / 2,
                            new Vector2(attackRange, 1f),
                            CapsuleDirection2D.Horizontal,
                            0,
                            LayerMask.NameToLayer("P1")
                        );
                        target.gameObject.GetComponent<Soldier>().UpdateHealth(-damage);
                    }
                    attackCooldown = 0;
                }
                else
                {
                    attackCooldown += Time.deltaTime;
                }
            }
        }
        else
        {
            transform.position += transform.right * speed;
        }
    }

    public void UpdateHealth(int damageTaken)
    {
        health -= damageTaken;
        if (health <= 0)
        {
            health = 0;
            StartCoroutine(Dead());
        }
        healthBar.SetHealth(health);
    }

    IEnumerator Dead()
    {
        isDead = true;
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}