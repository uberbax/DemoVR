using System;
using System.IO;
using System.Reflection;

using UnityEngine;

namespace Endemics {

using Serialization;

/**************************************************************************************************
**************************************************************************************************/
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class EdittimeDataAttribute : Attribute
{
	public string Name { get; private set; }

	public EdittimeDataAttribute( string name )
	{
		Name = name;
	}
}

/**************************************************************************************************
**************************************************************************************************/
public static class EdittimeUtils
{
	public static void InitializeObject( object o )
	{
		if( o == null )
			throw new ArgumentNullException( "obj" );

		var type = o.GetType();

		Func<string,string> getDataPath = dataName =>
		{
			var path = PathUtils.Combine( Application.dataPath, "_edittime", "{0}.json".Fmt(dataName) );
			return path;
		};


		foreach( var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ) )
		{
			var attr = ReflectionUtils.GetCustomAttribute<EdittimeDataAttribute>( prop );
			if( attr == null )
				continue;

			var setMethod = prop.GetSetMethod() ?? prop.GetSetMethod(true);
			if( setMethod == null )
			{
				Debug.LogWarning( "No setter {0}.{1}".Fmt( type.Name, prop.Name ) );
				continue;
			}

			var valueIsSet = false;
			if( Application.isEditor )
			{
				var path = getDataPath( attr.Name );
				if( File.Exists( path ) )
				{
					var content = File.ReadAllText( path );
					var value = JsonSerializer.DeserializeFromString( content, prop.PropertyType );
					valueIsSet = true;

					setMethod.Invoke( o, new object[] { value } );
				}
				else
					Debug.LogWarning( "File not found : {0}. Can't resore {1}.{2}".Fmt( path, type.Name, prop.Name ) );
			}
			
			if( !valueIsSet )
			{
				var value = Activator.CreateInstance( prop.PropertyType );
				setMethod.Invoke( o, new object[] { value } );
			}
		}

	}

}

}
