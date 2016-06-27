using System;

namespace Endemics {

public sealed class Nothing
{
	static Nothing m_value = new Nothing();

	public static Nothing Value
	{
		get { return m_value; }
	}
}

}
