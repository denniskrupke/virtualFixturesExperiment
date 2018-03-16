using UnityEngine;
using UnityEditor;

namespace BioIK {
	[CustomEditor(typeof(LookAt))]
	public class LookAt_CE : Objective_CE {
		public void OnSceneGUI() {
			Handles.color = Color.magenta;
			Handles.ArrowHandleCap(0, Target.transform.position, Target.transform.rotation*Quaternion.LookRotation(((LookAt)Target).Direction), 0.25f, EventType.Repaint);
		}
	}
}