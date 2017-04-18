using UnityEngine;
using System.Collections;

public static class MapManager
{
	public static int _height = 18;
	public static int _width = 10;
	public static string _curMap;
	public static GameObject[] _gos; 

	public static string LoadMap()
	{
		TextAsset ta = (TextAsset)Resources.Load("Map/Map_" + GameData._CurLevel); 
		if (ta == null)
		{
			return ""; 
		}

		Debug.Log("Origin data: " + ta.text); 
		string s = ""; 
		int row = 0; 
		int column = 0; 
		bool isComment = false; 
		for (int i = 0; i < ta.text.Length; ++i)
		{
			if (!isComment && ta.text[i] == '/' && i + 1 < ta.text.Length && ta.text[i + 1] == '/')
			{
				isComment = true; 
			}
			else if (isComment && ta.text[i] == '\n')
			{
				isComment = false; 
				continue; 
			}
			if (isComment)
			{
				continue; 
			}

			if (IsValid(ta.text[i]))
			{
				s += ta.text[i]; 
			}
			else if (ta.text[i] == '\n' && IsValid(ta.text[i - 1]))
			{
				//                Debug.Log("isValid: " + ta.text[i - 1] + ", " + ta.text[i + 1]); 
				if (column == 0)
				{
					column = s.Length; 
				}
				++row; 
			}
		}
		Debug.Log("row: " + row + ", column: " + column); 
		Debug.Log("Obtain data: " + s); 
		_width = column; 
		_height = row; 
		return s; 
	}

	public static bool IsValid(char character)
	{
		return character >= '0' && character <= '9'; 
	}

	public static void GenerateWall(string map, int width, int height)
	{
		GameData._Instance._wallParent = (new GameObject("WallParent")).transform; 
		GameData._Instance._wallParent.position = Vector3.zero; 
		GameData._Instance._wallPrefab.gameObject.SetActive(false); 
		_gos = new GameObject[map.Length]; 
		for (int i = 0; i < map.Length; ++i)
		{
			if (map[i] == MapCode.WALL)
			{
				Transform wall = GameObject.Instantiate(GameData._Instance._wallPrefab); 
				wall.SetParent(GameData._Instance._wallParent); 
				wall.gameObject.SetActive(true); 
				wall.position = new Vector3(i % width - width / 2 + 0.5f, i / width - height / 2 + 0.5f, 0); 
				wall.GetComponentInChildren<TextMesh>().text = string.Format("{0}, {1}", 
					i % width, i / width); 
				_gos[i] = wall.gameObject; 
			}
			else if (map[i] == MapCode.PIT)
			{
				Transform wall = GameObject.Instantiate(GameData._Instance._pitPrefab); 
				wall.SetParent(GameData._Instance._wallParent); 
				wall.gameObject.SetActive(true); 
				wall.position = new Vector3(i % width - width / 2 + 0.5f, i / width - height / 2 + 0.5f, 0); 
				//                wall.GetComponentInChildren<TextMesh>().text = string.Format("{0}, {1}", 
				//                    i % width, i / width); 
				_gos[i] = wall.gameObject;
			}
			else if (map[i] == MapCode.ENEMY)
			{
				Transform wall = GameObject.Instantiate(GameData._Instance._enemyPrefab); 
				wall.SetParent(GameData._Instance._wallParent); 
				wall.gameObject.SetActive(true); 
				wall.position = new Vector3(i % width - width / 2 + 0.5f, i / width - height / 2 + 0.5f, 0); 
				//                wall.GetComponentInChildren<TextMesh>().text = string.Format("{0}, {1}", 
				//                    i % width, i / width); 
				_gos[i] = wall.gameObject;
				wall.GetComponent<Enemy>().Init(new Pos((int)(i % width - width / 2), (int)(i / width - height / 2))); 
			}
			else
			{
				_gos[i] = null; 
			}
		}
	}

	public static void DestroyWall()
	{
		_gos = null; 
		if (GameData._Instance._wallParent != null)
		{
			GameObject.Destroy(GameData._Instance._wallParent.gameObject); 
		}
	}

}
