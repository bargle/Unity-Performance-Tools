using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetricRenderer : MonoBehaviour {
    public Material mat;

    Test m_testObject;
    short[] m_values = new short[200];
    int m_currentCounter = 0;

    void Start()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));

        m_testObject = gameObject.GetComponent<Test>();
    }

    void Update()
    {
        m_values[m_currentCounter] = m_testObject.m_currentCPUPercentage;
        m_currentCounter++;
        if ( m_currentCounter >= 200 )
        {
            m_currentCounter = 0;
        }
    }

    void DrawBox( Rect rect, float pixelWidth, float pixelHeight, Color color )
    {
        float xOffset = rect.x;
        float yOffset = rect.y;

        GL.Color(color);
        GL.Vertex3(xOffset,             yOffset,                0);
        GL.Vertex3(xOffset,             yOffset + rect.yMax,    0);
        GL.Vertex3(/*xOffset + */rect.xMax, yOffset + rect.yMax,    0);
        GL.Vertex3(/*xOffset + */rect.xMax, yOffset,                0);
    }

    void DrawLine( Rect rect, float pixelWidth, float pixelHeight, float height, Color color )
    {
        float xOffset = rect.x;

        float yOffset = rect.y;

        GL.Color( color );
        GL.Vertex3(xOffset + 0, yOffset + 0, 0);
        GL.Vertex3(xOffset + pixelWidth, yOffset + 0, 0);
        GL.Vertex3(xOffset + pixelWidth, yOffset + height, 0);
        GL.Vertex3(xOffset + 0f, yOffset + height, 0);
    }

    void DrawBackgroundBox()
    {

        GL.PushMatrix();
        mat.SetPass(0);

        float pixelWidth = 1.0f / Screen.width;
        float pixelHeight = 1.0f / Screen.height;

        float height = pixelHeight * 50.0f; //100 pixels

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);



        float _xOffset2 = pixelWidth * 4.0f;
        float _yOffset2 = pixelHeight * 9.0f;

        DrawBox(new Rect(_xOffset2, _yOffset2, 202.0f * pixelWidth, 53.0f * pixelHeight), pixelWidth, pixelHeight, Color.black);
 

        GL.End();
        GL.PopMatrix();
    }

    void OnPostRender() {

        DrawBackgroundBox();
	/*
        if (!mat) {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
		*/
        GL.PushMatrix();
        mat.SetPass(0);

        float pixelWidth = 1.0f / Screen.width;
        float pixelHeight = 1.0f / Screen.height;

        float height = pixelHeight * 50.0f; //100 pixels

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        float _xOffset = pixelWidth * 5.0f;
        float _yOffset = pixelHeight * 10.0f;
        DrawBox(new Rect(_xOffset, _yOffset, 200.0f * pixelWidth, 50.0f * pixelHeight), pixelWidth, pixelHeight, Color.grey);

        for (int i = 0; i < 200; i++)
        {
            float ht = m_values[i] * pixelHeight;//Random.value * pixelHeight *25.0f + (pixelHeight *25);
            float xOffset = pixelWidth * ( ( (float)i * 1.0f ) + 5.0f );
            float yOffset = pixelHeight * 10.0f;

            Color clr = (ht > (30 * pixelHeight)) ? Color.yellow : Color.green;
            clr = (ht > (40 * pixelHeight)) ? Color.red : clr;
            DrawLine(new Rect(xOffset, yOffset, 0.0f, 0.0f), pixelWidth, pixelHeight, ht, clr);
        }
        

        GL.End();
        GL.PopMatrix();
    }
}