﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetricRenderer : MonoBehaviour {
    public Material mat;

    Test m_testObject;
    short[] m_values = new short[200];
    int m_currentCounter = 0;

    CircularBuffer<float> m_valueBuffer;

    void Start()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));

        m_testObject = gameObject.GetComponent<Test>();

        m_valueBuffer = new CircularBuffer<float>(200);
    }

    void Update()
    {
        //m_valueBuffer.Add(m_testObject.m_currentCPUPercentage);

        m_valueBuffer.Add( Mathf.Clamp( 1.0f / Time.unscaledDeltaTime, 0.0f, 50.0f ) );
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
        float _yOffset2 = (Screen.height - 61.0f) * pixelHeight;// * 9.0f;

        GLUtils.DrawBox(new Rect(_xOffset2, _yOffset2, 202.0f * pixelWidth, 52.0f * pixelHeight), pixelWidth, pixelHeight, Color.black);

        float _xOffset = pixelWidth * 5.0f;
        float _yOffset = (Screen.height - 60.0f) * pixelHeight;// * 10.0f;
        GLUtils.DrawBox(new Rect(_xOffset, _yOffset, 200.0f * pixelWidth, 50.0f * pixelHeight), pixelWidth, pixelHeight, Color.grey);

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

        //float height = pixelHeight * 50.0f; //100 pixels

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        for (int i = 0; i < m_valueBuffer.Count; i++)
        {
            float ht = m_valueBuffer.GetValue( m_valueBuffer.Count - i ) * pixelHeight;//Random.value * pixelHeight *25.0f + (pixelHeight *25);
            float xOffset = pixelWidth * ( ( (float)i * 1.0f ) + 5.0f );
            float yOffset = (Screen.height - 60.0f) * pixelHeight;// * 10.0f;

            Color clr = (ht > (30 * pixelHeight)) ? Color.yellow : Color.red;
            clr = (ht > (45 * pixelHeight)) ? Color.green : clr;
            GLUtils.DrawLine(new Rect(xOffset, yOffset, 0.0f, 0.0f), pixelWidth, pixelHeight, ht, clr);
        }
        

        GL.End();
        GL.PopMatrix();
    }
}