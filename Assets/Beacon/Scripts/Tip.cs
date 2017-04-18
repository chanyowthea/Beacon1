using UnityEngine;
using System.Collections;

public class Tip : MonoBehaviour
{
	const string _touchDoorFormat = "你触碰到{0}墙壁上有扇门！";
	const string _tipFormat = "{0}是墙壁，你感受到风从{1}吹来。";
	const string _tipString_AllSide = "你感受到风从四面吹来。";
	static string[] _directions = new string[]{ "东边", "西边", "南边", "北边" };
	static string[] _directions_Rotated = new string[]{ "右边", "左边", "后面", "前面" };

	public static string GetShowTips(byte direct, bool isDoor = false)
	{
		string[] directions = _directions; 
		if (GameData._CanRotateCamera)
		{
			directions = _directions_Rotated; 
			int times = (int)(Camera.main.transform.eulerAngles.z / (Mathf.Rad2Deg * Mathf.PI / 2f)); 
			//            Debug.Log("times: " + times); 
			if (times == 1)
			{
				directions = new string[4]{ _directions_Rotated[2], _directions_Rotated[3], _directions_Rotated[1], _directions_Rotated[0] }; 
			}
			else if (times == 2)
			{
				directions = new string[4]{ _directions_Rotated[1], _directions_Rotated[0], _directions_Rotated[3], _directions_Rotated[2] }; 
			}
			else if (times == 3)
			{
				directions = new string[4]{ _directions_Rotated[3], _directions_Rotated[2], _directions_Rotated[0], _directions_Rotated[1] }; 
			}
		}

		string wallSides = ""; 
		string windSides = ""; 
		for (int i = 0; i < directions.Length; ++i)
		{
			if ((direct & 1 << i) != 0)
			{
				if (!(string.IsNullOrEmpty(wallSides)))
				{
					wallSides += "，"; 
				}
				wallSides += directions[i]; 
			}
			else
			{
				if (!(string.IsNullOrEmpty(windSides)))
				{
					windSides += "，"; 
				}
				windSides += directions[i]; 
			}
		}

		if (string.IsNullOrEmpty(wallSides)) // 四面都没有墙壁
		{
			return _tipString_AllSide; 
		}
		else if (isDoor) // 触碰到门
		{
			return string.Format(_touchDoorFormat, wallSides); 
		}
		else // 默认情况
		{
			return string.Format(_tipFormat, wallSides, windSides); 
		}
	}



	const string _pitString = "你踩到些许松散的泥土，{0}可能有陷阱！";

	public static string GetPitTips(byte direct)
	{
		string[] directions = _directions; 
		if (GameData._CanRotateCamera)
		{
			directions = _directions_Rotated; 
			int times = (int)(Camera.main.transform.eulerAngles.z / (Mathf.Rad2Deg * Mathf.PI / 2f)); 
			//            Debug.Log("times: " + times); 
			if (times == 1)
			{
				directions = new string[4]{ _directions_Rotated[2], _directions_Rotated[3], _directions_Rotated[1], _directions_Rotated[0] }; 
			}
			else if (times == 2)
			{
				directions = new string[4]{ _directions_Rotated[1], _directions_Rotated[0], _directions_Rotated[3], _directions_Rotated[2] }; 
			}
			else if (times == 3)
			{
				directions = new string[4]{ _directions_Rotated[3], _directions_Rotated[2], _directions_Rotated[0], _directions_Rotated[1] }; 
			}
		}

		string pitSides = ""; 
		for (int i = 0; i < directions.Length; ++i)
		{
			if ((direct & 1 << i) != 0)
			{
				if (!(string.IsNullOrEmpty(pitSides)))
				{
					pitSides += "，"; 
				}
				pitSides += directions[i]; 
			}
		}
		if (string.IsNullOrEmpty(pitSides))
		{
			return ""; 
		}
		return string.Format(_pitString, pitSides); 
	}



	//    const string _enemyString = "你听到细微的脚步声，{0}一米处可能有敌人！"; // 可以更改敌人检测距离
	const string _enemyString_Beside = "你察觉到危险靠近，{0}可能有敌人！";

	public static string GetEnemyTips(byte direct)
	{
		string[] directions = _directions; 
		if (GameData._CanRotateCamera)
		{
			directions = _directions_Rotated; 
			int times = (int)(Camera.main.transform.eulerAngles.z / (Mathf.Rad2Deg * Mathf.PI / 2f)); 
			//            Debug.Log("times: " + times); 
			if (times == 1)
			{
				directions = new string[4]{ _directions_Rotated[2], _directions_Rotated[3], _directions_Rotated[1], _directions_Rotated[0] }; 
			}
			else if (times == 2)
			{
				directions = new string[4]{ _directions_Rotated[1], _directions_Rotated[0], _directions_Rotated[3], _directions_Rotated[2] }; 
			}
			else if (times == 3)
			{
				directions = new string[4]{ _directions_Rotated[3], _directions_Rotated[2], _directions_Rotated[0], _directions_Rotated[1] }; 
			}
		}

		string enemySides = ""; 
		for (int i = 0; i < directions.Length; ++i)
		{
			if ((direct & 1 << i) != 0)
			{
				if (!(string.IsNullOrEmpty(enemySides)))
				{
					enemySides += "，"; 
				}
				enemySides += directions[i]; 
			}
		}
		if (string.IsNullOrEmpty(enemySides))
		{
			return ""; 
		}
		return string.Format(_enemyString_Beside, enemySides); 
	}

}
