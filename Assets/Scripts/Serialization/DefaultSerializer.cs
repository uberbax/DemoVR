using System;
using UnityEngine;


namespace Endemics.Serialization {

public sealed class DefaultSerializer
{
	public static DefaultSerializer Instance
	{
		get { return Singleton<DefaultSerializer>.Instance ; }
	}

	public string SerializeToString( object o )
	{
		return JsonSerializer.Serialize( o, false );
	}

	public object Deserilaize( string serializedState, Type type )
	{
		return JsonSerializer.DeserializeFromString( serializedState, type );
	}

	public T Deserilaize<T>( string serializedState )
	{
		var o = Deserilaize( serializedState, typeof(T) );
		if( !(o is T) )
			throw new InvalidOperationException( "Fail to deserialize to type: {0}".Fmt( typeof(T) ) );

		return (T)o;
	}
}

}
