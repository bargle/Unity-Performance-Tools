using UnityEngine;

public class Graph {
    Material mat;
    CircularBuffer<float> m_valueBuffer;

    Color[] m_colors = new Color[] { Color.green, Color.yellow, Color.red };

    public Graph( int valueCount )
    {
        if ( valueCount < 1 )
        {
            //TODO: THROW
            return;
        }

        m_valueBuffer = new CircularBuffer<float>( valueCount );
        mat = new Material(Shader.Find("GUI/Text Shader"));
    }

    public void SetColors( Color low, Color med, Color high )
    {
        m_colors[0] = low;
        m_colors[1] = med;
        m_colors[2] = high;
    }

    public void AddValue( float val )
    {
        m_valueBuffer.Add( val );
    }

    public void Render( Rect rect )
    {
        float pixelWidth = 1.0f / Screen.width;
        float pixelHeight = 1.0f / Screen.height;

        GL.PushMatrix();
        mat.SetPass(0);

        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        GLUtils.DrawBox( new Rect( rect.x * pixelWidth, ( ( Screen.height - rect.height ) - rect.y )  * pixelHeight, rect.width * pixelWidth, rect.height * pixelHeight) , pixelWidth, pixelHeight, Color.black );

        for (int i = 0; i < m_valueBuffer.Count; i++)
        {
            float perc = m_valueBuffer.GetValue(m_valueBuffer.Count - i);
            float xOffset = ( ( (float)i * pixelWidth) + ( rect.x * pixelWidth ) ) ;
            float yOffset = ((Screen.height - rect.height) - rect.y) * pixelHeight;

            Color clr = ( perc > 30 ) ? m_colors[1] : m_colors[0];
            clr = (perc > 45) ? m_colors[2] : clr;
            GLUtils.DrawLine(new Rect(xOffset , yOffset, 0.0f, 0.0f), pixelWidth, pixelHeight, ( (perc / 100.0f) * rect.height) * pixelHeight, clr);
        }

        GL.End();
        GL.PopMatrix();
    }


}
