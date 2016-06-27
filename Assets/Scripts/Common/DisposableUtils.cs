using System;
using UnityEngine;

namespace Endemics {

public static class DisposableUtils
{
	class AnonymusDisposable : IDisposable
	{
		Action m_callback;
		bool m_isDisposed = false;

		public AnonymusDisposable( Action callback )
		{
			m_callback = callback;
		}

		public void Dispose()
		{
			if( m_isDisposed )
			{
				throw new InvalidOperationException( "Dispose() call more than one time." );
			}

			m_isDisposed = true;
			if( m_callback != null )
				m_callback();
		}
	}


	public static IDisposable FromAction( Action action )
	{
		return new AnonymusDisposable(action);
	}

}


}