using UnityEngine;

public class MemoryInfo
{
	long m_lastGCCount = long.MaxValue;
	int m_gcIterations = 0;

	public void Update()
	{
        long currGCCount = System.GC.GetTotalMemory( false );
		if ( currGCCount < m_lastGCCount )
		{
			m_gcIterations++;
		}
		m_lastGCCount = currGCCount;
	}

	public float GetTotalGCMemoryMB()
	{
		return ( System.GC.GetTotalMemory( false ) / 1024 ) / 1024.0f;
	}

	public int GCIteration
	{
		get
		{
			return m_gcIterations;
		}
	}
}