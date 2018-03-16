using UnityEngine;
using UnityEditor;

namespace BioIK {
	[CustomEditor(typeof(IKSolver))]
	public class IKSolverEditor : Editor {

		public bool ShowGeometry = true;
		public Color LineColor = Color.cyan;
		public float LineWidth = 5f;
		public Color RootColor = Color.black;
		public float RootSize = 0.015f;
		public Color JointColor = Color.magenta;
		public float JointSize = 0.03f;
		public Color TipColor = Color.black;
		public float TipSize = 0.015f;
		public Color SegmentColor = Color.gray;
		public float SegmentSize = 0.015f;
		public float ArrowSize = 0.1f;

		private IKSolver Target;
		private Model Model;

		void Awake() {
			Target = (IKSolver)target;
			Model = new Model(Target.transform);
		}

		public override void OnInspectorGUI() {
			Undo.RecordObject(Target, Target.name);

			if(Model == null) {
				Model = new Model(Target.transform);
			}

			//Show DoF
			using (var degreeoffreedom = new EditorGUILayout.VerticalScope ("Button")) {
				EditorGUILayout.LabelField("Degree of Freedom: " + Model.DoF);
			}

			//Show Solver Settings
			using (var scope = new EditorGUILayout.VerticalScope ("Button")) {
				EditorGUILayout.HelpBox("Solver", MessageType.None);

				Target.MaximumFrameTime = EditorGUILayout.DoubleField("Maximum Frame Time", System.Math.Min(0.1, Target.MaximumFrameTime));
				Target.SetIndividuals(EditorGUILayout.IntField("Individuals", Target.Individuals));
				Target.SetElites(EditorGUILayout.IntField("Elites", Target.Elites));
			}

			Target.Smoothing = EditorGUILayout.Slider("Smoothing", Target.Smoothing, 0f, 1f);
			Target.SetSmoothings(Target.Smoothing);
			Target.AnimationWeight = EditorGUILayout.Slider("Animation Weight", Target.AnimationWeight, 0f, 1f);
			Target.SetAnimationWeights(Target.AnimationWeight);
			Target.AnimationBlend = EditorGUILayout.Slider("Animation Blend", Target.AnimationBlend, 0f, 1f);
			Target.SetAnimationBlends(Target.AnimationBlend);

			//Show IK Targets
			using (var scope = new EditorGUILayout.VerticalScope ("Button")) {
				EditorGUILayout.HelpBox("Objective Weights", MessageType.None);

				for(int i=0; i<Model.ObjectivePtrs.Length; i++) {
					Undo.RecordObject(Model.ObjectivePtrs[i].Objective, Model.ObjectivePtrs[i].Objective.name);

					using (var box = new EditorGUILayout.VerticalScope ("Box")) {
						EditorGUILayout.LabelField(Model.ObjectivePtrs[i].Objective.name + " (" + Model.ObjectivePtrs[i].Objective.GetObjectiveType() + ")");
						Model.ObjectivePtrs[i].Objective.Weight = Mathf.Max(0f, EditorGUILayout.FloatField("Weight", (float)Model.ObjectivePtrs[i].Objective.Weight));
					}

					EditorUtility.SetDirty(Model.ObjectivePtrs[i].Objective);
				}
			}

			//Execute In Edit Mode
			using (var executeineditmode = new EditorGUILayout.VerticalScope ("Button")) {
				Target.ExecuteInEditMode = EditorGUILayout.Toggle("Execute In Edit Mode: ", Target.ExecuteInEditMode);
				if(GUILayout.Button("Reset Posture")) {
					Target.ResetPosture();
				}
			}

			//Performance
			using (var degreeoffreedom = new EditorGUILayout.VerticalScope ("Button")) {
				EditorGUILayout.HelpBox("Performance", MessageType.None);
				EditorGUILayout.LabelField("Generations: " + Target.GetElapsedGenerations());
				EditorGUILayout.LabelField("Elapsed Time: " + Target.GetElapsedTime());
			}

			/*
			//Visualization
			using (var visualization = new EditorGUILayout.VerticalScope ("Button")) {
				EditorGUILayout.HelpBox("Visualization", MessageType.None);
				ShowGeometry = EditorGUILayout.Toggle("Show Geometry", ShowGeometry);
				LineColor = EditorGUILayout.ColorField("Line Color", LineColor);
				LineWidth = EditorGUILayout.FloatField("Line Width", LineWidth);
				RootColor = EditorGUILayout.ColorField("Root Color", RootColor);
				RootSize = EditorGUILayout.FloatField("Root Size", RootSize);
				JointColor = EditorGUILayout.ColorField("Joint Color", JointColor);
				JointSize = EditorGUILayout.FloatField("Joint Size", JointSize);
				TipColor = EditorGUILayout.ColorField("Tip Color", TipColor);
				TipSize = EditorGUILayout.FloatField("Tip Size", TipSize);
				SegmentColor = EditorGUILayout.ColorField("Segment Color", SegmentColor);
				SegmentSize = EditorGUILayout.FloatField("Segment Size", SegmentSize);
				ArrowSize = EditorGUILayout.FloatField("Arrow Size", ArrowSize);
			}
			*/

			EditorUtility.SetDirty(Target);
		}

		public virtual void OnSceneGUI() {
			if(Model == null) {
				Model = new Model(Target.transform);
			}

			if(ShowGeometry) {
				DrawGeometry(Target.transform, null);
				DrawModel(Model.Nodes[0]);
			}
		}

		private void DrawModel(Model.Node node) {
			DrawSphere(node.Segment.position, SegmentSize, SegmentColor);
			if(node.Joint != null) {
				DrawJoint(node.Joint);
			}
			for(int i=0; i<node.Childs.Length; i++) {
				DrawModel(node.Childs[i]);
			}
		}

		private void DrawGeometry(Transform node, Transform parent) {
			DrawSphere(node.position, SegmentSize, SegmentColor);
			if(parent != null) {
				DrawLine(parent.position, node.position, LineColor);
			} else {
				DrawSphere(node.position, RootSize, RootColor);
			}
			//KinematicJoint joint = node.GetComponent<KinematicJoint>();
			//if(joint != null) {
			//	DrawLine(node.position, joint.ComputeConnectionInWorldSpace(), JointColor);
			//	DrawJoint(joint);
			//}
			//IKTip tip = node.GetComponent<IKTip>();
			//if(tip != null) {
			//	DrawSphere(tip.transform.position, TipSize, TipColor);
			//}

			for(int i=0; i<node.childCount; i++) {
				DrawGeometry(node.GetChild(i), node);
			}
		}
		
		private void DrawJoint(KinematicJoint joint) {
			Vector3 connection = joint.GetAnchorInWorldSpace();
			//DrawSphere(connection, JointSize, JointColor);
			DrawCube(connection, joint.transform.rotation * Quaternion.Euler(joint.GetOrientation()), JointSize, JointColor);
			DrawLine(joint.transform.position, joint.GetAnchorInWorldSpace(), JointColor);

			//GUIStyle style = new GUIStyle();
			//style.normal.textColor = Color.black;
			//Handles.Label(connection, joint.name, style);

			if(joint.GetXMotion().IsEnabled()) {
				Handles.color = Color.red;
				Handles.ArrowHandleCap(0, connection, joint.transform.rotation * Quaternion.LookRotation(joint.GetXMotion().Axis), ArrowSize, EventType.Repaint);
			}
			if(joint.GetYMotion().IsEnabled()) {
				Handles.color = Color.green;
				Handles.ArrowHandleCap(0, connection, joint.transform.rotation * Quaternion.LookRotation(joint.GetYMotion().Axis), ArrowSize, EventType.Repaint);
			}
			if(joint.GetZMotion().IsEnabled()) {
				Handles.color = Color.blue;
				Handles.ArrowHandleCap(0, connection, joint.transform.rotation * Quaternion.LookRotation(joint.GetZMotion().Axis), ArrowSize, EventType.Repaint);
			}
		}

		private void DrawSphere(Vector3 position, float radius, Color color) {
			Handles.color = color;
			Handles.SphereHandleCap(0, position, Quaternion.identity, radius, EventType.Repaint);
		}

		private void DrawCube(Vector3 position, Quaternion rotation, float size, Color color) {
			Handles.color = color;
			Handles.CubeHandleCap(0, position, rotation, size, EventType.Repaint);
		}

		private void DrawLine(Vector3 a, Vector3 b, Color color) {
			Handles.color = color;
			Handles.DrawAAPolyLine(LineWidth, new Vector3[2] {a,b});
		}
	}
}