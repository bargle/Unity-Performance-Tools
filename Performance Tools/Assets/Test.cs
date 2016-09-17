﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware.Nvidia;

public class Test : MonoBehaviour {

    public int m_calcs = 0;

    public Text m_myText;

    float updateInterval = 1.0f;
    float lastUpdate = 0.0f;
    float timeStep = 1.0f;
    CpuUsageCs.CpuUsage cpuUsage = new CpuUsageCs.CpuUsage();
    public short m_currentCPUPercentage = 0;
	short m_currentGPUPercentage = 0;

	long m_lastGCCount = long.MaxValue;
	int m_lastGCFrame = 0;
	int m_gcIterations = 0;
    public bool m_gcIncrementedThisFrame = false;

    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 0;
        m_myText = gameObject.AddComponent<Text>();
        m_myText.text = "test";

        cpuUsage.GetUsage();

        if ( NVAPI.IsAvailable )
        {
            UnityEngine.Debug.Log("woop!");
        } else
		{
            UnityEngine.Debug.Log("fail!");
		}

        System.Threading.Thread.Sleep( 1000 );
    }
	
	// Update is called once per frame
	void Update () {

        for ( int i = 0; i < m_calcs; i++ )
        {
            Mathf.Sqrt(Time.realtimeSinceStartup);
        }

        //GC updates
        long currGCCount = System.GC.GetTotalMemory( false );
		if ( currGCCount < m_lastGCCount )
		{
            m_gcIncrementedThisFrame = true;
            m_lastGCFrame = Time.frameCount;
			m_gcIterations++;
		} else
        {
            m_gcIncrementedThisFrame = false;
        }
		m_lastGCCount = currGCCount;

        if ( ( Time.realtimeSinceStartup - lastUpdate ) < updateInterval )
        {
           // System.Threading.Thread.Sleep( 500 );
            return;
        }

        timeStep = Time.unscaledDeltaTime;

        lastUpdate = Time.realtimeSinceStartup;

        m_currentCPUPercentage = cpuUsage.GetUsage();


		UpdateGPUInfo();
        // System.Threading.Thread.Sleep(500);
    }

	void UpdateGPUInfo()
	{
		NvPhysicalGpuHandle[] handles = new NvPhysicalGpuHandle[NVAPI.MAX_PHYSICAL_GPUS];
		int count;
		if (NVAPI.NvAPI_EnumPhysicalGPUs == null) 
		{
			UnityEngine.Debug.Log("Error: NvAPI_EnumPhysicalGPUs not available");
			return;
		} 
		else
		{        
			NvStatus status = NVAPI.NvAPI_EnumPhysicalGPUs(handles, out count);
			if (status == NvStatus.OK) 
			{
				//UnityEngine.Debug.Log("GPU Count: " + count);

				for ( int i = 0; i < count; i++ )
				{ 
					NvPStates states = new NvPStates();
					states.Version = NVAPI.GPU_PSTATES_VER;
					states.PStates = new NvPState[NVAPI.MAX_PSTATES_PER_GPU];
					if (NVAPI.NvAPI_GPU_GetPStates != null &&
					NVAPI.NvAPI_GPU_GetPStates(handles[i], ref states) == NvStatus.OK) 
					{

						//GPU Core load perc
						m_currentGPUPercentage = (short)states.PStates[ 0 ].Percentage;
					} 
					else 
					{
						NvUsages usages = new NvUsages();
						usages.Version = NVAPI.GPU_USAGES_VER;
						usages.Usage = new uint[NVAPI.MAX_USAGES_PER_GPU];
						if (NVAPI.NvAPI_GPU_GetUsages != null &&
						NVAPI.NvAPI_GPU_GetUsages(handles[i], ref usages) == NvStatus.OK) 
						{
							//GPU Core load perc
							m_currentGPUPercentage = (short)usages.Usage[2];
						}
					}
				}
			}
			else 
			{ 
				UnityEngine.Debug.Log(" !OK - Status: " + status);
			}
		}
	}

    void DrawGUI( Rect rect )
    {
        GUI.Box( rect, "");// GUI.Box(rect, "");

        float yOffset = 5.0f;

        float fps = 1.0f / timeStep;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), fps.ToString("n2") + " FPS");
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), (timeStep * 100.0f).ToString("0.00") + "ms");
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Frame : " + Time.frameCount.ToString() );
        yOffset += 50.0f;


        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Sys Mem : " + SystemInfo.systemMemorySize.ToString() );
        yOffset += 20.0f;

		long mb = System.GC.GetTotalMemory( false ) / (1024);

		float fmb = (float)mb / 1000.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "GC Alloc : " + fmb.ToString("N2") + "MB" );
		yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "GC Frame : " + m_lastGCFrame.ToString() + " Iter : " + m_gcIterations.ToString());

        yOffset += 50.0f;

        //Vid Info
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceVersion.ToString() ) ;
        yOffset += 20.0f;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceVendor );
        yOffset += 20.0f;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), GetGPUTitle() );
        yOffset += 20.0f;
        GUI.Label( new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "VRAM " + SystemInfo.graphicsMemorySize.ToString() + " MB" );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "GPU Load: " + m_currentGPUPercentage + "%" );
        yOffset += 40.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 400.0f, 25.0f), "CPU Type : " + GetCPUTitle() );
        
        //GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 400.0f, 25.0f), "CPU Type : " + SystemInfo.processorType.Replace( "(TM)", "" ).Replace( "(R)", "" ) );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Cores : " + SystemInfo.processorCount.ToString());
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Speed : " + SystemInfo.processorFrequency.ToString() + " MHZ" );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Total CPU Time: " + m_currentCPUPercentage + "%" );
        yOffset += 20.0f;
    }

    void OnGUI()
    {
        float width = 225.0f;
        DrawGUI(new Rect( Screen.width - width, 0.0f, width, Screen.height));
    }

    public static Double Calculate(CounterSample oldSample, CounterSample newSample)
    {
        double difference = newSample.RawValue - oldSample.RawValue;
        double timeInterval = newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec;
        if (timeInterval != 0) return 100 * (1 - (difference / timeInterval));
        return 0;
    }

    string GetCPUTitle()
    {
        string[] tokens = SystemInfo.processorType.Replace("CPU", "").Replace("(TM)", "").Replace("(R)", "").Split(new char[] { '@' });

        return tokens[0];
    }

    string GetGPUTitle()
    {
        return SystemInfo.graphicsDeviceName.Replace("(TM)", "").Replace("(R)", "");
    }
}
