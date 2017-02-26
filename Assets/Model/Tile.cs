using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Tile
{
	Action<Tile> cbTileTypeChanged;
	TileType type;
	Level level;
	int x;
	int y;

	public TileType Type
	{
		get
		{
			return type;
		}
		set
		{
			if (!type.Equals(value))
			{
				type = TileType.CopyOf(value);
				if (cbTileTypeChanged != null)
					cbTileTypeChanged(this);
			}
		}
	}

	public int X
	{
		get
		{
			return x;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
	}

	public Tile(Level level, int x, int y, TileType type)
	{
		this.level = level;
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public void RegisterCBTileTypeChanged(Action<Tile> callback)
	{
		cbTileTypeChanged += callback;
	}

	public void UnregisterCBTileTypeChanged(Action<Tile> callback)
	{
		cbTileTypeChanged -= callback;
	}

	public Tile Copy()
	{
		return new Tile(level, x, y, type.Copy());
	}

	public bool Equals(Tile otherTile)
	{
		if (x == otherTile.X && y == otherTile.Y && level == otherTile.level && type.Equals(otherTile.type))
			return true;
		return false;
	}
}
