using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private PlayerStats playerStats;    

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AudioSource audioSource;

    [Header("Stats")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float chaseSpeed;

    [SerializeField]
    private float detectionRadius;

    [SerializeField]
    private float attackRadius;

    [SerializeReference]
    private float attackDelay;

    [SerializeField]
    private float damageDealt;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;

    [Header("Wandering parametres")]
    [SerializeField]
    private float wanderingWaitTimeMin;

    [SerializeField]
    private float wandringWaitTimeMax;

    [SerializeField]
    private float wanderingDistanceMin;

    [SerializeField]
    private float wanderingDistanceMax;

    [SerializeField]
    private ItemData itemData;

    private bool hasDestination;
    private bool isAttacking;
    private bool isDead;
    private Vector3 spawnItemOffset = new Vector3(0, 0.5f, 0);

    private void Awake()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform;
        playerStats = playerTransform.GetComponent<PlayerStats>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if(Vector3.Distance(player.position, transform.position) < detectionRadius && !playerStats.isDead)
        {
            agent.speed = chaseSpeed;

            Quaternion rot = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);

            if(!isAttacking)
            {
                if (Vector3.Distance(player.position, transform.position) < attackRadius)
                {
                    StartCoroutine(AttackPlayer());
                }
                else
                {
                    agent.SetDestination(player.position);
                }
            }
        }
        else
        {
            agent.speed = walkSpeed;
            if (agent.remainingDistance < 0.75f && !hasDestination)
            {
                StartCoroutine(GetNewDestination());
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void TakeDamage(float damages)
    {
        if (isDead)
            return;

        currentHealth -= damages;

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("GetHit");
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        agent.enabled = false;
        enabled = false;
        yield return new WaitForSeconds(5);
        GameObject instantiatedRessource = Instantiate(itemData.Prefab);
        instantiatedRessource.transform.position = gameObject.transform.position + spawnItemOffset;
        Destroy(gameObject);
    }

    IEnumerator GetNewDestination()
    {
        hasDestination = true;  
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin,wandringWaitTimeMax)); 

        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin,wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1), 0f, Random.Range(-1f, 1f)).normalized;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        hasDestination = false; 
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true;

        audioSource.Play();

        animator.SetTrigger("Attack");

        playerStats.TakeDamage(damageDealt);

        yield return new WaitForSeconds(attackDelay);

        if(agent.isOnNavMesh)
            agent.isStopped = false;
        isAttacking=false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
