using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
	private const float ZOOM_SPEED = 0.25f;
	private const float PAN_SPEED = 0.1f;
	public GameObject circleCursor;
	public GameObject background;
	int selectedUnitType = 1;

	Vector3 lastFramePosition;
	Vector3 dragStartPosition;
	Vector3 currFramePosition;

	void Start()
	{
		
	}

	void Update()
	{
		//Get current mouse position
		currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currFramePosition.z = 0;

		UpdateCursor();
		UpdateDragging();
		UpdateCamera();
		UpdateHotkeys();

		//Set position for next frame
		lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void UpdateCursor()
	{
		Tile tileUnderMouse = LevelController.GetTileAtCoord(currFramePosition);

		if (tileUnderMouse != null)
		{
			circleCursor.SetActive(true);
			Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
		}
		else
		{
			circleCursor.SetActive(false);
		}
	}

	void UpdateDragging()
	{
		//Start drag
		if (Input.GetMouseButtonDown(0))
		{
			dragStartPosition = currFramePosition;
		}

		//End drag
		if (Input.GetMouseButtonUp(0))
		{
			int start_x = (int) Math.Round(dragStartPosition.x);
			int end_x = (int) Math.Round(currFramePosition.x);

			if (end_x < start_x)
			{
				int temp = end_x;
				end_x = start_x;
				start_x = temp;
			}

			int start_y = (int) Math.Round(dragStartPosition.y);
			int end_y = (int) Math.Round(currFramePosition.y);

			if (end_y < start_y)
			{
				int temp = end_y;
				end_y = start_y;
				start_y = temp;
			}

			for (int x = start_x; x <= end_x; x++)
			{
				for (int y = start_y; y <= end_y; y++)
				{
					Tile t = LevelController.Instance.Level.GetTile(x, y);
					if (t != null)
					{
						t.Type = DatabaseReader.GetTileType(t.Type.ID + 1);
					}
				}
			}


		}
	}

	void UpdateCamera()
	{
		//Handle Panning

		/* Pan with click and drag left and right mouse buttons. Not in use.
		if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
		{
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate(diff);
		}
		*/

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			Camera.main.transform.Translate(new Vector3(-PAN_SPEED, 0, 0));
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			Camera.main.transform.Translate(new Vector3(PAN_SPEED, 0, 0));
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			Camera.main.transform.Translate(new Vector3(0, PAN_SPEED, 0));
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			Camera.main.transform.Translate(new Vector3(0, -PAN_SPEED, 0));
		}

		//Handle zooming
		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
		if (Input.GetKeyDown(KeyCode.Minus))
			Camera.main.orthographicSize += Camera.main.orthographicSize * ZOOM_SPEED;
		if (Input.GetKeyDown(KeyCode.Equals))
			Camera.main.orthographicSize -= Camera.main.orthographicSize * ZOOM_SPEED;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 4f, 20f);
	}

	void UpdateHotkeys()
	{
		if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftAlt))
		{
			DatabaseReader.SetLevel(LevelController.Instance.Level.Tiles, LevelController.Instance.Level.Units, LevelController.Instance.Level.LevelNum);
		}

		if (Input.GetKeyDown(KeyCode.U))
		{
			Tile tileUnderMouse = LevelController.GetTileAtCoord(currFramePosition);

			if (tileUnderMouse != null)
			{
				LevelController.Instance.Level.AddUnit(new Unit(LevelController.Instance.Level, tileUnderMouse.X, tileUnderMouse.Y, DatabaseReader.GetUnitType(selectedUnitType)));
				LevelController.Instance.SetUnit(LevelController.Instance.Level.GetUnit(tileUnderMouse.X, tileUnderMouse.Y));
			}
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			Tile tileUnderMouse = LevelController.GetTileAtCoord(currFramePosition);

			if (tileUnderMouse != null)
			{
				LevelController.Instance.Level.DeleteUnit(tileUnderMouse.X, tileUnderMouse.Y);
			}
		}
	}
}
