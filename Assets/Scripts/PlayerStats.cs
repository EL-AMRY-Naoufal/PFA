using UnityEngine;
using UnityEngine.UI;
public class PlayerStats : MonoBehaviour
{
    [Header("Other elements refernces")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MoveBehaviour playerMovementScript;

    [Header("Health")]
    [SerializeField]
    private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private float healthDecraseRateForHungerAndThrist;

    [Header("Hunger")]
    [SerializeField]
    private float maxhunger = 100f;

    [SerializeField]
    public float currentHunger;

    [SerializeField]
    private Image hungerBarFill;

    [SerializeField]
    private float hungerDecreaseRate;

    [Header("Thirst")]
    [SerializeField]
    private float maxThirst = 100f;
    public float currentThirst;

    [SerializeField]
    private Image thirstBarFill;

    [SerializeField]
    private float thirstDecreaseRate;

    public float currentArmorpoints;

    [HideInInspector]
    public bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        currentHunger = maxhunger;
        currentThirst = maxThirst;
    }

    
    void Update()
    {
        UpdateHungerAndThirstBarfill();
        UpdateHealthBarFill();
        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50f);
        }
    }
       
    public void TakeDamage(float damage,bool overTime = false)
    {
        if(overTime)
        {
            currentHealth -= damage * Time.deltaTime;
        }
        else
            currentHealth -= damage  * (1 - (currentArmorpoints / 100));

        if(currentHealth <= 0 && !isDead)
        {
            Die();
            Debug.Log("Player Died");
        }

        UpdateHealthBarFill();
    }

    private void Die()
    {
        isDead = true;
        playerMovementScript.canMove = false;

        hungerDecreaseRate = 0;
        thirstDecreaseRate = 0;
        animator.SetTrigger("Die");
    }

    public void ConsumeItem(float health,float hunger,float thrist)
    {
        currentHealth += health;
        if(currentHealth>maxHealth)
            currentHealth = maxHealth;

        currentHunger += hunger;
        if (currentHunger > maxhunger)
            currentHunger = maxhunger;

        currentThirst += thrist;
        if(currentThirst>maxThirst)
            currentThirst=maxThirst;

        UpdateHealthBarFill();
    }

    public void UpdateHealthBarFill()
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void UpdateHungerAndThirstBarfill()
    {
        //Diminue la faim / soif au fil du temps
        currentHunger -= hungerDecreaseRate * Time.deltaTime;
        currentThirst -= thirstDecreaseRate * Time.deltaTime;

        //On empêche de passer dans le négatif
        currentHunger = currentHunger < 0 ? 0 : currentHunger;
        currentThirst = currentThirst < 0 ? 0 : currentThirst;
    
        //Mettre à jour les visuels
        hungerBarFill.fillAmount = currentHunger / maxhunger;
        thirstBarFill.fillAmount = currentThirst / maxThirst;

        //si la barre de la faim rt/ou soif est zéro -> Le joueur prend des dégats (*2 si les deux barres sont a 0
        if(currentHunger <= 0 || currentThirst <= 0)
        {
            TakeDamage((currentHunger <= 0 && currentThirst <= 0 ? healthDecraseRateForHungerAndThrist * 2 : healthDecraseRateForHungerAndThrist), true);
        }
    }

}
