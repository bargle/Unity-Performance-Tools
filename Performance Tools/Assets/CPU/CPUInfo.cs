using UnityEngine;

public class CPUInfo
{
	string m_CPUTitle;
	int m_numCPUCores;
	CpuUsageCs.CpuUsage m_cpuUsage;

	float m_currentCPUUsage = 0.0f;

	public CPUInfo()
	{
		//SETUP
		m_cpuUsage = new CpuUsageCs.CpuUsage();
		m_cpuUsage.GetUsage(); //Pump the system

		m_CPUTitle = GetCPUTitle();
		m_numCPUCores = GetNumCPUCores();

	}

	public void Update()
	{
		m_currentCPUUsage = m_cpuUsage.GetUsage();
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