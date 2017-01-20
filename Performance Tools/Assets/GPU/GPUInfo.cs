using UnityEngine;
using OpenHardwareMonitor.Hardware.Nvidia;

//FIXME: assuming NVidia hardware currently...
public class GPUInfo
{
	string m_gpuTitle;
	string m_gpuMake;
	string m_driverVersion;

	NvPhysicalGpuHandle[] m_handles;
	int m_usagePercentage;

	public GPUInfo()
	{
		m_gpuTitle = SystemInfo.graphicsDeviceName.Replace("(TM)", "").Replace("(R)", "");
		m_gpuMake = SystemInfo.graphicsDeviceVendor;
		NVAPI.NvAPI_GetInterfaceVersionString( out m_driverVersion );
	}

	public void Update()
	{
		if ( m_handles == null )
		{ 
			m_handles = new NvPhysicalGpuHandle[NVAPI.MAX_PHYSICAL_GPUS];
		}

		try
		{
		    int count;
            if (NVAPI.NvAPI_EnumPhysicalGPUs == null)
            {
                UnityEngine.Debug.Log("Error: NvAPI_EnumPhysicalGPUs not available");
                return;
            }
			else
            {
                //NVAPI.NvAPI_GetInterfaceVersionString( out m_driverVersion );

                //m_driverVersion

                NvStatus status = NVAPI.NvAPI_EnumPhysicalGPUs(m_handles, out count);
                if (status == NvStatus.OK)
                {
                    //UnityEngine.Debug.Log("GPU Count: " + count);

                    for (int i = 0; i < count; i++)
                    {
                        NvPStates states = new NvPStates();
                        states.Version = NVAPI.GPU_PSTATES_VER;
                        states.PStates = new NvPState[NVAPI.MAX_PSTATES_PER_GPU];
                        if ( NVAPI.NvAPI_GPU_GetPStates != null && NVAPI.NvAPI_GPU_GetPStates(m_handles[i], ref states) == NvStatus.OK )
                        {
                            //GPU Core load perc
							int core_count = 0;
							int core_usage = 0;
							for( int core = 0; core < NVAPI.MAX_PSTATES_PER_GPU; core++ )
							{
								if ( states.PStates[ core ].Present )
								{ 
									core_usage += states.PStates[ core ].Percentage;
									core_count++;
								}
							}
							
							//m_usagePercentage = (short)states.PStates[0].Percentage;

							if ( core_count > 0 )
							{
								m_usagePercentage = core_usage / core_count;
							}
                        }
                        else
                        {
                            NvUsages usages = new NvUsages();
                            usages.Version = NVAPI.GPU_USAGES_VER;
                            usages.Usage = new uint[NVAPI.MAX_USAGES_PER_GPU];
                            if (NVAPI.NvAPI_GPU_GetUsages != null &&
                            NVAPI.NvAPI_GPU_GetUsages(m_handles[i], ref usages) == NvStatus.OK)
                            {
                                //GPU Core load perc
                                m_usagePercentage = (short)usages.Usage[2];
                            }
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.Log(" !OK - Status: " + status);
                }
            }
		} catch ( System.Exception ) { }



	}

	public string GPUMake
	{
		get
		{
			return m_gpuMake;
		}
	}
	public string GPUTitle
	{
		get
		{
			return m_gpuTitle;
		}
	}

	public int GPUMemorySize
	{
		get
		{
			return SystemInfo.graphicsMemorySize;
		}
	}

	public int GPUUsage
	{
		get
		{
			return m_usagePercentage;
		}
	}

	public string DriverVersion
	{
		get
		{
			return m_driverVersion;
		}
	}

}