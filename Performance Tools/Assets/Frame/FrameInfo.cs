using UnityEngine;

public class FrameInfo
{
	float m_cachedFrameUpdateDelay = 0.1f;
	float m_lastUpdateTime;
	float m_cachedFrameTime;
	CircularBuffer<float> m_frameRateSamples = new CircularBuffer<float>( 10 );
	float m_avgFrameRate;

	public float FrameTime
	{
		get
		{
			return Time.unscaledDeltaTime;
		}
	}

	public float FrameRate
	{
		get
		{
			return 1.0f / FrameTime;
		}
	}

	public float FrameCount
	{
		get
		{
			return Time.frameCount;
		}
	}

	public float CachedFrameTime
	{
		get
		{
			return m_cachedFrameTime;
		}
	}

	public float CachedFrameRate
	{
		get
		{
			return 1.0f / m_cachedFrameTime;
		}
	}

	public float AvgFrameRate
	{
		get
		{
			return m_avgFrameRate;
		}
	}

	public void Update()
	{
		if ( Time.realtimeSinceStartup >= ( m_lastUpdateTime + m_cachedFrameUpdateDelay ) )
		{

			m_cachedFrameTime = FrameTime;


			m_lastUpdateTime = Time.realtimeSinceStartup;
		}

		m_frameRateSamples.Add( FrameRate );
		m_avgFrameRate = 0;
		for( int sample = 0; sample < m_frameRateSamples.Count; sample++ )
		{
			m_avgFrameRate += m_frameRateSamples.GetValue( sample );
		}

		m_avgFrameRate *= 0.1f; //divide by 10.
	}
}