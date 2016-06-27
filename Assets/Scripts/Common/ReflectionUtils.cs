using System;
using System.Collections.Generic;
using System.Reflection;

#if NETFX_CORE
using System.Linq;
#endif

namespace Endemics {


public static class ReflectionUtils
{
#if !NETFX_CORE
	public static IEnumerable<Assembly> GetAvailableAssemblies()
	{
		foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
		{
			var assemblyName = assembly.GetName().FullName;

			if( !(assemblyName.StartsWith( "System" ) || assemblyName.StartsWith( "Unity") || assemblyName.StartsWith( "Mono" )) )
				yield return assembly;
		}
	}

	//---------------------------------------------------------------------------------------------
	public static T GetCustomAttribute<T>( ICustomAttributeProvider type ) where T : Attribute
	{
		if( type == null )
			throw new ArgumentNullException( "type" );

		var attribs = type.GetCustomAttributes( typeof(T), true );
		return attribs.Length == 0 ? null : (T)attribs[0];
	}

	//---------------------------------------------------------------------------------------------
	public static IEnumerable<MethodInfo> GetAllMethods( Type type, System.Reflection.BindingFlags flags )
	{
		var currentType = type;

		while( currentType != null )
		{
			var typeMethods = currentType.GetMethods(flags);
			foreach( var methodInfo in typeMethods )
				yield return methodInfo;

			currentType = currentType.BaseType;
		}
	}

	//---------------------------------------------------------------------------------------------
	public static IEnumerable<PropertyInfo> GetAllProperties( Type type, System.Reflection.BindingFlags flags )
	{
		var currentType = type;

		while( currentType != null )
		{
			var properties = currentType.GetProperties(flags);
			foreach( var property in properties )
				yield return property;

			currentType = currentType.BaseType;
		}
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsGenericType( Type type )
	{
		return type.IsGenericType;
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsValueType( Type type )
	{
		return type.IsValueType;
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsEnum( Type type )
	{
		return type.IsEnum;
	}

	//---------------------------------------------------------------------------------------------
	public static Type[] GetGenericArguments( Type type )
	{
		return type.GetGenericArguments();
	}

	//---------------------------------------------------------------------------------------------
	public static Type GetGenericTypeDefinition( Type type )
	{
		return type.GetGenericTypeDefinition();
	}
	
	//---------------------------------------------------------------------------------------------
	public static bool IsAssignableFrom( Type type, Type type2 )
	{
		return type.IsAssignableFrom( type2 );
	}

	//---------------------------------------------------------------------------------------------
	public static bool SupportsInterface<T>( Type type )
	{
		return type.GetInterface( typeof(T).Name ) != null;
	}

	//---------------------------------------------------------------------------------------------
	public static Type FindInterface( Type type, Func<Type,bool> predicate )
	{
		foreach( var interfaceType in type.GetInterfaces() )
			if( predicate( interfaceType ) )
				return interfaceType;

		return null;
	}

	//---------------------------------------------------------------------------------------------
	//public static IEnumerable<Type> FindAllTypesWithAttribute<T>( IEnumerable<Asse


	
#else
	public static IEnumerable<Assembly> GetAvailableAssemblies()
	{
		var runtimeAssembly = typeof(ReflectionUtils).GetTypeInfo().Assembly;
		yield return runtimeAssembly;
	}


	public static bool SupportsInterface<T>( Type type )
	{
		foreach( var i in type.GetTypeInfo().ImplementedInterfaces )
			if( i == typeof(T) )
				return true;

		return false;
	}

	public static Type FindInterface( Type type, Func<Type,bool> predicate )
	{
		foreach( var i in type.GetTypeInfo().ImplementedInterfaces )
			if( predicate( i ) )
				return i;

		return null;
	}

	//---------------------------------------------------------------------------------------------
	public static T GetCustomAttribute<T>( MemberInfo type ) where T : Attribute
	{
		if( type == null )
			throw new ArgumentNullException( "type" );
		
		return type.GetCustomAttribute<T>();
	}

	//---------------------------------------------------------------------------------------------
	public static A GetCustomAttribute<A>( Type type ) where A : Attribute
	{
		return type.GetTypeInfo().GetCustomAttribute<A>();
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsGenericType( Type type )
	{
		return type.GetTypeInfo().IsGenericType;
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsValueType( Type type )
	{
		return type.GetTypeInfo().IsValueType;
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsEnum( Type type )
	{
		return type.GetTypeInfo().IsEnum;
	}

	//---------------------------------------------------------------------------------------------
	public static Type[] GetGenericArguments( Type type )
	{
		return type.GetTypeInfo().GenericTypeArguments;
	}

	public static Type GetGenericTypeDefinition( Type type )
	{
		return type.GetTypeInfo().GetGenericTypeDefinition();
	}
	
	public static MethodInfo GetMethod( Type type, string methodName, Predicate<MethodInfo> predicate )
	{
		MethodInfo method = null;

		foreach( var m in type.GetRuntimeMethods() )
		{
			var matchByName = m.Name == methodName;
			if( !matchByName && m.Name.Contains( "." ) )
			{
				var names = m.Name.Split( '.' );
				matchByName = names[names.Length-1] == methodName;
			}
			
			if( matchByName && predicate(m) )
			{
				method = m;
				break;
			}
		}

		if( method == null )
			throw new Exception( "Method [{0}] not found in [{1}] type or does not satisfy to specified predicate".Fmt( methodName, type.Name ) );

		return method;
	}


	//---------------------------------------------------------------------------------------------
	public static bool IsAssignableFrom( Type type, Type type2 )
	{
		return type.GetTypeInfo().IsAssignableFrom( type2.GetTypeInfo() );
	}

#endif

}

}
