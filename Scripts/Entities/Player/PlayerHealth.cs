using Godot;
using System;

public class PlayerHealth
{
	private PlayerAnimation animation;
	
	public float BaseMaxHealth { get; private set; } = 100f; // Base health
	public float CurrentHealth { get; private set; }
	public float BaseDEF {get; private set; } = 2f; // Base defense
	
	public float MaxHealth { get; private set; }
	public float DEF {get; private set; }
	
	private bool hasNotifiedDeath = false;
	public delegate void PlayerDiedHandler();
	public event PlayerDiedHandler OnPlayerDied;
	
	public PlayerHealth(PlayerAnimation animation)
	{
		this.animation = animation;
		CurrentHealth = BaseMaxHealth;
		MaxHealth = BaseMaxHealth;
		DEF = BaseDEF;
	}

	public void TakeDamage(float amount)
	{
		CurrentHealth = Mathf.Max(CurrentHealth - (amount - DEF), 0f);
		
		if (CurrentHealth <= 0 && !hasNotifiedDeath)
		{
			hasNotifiedDeath = true;
			OnPlayerDied?.Invoke();
			animation?.Play("Die", isOverride: true);
		}
	}

	public void Heal(float amount)
	{
		CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
	}

	public bool IsDead() => CurrentHealth <= 0f;
	
	public void UpdateStatsFromWeapon(WeaponData weapon)
	{
		MaxHealth = BaseMaxHealth;
		DEF = BaseDEF;

		if (weapon != null)
		{
			MaxHealth += weapon.HealthBonus;
			DEF += weapon.DefenseBonus;
		}

		CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
	}
}
