using UnityEngine;

//!!!!!!
//This objective type is still under development
//!!!!!!

namespace BioIK {
	[System.Serializable]
	public class DistanceTarget {
		public Transform Target;
		[HideInInspector] public double TPX, TPY, TPZ;
	}

	public class Distance : Objective {

		public double Radius = 0.1;
		public Transform[] Targets = new Transform[0];
		[HideInInspector] public Vector3[] Positions = new Vector3[0];

		private const double PI = 3.14159265358979;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Distance;
		}

		public override void UpdateObjective() {
			if(Targets.Length != Positions.Length) {
				System.Array.Resize(ref Positions, Targets.Length);
			}
			for(int i=0; i<Targets.Length; i++) {
				if(Targets[i] != null) {
					Positions[i] = Targets[i].position;
				}
			}
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double loss = 0.0;
			for(int i=0; i<Targets.Length; i++) {
				if(Targets[i] != null) {
					double dist = System.Math.Sqrt((Positions[i].x-WPX)*(Positions[i].x-WPX) + (Positions[i].y-WPY)*(Positions[i].y-WPY) + (Positions[i].z-WPZ)*(Positions[i].z-WPZ));
					double x = dist - Radius;
					if(x <= 0.0) {
						return float.MaxValue;
					} else {
						loss += 1.0/x;
					}
				}
			}
			loss /= Targets.Length;
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			for(int i=0; i<Targets.Length; i++) {
				if(Targets[i] != null) {
					if(System.Math.Sqrt((Positions[i].x-WPX)*(Positions[i].x-WPX) + (Positions[i].y-WPY)*(Positions[i].y-WPY) + (Positions[i].z-WPZ)*(Positions[i].z-WPZ)) <= Radius) {
						return false;
					}
				}
			}
			return true;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double dist = 0.0;
			for(int i=0; i<Targets.Length; i++) {
				if(Targets[i] != null) {
					dist = System.Math.Max(dist, System.Math.Sqrt((Positions[i].x-WPX)*(Positions[i].x-WPX) + (Positions[i].y-WPY)*(Positions[i].y-WPY) + (Positions[i].z-WPZ)*(Positions[i].z-WPZ)));
				}
			}
			return dist;
		}

		/*
		public void SetTarget(Transform target) {
			Target = target;
		}

		public void SetTarget(Vector3 position) {
			TPX = position.x;
			TPY = position.y;
			TPZ = position.z;
		}
		*/

		/*
		public void SetMinimum(double value) {
			Minimum = value;
		}
		*/

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position, (float)Radius);
			for(int i=0; i<Targets.Length; i++) {
				if(Targets[i] != null) {
					Gizmos.DrawWireSphere(Targets[i].position, (float)Radius);
				}
			}
		}
	}
}