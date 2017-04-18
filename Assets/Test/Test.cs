using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class Test : MonoBehaviour
{
	System.Random rand = new System.Random(); 

	void Start()
	{
		for (int i = 0; i < 10; ++i)
		{
			Debug.Log(rand.Next(-1, 2)); 
		}
	}
}
