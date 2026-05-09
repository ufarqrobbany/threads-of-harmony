using Godot;
// Hapus 'using System.Collections.Generic;' karena HashSet tidak lagi digunakan
// untuk implementasi ini yang berfokus pada hit tunggal per serangan.

public class PlayerAttack
{
	private CharacterBody3D player;
	private PlayerAnimation animation;
	private PlayerHealth health;
	
	public float BaseATK { get; private set; } = 20f;
	public float ATK { get; private set; }
	
	private bool canAttack = true;
	private float attackCooldown = 0.5f; // Atur cooldown yang sesuai
	private float attackTimer = 0f;
	private bool _isEnabled = true; 
	private float attackActiveTimer = 0f; // DIGUNAKAN KEMBALI: Untuk timing damage
	private bool isAttacking = false;
	private bool hasDealtDamage = false; // DIGUNAKAN KEMBALI: Untuk memastikan damage hanya sekali per serangan
		
	private Area3D attackArea;
	
	public delegate void AttackStateChangedHandler(bool attacking);
	public event AttackStateChangedHandler OnAttackStateChanged;
	
	public PlayerAttack(CharacterBody3D player, PlayerAnimation animation, PlayerHealth health)
	{
		this.player = player;
		this.animation = animation;
		this.health = health;
		ATK = BaseATK;
		
		this.animation.AnimationFinished += OnAnimationFinished;
		attackArea = player.GetNode<Area3D>("AttackArea");
	}
	
	public void Update(double delta)
	{
		// Manajemen cooldown serangan
		if (!canAttack)
		{
			attackTimer += (float)delta;
			if (attackTimer >= attackCooldown)
			{
				canAttack = true;
				attackTimer = 0f;
			}
		}
		
		// Deteksi hit berdasarkan timer aktif serangan
		if (isAttacking)
		{
			attackActiveTimer += (float)delta; // Mengakumulasi waktu serangan aktif

			// Cek hit hanya sekali saat attackActiveTimer mencapai ambang batas dan belum ada damage yang diberikan
			if (!hasDealtDamage && attackActiveTimer >= 0.5f) // Atur 0.5f agar sesuai dengan timing animasi pukul
			{
				if (health.IsDead()) // Jika player mati di tengah serangan
				{
					isAttacking = false;
					OnAttackStateChanged?.Invoke(false);
					// Tidak perlu reset hasDealtDamage atau attackActiveTimer di sini, akan direset saat Attack() dipanggil lagi.
					return;
				}

				CheckAttackHit(); // Lakukan pengecekan hit
				hasDealtDamage = true; // Tandai bahwa damage sudah diberikan untuk serangan ini
			}
		}
	}
	
	private void CheckAttackHit()
	{
		var bodies = attackArea.GetOverlappingBodies();
		
		// Dapatkan arah pandang depan player pada bidang XZ
		Vector3 playerForward = -player.GlobalTransform.Basis.Z.Normalized();
		playerForward.Y = 0; // Abaikan komponen vertikal untuk cek arah
		playerForward = playerForward.Normalized();

		// Definisikan sudut serangan (total 90 derajat, jadi 45 derajat di setiap sisi dari depan)
		float attackAngleRad = Mathf.DegToRad(90);

		foreach (CharacterBody3D body in bodies)
		{
			if (body is Enemy enemy)
			{
				// Dapatkan arah dari player ke musuh
				Vector3 toEnemy = (enemy.GlobalTransform.Origin - player.GlobalTransform.Origin).Normalized();
				toEnemy.Y = 0; // Abaikan komponen vertikal
				toEnemy = toEnemy.Normalized();

				// Hitung sudut antara arah depan player dan arah ke musuh
				float angleToEnemy = Mathf.Acos(playerForward.Dot(toEnemy));

				// Cek apakah musuh berada dalam kerucut 80 derajat (40 derajat dari tengah)
				if (angleToEnemy <= attackAngleRad)
				{
					enemy.TakeDamage(ATK);
					// Tidak perlu menambahkan ke HashSet, karena hasDealtDamage mencegah double hit
					// untuk *serangan ini*.
				}
			}
		}
	}
	
	public bool IsEnabled
	{
		get => _isEnabled;
		set => _isEnabled = value;
	}
	
	public bool IsAttacking() => isAttacking; 
	
	public void HandleAttack()
	{
		if (!_isEnabled || health.IsDead()) return;

		if (Input.IsActionJustPressed("attack") && player.IsOnFloor() && canAttack && !isAttacking)
		{
			Attack();
		}
	}
	
	private void Attack()
	{
		canAttack = false;
		isAttacking = true;
		attackActiveTimer = 0f; // Reset timer saat serangan dimulai
		hasDealtDamage = false; // Reset flag damage saat serangan dimulai
		animation.Play("Punch_Right", isOverride: true);
		OnAttackStateChanged?.Invoke(true);
	}

	private void OnAnimationFinished(string animName)
	{
		// Pastikan ini hanya memengaruhi animasi serangan saat ini
		if (animName == "Punch_Right")
		{
			if (isAttacking) 
			{
				isAttacking = false; 
				OnAttackStateChanged?.Invoke(false);
				// Tidak perlu reset timer atau flag di sini, karena sudah direset di Attack()
			}
		}
	}
	
	public void UpdateStatsFromWeapon(WeaponData weapon)
	{
		ATK = BaseATK;

		if (weapon != null)
		{
			ATK += weapon.AttackBonus;
		}
	}
}
