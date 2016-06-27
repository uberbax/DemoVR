using System;
using UnityEngine;



public static class StringExt
{
	public static string Fmt( this string str, params object[] formatParameters )
	{
		return string.Format( str, formatParameters );
	}
}
