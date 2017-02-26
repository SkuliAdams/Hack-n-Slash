using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Unit
{
	Action<Unit> cbUnitTypeChanged;
	UnitType type;
	Level level;
	int x;
	int y;

	public UnitType Type
	{
		get
		{
			return type;
		}
		set
		{
			if (!type.Equals(value))
			{
				type = UnitType.CopyOf(value);
				if (cbUnitTypeChanged != null)
					cbUnitTypeChanged(this);
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

	public Unit(Level level, int x, int y, UnitType type)
	{
		this.level = level;
		this.x = x;
		this.y = y;
		this.type = type;
		if (cbUnitTypeChanged != null)
			cbUnitTypeChanged(this);
	}

	public void RegisterCBUnitTypeChanged(Action<Unit> callback)
	{
		cbUnitTypeChanged += callback;
	}

	public void UnregisterCBUnitTypeChanged(Action<Unit> callback)
	{
		cbUnitTypeChanged -= callback;
	}
}
