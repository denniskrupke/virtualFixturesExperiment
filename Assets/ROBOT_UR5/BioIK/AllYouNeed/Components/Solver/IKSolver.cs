using UnityEngine;

namespace BioIK {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class IKSolver : MonoBehaviour {

		public bool ExecuteInEditMode = false;

		//Algorithm parameters
		public double MaximumFrameTime = 0.005; //Specify the maximum allowed time for optimization during one frame
		public int Individuals = 40;			//Can be chosen much higher than the number of elites
		public int Elites = 1;					//Should be chosen comparatively low.
												//Lower values cause more exploitation (smooth trajectory and no posture fluctuations).
												//Higher values improve exploration (success rate in finding reachable postures), but might cause jumps between configurations.

		public float Smoothing = 0.0f;
		public float AnimationWeight = 0.0f;
		public float AnimationBlend = 0.0f;

		//Optimization algorithm
		private Model Model;
		private Evolution Evolution;

		private int Generations;
		private double ElapsedTime;
		private double IterationTime;

		private bool RequireReset;

		//Joints and Objectives
		private KinematicJoint[] Joints = new KinematicJoint[0];
		private Objective[] Objectives = new Objective[0];

		void Awake() {
			Initialise(Individuals, Elites);
		}

		void LateUpdate() {
			if(RequireReset || Model == null) {
				Initialise(Individuals, Elites);
				UpdateControls(HasControl());
				if(!HasControl()) {
					Evolution.StopThreads();
					return;
				} else {
					Evolution.StartThreads();
				}
			} else {
				UpdateControls(HasControl());
				if(!HasControl()) {
					Evolution.StopThreads();
					return;
				} else {
					Evolution.StartThreads();
				}
				Model.UpdateConfiguration();
				Generations = 0;
				ElapsedTime = 0.0;
				IterationTime = 0.0;
			}

			if(Application.isPlaying) {
				Elites = Evolution.GetElites();
			}

			UpdateObjectives();
			Prepare();
			while(ElapsedTime < MaximumFrameTime && !IsConverged()) {
			//for(int i=0; i<25; i++) {
				Iterate();
			}
			Assign(Evolution.GetSolution());
			UpdateJoints();
		}

		public void Initialise(int individuals, int elites) {
			Model = new Model(transform);
			if(Evolution != null) {
				Evolution.StopThreads();
			}
			Evolution = new Evolution(Model, individuals, elites, Model.GetTargetConfiguration());

			Joints = Model.GetJoints();
			Objectives = Model.GetObjectives();
			
			Generations = 0;
			ElapsedTime = 0.0;
			IterationTime = 0.0;
			RequireReset = false;
		}

		public void Iterate() {
			System.DateTime then = System.DateTime.Now;
			Evolution.Evolve();
			IterationTime = (System.DateTime.Now-then).Duration().TotalSeconds;
			ElapsedTime += IterationTime;
			Generations += 1;
		}

		public void SetIndividuals(int value) {
			if(Individuals != value) {
				Individuals = Mathf.Max(1, value);
				Elites = Mathf.Min(Individuals, Elites);
				Rebuild();
			}
		}

		public void SetElites(int value) {
			if(Elites != value) {
				Elites = Mathf.Min(Individuals, value);
				Rebuild();
			}
		}

		public void Rebuild() {
			RequireReset = true;
		}

		public Model GetModel() {
			return Model;
		}

		public Evolution GetEvolution() {
			return Evolution;
		}

		public int GetElapsedGenerations() {
			return Generations;
		}

		public double GetElapsedTime() {
			return ElapsedTime;
		}

		public double GetIterationTime() {
			return IterationTime;
		}

		public void Assign(double[] configuration) {
			for(int i=0; i<configuration.Length; i++) {
				if(Model.MotionPtrs[i].Motion.Joint.GetJointType() == JointType.Revolute) {
					Model.MotionPtrs[i].Motion.SetTargetValue((float)configuration[i]);
				} else if(Model.MotionPtrs[i].Motion.Joint.GetJointType() == JointType.Continuous) {
					Model.MotionPtrs[i].Motion.SetTargetValue(Model.MotionPtrs[i].Motion.GetTargetValue() + Mathf.Deg2Rad*Mathf.DeltaAngle(Mathf.Rad2Deg*Model.MotionPtrs[i].Motion.GetTargetValue(), Mathf.Rad2Deg*(float)configuration[i]));
				} else if(Model.MotionPtrs[i].Motion.Joint.GetJointType() == JointType.Prismatic) {
					Model.MotionPtrs[i].Motion.SetTargetValue((float)configuration[i]);
				}
			}
		}

		public bool IsConverged() {
			return Evolution.IsConverged();
		}
		
		public bool HasControl() {
			return ExecuteInEditMode || Application.isPlaying;
		}

		public void UpdateControls(bool value) {
			if(Model == null) {
				Model = new Model(transform);
			}
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].SetControlled(value);
			}
			if(Objectives == null) {
				Objectives = Model.GetObjectives();
			}
			for(int i=0; i<Objectives.Length; i++) {
				Objectives[i].SetControlled(value);
			}
		}

		public void Prepare() {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].Prepare();
			}
			Evolution.Prepare();
		}

		public void UpdateObjectives() {
			if(Objectives == null) {
				Objectives = Model.GetObjectives();
			}
			for(int i=0; i<Objectives.Length; i++) {
				Objectives[i].UpdateObjective();
			}
		}

		public void UpdateJoints() {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].UpdateJoint();
			}
		}

		public void ResetPosture() {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].GetXMotion().SetTargetValue(0f);
				Joints[i].GetYMotion().SetTargetValue(0f);
				Joints[i].GetZMotion().SetTargetValue(0f);
			}
		}

		public void EnableJoints(bool value) {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].enabled = value;
			}
		}

		public void SetSmoothings(float value) {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].SetSmoothing(value);
			}	
		}

		public void SetAnimationWeights(float value) {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].SetAnimationWeight(value);
			}
		}

		public void SetAnimationBlends(float value) {
			if(Joints == null) {
				Joints = Model.GetJoints();
			}
			for(int i=0; i<Joints.Length; i++) {
				Joints[i].SetAnimationBlend(value);
			}
		}

		void OnEnable() {
			UpdateControls(HasControl());
		}

		void OnDisable() {
			UpdateControls(false);
			if(Evolution != null) {
				Evolution.StopThreads();
			}
		}

		void OnDestroy() {
			UpdateControls(false);
			if(Evolution != null) {
				Evolution.StopThreads();
			}
		}

		void OnApplicationQuit() {
			UpdateControls(false);
			if(Evolution != null) {
				Evolution.StopThreads();
			}
		}
	}
}