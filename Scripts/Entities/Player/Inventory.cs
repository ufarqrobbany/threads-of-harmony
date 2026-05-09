// Inventory.cs
using Godot;
using System.Collections.Generic;

public partial class Inventory : Node
{
	[Signal] public delegate void WeaponEquippedEventHandler(WeaponData newWeapon);
	[Signal] public delegate void InventoryChangedEventHandler();

	private List<WeaponData> weapons = new();
	private WeaponData equippedWeapon;

	public WeaponData GetEquippedWeapon() => equippedWeapon;
	public Godot.Collections.Array<WeaponData> GetAllWeapons() => new Godot.Collections.Array<WeaponData>(weapons);

	public void AddWeapon(WeaponData weapon)
	{
		if (weapon != null && !weapons.Contains(weapon))
		{
			weapons.Add(weapon);
			EmitSignal(SignalName.InventoryChanged);
			GD.Print($"Added weapon: {weapon.WeaponName}");
		}
	}

	public void EquipWeapon(WeaponData weapon)
	{
		if (weapons.Contains(weapon) || weapon == null) // Allow unequipping by passing null
		{
			equippedWeapon = weapon;
			EmitSignal(SignalName.WeaponEquipped, equippedWeapon);
			GD.Print($"Equipped weapon: {weapon?.WeaponName ?? "None"}");
		}
		else
		{
			GD.PrintErr($"Attempted to equip weapon '{weapon?.WeaponName}' not in inventory.");
		}
	}

	public void EquipWeaponByName(string weaponName)
	{
		WeaponData weaponToEquip = weapons.Find(w => w.WeaponName == weaponName);
		if (weaponToEquip != null)
		{
			EquipWeapon(weaponToEquip);
		}
		else
		{
			GD.PrintErr($"Weapon '{weaponName}' not found in inventory.");
		}
	}
}
