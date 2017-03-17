using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Level
{
	Tile[,] tiles;
	List<Unit> units;
	int width;
	int height;
	int levelNum;

	public Tile[,] Tiles
	{
		get
		{
			return tiles;
		}
	}

	public List<Unit> Units
	{
		get
		{
			return units;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
	}

	public int LevelNum
	{
		get
		{
			return levelNum;
		}
	}

	public Level(int levelNum = 0)
	{
		this.width = 64;
		this.height = 64;
		this.levelNum = levelNum;

		tiles = DatabaseReader.GetLevel(levelNum, this);
		units = DatabaseReader.GetUnits(levelNum, this);

		Debug.Log("World created with " + width * height + " tiles and " + units.Count + " units.");
	}

	public Tile GetTile(int x, int y)
	{
		try
		{
			return tiles[x, y];
		}
		catch(IndexOutOfRangeException)
		{
			return null;
		}
	}

	public bool HasUnit(int x, int y)
	{
		foreach (Unit u in units)
		{
			if (u.X == x && u.Y == y)
				return true;
		}

		return false;
	}

	public Unit GetUnit(int x, int y)
	{
		for (int i = 0; i < units.Count; i++)
		{
			if (units[i].X == x && units[i].Y == y)
				return units[i];
		}

		return null;
	}

	public void AddUnit(Unit unit)
	{
		DeleteUnit(unit.X, unit.Y);
		units.Add(unit);
	}

	public void DeleteUnit(int x, int y)
	{
		if (this.HasUnit(x, y))
		{
			GetUnit(x, y).Type = new UnitType();
			units.Remove(GetUnit(x, y));
		}
	}

	//Method is depricated, no longer use enum
	/*
	public void RandomizeTiles()
	{
		Debug.Log("Randomized");
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int randomNum = Random.Range(0, 3);

				if (randomNum == 0)
				{
					tiles[x, y].Type = TileType.Empty;
				}
				else if (randomNum == 1)
				{
					tiles[x, y].Type = TileType.Floor;
				}
				else
				{
					tiles[x, y].Type = TileType.Wall;
				}
			}
		}
	}
	*/
}
