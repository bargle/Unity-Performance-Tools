using UnityEngine;

public class GLUtils {

    public static void DrawBox( Rect rect, float pixelWidth, float pixelHeight, Color color )
    {
        float xOffset = rect.x;
        float yOffset = rect.y;

        GL.Color(color);
        GL.Vertex3(xOffset,     yOffset,                0);
        GL.Vertex3(xOffset,     rect.yMax,    0);
        GL.Vertex3(rect.xMax,   rect.yMax,    0);
        GL.Vertex3(rect.xMax,   yOffset,                0);
    }

    public static void DrawLine( Rect rect, float pixelWidth, float pixelHeight, float height, Color color )
    {
        float xOffset = rect.x;
        float yOffset = rect.y;

        GL.Color( color );
        GL.Vertex3(xOffset, yOffset, 0);
        GL.Vertex3(xOffset + pixelWidth, yOffset, 0);
        GL.Vertex3(xOffset + pixelWidth, yOffset + height, 0);
        GL.Vertex3(xOffset, yOffset + height, 0);
    }

}
