using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public Sprite floorSprite;
	public Sprite wallSprite;
	static LevelController _instance;

	Level level;
	Dictionary<Tile, GameObject> tileGameObjectMap;
	Dictionary<Unit, GameObject> unitGameObjectMap;
	int unitNum = 0;

	public Level Level
	{
		get
		{
			return level;
		}
		protected set
		{
			level = value;
		}
	}

	public static LevelController Instance
	{
		get
		{
			return _instance;
		}
		protected set
		{
			_instance = value;
		}
	}

	// Use this for initialization
	void Start()
	{
		_instance = this;
		level = new Level(0);
		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		unitGameObjectMap = new Dictionary<Unit, GameObject>();

		//Create gameobject for each tile
		for (int x = 0; x < level.Width; x++)
		{	
			for (int y = 0; y < level.Height; y++)
			{
				//Create and transform
				GameObject tile_go = new GameObject();
				tile_go.name = "Tile_" + x + "_" + y;

				Tile tile_data = level.GetTile(x, y);
				tileGameObjectMap.Add(tile_data, tile_go);

				tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);

				//Add sprite renderer.
				tile_go.AddComponent<SpriteRenderer>();
				tile_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tile_data.Type.Sprite);

				tile_data.RegisterCBTileTypeChanged(OnTileTypeChanged);
			}
		}

		//Create gameobject for each unit
		for (int i = 0; i < level.Units.Count; i++)
		{
			GameObject unit_go = new GameObject();
			if (level.Units[i].Type.IsPlayer)
				unit_go.name = "Player";
			else
				unit_go.name = "Unit_" + unitNum;
			unitNum++;

			Unit unit_data = level.Units[i];
			unitGameObjectMap.Add(unit_data, unit_go);

			unit_go.transform.position = new Vector3(unit_data.X, unit_data.Y, -0.5f);
			unit_go.transform.SetParent(this.transform, true);

			//Add sprite and renderer.
			unit_go.AddComponent<SpriteRenderer>();
			unit_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit_data.Type.Sprite);

			unit_data.RegisterCBUnitTypeChanged(OnUnitTypeChanged);
		}

		//level.RandomizeTiles();
	}

	// Update is called once per frame
	void Update()
	{
	}

	void OnTileTypeChanged(Tile tile_data)
	{
		if (tileGameObjectMap.ContainsKey(tile_data) == false)
		{
			Debug.LogError("tileGameObjectMap doesn't contain tile_data");
			return;
		}

		GameObject tile_go = tileGameObjectMap[tile_data];

		if (tile_go == null)
		{
			Debug.LogError("tileGameObjectMap's returned game object is null");
			return;
		}

		tile_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tile_data.Type.Sprite);
	}

	void OnUnitTypeChanged(Unit unit_data)
	{
		if (unit_data.Type.ID == 0)
		{
			Destroy(unitGameObjectMap[unit_data]);
			unitGameObjectMap[unit_data] = null;
			unitGameObjectMap.Remove(unit_data);
			unit_data.UnregisterCBUnitTypeChanged(OnUnitTypeChanged);
			unit_data = null;
			return;
		}

		if (unitGameObjectMap.ContainsKey(unit_data) == false)
		{
			Debug.LogError("unitGameObjectMap doesn't contain unit_data");
			return;
		}

		GameObject unit_go = unitGameObjectMap[unit_data];

		if (unit_go == null)
		{
			Debug.LogError("unitGameObjectMap's returned game object is null");
			return;
		}

		unit_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit_data.Type.Sprite);
	}

	public void SetUnit(Unit unit_data)
	{
		GameObject unit_go = new GameObject();

		if (unit_data.Type.IsPlayer)
			unit_go.name = "Player";
		else
			unit_go.name = "Unit_" + unitNum;
		unitNum++;
		
		unitGameObjectMap.Add(unit_data, unit_go);

		unit_go.transform.position = new Vector3(unit_data.X, unit_data.Y, -0.5f);
		unit_go.transform.SetParent(this.transform, true);

		//Add sprite and renderer.
		unit_go.AddComponent<SpriteRenderer>();
		unit_go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(unit_data.Type.Sprite);

		unit_data.RegisterCBUnitTypeChanged(OnUnitTypeChanged);
	}

	public static Tile GetTileAtCoord(Vector3 coord)
	{
		int x = (int) Math.Round(coord.x);
		int y = (int) Math.Round(coord.y);

		return Instance.Level.GetTile(x, y);
	}
}