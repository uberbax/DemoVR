using System;
using System.Reflection;
using UnityEngine;

#if NETFX_CORE
using System.Linq;
#endif

namespace Endemics {

/**************************************************************************************************
**************************************************************************************************/
public static class Singleton<T>
	where T:class
{
	static T m_instance;
	static bool m_isDestroyed = false;
	//---------------------------------------------------------------------------------------------
	public static T Instance
	{
		get
		{
			if( m_isDestroyed && IsPersistent )
			{
				throw new InvalidOperationException( "Attempt to re-create persistent singleton. Possible there is access to singleton from Destory ?" );
			}

			if( m_instance == null )
				m_instance = InitializeInstance();
		
			return m_instance;
		}
	}

	//---------------------------------------------------------------------------------------------
	public static bool Exists
	{
		get { return m_instance != null; }
	}

	//---------------------------------------------------------------------------------------------
#pragma warning disable 168

	public static void Touch()
	{
		var temp = Instance ;
	}

#pragma warning restore 168

	//---------------------------------------------------------------------------------------------
	public static void Awake( T instance  )
	{
		if( !IsMonoBehaviour )
			throw new InvalidOperationException( "Singleton<{0}>.Awake( ... ) can not be called, because ({0}) isn't a MonoBehaviour".Fmt( typeof(T) ) );

		if( m_instance != null )
		{
			if( m_instance != instance )
			{
				var component = (instance as Component);

				if( !IsPersistent )
					Debug.LogError( "There is more than one singleton of ({0}). GameObject '{1}' was destroyed.".Fmt( typeof(T), component.gameObject ) );
				
				GameObject.Destroy( component.gameObject );
			}
		}
		else
		{
			m_instance = instance;
			DoSingletonInitialization( m_instance );
		}
	}

	//---------------------------------------------------------------------------------------------
	public static void OnDestroy( T instance )
	{ //TODO: Singelton<T>.OnDestroy
		if( instance == m_instance )
		{
			m_isDestroyed = true;

			if( m_instance != null )
			{
				var disposabe = m_instance as IDisposable;
				if( disposabe != null )
					disposabe.Dispose();
			}

			m_instance = null;
		}
	}

	//---------------------------------------------------------------------------------------------
	static void DoSingletonInitialization( T instance)
	{
		var initialization = instance as ISingletonInitialization;
		if( initialization != null )
			initialization.AwakeSingleton();
	}

	//---------------------------------------------------------------------------------------------

#if UNITY_EDITOR
	static bool MethodDeclared( Type someType, string metodName)
	{
		foreach( var m in ReflectionUtils.GetAllMethods( someType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy  ) )
			if( m.Name == metodName )
				return true;
		
		return false;
	}

#endif
	//---------------------------------------------------------------------------------------------

	static T InitializeInstance()
	{ // если этот метод вызван, значит m_instance ещё == null ;
		T instance = default(T);
		m_isDestroyed = false;

		if( IsMonoBehaviour )
		{
#if UNITY_EDITOR
			Action<string> checkMethodExistance = methodName =>
			{
				if( !MethodDeclared( typeof(T), methodName ) )
				{
					UnityEditor.EditorUtility.DisplayDialog
						( "Singleton declaration error !"
						, "Did you forget to implement {0}.{1}() {{ Singleton<{0}>.{1}(this); }} method ?!".Fmt( typeof(T).Name, methodName)
						, "ok" )
						;

					Debug.Break();

					throw new InvalidOperationException( "Singeton of ({0}) not declared {1} method".Fmt( typeof(T), methodName ) );
				}
			};

			checkMethodExistance( "Awake" );
			checkMethodExistance( "OnDestroy" );

#endif

			var existing = (MonoBehaviour)GameObject.FindObjectOfType(typeof(T));
			if( existing != null )
			{
				Awake( existing as T);

				return existing as T;
			}
			else
				instance = CreateMonoBehaviourInstance();
		}
		else
		{
			//var singletonType = typeof(T);

			{
				instance = Activator.CreateInstance<T>();
			}
		
			DoSingletonInitialization(instance);
		}

		return instance;
	}

	//---------------------------------------------------------------------------------------------
	static T CreateMonoBehaviourInstance()
	{
		if( !AllowAutoCreateGameObject )
			throw new InvalidOperationException( "Attempt to automatically create GameObject for singleton:({0}), but auto creation isn't allowed on this type".Fmt(typeof(T)) );

		var singletonType = typeof(T);

		var nameAttribute = Endemics.ReflectionUtils.GetCustomAttribute<SingletonGameObjectNameAttribute>(singletonType);
		string gameObjectName =
				 nameAttribute == null 
			? "Singleton of ({0})".Fmt( singletonType.Name )
			: nameAttribute.GameObjectName;

		GameObject singletonInstance = null;
		
		GameObject singletonPrefab = LoadPrefabResource();
		if( singletonPrefab == null )
		{
			singletonInstance = new GameObject( gameObjectName );
			
			// такая последовательность создания необходима, что бы все компоненты указанные через RequireComponent работали с полностью сконструированным объектом.
			singletonInstance.SetActive(false);
			singletonInstance.AddComponent( singletonType );
			singletonInstance.SetActive(true);
		}
		else
		{
			singletonInstance = (GameObject)GameObject.Instantiate( singletonPrefab  );
			singletonInstance.name = gameObjectName;
		}
			
		MonoBehaviour instance = (MonoBehaviour)singletonInstance.GetComponent( singletonType );
		
		if( instance == null )
		{
			GameObject.Destroy( instance );
			throw new Exception( "Singleton of ({0}) is created, but Component isn't found ! Possible prefab: {1} don't have it".Fmt( singletonType, singletonPrefab ) );
		}
		else
		{
			singletonInstance.transform.parent = AutoSingletonsRoot.GetRoot(IsPersistent);
		}

		
		return instance as T;
	}

	//---------------------------------------------------------------------------------------------
	static string GetPrefabResourceName()
	{
		if( !IsMonoBehaviour )
			throw new InvalidOperationException( "Attempt to obtain singleton's prefab resource, but ({0}) isn't a MonoBehaviour".Fmt( typeof(T) ) );

		var singletonType = typeof(T);
		string resourceName = string.Empty;

		var attrib = Endemics.ReflectionUtils.GetCustomAttribute<SingletonResourcePrefabAttribute>( singletonType );

		if( attrib != null )
			resourceName = attrib.ResourceName;
		else
		{
			resourceName = singletonType.Name;
			//Debug.LogWarning( "Make default Singleton resource name '{0}'".Fmt(resourceName) );
		}

		return resourceName;
	}

	//---------------------------------------------------------------------------------------------
	public static GameObject LoadPrefabResource()
	{
		var resourceName = GetPrefabResourceName();

		return string.IsNullOrEmpty(resourceName) ? null : (GameObject)Resources.Load( resourceName, typeof(GameObject) );
	}


	//---------------------------------------------------------------------------------------------
	public static bool IsMonoBehaviour
	{
		get
		{
			return Endemics.ReflectionUtils.IsAssignableFrom( typeof(MonoBehaviour), typeof(T) );
		}
	}

	//---------------------------------------------------------------------------------------------
	public static bool IsPersistent
	{
		get
		{
			return Endemics.ReflectionUtils.GetCustomAttribute<PersistentSingletonAttribute>( typeof(T) ) != null;
		}
	}

	//---------------------------------------------------------------------------------------------
	static bool AllowAutoCreateGameObject
	{
		get
		{
			return Endemics.ReflectionUtils.GetCustomAttribute<DisallowSingletonAutoCreationAttribute>( typeof(T) ) == null;
		}
	}

	//---------------------------------------------------------------------------------------------
	public static void Destroy()
	{
		if( Exists )
		{
			var temp = m_instance;
			m_instance = null;

			var disposable = temp as IDisposable;
			if( disposable != null )
			{
				disposable.Dispose();
			}

			if( IsMonoBehaviour )
			{
				var component = m_instance as Component;
				GameObject.Destroy( component );
			}
			



		}
	}

}

public interface ISingletonInitialization
{
	void AwakeSingleton();
}


/**************************************************************************************************
**************************************************************************************************/
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
public class SingletonResourcePrefabAttribute : Attribute
{
	string m_resourceName;

	public SingletonResourcePrefabAttribute(string resourceName )
	{
		m_resourceName = resourceName;
	}

	public string ResourceName
	{
		get{ return m_resourceName; }
	}
}

/**************************************************************************************************
**************************************************************************************************/
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
public class PersistentSingletonAttribute : Attribute
{
}

/**************************************************************************************************
**************************************************************************************************/
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
public class DisallowSingletonAutoCreationAttribute : Attribute
{
}

/**************************************************************************************************
**************************************************************************************************/
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
public class SingletonGameObjectNameAttribute : Attribute
{
	string m_gameObjectName;

	public SingletonGameObjectNameAttribute( string gameObjectName )
	{
		m_gameObjectName = gameObjectName;
	}

	public string GameObjectName
	{
		get { return m_gameObjectName; }
	}
}

}
