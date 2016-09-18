using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetricRenderer : MonoBehaviour {
    public Material mat;

    Test m_testObject;

    CircularBuffer<float> m_valueBuffer;

    Graph m_fpsGraph;
    Graph m_cpuGraph;
	Graph m_gpuGraph;
    Graph m_gcGraph;

    void Start()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));

        m_testObject = gameObject.GetComponent<Test>();

        m_valueBuffer = new CircularBuffer<float>(200);

        m_fpsGraph = new Graph(200);
        m_fpsGraph.SetColors( Color.red, Color.yellow, Color.green );
        m_cpuGraph = new Graph(200);
		m_gpuGraph = new Graph(200);
        m_gcGraph = new Graph(200);
    }

    void Update()
    {
        m_fpsGraph.AddValue( Mathf.Clamp( ( ( 1.0f / Time.unscaledDeltaTime ) / 60.0f ) * 100.0f, 0.0f, 100.0f ) );

        m_cpuGraph.AddValue( (float)m_testObject.m_currentCPUPercentage );

        m_gpuGraph.AddValue( (float)m_testObject.m_currentGPUPercentage );

        if (m_testObject.m_gcIncrementedThisFrame)
        {
            m_gcGraph.AddValue( 100.0f );
        }
        else
        {
            m_gcGraph.AddValue( 0.0f );
        }

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

    void DrawData( float x, float y )
    {
        float pixelWidth = 1.0f / Screen.width;
        float pixelHeight = 1.0f / Screen.height;

        for (int i = 0; i < m_valueBuffer.Count; i++)
        {
            float ht = m_valueBuffer.GetValue(m_valueBuffer.Count - i) * pixelHeight;
            float xOffset = pixelWidth * (((float)i * 1.0f) + x );
            float yOffset = (Screen.height - y ) * pixelHeight;

            Color clr = (ht > (30 * pixelHeight)) ? Color.yellow : Color.red;
            clr = (ht > (45 * pixelHeight)) ? Color.green : clr;
            GLUtils.DrawLine(new Rect(xOffset, yOffset, 0.0f, 0.0f), pixelWidth, pixelHeight, ht * 0.5f, clr);
        }
    }

    void OnPostRender() {
        float xOffset = 220.0f;

        m_fpsGraph.Render(new Rect(Screen.width - xOffset, 70.0f, 200.0f, 20.0f));

        m_gcGraph.Render(new Rect(Screen.width - xOffset, 160.0f, 200.0f, 20.0f));

		m_gpuGraph.Render(new Rect(Screen.width - xOffset, 285.0f, 200.0f, 20.0f));

        m_cpuGraph.Render(new Rect(Screen.width - xOffset, 390.0f, 200.0f, 20.0f));
    }
}