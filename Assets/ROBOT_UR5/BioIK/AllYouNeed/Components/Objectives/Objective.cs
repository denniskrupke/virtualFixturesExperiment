using UnityEngine;

namespace BioIK {
	//Objective to specify kinematic postures.
	[ExecuteInEditMode]
	public abstract class Objective : MonoBehaviour {

		public double Weight = 1.0;
		private bool Controlled = false;

		void Awake() {
			UpdateObjective();

			Refresh();
		}

		void LateUpdate() {
			if(Controlled) {
				return;
			}

			UpdateObjective();
		}

		public void SetControlled(bool value) {
			Controlled = value;
		}

		public abstract ObjectiveType GetObjectiveType();
		public abstract void UpdateObjective();
		public abstract double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration);
		public abstract bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration);
		public abstract double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration);

		void OnEnable() {
			Refresh();
		}

		void OnDisable() {
			Refresh();
		}

		void OnDestroy() {
			Refresh();
		}

		private void Refresh() {
			IKSolver solver = SearchIKSolver();
			if(solver != null) {
				solver.Rebuild();
			}
		}

		private IKSolver SearchIKSolver() {
			Transform t = transform;
			while(true) {
				IKSolver solver = t.GetComponent<IKSolver>();
				if(solver != null) {
					return solver;
				} else if(t != t.root) {
					t = t.parent;
				} else {
					return null;
				}
			}
		}
	}
}



		/*
		public double ComputeTranslationalDistance(double WPX, double WPY, double WPZ) {
			//Euclidean Distance: ||A-B||
			return System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
		}

		public double ComputeRotationalDistance(double WRX, double WRY, double WRZ, double WRW) {
			//Quaternion Angle: 2*ACos(|AxB|)
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
			}
			if(d > 1.0) {
				d = 1.0;
			}
			return 2.0 * System.Math.Acos(d);
		}

		public double ComputeDirectionalDistance(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW) {
			//Vector Angle: Acos(Dot/Length)
			double aX = 2.0 * ((0.5 - (WRY * WRY + WRZ * WRZ)) * Direction.x + (WRX * WRY - WRW * WRZ) * Direction.y + (WRX * WRZ + WRW * WRY) * Direction.z);
			double aY = 2.0 * ((WRX * WRY + WRW * WRZ) * Direction.x + (0.5 - (WRX * WRX + WRZ * WRZ)) * Direction.y + (WRY * WRZ - WRW * WRX) * Direction.z);
			double aZ = 2.0 * ((WRX * WRZ - WRW * WRY) * Direction.x + (WRY * WRZ + WRW * WRX) * Direction.y + (0.5 - (WRX * WRX + WRY * WRY)) * Direction.z);
			double bX = TPX-WPX;
			double bY = TPY-WPY;
			double bZ = TPZ-WPZ;
			double dot = aX*bX + aY*bY + aZ*bZ;
			double len = System.Math.Sqrt(aX*aX + aY*aY + aZ*aZ) * System.Math.Sqrt(bX*bX + bY*bY + bZ*bZ);
			double arg = dot/len;
			if(arg > 1.0) {
				arg = 1.0;
			} else if(arg < -1.0) {
				arg = -1.0;
			}
			return System.Math.Acos(arg);
		}

		public double ComputeAngularScale(double Length) {
			return Length / PI;
		}

		public double ComputeAngularScale(double WPX, double WPY, double WPZ, double RootX, double RootY, double RootZ, double Length) {
			return System.Math.Sqrt(Length*System.Math.Sqrt((WPX-RootX)*(WPX-RootX) + (WPY-RootY)*(WPY-RootY) + (WPZ-RootZ)*(WPZ-RootZ))) / PI;
		}
		*/
