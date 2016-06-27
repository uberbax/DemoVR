using System;
using System.Collections;
using UnityEngine;


namespace Endemics {

public static class CoroutineUtils
{

	public static IEnumerator WaitPredicate_Routine( Func<bool> predicate, System.Action action )
	{
		while( !predicate() )
		{
			if( action != null )
				action();
			yield return null;
		}
	}

	public static Coroutine WaitPredicate( MonoBehaviour host, Func<bool> predicate, System.Action action )
	{
		return host.StartCoroutine( WaitPredicate_Routine( predicate, action ) );
	}

	public static Coroutine WaitPredicate( MonoBehaviour host, Func<bool> predicate )
	{
		return WaitPredicate( host, predicate, null );
	}


}


}