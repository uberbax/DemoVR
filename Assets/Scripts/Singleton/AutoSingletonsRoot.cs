using System;
using UnityEngine;

public class AutoSingletonsRoot : MonoBehaviour
{
	bool m_isPersistent = false;

	public static Transform GetRoot(bool isPersistent)
	{
		AutoSingletonsRoot root = null;

		foreach( var o in (AutoSingletonsRoot[])GameObject.FindObjectsOfType( typeof(AutoSingletonsRoot) ) )
			if( o.IsPersistent == isPersistent )
			{
				root = o;
				break;
			}

		if( root == null )
		{
			var o = new GameObject( isPersistent ? "Persistent Singletons" : "Singletons", typeof(AutoSingletonsRoot) );
			root = o.GetComponent<AutoSingletonsRoot>();
			if( isPersistent )
				root.MakePersistent();
		}

		return root.transform;
	}

	public bool IsPersistent
	{
		get { return m_isPersistent; }
	}

	public void MakePersistent()
	{
		m_isPersistent = true;
		GameObject.DontDestroyOnLoad(gameObject);
	}
}

