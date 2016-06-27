using System;


namespace Endemics {

public class SimpleStopwatch
{
	DateTime m_lastTime;
	DateTime m_initialTime;

	public SimpleStopwatch()
	{
		m_initialTime = DateTime.Now;
		m_lastTime = DateTime.Now;
	}

	public TimeSpan Lap()
	{
		var now = DateTime.Now;
		var period = now - m_lastTime;
		m_lastTime = now;

		return period;
	}

	public TimeSpan Elapsed
	{
		get
		{
			return DateTime.Now - m_initialTime;
		}
	}
}

}
