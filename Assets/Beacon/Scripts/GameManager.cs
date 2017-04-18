
// --功能--
// 3 怪物（位置不固定）、陷阱或危险品（位置固定）（陷阱和怪物都要扣血）
// 4 有怪物，没有武器和装备
// 5 找到孙女可以开启光明模式，要火把才行
// 6 怪物攻击，闻到敌人气息
// 7 体力下降，移动需要CD
// 8 见到少年，如果老人伤害了孙女，那么孙女应少年请求，杀掉老人

// 死亡和敌人血量
// 移动是主动攻击，站在原地不动是被动攻击


// --策划原则--
// 不需要的功能坚决砍掉，尽量简化，突核心玩法

// --BUG--
// 敌人有血条，敌人可以动，掉血的时候要提示


// --编程Tips--
// Clear和Init相互独立，Init内部不能调用Clear
// Player是一个独立于GameManager的系统
// 父级调用子级函数，但是子级只能通过时间来通知父级调用函数



using UnityEngine;
using System.Collections;
using System; 

public class GameManager : MonoBehaviour
{
	public static GameManager _Instance; 

    void Start()
    {
        Init(); 
    }

    void Init() 
    {
		_Instance = this; 
		GameData._Instance.Init(); 
		UIManager._Instance.Init(); 

		Player._Instance.Init(); 
		Player._Instance._OnReset = Reset; 
		Player._Instance._OnClear = Clear; 
    }

    public void Reset()
    {
		GameData._Instance.Reset(); 
		Player._Instance.Reset(); 
		UIManager._Instance.Reset(); 
    }

	void OnDestroy()
	{
		Clear(); 
	}

	public void Clear()
	{
		GameData._Instance.Clear(); 
		UIManager._Instance.Clear(); 
		Player._Instance.Clear(); 
	}
}
