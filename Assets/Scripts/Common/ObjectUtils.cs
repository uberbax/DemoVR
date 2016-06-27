using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Endemics {

public static class ObjectUtils
{
	//-----------------------------------------------------------------------------------------------
	public static bool HasInterface<T>( GameObject go ) where T : class
	{
		foreach( var c in go.GetComponents<Component>() )
			if( c is T )
				return true;
		
		return false;
	}

	//-----------------------------------------------------------------------------------------------
	public static T GetInterface<T>( GameObject go ) where T : class
	{
		foreach( var c in go.GetComponents<Component>() )
			if( c is T )
				return c as T;
		
		return null;
	}

	//-----------------------------------------------------------------------------------------------
	public static T[] GetAllInterfaces<T>( GameObject go ) where T : class
	{
		List<T> interfaces = new List<T>();
		foreach( var c in go.GetComponents<Component>() )
			if( c is T )
				interfaces.Add(c as T);
		
		return interfaces.ToArray();
	}

	//-----------------------------------------------------------------------------------------------
	public static T[] GetAllInterfacesWithChildren<T>( GameObject go ) where T : class
	{
		List<T> interfaces = new List<T>();
		foreach( var c in go.GetComponentsInChildren<Component>() )
			if( c is T )
				interfaces.Add(c as T);
		
		return interfaces.ToArray();
	}

	//-----------------------------------------------------------------------------------------------
	public static void AccessInterface<T>( GameObject o, Action<T> func) where T:class
	{
		if( o != null )
		{
			T itf = GetInterface<T>(o);
			if( itf != null )
				func( itf );
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static void AccessAllInterfaces<T>( GameObject o, Action<T> func) where T:class
	{
		if( o != null )
		{
			foreach( T itf in GetAllInterfaces<T>( o ) )
				func(itf);
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static T GetOrAddComponent<T>( GameObject go ) where T : Component
	{
		var c = go.GetComponent<T>();
		return c == null ? go.AddComponent<T>() : c;
	}

	//-----------------------------------------------------------------------------------------------
	public static void SafeAccessComponent<T>( GameObject go, Action<T> func ) where T : Component
	{
		if( go == null )
			return;

		var c = go.GetComponent<T>();
		if( c != null )
			func(c);
		else
			Debug.LogWarning( "Attempt to access to component [{0}], but it isn't exsists on [{1}]".Fmt( typeof(T), go ) );
	}

	//-----------------------------------------------------------------------------------------------
	public static void SafeAccessComponentNw<T>( GameObject go, Action<T> func ) where T : Component
	{
		if( go == null )
			return;

		var c = go.GetComponent<T>();
		if( c != null )
			func(c);
	}
}

}
