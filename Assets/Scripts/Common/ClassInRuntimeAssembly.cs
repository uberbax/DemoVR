using System;
using System.Reflection;
using UnityEngine;


namespace Endemics.Internal {

public static class ClassInRuntimeAssembly
{
	public static Assembly EditorAssembly { get; private set; }

	public static void SetEditorAssembly( Assembly a )
	{
		EditorAssembly = a;
	}

	public static Assembly RuntimeAssembly { get { return typeof(ClassInRuntimeAssembly).Assembly; } }


}

}