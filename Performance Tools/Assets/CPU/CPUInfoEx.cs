using UnityEngine;

using System;
using System.Runtime.InteropServices;

public class CPUInfoEx
{
    [StructLayout(LayoutKind.Sequential)]
    protected struct SystemProcessorPerformanceInformation {
      public long IdleTime;
      public long KernelTime;
      public long UserTime;
      public long Reserved0;
      public long Reserved1;
      public ulong Reserved2;
    }

    protected enum SystemInformationClass {
      SystemBasicInformation = 0,
      SystemCpuInformation = 1,
      SystemPerformanceInformation = 2,
      SystemTimeOfDayInformation = 3,
      SystemProcessInformation = 5,
      SystemProcessorPerformanceInformation = 8
    }

    protected static class NativeMethods {

      [DllImport("ntdll.dll")]
      public static extern int NtQuerySystemInformation(
        SystemInformationClass informationClass,
        [Out] SystemProcessorPerformanceInformation[] informations,
        int structSize, out IntPtr returnLength);
    }

	string m_CPUTitle;
	int m_numCPUCores;

	float m_currentCPUUsage = 0.0f;


	private long[] idleTimes;
    private long[] totalTimes;

	public float[] coreLoad;

	public float m_lastUpdateTime;

	 //If this is set lower, then the perf counters do not have enough time in between samples
	 //to accurately record the usage.
	public float m_updateRate = 0.5f;

	public CPUInfoEx()
	{

		m_CPUTitle = GetCPUTitle();
		m_numCPUCores = GetNumCPUCores();
		coreLoad = new float[ m_numCPUCores ];
		GetTimes(out idleTimes, out totalTimes);
		m_lastUpdateTime = Time.realtimeSinceStartup;
	}

    private static bool GetTimes(out long[] idle, out long[] total) {      
      SystemProcessorPerformanceInformation[] informations = new
        SystemProcessorPerformanceInformation[64];

      int size = Marshal.SizeOf(typeof(SystemProcessorPerformanceInformation));

      idle = null;
      total = null;

      IntPtr returnLength;
      if (NativeMethods.NtQuerySystemInformation(
        SystemInformationClass.SystemProcessorPerformanceInformation,
        informations, informations.Length * size, out returnLength) != 0)
        return false;

      idle = new long[(int)returnLength / size];
      total = new long[(int)returnLength / size];

      for (int i = 0; i < idle.Length; i++) {
        idle[i] = informations[i].IdleTime;
        total[i] = informations[i].KernelTime + informations[i].UserTime;
      }

      return true;
    }

	public void Update()
	{
		if (  ( Time.realtimeSinceStartup - m_lastUpdateTime ) < m_updateRate )
		{
			return;
		}
		m_lastUpdateTime = Time.realtimeSinceStartup;

		long[] newIdleTimes;
		long[] newTotalTimes;

		if ( !GetTimes( out newIdleTimes, out newTotalTimes ) )
		{
			return;
		}

		for (int i = 0; i < Math.Min(newTotalTimes.Length, totalTimes.Length); i++)
		{
			if (newTotalTimes[i] - totalTimes[i] < 100000)
			{
				return;
			}
		}

		if ( newIdleTimes == null || newTotalTimes == null )
		{
			return;
		}

		if( coreLoad.Length < newTotalTimes.Length )
		{
			return;
		}

		if ( idleTimes != null && totalTimes != null )
		{
			for ( int i = 0; i < NumCores; i++ )
			{
				float value = (float)( newIdleTimes[i] - idleTimes[i] ) / (float)( newTotalTimes[i] - totalTimes[i] );
				value = 1.0f - value;
				value = value < 0 ? 0 : value;
				coreLoad[i] = value * 100.0f;
			}
		}

		idleTimes = newIdleTimes;
		totalTimes = newTotalTimes;
	}

    private string GetCPUTitle()
    {
        string[] tokens = SystemInfo.processorType.Replace("CPU", "").Replace("(TM)", "").Replace("(R)", "").Split(new char[] { '@' });
        return tokens[0];
    }

	private int GetNumCPUCores()
	{
		return SystemInfo.processorCount;
	}

	//PUBLIC
	public string CPUTitle
	{
		get
		{
			return m_CPUTitle;
		}
	}

	public int NumCores
	{
		get
		{
			return m_numCPUCores;
		}
	}

	public int CPUFrequency
	{
		get
		{
			return SystemInfo.processorFrequency;
		}
	}

	public float CPUUsage
	{
		get
		{
			return m_currentCPUUsage;
		}
	}

}