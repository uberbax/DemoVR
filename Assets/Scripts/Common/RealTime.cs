using System;
using UnityEngine;

using Endemics;

[PersistentSingleton]
public sealed class RealTime
	: MonoBehaviourSingleton<RealTime>
	, ISingletonInitialization
{
	float m_prevTime = 0.0f;
	float m_dt = 0.0f;


	//---------------------------------------------------------------------------------------------
	public float DeltaTime
	{
		get
		{
			return m_dt;
		}
	}

	//---------------------------------------------------------------------------------------------
	void LateUpdate()
	{
		var time = Time.realtimeSinceStartup;
		m_dt = time - m_prevTime;
		m_prevTime = time;
	}

	//---------------------------------------------------------------------------------------------
	void ISingletonInitialization.AwakeSingleton()
	{
		m_prevTime = Time.realtimeSinceStartup;
	}
}

