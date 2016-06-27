using System;
using System.Collections.Generic;
using UnityEngine;

namespace Endemics {

public static class Seq
{
	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> Empty<X>()
	{
		yield break;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<Y> Map<X,Y>( this IEnumerable<X> seq, Func<X,Y> map )
	{
		foreach( var e in seq )
			yield return map(e);
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<Y> MapMany<X,Y>( this IEnumerable<X> seq, Func<X,IEnumerable<Y>> map )
	{
		foreach( var e in seq )
		{
			var mappedSeq = map(e);
			if( mappedSeq != null )
			{
				foreach( var mappedElement in mappedSeq )
					yield return mappedElement;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> Unique<X>( this IEnumerable<X> seq, Func<X,X,bool> equalityFunc )
	{
		var equals = equalityFunc == null ? (e1,e2) => e1.Equals(e2) : equalityFunc;
		var unique = new List<X>();

		foreach( var e in seq )
			if( !unique.Exists( existing => equals( existing, e ) ) )
			{
				unique.Add( e );
				yield return e;
			}
	}

	public static IEnumerable<X> Unique<X>( this IEnumerable<X> seq  )
	{
		return Unique( seq, null );
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> Filter<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		foreach( var e in seq )
			if( predicate(e) )
				yield return e;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<Y> FilterAndMap<X,Y>( this IEnumerable<X> seq, Func<X,bool> predicate, Func<X,Y> map )
	{
		foreach( var e in seq )
			if( predicate(e) )
				yield return map(e);
	}

	//-----------------------------------------------------------------------------------------------
	public static List<X> MakeList<X>( this IEnumerable<X> seq )
	{
		return new List<X>( seq );
	}

	//-----------------------------------------------------------------------------------------------
	public static X[] MakeArray<X>( this IEnumerable<X> seq )
	{
		return seq.MakeList().ToArray();
	}
	//-----------------------------------------------------------------------------------------------
	public static bool ElementExists<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		if( seq is List<X> )
			return ((List<X>)seq).Exists( e => predicate(e) );

		if( seq is X[] )
			return Array.Exists<X>( (X[])seq, ( e => predicate(e) ) );
		
		foreach( var e in seq )
			if( predicate(e) )
				return true;
	
		return false;
	}
	
	//-----------------------------------------------------------------------------------------------
	public static X FindFirstIf<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		foreach( var e in seq )
			if( predicate(e) )
				return e;
		
		return default(X);
	}

	//-----------------------------------------------------------------------------------------------
	public static X GetFirstIf<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		foreach( var e in seq )
			if( predicate(e) )
				return e;
		
		throw new Exception( "Element not found in seq<{0}>".Fmt( typeof(X) ) );
	}

	//-----------------------------------------------------------------------------------------------
	public static bool All<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		var isEmpty = true;

		foreach( var e in seq )
		{
			isEmpty = false;
			if( !predicate(e) )
				return false;
		}

		return !isEmpty;
	}

	//-----------------------------------------------------------------------------------------------
	public static bool Any<X>( this IEnumerable<X> seq, Func<X,bool> predicate )
	{
		foreach( var e in seq )
			if( predicate(e) )
				return true;

		return false;
	}

	//-----------------------------------------------------------------------------------------------
	public static X Accum<X>( this IEnumerable<X> seq, X initialValue, Func<X,X,X> accumulator )
	{
		var result = initialValue;
		foreach( var e in seq )
			result = accumulator( result, e );
		
		return result;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> Concat<X>( this IEnumerable<X> seq, IEnumerable<X> seq2 )
	{
		if( seq2 == null )
			throw new ArgumentNullException( "seq2" );

		foreach( var e in seq )
			yield return e;

		foreach( var e in seq2 )
			yield return e;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> Append<X>( this IEnumerable<X> seq, params X[] elements )
	{
		foreach( var e in seq )
			yield return e;
		
		for( var i = 0; i < elements.Length; ++i )
			yield return elements[i];
	}

	//-----------------------------------------------------------------------------------------------
	public static void Iter<X>( this IEnumerable<X> seq, Action<X> act )
	{
		var list = seq as IList<X>;
		if( list != null )
		{
			for( var i = 0; i < list.Count; ++i )
				act( list[i] );
		}
		else
			foreach( var e in seq )
				act(e);
	}

	//-----------------------------------------------------------------------------------------------
	public static void Iteri<X>( this IEnumerable<X> seq, Action<X,int> act )
	{
		var list = seq as IList<X>;
		if( list != null )
		{
			for( var i = 0; i < list.Count; ++i )
				act( list[i], i );
		}
		else
		{
			var counter = 0;
			foreach( var e in seq )
				act(e, counter++);
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static X First<X>( this IEnumerable<X> seq )
	{
		var enumerator = seq.GetEnumerator();
		if( !enumerator.MoveNext() )
			throw new Exception( "Sequence is empty !" );

		return enumerator.Current;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<X> First<X>( this IEnumerable<X> seq, int count)
	{
		int counter = 0;
		foreach( var e in seq )
			if( ++counter <= count )
				yield return e;
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<T> MakeSeq<T>( params T[] elements )
	{
		for( var i = 0; i < elements.Length; ++i )
			yield return elements[i];
	}

	//-----------------------------------------------------------------------------------------------
	public static IEnumerable<T> MakeSeq<T>( T element )
	{
		yield return element;
	}

	//-----------------------------------------------------------------------------------------------
	public static int GetCount<X>( this IEnumerable<X> seq )
	{
		var asList = seq as IList<X>;
		if( asList != null )
			return asList.Count;

		int counter = 0;
		foreach( var e in seq )
			++counter;

		return counter;
	}

}


}