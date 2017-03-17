using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Diagnostics;
using UnityEngine;

public class DatabaseReader
{
	
	public static int GetWidth(int levelNum)
	{
		int max = 0;

		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM Level" + levelNum + ";");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int x = reader.GetInt32(1);
						if (x > max)
							max = x;
					}
				}
			}
		}

		return (max + 1);
	}

	public static int GetHeight(int levelNum)
	{
		int max = 0;

		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM Level" + levelNum + ";");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int y = reader.GetInt32(2);
						if (y > max)
							max = y;
					}
				}
			}
		}

		return (max + 1);
	}

	public static void SetLevel(Tile[,] levelMap, List<Unit> unitList, int levelNum)
	{
		Stopwatch timer = new Stopwatch();
		timer.Start();
		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbTransaction transaction = Dbconn.BeginTransaction())
			{
				using (IDbCommand Dbcmd = Dbconn.CreateCommand())
				{
					Dbcmd.CommandText = ("CREATE TABLE Backup(id INTEGER PRIMARY KEY NOT NULL, x INTEGER, y INTEGER, tiletype INTEGER, unittype INTEGER);");
					Dbcmd.ExecuteNonQuery();
					Dbcmd.CommandText = ("INSERT INTO Backup SELECT * FROM Level" + levelNum + ";");
					Dbcmd.ExecuteNonQuery();

					try
					{
						List<Vector2> xyPairs = new List<Vector2>();

						for (int x = 0; x < levelMap.GetLength(0); x++)
						{
							for (int y = 0; y < levelMap.GetLength(1); y++)
							{
								xyPairs.Add(new Vector2(x, y));
							}
						}

						for (int i = 0; i < xyPairs.Count; i++)
						{
							Tile tempTile = levelMap[(int) xyPairs[i].x, (int) xyPairs[i].y];
							Dbcmd.CommandText = ("UPDATE Level" + levelNum + " SET x=" + (int) xyPairs[i].x + ", y=" + (int) xyPairs[i].y +
							", tiletype=" + tempTile.Type.ID + " WHERE x=" + (int) xyPairs[i].x + " AND y=" + (int) xyPairs[i].y);
							Dbcmd.ExecuteNonQuery();
						}

						Dbcmd.CommandText = ("UPDATE Level" + levelNum + " SET unittype = 0");
						Dbcmd.ExecuteNonQuery();

						for (int x = 0; x < levelMap.GetLength(0); x++)
						{
							for (int y = 0; y < levelMap.GetLength(1); y++)
							{
								for (int i = 0; i < unitList.Count; i++)
								{
									if (unitList[i].X == x && unitList[i].Y == y)
									{
										Dbcmd.CommandText = ("UPDATE Level" + levelNum + " SET unittype=" + unitList[i].Type.ID +
										" WHERE x=" + x + " AND y=" + y);
										Dbcmd.ExecuteNonQuery();
									}
								}
							}
						}
					}
					catch(Exception e)
					{
						Dbcmd.CommandText = ("DELETE FROM Level" + levelNum);
						Dbcmd.ExecuteNonQuery();
						Dbcmd.CommandText = ("INSERT INTO Level" + levelNum + " SELECT * FROM Backup;");
						Dbcmd.ExecuteNonQuery();
						UnityEngine.Debug.Log(e);
						UnityEngine.Debug.LogError("Exception caught during save.");
					}

					Dbcmd.CommandText = ("DROP TABLE Backup;");
					Dbcmd.ExecuteNonQuery();
				}

				transaction.Commit();
				UnityEngine.Debug.Log("Time elapsed during save: " + timer.ElapsedMilliseconds + " Miliseconds.");
				timer.Stop();
				timer = null;
			}
		}
	}

	public static Tile[,] GetLevel(int levelNum, Level level)
	{
		Tile[,] levelMap = new Tile[level.Width, level.Height];

		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM Level" + levelNum + ";");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int x = reader.GetInt32(1);
						int y = reader.GetInt32(2);

						TileType tType = GetTileType(reader.GetInt32(3));
						levelMap[x, y] = new Tile(level, x, y, tType);
					}

					for (int x = 0; x < level.Width; x++)
					{
						for (int y = 0; y < level.Height; y++)
						{
							if (levelMap[x, y] == null)
							{
								levelMap[x, y] = new Tile(level, x, y, new TileType());
							}
						}
					}

					return levelMap;
				}
			}
		}
	}

	public static List<Unit> GetUnits(int levelNum, Level level)
	{
		List<Unit> unitList = new List<Unit>();

		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM Level" + levelNum + ";");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int x = reader.GetInt32(1);
						int y = reader.GetInt32(2);
						UnitType type = GetUnitType(reader.GetInt32(4));

						if (type.ID != 0)
							unitList.Add(new Unit(level, x, y, type));
					}

					return unitList;
				}
			}
		}
	}

	public static TileType GetTileType(int id)
	{
		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM TileTypes;");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						if (id == reader.GetInt32(0))
						{
							string name = reader.GetString(1);
							string description = reader.GetString(2);
							string sprite = reader.GetString(3);
							bool isWalkable = reader.GetBoolean(4);

							return new TileType(id, name, description, sprite, isWalkable);
						}
					}
				}
			}
		}

		return new TileType();
		//Use this when we have choosing tiletypes when constructing rather than cycling
		//throw new Exception("Unknown TileType");
	}

	public static UnitType GetUnitType(int id)
	{
		using (IDbConnection Dbconn = (IDbConnection) new SqliteConnection("URI = file:" + Application.dataPath + "/Databases/MainDatabase.sqlite"))
		{
			Dbconn.Open();
			using (IDbCommand Dbcmd = Dbconn.CreateCommand())
			{
				Dbcmd.CommandText = ("SELECT * FROM UnitTypes;");
				using (IDataReader reader = Dbcmd.ExecuteReader())
				{
					while (reader.Read())
					{
						if (id == reader.GetInt32(0))
						{
							string name = reader.GetString(1);
							string description = reader.GetString(2);
							string sprite = reader.GetString(3);
							bool isPlayer = reader.GetBoolean(4);

							return new UnitType(id, name, description, sprite, isPlayer);
						}
					}
				}
			}
		}

		throw new Exception("Unknown UnitType");
	}
}
