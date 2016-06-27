using System;
using System.IO;
using UnityEngine;

namespace Endemics {

public static class PathUtils
{
	//-----------------------------------------------------------------------------------------------
	public static string AppDataPath
	{
		get
		{
#if UNITY_EDITOR
			var edittimePath = Path.Combine( ProjectFolder, ".edittime" );
			return edittimePath;
#else
			return Application.persistentDataPath;
#endif
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static string ProjectFolder
	{
		get
		{
			return new DirectoryInfo( Application.dataPath ).Parent.FullName;
		}
	}

	//-----------------------------------------------------------------------------------------------
	public static string ExpandPath( string path )
	{
		return 
			path
				.Replace( "%AppData%", AppDataPath )
				.Replace( "%Project%", ProjectFolder )
				;
	}

	//-----------------------------------------------------------------------------------------------
	public static string GetAppDataDirectory( params string[] directories )
	{
		return Combine( AppDataPath, directories );
	}

	//-----------------------------------------------------------------------------------------------
	public static string UseAppDataDirectory( params string[] directories )
	{
		var path = GetAppDataDirectory( directories );
	
		if( !Directory.Exists( path ) )
		{
			Directory.CreateDirectory( path );
#if UNITY_IPHONE
			if( directories.Length > 0 )
			{
				var root = Path.Combine( AppDataPath, directories[0] );
				UnityEngine.iOS.Device.SetNoBackupFlag( root );
			}
#endif
		}

		return path;
	}

	//-----------------------------------------------------------------------------------------------
	public static string Combine( string path, params string[] subPaths )
	{
		var result = path;
		for( var i = 0; i < subPaths.Length; ++i )
			result = Path.Combine( result, subPaths[i] );

		return result;
	}

}

}
