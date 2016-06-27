using System;
using UnityEngine;


namespace Endemics {

public interface IMonoBehaviourSingleton<T>
{
	void Awake();

	void OnDestroy();
}

}
