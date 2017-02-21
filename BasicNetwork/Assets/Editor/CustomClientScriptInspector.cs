using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClientScript))]
public class CustomClientScriptInspector : Editor {


	void OnEnable()
	{

	}

	public override void OnInspectorGUI ()
	{
		base.DrawDefaultInspector ();
	}
}