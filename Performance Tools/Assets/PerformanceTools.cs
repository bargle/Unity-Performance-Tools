using UnityEngine;

public class PerformanceTools : MonoBehaviour
{
	private bool m_enabled = false;

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.F12 ) )
		{
			m_enabled = !m_enabled;

			RemovePerformanceTools();
			if ( m_enabled )
			{
				Camera cam = Camera.current;
				if ( cam == null )
				{
					cam = Camera.main;
				}

				if ( cam != null )
				{
					cam.gameObject.AddComponent<MetricRenderer>();
				}
			}
		}
	}

	private void RemovePerformanceTools()
	{
		foreach ( Camera cam in Camera.allCameras )
		{
			MetricRenderer renderer = cam.GetComponent<MetricRenderer>();
			if ( renderer != null )
			{
				GameObject.Destroy( renderer );
			}
		}
	}
}