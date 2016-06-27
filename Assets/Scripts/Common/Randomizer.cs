using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace Endemics {

public static class Randomizer
{
	//------------------------------------------------------------------------------------------------
	public static int[] RandomizedIndices( int indicesCount )
	{
		int[] indices = new int[indicesCount];
		List<int> freeIndices = new List<int>();
		for( int i = 0; i < indices.Length; ++i )
			freeIndices.Add( i );

		if( indicesCount == 1 )
		{
			indices[0] = 0;
		}
		else
		{
			System.Random rnd = new System.Random(DateTime.Now.Millisecond );

			for( int i = 0; i < indices.Length; ++i )
			{
				int newIndex = rnd.Next(indices.Length);
			
				int attemptCounter = 10;
				bool acceptIndex = false;

				do
				{
					int listIndex = freeIndices.IndexOf( newIndex );
					if( listIndex < 0 )
						--attemptCounter;
					else
					{
						acceptIndex = true;
						freeIndices.RemoveAt( listIndex );
					}
				
				} while( !acceptIndex && attemptCounter > 0 );

				if( !acceptIndex )
				{
					newIndex = freeIndices[0];
					freeIndices.RemoveAt(0);
				}
			
				indices[i] = newIndex;
			}
		}

		return indices;
	}
//------------------------------------------------------------------------------------------------
	public static int[] RandomizedIndicesEx( int count )
	{
		int[] indices = new int[count];
		List<int> used = new List<int>();
		int maxAttemptsCount = 5;

		Func<int> getAnyUnusedIndex = () =>
		{
			int index = 0;
			for( ; index < count; ++index )
				if( !used.Contains(index) )
					break;
			
			return index;
		};

		Func<int, int> getNewIndex = id =>
		{
			int attempt = maxAttemptsCount;
			int index ;
			bool found = false;

			do
			{
				index = UnityEngine.Random.Range( 0, count );
				if( index == id || used.Contains( index ) )
					--attempt;
				else
					found = true;
			} while( attempt > 0 && !found ) ;

			if( !found )
				index = getAnyUnusedIndex();
			
			used.Add( index );
		
			return index;
		};


		for( int i = 0; i < indices.Length; ++i )
			indices[i] = getNewIndex(i);

		return indices;
	}

	//------------------------------------------------------------------------------------------------
	public static string PermutateString( string inputString )
	{
		int[] indices = RandomizedIndices( inputString.Length );
	
		StringBuilder result = new StringBuilder();
		for( int i = 0; i < indices.Length; ++i )
			result.Append( inputString[indices[i]] );

		string newString = result.ToString();
		if(newString.Length != inputString.Length )
			throw new Exception( string.Format( "Incorrect behaviour when permutate string '{0}'", inputString ) );

		return (newString != inputString ) ? newString : PermutateString( inputString );
	}

	//------------------------------------------------------------------------------------------------
	public static List<T> PermutateList<T>( List<T> inputList )
	{
		int[] indices = RandomizedIndices( inputList.Count );
		List<T> result = new List<T>();

		foreach( int index in indices )
			result.Add( inputList[index] );

		return result;
	}

	//------------------------------------------------------------------------------------------------
	public static IEnumerator<T> CreateRandomGenerator<T>( int count, Func<int,T> getFunc )
	{
		int[] indices = RandomizedIndices( count );
		int index = 0;
		int lastUsedIndex = -1;

		while( true )
		{
			lastUsedIndex = indices[index];

			yield return getFunc(lastUsedIndex);
			if( ++index == indices.Length )
			{
				index = 0;
				indices = Randomizer.RandomizedIndices( count );

				if( count > 1 && lastUsedIndex == indices[0] )
				{ // что бы при новой последовательности - старый последний индекс не совпадал с новым первым ( для неразрывной последовательности разных событий);
					indices[0] = indices[1];
					indices[1] = lastUsedIndex;
				}
			}
		}
	}

	//------------------------------------------------------------------------------------------------
	public static Func<T> CreateRandomGenerator<T>( IEnumerable<T> seq )
	{
		var instances = new List<T>( seq );
		var generator = CreateRandomGenerator<T>( instances.Count, index => instances[index] );

		Func<T> f = () =>
		{
			if( !generator.MoveNext() )
				throw new Exception( "Fail to generate next instance with infinite random generator" );

			return generator.Current;
		};

		return f;
	}

	//------------------------------------------------------------------------------------------------
	public static T ChooseAny<T>( this List<T> list )
	{
		if( list == null || list.Count == 0)
			return default(T);

		return list[UnityEngine.Random.Range(0,list.Count)];
	}

	//------------------------------------------------------------------------------------------------
	public static T ChooseAny<T>( this T[] array )
	{
		if( array == null || array.Length == 0)
			return default(T);

		return array[UnityEngine.Random.Range(0,array.Length)];
	}

	//------------------------------------------------------------------------------------------------
	public static int ChooseIndexWithProbability<T>( T []items, Func<T,float> getProbability )
	{
		Func< T[], float[] > getNormalizedProbabilities = (input) =>
		{
			var output = new float[input.Length];
			float sum = 0.0f;
			
			for( int i = 0; i < input.Length; ++i )
				sum = sum + getProbability(input[i]);

			for( int i = 0; i < output.Length; ++i )
				output[i] = getProbability(input[i]) / sum;

			return output;
		};

		Func< float[], int, KeyValuePair<int,int>[]> getProbabilityRanges = (probabilities, maxRange ) =>
		{
			var ranges = new KeyValuePair<int,int>[probabilities.Length];

			int startRange = 1;

			for( int i = 0; i < probabilities.Length; ++i )
			{
				var p = probabilities[i];
				if( p > 0.0f )
				{
					var endRange = startRange + Mathf.RoundToInt( (float)maxRange * p );

					ranges[i] = new KeyValuePair<int,int>( startRange, endRange );
					startRange = endRange + 1;
				}
				else
				{
					ranges[i] = new KeyValuePair<int,int>( 0, 0);
				}
			}

			return ranges;
		};

		Func<KeyValuePair<int,int>[], int, int> getRangedIndex = (ranges,value) =>
		{
			int index = 0;

			for( ; index < ranges.Length; ++index )
			{
				int start = ranges[index].Key;
				int end = ranges[index].Value;
				if( start <= value && value <= end )
					break;
			}

			return index;
		};

		int probabilityRangeMax = items.Length * 100;

		var normalizedProbabilities = getNormalizedProbabilities( items );
		var ranges1 = getProbabilityRanges( normalizedProbabilities, probabilityRangeMax );

		int anyRangeValue = UnityEngine.Random.Range( 0, probabilityRangeMax + 1 );
		return getRangedIndex( ranges1, anyRangeValue );
	}


	//------------------------------------------------------------------------------------------------
	public static List<T> CollectItemsWithProbability<T>( T []items, int count, Func<T,float> getProbability)
	{
		if( items.Length == count )
			return PermutateList<T>( new List<T>( items ) );

		if( items.Length < count )
		{
			throw new ArgumentException( "Can't CollectItemsWithProbability( [], {0}, f ) because items only = {1}".Fmt( count, items.Length ) );
		}

		var result = new List<T>();
		var currentItems = new List<T>( items );

		do
		{
			int index = ChooseIndexWithProbability( currentItems.ToArray(), getProbability );

			result.Add( currentItems[index] );
			currentItems.RemoveAt( index );

		} while( result.Count < count )
		;
		
		return result;
	}
}

}
