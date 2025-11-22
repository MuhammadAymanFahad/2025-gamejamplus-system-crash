using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayer
{
    public static PlayerManager Instance { get; private set; }
    private PlayerStats stats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            stats = GetComponent<PlayerStats>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // IPlayer implementation
    public int CurrentHP => stats.currentHP;
    public int MaxHP => stats.maxHP;
    public int WeaponGrade => stats.weaponGrade;
    public bool CanFlee => stats.canFlee;
    public bool IsAlive => stats.IsAlive();

    public void SetCombatState(bool inCombat)
    {
        stats.isInCombat = inCombat;
        Debug.Log($"Combat state: {(inCombat ? "ENTERED" : "EXITED")}");
    }

    public void TakeDamage(int damage)
    {
        int oldHP = stats.currentHP;
        stats.currentHP = DamageCalculator.CalculateRemainingHP(stats.currentHP, damage);

        Debug.Log($"Player takes {damage} damage! HP: {stats.currentHP}/{stats.maxHP}");

        if (!IsAlive)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        int oldHP = stats.currentHP;
        stats.currentHP += amount;

        if (stats.currentHP > stats.maxHP)
        {
            stats.currentHP = stats.maxHP;
        }

        int actualHeal = stats.currentHP - oldHP;
        Debug.Log($"Player healed {actualHeal} HP! ({oldHP} → {stats.currentHP}/{stats.maxHP})");
    }

    public void EquipWeapon(int grade)
    {
        if (stats.weaponGrade > 0)
        {
            DiscardWeapon();
        }
        stats.weaponGrade = grade;
        stats.weaponKillCount = 0;
        Debug.Log($"Equipped weapon grade {grade}");
    }

    public void AdoptWeaponGrade(int monsterGrade)
    {
        if (!stats.HasWeapon()) return;

        stats.weaponGrade = monsterGrade;
        stats.weaponKillCount++;
        Debug.Log($"Weapon adopted grade {monsterGrade} (Kills: {stats.weaponKillCount}/5)");

        if (stats.ShouldDropWeapon())
        {
            DiscardWeapon();
        }
    }

    public void DiscardWeapon()
    {
        Debug.Log($"Discarded weapon grade {stats.weaponGrade}");
        stats.weaponGrade = 0;
        stats.weaponKillCount = 0;
        // TODO: Send to discard pile via CardManager
    }

    public void Flee()
    {
        if (!stats.canFlee)
        {
            Debug.Log("Cannot flee consecutively!");
            return;
        }

        Debug.Log("Player fled!");
        stats.canFlee = false;
        stats.isInCombat = false;
        // TODO: Notify RoomManager
    }

    public void EnterRoom()
    {
        stats.canFlee = true;
        Debug.Log("Entered new room. Flee available again.");
    }

    // Death
    void Die()
    {
        Debug.Log("PLAYER DIED!");
        CombatEvents.TriggerPlayerDeath();
        // TODO: Trigger Game Over
    }

    // Testing
    void Update()
    {
        // Test equip weapon
        if (Input.GetKeyDown(KeyCode.K))
        {
            EquipWeapon(6);
        }
    }
}