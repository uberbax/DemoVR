using System;
using UnityEngine;

using Endemics;

public class MonoBehaviourSingleton<T> 
	: MonoBehaviour
	, IMonoBehaviourSingleton<T>
	where T : MonoBehaviourSingleton<T>
{
	public static T Instance
	{
		get
		{
			return Singleton<T>.Instance;
		}
	}

	public static bool Exists
	{
		get
		{
			return Singleton<T>.Exists;
		}
	}

	public static void Touch()
	{
		Singleton<T>.Touch();
	}

	public void Awake()
	{
		Singleton<T>.Awake( (T)this );
	}

	public void OnDestroy()
	{
		Singleton<T>.OnDestroy( (T)this );
	}
}

