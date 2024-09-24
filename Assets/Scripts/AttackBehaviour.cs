using UnityEditor;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Equipment equipmentSystem;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private InteractBehaviour interactBehaviour;

    [SerializeField]
    private PlayerStats playerStats;

    [Header("Configuration")]
    private bool isAttacking;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Vector3 attackOffset;

    void Update()
    {
        Debug.DrawRay(transform.position + attackOffset, transform.forward * attackRange, Color.red);
        if(Input.GetMouseButtonDown(0) && CanAttack())
        {
            isAttacking = true;
            SendAttack();
            animator.SetTrigger("Attack");
        }
    }

    void SendAttack()
    {
        Debug.Log("Attack");
        RaycastHit hit;
        if(Physics.Raycast(transform.position + attackOffset, transform.forward, out hit, attackRange, layerMask))
        {
            if(hit.transform.CompareTag("AI"))
            {
                EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
                enemy.TakeDamage(equipmentSystem.equipedWeaponItem.attackPoints);
            }
        }
    }

    bool CanAttack()
    {
        /*Pour attacker on doit:
          -Avoir une arme équiper
          -Ne pas être en train d'attaquer
          -Ne pas avoir un inventaire overt
         */
        return equipmentSystem.equipedWeaponItem != null && !isAttacking && !uiManager.atLeadtOnePanelOpened && !interactBehaviour.isBusy && !playerStats.isDead;
    }
    public void AttackFinished()
    {
        isAttacking = false;
    }

}
