using UnityEngine;
using UnityEditor;
using System.Collections;

//namespace UnityEditor
//{
	[CustomEditor (typeof (Sketch))]
	public class SketchEditor : Editor
	{
		ToolType m_ActiveTool = ToolType.Movement;
	
		public enum ToolType { Movement, Extrusion, SomethingElse };
		
		public static string GetUniqueAssetName (string path, string extension)
		{
			int id = 0;
			
			while (System.IO.File.Exists (path + id + extension))
			{
				id++;
			}
			
			return path + id + extension;
		}
		
		public static string GetProjectRelativePath (string path)
		{
			if (path.IndexOf (Application.dataPath) != 0)
			{
				return path;
			}
			
			return "Assets" + path.Substring (Application.dataPath.Length);
		}
		
		[MenuItem ("GameObject/Create Other/Sketch Cube")]
		public static void CreateSketchCube ()
		{
			GameObject cube;
			Mesh cubeMesh;
			string cubeMeshPath;
			
			cubeMeshPath = GetProjectRelativePath (GetUniqueAssetName (Application.dataPath + "/Meshes/Cube", ".blend"));
			
			AssetDatabase.CopyAsset ("Assets/Sketch/Meshes/Cube.blend", cubeMeshPath);
			AssetDatabase.Refresh ();
			cubeMesh = AssetDatabase.LoadAssetAtPath (cubeMeshPath, typeof (Mesh)) as Mesh;
			
			if (cubeMesh == null)
			{
				Debug.LogError ("No mesh");
				return;
			}
			
			cube = EditorUtility.InstantiatePrefab (AssetDatabase.LoadAssetAtPath ("Assets/Sketch/Prefabs/Cube.prefab", typeof (GameObject))) as GameObject;			
			if (cube == null)
			{
				Debug.LogError ("No instance");
				return;
			}
			
			cube.GetComponent <MeshFilter> ().sharedMesh = cubeMesh;
			cube.GetComponent <Sketch> ().UpdateCollider ();
			
			Selection.activeObject = cube;
		}
	
		void OnSceneGUI ()
		{
			Sketch sketch;
		
			sketch = target as Sketch;
		
			if (sketch == null)
			{
				return;
			}
		
			//Tools.current = 4;
		
			if (sketch.HasSelection ())
			{
				Vector3 trianglePosition = sketch.TriangleCentre (sketch.Selection), editedPosition;
			
				editedPosition = Handles.PositionHandle (sketch.transform.position + trianglePosition, Quaternion.identity);
				if (editedPosition != trianglePosition)
				{
					switch (m_ActiveTool)
					{
						case ToolType.Movement:
							sketch.MoveTriangle (sketch.Selection, editedPosition - trianglePosition);
						break;
						case ToolType.Extrusion:
						break;
						case ToolType.SomethingElse:
						break;
					}
				}
			}
		
			if (Event.current.type == EventType.MouseDown)
			{
				ResolveGUISelection (sketch);
			}
		
			Handles.BeginGUI (new Rect (4.0f, 20.0f, 250.0f, 20.0f));
				GUILayout.BeginHorizontal ();
					m_ActiveTool = GUILayout.Toggle (m_ActiveTool == ToolType.Movement, "Movement", GUI.skin.GetStyle ("Button")) ? ToolType.Movement : m_ActiveTool;
					m_ActiveTool = GUILayout.Toggle (m_ActiveTool == ToolType.Extrusion, "Extrusion", GUI.skin.GetStyle ("Button")) ? ToolType.Extrusion : m_ActiveTool;
					m_ActiveTool = GUILayout.Toggle (m_ActiveTool == ToolType.SomethingElse, "SomethingElse", GUI.skin.GetStyle ("Button")) ? ToolType.SomethingElse : m_ActiveTool;
				GUILayout.EndHorizontal ();
			Handles.EndGUI ();
		
			//Event.current.Use ();
		}
	
		static void ResolveGUISelection (Sketch sketch)
		{
			RaycastHit hitInfo;
			int hitTriangle;

			if (Physics.Raycast (Event.current.mouseRay.origin, Event.current.mouseRay.direction, out hitInfo))
			{
				if (hitInfo.transform == sketch.transform)
				{
					Event.current.Use ();
			
					hitTriangle = sketch.GetNearestTriangle (hitInfo.point/* - sketch.transform.position*/);
					sketch.Select (hitTriangle);
				}
				else
				{
					Debug.Log ("Obstructed by " + hitInfo.transform.gameObject.name);
				}
			}
		}
	
		public override void OnInspectorGUI ()
		{
			Sketch sketch;
		
			sketch = target as Sketch;
		
			if (sketch == null)
			{
				return;
			}
		
			DrawDefaultInspector ();
		}
	}
//}