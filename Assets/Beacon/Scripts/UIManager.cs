using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Main
    public static UIManager _Instance; 

	void Awake()
	{
		_Instance = this; 
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ClearTip();
        }
        if (!(string.IsNullOrEmpty(_tipText.text)))
        {
            TipShade(); 
        }
        if (!(string.IsNullOrEmpty(_sysText.text)))
        {
            SysMsgShade(); 
        }
    }

    public void Init() 
    {
        Reset(); 
		InitStep(); 
    }

    public void Reset()
    {
        InitLevel(); 
        InitTip(); 
        InitSysMsg(); 
		InitCompass(); 
    }

	public void Clear()
	{
		
	}
	#endregion



    #region Tip

    [HideInInspector] public int _MaxTipCount = 12; 
    [SerializeField] Text _tipText; 
    int _curTipCount; 
    IEnumerator _tipShadeRoutine; 
    float _time = 0; 
    float _maxTime = 5f; 

    void InitTip()
    {
        _tipText.text = ""; 
        _curTipCount = 0; 
        _time = 0; 
    }

    public void SetTipInfo(string info) 
    {
        if (string.IsNullOrEmpty(info))
        {
            return; 
        }

        string s = _tipText.text; 
        if (_curTipCount >= _MaxTipCount)
        {
            s = TipBoost(s); 
        }
        else
        {
            ++_curTipCount; 
        }
        _tipText.text = s + (string.IsNullOrEmpty(s) ? "" : "\n") + info; 
    }

    void ClearTip()
    {
        InitTip(); 
    }

    void TipShade()
    {
        _time += Time.deltaTime; 
        if (_time > _maxTime)
        {
            --_curTipCount; 
            _time = 0; 
            _tipText.text = TipBoost(_tipText.text); 
        }
    }

    string TipBoost(string s)
    {
        string[] tips = s.Split('\n'); 
        string totalTip = ""; 
        for (int i = 1; i < tips.Length; ++i)
        {
            totalTip += (string.IsNullOrEmpty(totalTip) ? "" : "\n") + tips[i]; 
        }
        return totalTip; 
    }
    #endregion


    #region SystemMsg

	// 系统相关的提示：例如可以改变方向、扣血等在这里
	// 普通的移动提示在左上角

    [SerializeField] Text _sysText; 
    float _sysMsgTime = 0; 
    const float _maxSysMsgTime = 3f; 

    void InitSysMsg()
    {
        _sysText.text = ""; 
        _sysMsgTime = 0; 
    }

    void SysMsgShade()
    {
        _sysMsgTime += Time.deltaTime; 
        if(_sysMsgTime >= _maxSysMsgTime)
        {
            InitSysMsg(); 
        }
    }

    public void SetSysMsgInfo(string info)
    {
        if (string.IsNullOrEmpty(info))
        {
            return; 
        }
        _sysText.text = info; 
    }
    #endregion


    #region Level
    [SerializeField] Text _levelText; 
    string _levelFormat = "Floor {0}";

    void InitLevel()
    {
        SetCurLevel(GameData._CurLevel); 
    }

    public void SetCurLevel(int curLevel)
    {
        _levelText.text = string.Format(_levelFormat, curLevel); 
    }
    #endregion


	#region Step
	[SerializeField] Text _stepText; 
	string _stepFormat = "Step {0}"; 

	public void InitStep()
	{
		SetStep(0); 
	}

	public void SetStep(int step)
	{
		_stepText.text = string.Format(_stepFormat,step); 
	}
	#endregion

	#region Compass
	[SerializeField] Image _compassImg; 

	public void ChangeCompassDir(Vector3 angle)
	{
		_compassImg.transform.eulerAngles += angle; 
	}

	void InitCompass()
	{
		_compassImg.transform.localEulerAngles = Vector3.zero; 
	}
	#endregion
}
