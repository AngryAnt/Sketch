using UnityEngine;
using System.Collections;

//namespace UnityEditor
//{
	[ExecuteInEditMode]
	[RequireComponent (typeof (MeshFilter))]
	[RequireComponent (typeof (Collider))]
	public class Sketch : MonoBehaviour
	{
		public int m_SelectedTriangle = -1;

		public Mesh Mesh
		{
			get
			{
				return GetComponent <MeshFilter> ().sharedMesh;
			}
		}
		
		public bool HasSelection ()
		{
			return m_SelectedTriangle != -1;
		}
		
		public void ClearSelection ()
		{
			m_SelectedTriangle = -1;
		}
		
		public void Select (int triangle)
		{
			m_SelectedTriangle = triangle;
		}
		
		public int Selection
		{
			get
			{
				return m_SelectedTriangle;
			}
		}
		
		public void UpdateCollider ()
		{
			GetComponent <MeshCollider> ().sharedMesh = Mesh;
		}
		
		void Awake ()
		{
			if (Application.isPlaying)
			{
				Destroy (this);
			}
		}
		
		float GetTriangleDistance (int triangle, Vector3 position)
		{
			return 	(Mesh.vertices [Mesh.triangles [triangle]] - position).magnitude + 
					(Mesh.vertices [Mesh.triangles [triangle + 1]] - position).magnitude + 
					(Mesh.vertices [Mesh.triangles [triangle + 2]] - position).magnitude;
		}
		
		public Vector3 TriangleCentre (int triangle)
		{
			return 	(Mesh.vertices [Mesh.triangles [triangle]] +
					Mesh.vertices [Mesh.triangles [triangle + 1]] +
					Mesh.vertices [Mesh.triangles [triangle + 2]]) / 3.0f;
		}
		
		public int GetNearestTriangle (Vector3 position)
		{
			float minimumDistance = Mathf.Infinity, distance;
			int nearestTriangle = 0;
			
			for (int i = 0; i < Mesh.triangles.Length; i += 3)
			{
				distance = GetTriangleDistance (i, position);
				if (distance < minimumDistance)
				{
					minimumDistance = distance;
					nearestTriangle = i;
				}
			}
			
			return nearestTriangle;
		}
		
		public void MoveTriangle (int triangle, Vector3 offset)
		{
			Vector3[] verts = Mesh.vertices;

			verts [Mesh.triangles [triangle]] += offset;
			verts [Mesh.triangles [triangle + 1]] += offset;
			verts [Mesh.triangles [triangle + 2]] += offset;
			
			Mesh.vertices = verts;
			Mesh.RecalculateBounds ();
			
			UpdateCollider ();
		}
		
		void OnDrawGizmosSelected ()
		{
			if (HasSelection ())
			{
				Gizmos.DrawLine (transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle]], transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle + 1]]);
				Gizmos.DrawLine (transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle]], transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle + 2]]);
				Gizmos.DrawLine (transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle + 1]], transform.position + Mesh.vertices [Mesh.triangles [m_SelectedTriangle + 2]]);
			}
		}
	}
//}