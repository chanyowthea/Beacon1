using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EDirection
{
	East, 
	West, 
	South, 
	North
}

public class Player : MonoBehaviour
{
	#region Main

	public static Player _Instance; 	

	[SerializeField] int _x;
	[SerializeField] int _y;

	void Awake()
	{
		_Instance = this; 
	}

	public void Init()
	{
		Debug.Log("isUpstairs: " + _isUpstairs); 
		InitLevel(); 
		Reset(); 
		InitHP(); 
	}

	public void Reset()
	{
		MapManager.DestroyWall(); 
		UIManager._Instance.Reset(); 
		UIManager._Instance.SetStep(GameData._Step); 
		MapManager._curMap = MapManager.LoadMap(); 
		MapManager.GenerateWall(MapManager._curMap, MapManager._width, MapManager._height); 
		ResetPlayerPos(); 
		UIManager._Instance.SetCurLevel(GameData._CurLevel); 
		ResetHP(); 
	}

	public void Clear()
	{
		
	}

	#endregion


	#region Move

	const byte _DIR_EAST = 1;
	const byte _DIR_WEST = 2;
	const byte _DIR_SOUTH = 4;
	const byte _DIR_NORTH = 8;

	void ResetPlayerPos()
	{
		char role = _isUpstairs ? '8' : '9'; // 上楼后达到的位置
		for (int i = 0; i < MapManager._curMap.Length; ++i)
		{
			if (MapManager._curMap[i] == role)
			{
				_x = i % MapManager._width - MapManager._width / 2; 
				_y = i / MapManager._width - MapManager._height / 2; 
				break; 
			}
		}

		Vector2[] dirs = new Vector2[4]; 
		dirs[0] = new Vector2(1, 0); 
		dirs[1] = new Vector2(-1, 0); 
		dirs[2] = new Vector2(0, -1); 
		dirs[3] = new Vector2(0, 1); 
		float[] angles = new float[]{-90, 90, 180, 0}; 
		transform.eulerAngles = Vector3.zero; 
		for (int i = 0, count = dirs.Length; i < count; ++i)
		{
			int index = CurIndex(_x + (int)dirs[i].x, _y + (int)dirs[i].y); 
			if (index > 0) 
			{
				char c = 
				MapManager._curMap[index]; 
				if (c == '0')
				{
					Vector3 angle = new Vector3(0, 0, angles[i]); 
					transform.localEulerAngles += angle; 
					UIManager._Instance.ChangeCompassDir(-angle); 
					GameData._HasRotated = i <= 1;
//					Debug.Log("HasRotated: " + GameData._HasRotated); 
					break; 
				}
			}
		}
	}

	void Update()
	{
		float h = Input.GetAxisRaw("Horizontal"); 
		float v = Input.GetAxisRaw("Vertical"); 
		if (Input.GetButtonDown("Horizontal"))
		{
			if (GameData._CanRotateCamera)
			{
				Vector3 angle = new Vector3(0, 0, (h < 0 ? 1 : -1) * 90); 
//                Camera.main.transform.eulerAngles += angle; 
				Player._Instance.transform.eulerAngles += angle; 
				UIManager._Instance.ChangeCompassDir(-angle); 
				GameData._HasRotated = !(GameData._HasRotated); 
			}
			else
			{
				Move((int)h, (int)v); 
			}
		}
		else if (Input.GetButtonDown("Vertical"))
		{
//			Debug.Log("can rotate: " + GameData._CanRotateCamera + ", is rotate: " + GameData._HasRotated); 
			if (GameData._CanRotateCamera)
			{
				int rotateTimes = (int)(Camera.main.transform.eulerAngles.z / 90); 
				int changeValue = (rotateTimes == 1 || rotateTimes == 2) ? -(int)v : (int)v; 
				if (GameData._HasRotated)
				{
					Move(changeValue, 0); 
				}
				else
				{
					Move(0, changeValue); 
				}
			}
			else
			{ 
				Move((int)h, (int)v); 
			}
		}
		transform.position = new Vector3(_x + 0.5f, _y + 0.5f, 0); 
	}

	void Move(int x, int y) // 3 Enemy, 2 Pit, 1 Wall, 0 Road, 
	{
		int newX = _x + (int)x; 
		int newY = _y + (int)y; 
		int curIndex = CurIndex(newX, newY); 

		if (curIndex >= 0)
		{
//			Debug.Log("curIndex: " + curIndex + ", curMap: " + MapManager._curMap[curIndex]); 
			if (MapManager._curMap[curIndex] == '0' || MapManager._curMap[curIndex] == '9' || MapManager._curMap[curIndex] == '8')
			{
				_x = newX; 
				_y = newY; 
				ChangeStep(GameData._Step + 1); 
			}
			CalculateDirection(); // 切换下一关不要显示tip
			if (MapManager._curMap[curIndex] == '3' || MapManager._curMap[curIndex] == '2')
			{
				// do hurt
				SetHP(_curHP - 1); 
				UIManager._Instance.SetSysMsgInfo(string.Format("受到{0}点伤害！", 1)); // TODO 要设置敌人的攻击力，玩家的防御力

				// reset the map data
				char[] chars = MapManager._curMap.ToCharArray(); 
				chars[curIndex] = MapCode.NONE; 
//				Debug.Log("before: " + MapManager._curMap); 
				string s = ""; 
				for (int i = 0, count = chars.Length; i < count; ++i)
				{
					s += chars[i]; 
				}
				MapManager._curMap = s; 
//				Debug.Log("after: " + MapManager._curMap); 
				GameObject.Destroy(MapManager._gos[curIndex]); 
//				Debug.Log("current hp: " + _curHP); 
			}
		}
		else
		{
			CalculateDirection(); 
			NextLevel(GameData._CurLevel + (MapManager._curMap[CurIndex(_x, _y)] == '9' ? 1 : -1)); 
		}
	}

	void ChangeStep(int step)
	{
		GameData._Step = step; // 新的x，y必须是可以行走的才加步数 
		UIManager._Instance.SetStep(GameData._Step); 
	}

	void CalculateDirection()
	{
		byte[] constDir = new byte[4]{ _DIR_EAST, _DIR_WEST, _DIR_SOUTH, _DIR_NORTH }; 
		Vector2[] dirPos = new Vector2[4]{ new Vector2(_x + 1, _y), new Vector2(_x - 1, _y), new Vector2(_x, _y - 1), new Vector2(_x, _y + 1) }; 
		byte dir = 0; 
		for (int i = 0; i < dirPos.Length; ++i)
		{
			if (CheckWall(ref dir, constDir[i], dirPos[i]) == 0)
			{
				UIManager._Instance.SetTipInfo(Tip.GetShowTips(dir, true)); 
				if (GameData._IsShowedOpenDoorTutorial
					&& GameData._IsOpenTutorial)
				{
					GameData._IsShowedOpenDoorTutorial = false; 
					UIManager._Instance.SetSysMsgInfo(SystemMessage._tutorialString_OpenDoor); 
				}
				return; 
			}
		}
		UIManager._Instance.SetTipInfo(Tip.GetShowTips(dir)); 

		dir = 0; 
		for (int i = 0; i < dirPos.Length; ++i)
		{
			CheckPit(ref dir, constDir[i], dirPos[i]); 
		}
		string pitString = Tip.GetPitTips(dir); 
		if (!(string.IsNullOrEmpty(pitString)))
		{
			UIManager._Instance.SetTipInfo(pitString); 
		}

		dir = 0; 
		for (int i = 0; i < dirPos.Length; ++i)
		{
			CheckEnemy(ref dir, constDir[i], dirPos[i]); 
		}
		string enemyString = Tip.GetEnemyTips(dir); 
		if (!(string.IsNullOrEmpty(enemyString)))
		{
			UIManager._Instance.SetTipInfo(enemyString); 
		}
	}


	int CheckWall(ref byte dir, byte constDir, Vector2 pos)
	{
		int curIndex = CurIndex((int)pos.x, (int)pos.y); 
		if (curIndex < 0 || MapManager._curMap[curIndex] == '1')
		{
			dir |= constDir; 
			if (curIndex < 0)
			{
				dir = 0; 
				dir |= constDir; 
				return 0; // 摸到门
			}
			return 1; // 墙壁
		}
		return -1; // 空气
	}


	int CurIndex(int x, int y)
	{
		int newX = x + MapManager._width / 2; 
		int newY = y + MapManager._height / 2; 
//        Debug.Log("x: " + newX + ", y: " + newY); 
//        Debug.Log("_height: " + _height); 
		if (newX >= 0 && newX < MapManager._width
			&& newY >= 0 && newY < MapManager._height)
		{
//            Debug.Log("curIndex: " + (newX + newY * _width)); 
			return newX + newY * MapManager._width; 
		}
		return -1; 
	}

	#endregion



	#region SwitchLevel

	bool _isUpstairs;

	void InitLevel()
	{
		_isUpstairs = true;
	}

	void NextLevel(int nextLevel)
	{
		if (nextLevel > GameData._MaxLevel || nextLevel <= 0)
		{
			return; 
		}
		if (nextLevel > GameData._CurLevel)
		{
			_isUpstairs = true; 
		}
		else
		{
			_isUpstairs = false; 
		}
		GameData._CurLevel = nextLevel; 
		ChangeStep(GameData._Step + 1); 
		Reset(); 
		if (GameData._CurLevel == 2 && !GameData._CanRotateCamera)
		{
			UIManager._Instance.SetSysMsgInfo(SystemMessage._getLost); 
            GameData._CanRotateCamera = true; 
		}

		if (GameData._CurLevel == 3 && UIManager._Instance._MaxTipCount == 12)
		{
			UIManager._Instance._MaxTipCount /= 2; 
		}
	}

	#endregion


	#region Pit

	int CheckPit(ref byte dir, byte constDir, Vector2 pos)
	{
		int curIndex = CurIndex((int)pos.x, (int)pos.y); 
		if (MapManager._curMap[curIndex] == '2')
		{
			dir |= constDir; 
			return 1; 
		}
		return -1; 
	}

	#endregion


	#region Enemy

	int CheckEnemy(ref byte dir, byte constDir, Vector2 pos)
	{
		int curIndex = CurIndex((int)pos.x, (int)pos.y); 
		if (MapManager._curMap[curIndex] == '3')
		{
			dir |= constDir; 
			return 1; 
		}
		return -1; 
	}

	#endregion



	#region hitPoints

	[SerializeField] Transform _hitPointPrefab;
	Transform _hpParent;
	const int _MaxHP = 5;
	const float _hpDisplayGap = 0.17f;
	int _curHP = 5;

	void InitHP()
	{
		ResetHP(); 
	}

	void ResetHP()
	{
		SetHP(_MaxHP); 
	}

	void SetHP(int hp)
	{
		Debug.Log("SetHP: " + hp); 
		if (hp > _MaxHP)
		{
			return; 
		}
		if (hp <= 0)
		{
			Die(); 
			return; 
		}
		if (_hpParent != null)
		{
			GameObject.Destroy(_hpParent.gameObject); 
		}
		_hpParent = (new GameObject()).transform; 
		_hpParent.name = "HP Parent"; 
		_hpParent.SetParent(transform); 
		_hpParent.localPosition = Vector3.zero; 
		_hpParent.localEulerAngles = Vector3.zero; 
		float x = 0 - (int)(_MaxHP / 2f) * _hpDisplayGap; 
		float y = 0.35f; 
		for (int i = 0; i < hp; ++i)
		{
			Transform tf = GameObject.Instantiate(_hitPointPrefab); 
			tf.SetParent(_hpParent); 
			tf.localPosition = new Vector3(x, y); 
			x += _hpDisplayGap; 
		}
		_curHP = hp; 
	}

	#endregion


	#region Die
	[NonSerialized] public Action _OnClear;
	[NonSerialized] public Action _OnReset; 

	void Die()
	{
		_OnClear(); 
		InitLevel(); 
		_OnReset(); 
	}

	#endregion
}
