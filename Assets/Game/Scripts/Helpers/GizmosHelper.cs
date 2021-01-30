using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Helpers
{
	public class GizmosHelper : MonoBehaviour
	{
		private static GizmosHelper _instance;
		private static GizmosHelper Instance
		{
			get
			{
				if (_instance != null) return _instance;
				GameObject go = new GameObject("~GizmosHelper");
				_instance = go.AddComponent<GizmosHelper>();
				return _instance;
			}
		}

		private readonly List<GizmoHandler> _gizmos = new List<GizmoHandler>();

		private void OnDestroy()
		{
			_instance = null;
		}
		
		private void OnDrawGizmos()
		{
			for (int i = _gizmos.Count - 1; i >= 0; i--)
			{
				Gizmos.color = _gizmos[i].Color;
				_gizmos[i].GizmoCallback();
				_gizmos[i].Duration -= Time.deltaTime;

				if (_gizmos[i].Duration <= 0f)
				{
					_gizmos.RemoveAt(i);
				}
			}
		}
		
		public static void DrawSphere(Vector3 center, float radius, Color color, float duration = 0f)
		{
			Instance._gizmos.Add(new GizmoHandler()
			{
				Duration = duration,
				Color = color,
				GizmoCallback = () =>
                {
	                Gizmos.DrawSphere(center, radius);
                }
			});
		}

		public static void DrawCube(Vector3 center, Vector3 size, Color color, float duration = 0f)
		{
			Instance._gizmos.Add(new GizmoHandler()
			{
				Duration = duration,
				Color    = color,
				GizmoCallback = () =>
                {
	                Gizmos.DrawCube(center, size);
                }
			});
		}

		public static void DrawLine(Vector3 from, Vector3 to, Color color, float duration = 0f)
		{
			Instance._gizmos.Add(new GizmoHandler()
			{
				Duration = duration,
				Color    = color,
				GizmoCallback = () =>
                {
	                Gizmos.DrawLine(from, to);
                }
			});
		}
		
		private class GizmoHandler
		{
			public float  Duration;
			public Color  Color;
			public Action GizmoCallback;
		}
	}
}