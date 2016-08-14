﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;

public class Test : MonoBehaviour {

    public Text m_myText;

    List<PerformanceCounter> pc;
    List<CounterSample> samples;
    List<Double> m_values = new List<Double>();
    float updateInterval = 1.0f;
    float lastUpdate = 0.0f;
    float timeStep = 1.0f;
    CpuUsageCs.CpuUsage cpuUsage = new CpuUsageCs.CpuUsage();
    short m_currentCPUPercentage = 0;

    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 0;
        m_myText = gameObject.AddComponent<Text>();
        m_myText.text = "test";

        cpuUsage.GetUsage();

        pc = new List<PerformanceCounter>();

        //for ( int i  = 0; i < SystemInfo.processorCount; ++i )

        //pc.Add(new PerformanceCounter("Processor", "% Processor Time", Process.GetCurrentProcess().ProcessName ) ); //; "_Total"));

        //pc.Add(new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName));
        //pc.Add(new PerformanceCounter("Processor Information", "% Idle Time", "_Total"));

        for (int i = 0; i < SystemInfo.processorCount; ++i)
        {
            pc.Add(new PerformanceCounter("Processor", "% User Time", i.ToString() ) );
            //pc.Add(new PerformanceCounter("Processor", "% Idle Time", i.ToString()));
        }

        samples = new List<CounterSample>();

        //System.Threading.Thread.Sleep(1000);

        for (int i = 0; i < pc.Count; ++i)
        {
            //System.Threading.Thread.Sleep(500);

            pc[i].NextValue();

            samples.Add( pc[i].NextSample() );
            m_values.Add(0.0);
        }

        System.Threading.Thread.Sleep( 1000 );
    }
	
	// Update is called once per frame
	void Update () {

        if ( ( Time.realtimeSinceStartup - lastUpdate ) < updateInterval )
        {
           // System.Threading.Thread.Sleep( 500 );
            return;
        }

        timeStep = Time.unscaledDeltaTime;

        lastUpdate = Time.realtimeSinceStartup;

        //System.Threading.Thread.Sleep(1000);

        for ( int i = 0; i < pc.Count; i++ )
        {
            CounterSample s1 = samples[i];
            CounterSample s2 = pc[i].NextSample();

            //m_values[i] = pc[i].NextValue();
            //m_values[i] = Calculate( s1, s2 ); //CounterSample.Calculate( s1, s2 );
            //m_values[i] = CounterSample.Calculate( s1, s2 );

            m_values[i] = (float)(((s2.TimeStamp100nSec - s1.TimeStamp100nSec) ) / 10000000 );
            //m_values[i] = (float)( (s2.TimeStamp - s1.TimeStamp) / s2.SystemFrequency );

            //m_values[i] = (float)((s2.TimeStamp100nSec - s1.TimeStamp100nSec) * ( Time.unscaledDeltaTime / 100000.0f ) );

            //m_values[i] = (float)(s2.TimeStamp - s1.TimeStamp);

            samples[i] = pc[i].NextSample();
        }

        m_currentCPUPercentage = cpuUsage.GetUsage();

        // System.Threading.Thread.Sleep(500);
    }

    void DrawGUI( Rect rect )
    {
        GUI.Box( rect, ""); GUI.Box(rect, "");

        float yOffset = 5.0f;

        float fps = 1.0f / timeStep;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), fps.ToString("n2") + " FPS");
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), (timeStep * 100.0f).ToString("0.00") + "ms");
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Time : " + Time.realtimeSinceStartup.ToString( "0.00" ));
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Frame : " + Time.frameCount.ToString() );
        yOffset += 20.0f;

        //Vid Info
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceVersion.ToString() ) ;
        yOffset += 20.0f;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceVendor );
        yOffset += 20.0f;
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceName );
        yOffset += 20.0f;
        GUI.Label( new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "VRAM " + SystemInfo.graphicsMemorySize.ToString() + " MB" );
        yOffset += 40.0f;


        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Cores : " + SystemInfo.processorCount.ToString());
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Speed : " + SystemInfo.processorFrequency.ToString() + " MHZ" );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Total CPU Time: " + m_currentCPUPercentage + "%" );
        yOffset += 20.0f;

        for (int i = 0; i < m_values.Count; i++)
        {
            GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "[CORE" + i + "] " + m_values[i].ToString("0.0000") + " (" + ( 1.0f / (m_values[i] * 0.01f) ).ToString("0.00") + ")");
            yOffset += 20.0f;
        }

        /*
        for( int i = 0; i < pc.Count; i++ )
        {
            GUI.Label(new Rect(5.0f, yOffset + 5.0f + (i * 20), 250.0f, 25.0f), pc[i].CounterName );
        }
        */
    }

    void OnGUI()
    {
        float width = 250.0f;
        DrawGUI(new Rect( Screen.width - width, 0.0f, width, Screen.height));
    }

    public static Double Calculate(CounterSample oldSample, CounterSample newSample)
    {
        double difference = newSample.RawValue - oldSample.RawValue;
        double timeInterval = newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec;
        if (timeInterval != 0) return 100 * (1 - (difference / timeInterval));
        return 0;
    }

}