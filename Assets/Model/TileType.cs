using System;
using System.Collections;

public class TileType
{
	int id = 0;
	string name;
	string description;
	string sprite;
	bool isWalkable = false;

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
		set
		{
			sprite = value;
		}
	}

	public TileType() : this(0, "", "", "", false)
	{
	}

	public TileType(int id, String name, String description, String sprite, Boolean isWalkable)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.sprite = sprite;
		this.isWalkable = isWalkable;
	}

	public Boolean Equals(TileType other)
	{
		if (this.id == other.id)
			return true;
		return false;
	}

	public static TileType CopyOf(TileType tile)
	{
		return new TileType(tile.id, tile.name, tile.description, tile.sprite, tile.isWalkable);
	}

	public TileType Copy()
	{
		return new TileType(id, name, description, sprite, isWalkable);
	}
}