using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	IEnumerator _updateRoutine; 

	public void Init(Pos pos)
	{
		_x = pos._x; 
		_y = pos._y; 
		if (_updateRoutine != null)
		{
			StopCoroutine(_updateRoutine); 
			_updateRoutine = null; 
		}
		_updateRoutine = UpdateRoutine(); 
		StartCoroutine(_updateRoutine);
		InitHP(); 
	}

	void Clear()
	{
		if (_updateRoutine != null)
		{
			StopCoroutine(_updateRoutine); 
			_updateRoutine = null; 
		}
	}

	IEnumerator UpdateRoutine()
	{
		System.Random random = new System.Random(); 
		Pos[] coordinates = new Pos[4]{new Pos(-1, 0), new Pos(1, 0), new Pos(0, -1), new Pos(0, 1)}; 

		while(true)
		{
			yield return null; 
			if ((int)(random.Next(20)) == 0)
			{
				Debug.Log("Enemy Move"); 

				int index = random.Next(0, 4); 
				Pos pos = coordinates[index]; 
				Move(pos._x, pos._y); 
			}
		}
	}

	void OnDestroy()
	{
		Clear(); 
	}
	#region hitPoints

//	[SerializeField] Transform _hitPointPrefab;
	Transform _hpParent;
	const int _MaxHP = 2;
	const float _hpDisplayGap = 0.17f;
	int _curHP = 5;

	void InitHP()
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
			Transform tf = GameObject.Instantiate(GameData._Instance._hitPointPrefab); 
			tf.SetParent(_hpParent); 
			tf.localPosition = new Vector3(x, y); 
			x += _hpDisplayGap; 
		}
		_curHP = hp; 
	}

	#endregion


	#region Die
	void Die()
	{

	}

	#endregion



	#region Move
	int _x; 
	int _y; 
	void Move(int x, int y) // 3 Enemy, 2 Pit, 1 Wall, 0 Road, 
	{
		int newX = _x + (int)x; 
		int newY = _y + (int)y; 
		int curIndex = CurIndex(newX, newY); 

		if (curIndex >= 0)
		{
			Debug.Log("x, y: " + x + ", " + y); 
//			Debug.Log("curIndex: " + curIndex + ", curMap: " + MapManager._curMap[curIndex]); 
			if (MapManager._curMap[curIndex] == MapCode.NONE || MapManager._curMap[curIndex] == MapCode.BEFORE_UPSTAIR 
				|| MapManager._curMap[curIndex] == MapCode.BEFORE_DOWNSTAIR)
			{
				_x = newX; 
				_y = newY; 
			}
			if (MapManager._curMap[curIndex] == MapCode.PIT)
			{
				SetHP(_curHP - 1); 
//				Debug.Log("current hp: " + _curHP); 
			}
			transform.position = new Vector3(_x + 0.5f, _y + 0.5f, 0); 
		}
	}

	int CurIndex(int x, int y)
	{
		int newX = x + MapManager._width / 2; 
		int newY = y + MapManager._height / 2; 
		if (newX >= 0 && newX < MapManager._width
			&& newY >= 0 && newY < MapManager._height)
		{
			return newX + newY * MapManager._width; 
		}
		return -1; 
	}


	#endregion
}
