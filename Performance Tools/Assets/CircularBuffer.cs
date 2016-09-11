using UnityEngine;
using System.Collections;

public class CircularBuffer<T>
{
    T[] m_values;
    int m_count;
    int m_position;

    public CircularBuffer( int size )
    {
        m_values = new T[size];
        m_count = size;
        m_position = size - 1;
    }

    public int Count
    {
        get
        {
            return m_count;
        }
    }

    public void Add( T val )
    {
        m_values[ m_position ] = val;
        m_position--;
        if ( m_position < 0 )
        {
            m_position = ( m_count - 1 );
        }
    }

    public T GetValue( int index )
    {
        int calculatedIndex = ( index + m_position ) % m_count;
        return m_values[ calculatedIndex ];
    }

}
