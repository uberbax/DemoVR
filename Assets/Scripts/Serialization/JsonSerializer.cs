using System;

//using System.IO;
using System.Text;
using UnityEngine;

using Newtonsoft.Json;

namespace Endemics.Serialization {


public static class JsonSerializer
{
	public static string Serialize( object o, bool format )
	{
		var json = JsonConvert.SerializeObject( o, format ? Formatting.Indented : Formatting.None );
		return json;
	}

	public static object DeserializeFromString( string json, Type t )
	{
		return JsonConvert.DeserializeObject( json, t );
	}

	public static T DeserializeFromString<T>( string json )
	{
		return JsonConvert.DeserializeObject<T>( json );
	}

	public static T DeserializeAsJson<T>( this string jsonString )
	{
		return DeserializeFromString<T>(jsonString);
	}
}

}
