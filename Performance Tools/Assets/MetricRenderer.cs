using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetricRenderer : MonoBehaviour {
    public Material mat;

    CircularBuffer<float> m_valueBuffer;

    Graph m_fpsGraph;
    Graph m_gcGraph;
	Graph m_gpuGraph;
	Graph m_cpuGraph;

	FrameInfo m_frameInfo;
	MemoryInfo m_memoryInfo;
	GPUInfo m_gpuInfo;
	CPUInfo m_cpuInfo;

	float m_maxGCMemory = 0.0f;
	float m_minGCMemory = float.MaxValue;
	float m_gcMemoryGraphScale = 0.0f;

    void Start()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));

        m_fpsGraph = new Graph(200);
        m_fpsGraph.SetColors( Color.red, Color.yellow, Color.green );
        m_cpuGraph = new Graph(200);
		m_gpuGraph = new Graph(200);
        m_gcGraph = new Graph(200);
		m_gcGraph.SetColors( Color.yellow, Color.yellow, Color.yellow );

		m_cpuInfo = new CPUInfo();
		m_frameInfo = new FrameInfo();
		m_memoryInfo = new MemoryInfo();
		m_gpuInfo = new GPUInfo();
    }

    void Update()
    {
		//Update Info
		m_memoryInfo.Update();
		m_gpuInfo.Update();
		m_cpuInfo.Update();

		//Add graphing data
        m_fpsGraph.AddValue( Mathf.Clamp( ( ( 1.0f / m_frameInfo.FrameTime ) / 60.0f ) * 100.0f, 0.0f, 100.0f ) );
		m_gcGraph.AddValue( GetAdjustedGCValue() );
		m_gpuGraph.AddValue( (float)m_gpuInfo.GPUUsage );
		m_cpuGraph.AddValue( m_cpuInfo.CPUUsage );
    }

	float GetAdjustedGCValue()
	{
		float currentGCMemory = m_memoryInfo.GetTotalGCMemoryMB();
		if ( currentGCMemory > m_maxGCMemory )
		{
			m_maxGCMemory = currentGCMemory;
		}

		if ( currentGCMemory < m_minGCMemory )
		{
			m_minGCMemory = currentGCMemory;
		}

		float adjustedValue = currentGCMemory - m_minGCMemory;
		float range = m_maxGCMemory - m_minGCMemory;

		if ( m_memoryInfo.GCIteration > 1 && m_minGCMemory < m_maxGCMemory )
		{
			m_gcMemoryGraphScale = 100.0f / range;
			return adjustedValue * m_gcMemoryGraphScale;
		}

		return currentGCMemory;
		
	}

	void OnGUI()
    {
        float width = 215.0f;
        DrawGUI(new Rect( Screen.width - width, 0.0f, width, Screen.height));
    }

    void DrawGUI( Rect rect )
    {
        GUI.Box( rect, "");

        float yOffset = 5.0f;

		//
		//Frame info
		//
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), m_frameInfo.FrameRate.ToString("n2") + " FPS");
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), (m_frameInfo.FrameTime * 100.0f).ToString("0.00") + "ms");
        yOffset += 20.0f;
     
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Frame : " + m_frameInfo.FrameCount.ToString() );
        yOffset += 50.0f;

		//
		//Memory
		//
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Sys Mem : " + Mathf.CeilToInt( (float)SystemInfo.systemMemorySize / 1024.0f ).ToString() + "GB" );
        yOffset += 20.0f;

		float fmb = m_memoryInfo.GetTotalGCMemoryMB();
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "GC Alloc : " + fmb.ToString("N2") + "MB" );
		yOffset += 50.0f;

		//
        //GPU
		//
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), SystemInfo.graphicsDeviceVersion.ToString() ) ;
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), m_gpuInfo.GPUTitle );
        yOffset += 20.0f;

		GUI.Label( new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "VRAM " + m_gpuInfo.GPUMemorySize.ToString() + " MB" );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "GPU Load : " + m_gpuInfo.GPUUsage + "%" );
        yOffset += 50.0f;

		//
		//CPU
		//
        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 400.0f, 25.0f), "CPU Type : " + m_cpuInfo.CPUTitle );     
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Cores : " + m_cpuInfo.NumCores.ToString());
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "CPU Speed : " + m_cpuInfo.CPUFrequency.ToString() + " MHZ" );
        yOffset += 20.0f;

        GUI.Label(new Rect(rect.x + 5.0f, rect.y + yOffset, 250.0f, 25.0f), "Total CPU Time: " + m_cpuInfo.CPUUsage + "%" );
        yOffset += 20.0f;
    }

    void DrawBackgroundBox( float x, float y )
    {
        GL.PushMatrix();
        mat.SetPass(0);

        float pixelWidth = 1.0f / Screen.width;
        float pixelHeight = 1.0f / Screen.height;

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        float _xOffset2 = pixelWidth * (x - 1);
        float _yOffset2 = (Screen.height - (y + 1) ) * pixelHeight;

        GLUtils.DrawBox(new Rect(_xOffset2, _yOffset2, 202.0f * pixelWidth, 27.0f * pixelHeight), pixelWidth, pixelHeight, Color.black);

        float _xOffset = pixelWidth * x;
        float _yOffset = (Screen.height - y ) * pixelHeight;
        GLUtils.DrawBox(new Rect(_xOffset, _yOffset, 200.0f * pixelWidth, 25.0f * pixelHeight), pixelWidth, pixelHeight, Color.grey);

        GL.End();
        GL.PopMatrix();
    }

    void OnPostRender() {
        float xOffset = 210.0f;
        m_fpsGraph.Render(new Rect(Screen.width - xOffset, 70.0f, 200.0f, 20.0f));
        m_gcGraph.Render(new Rect(Screen.width - xOffset, 140.0f, 200.0f, 20.0f));
		m_gpuGraph.Render(new Rect(Screen.width - xOffset, 250.0f, 200.0f, 20.0f));
        m_cpuGraph.Render(new Rect(Screen.width - xOffset, 360.0f, 200.0f, 20.0f));
    }
}