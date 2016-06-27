using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using UnityEngine;



namespace Endemics {


public static class Utils
{
	static MD5 m_md5Hasher;

	public static MD5 GetMD5()
	{
		if( m_md5Hasher == null )
			m_md5Hasher = MD5.Create();
		
		return m_md5Hasher;
	}

	public static byte[] MD5OfString( string str )
	{
		var bytes = Encoding.UTF8.GetBytes(str);
		return GetMD5().ComputeHash(bytes);
	}

	public static string FormatBytesAsString( byte[] bytes)
	{
		var parts = new string[bytes.Length];
		for( int i = 0; i < bytes.Length; ++i )
			parts[i] = bytes[i].ToString("x2");
		
		return string.Join( string.Empty, parts );
	}

	public static byte[] StringToBytes( string text )
	{
		return Encoding.UTF8.GetBytes( text );
	}

	public static string StringFromBytes( byte[] bytes )
	{
		return Encoding.UTF8.GetString( bytes, 0, bytes.Length );
	}

    public static Coroutine DoWithDelay(MonoBehaviour container, float time, Action callback)
    {
       return container.StartCoroutine(DelayedRoutine(() => new WaitForSeconds(time), callback));
    }

    public static IEnumerator DoWithDelayEnumerator(MonoBehaviour container, float time, Action callback)
    {
        var delayEnumerator = DelayedRoutine(() => new WaitForSeconds(time), callback);

        container.StartCoroutine(delayEnumerator);
                
        return delayEnumerator;
    }

        public static Coroutine DoAfter(MonoBehaviour host, IEnumerator routine, Action feedback)
    {
        return host.StartCoroutine(DoAfter_Routine(() => host.StartCoroutine(routine), feedback));
    }

    public static Hashtable Hash(params object[] args)
    {
        Hashtable hashTable = new Hashtable(args.Length / 2);

        int i = 0;

        while (i < args.Length - 1)
        {
            hashTable.Add(args[i], args[i + 1]);

            i += 2;
        }

        return hashTable;
    }

    private static IEnumerator DelayedRoutine(Func<YieldInstruction> routine, Action callback)
    {
        yield return routine();

        callback();
    }

    public static IEnumerator ChangeLinear(float from, float to, float time, Func<float> step, Action<float> handler)
    {
        float t = 0.0f;        

        do
        {
            t += (step != null ? step() : Time.deltaTime);
            if (t > time)
                t = time;

            float value = Mathf.Lerp(from, to, t / time);
            handler(value);

            if (t < time)
                yield return null;

        } while (t != time);
    }

    public static IEnumerator ChangeColor(Color from, Color to, float time, Action<Color> feedback)
    {
        var changeProccess = ChangeLinear(0.0f, 1.0f, time, null,  (value) => feedback(Color.Lerp(from, to, value)));

        while (changeProccess.MoveNext())
            yield return null;
    }

    public static Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    static IEnumerator DoAfter_Routine(Func<YieldInstruction> couratine, Action feedback)
    {
        yield return couratine();
        if (feedback != null)
            feedback();
    }

}

}
