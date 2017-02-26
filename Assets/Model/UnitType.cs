using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitType
{
	int id;
	string name;
	string description;
	string sprite;
	bool isPlayer = false;

	public int ID
	{
		get
		{
			return id;
		}
	}

	public string Sprite
	{
		get
		{
			return sprite;
		}
	}

	public bool IsPlayer
	{
		get
		{
			return isPlayer;
		}
	}

	public UnitType() : this(0, "", "", "", false)
	{
	}

	public UnitType(int id, string name, string description, string sprite, bool isPlayer)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.sprite = sprite;
		this.isPlayer = isPlayer;
	}

	public static UnitType CopyOf(UnitType type)
	{
		return new UnitType(type.id, type.name, type.description, type.sprite, type.isPlayer);
	}
}
