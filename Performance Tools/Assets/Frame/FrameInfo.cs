using UnityEngine;

public class FrameInfo
{
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
			return 1.0f / Time.unscaledDeltaTime;
		}
	}

	public float FrameCount
	{
		get
		{
			return Time.frameCount;
		}
	}
}