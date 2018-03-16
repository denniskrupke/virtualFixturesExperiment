using UnityEngine;

namespace BioIK {
	public class Orientation : Objective {

		public Transform Target;
		public double MaximumError = 0.01;

		private double TRX, TRY, TRZ, TRW;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Orientation;
		}

		public override void UpdateObjective() {
			if(Target != null) {
				Quaternion rotation = Target.rotation;
				TRX = rotation.x;
				TRY = rotation.y;
				TRZ = rotation.z;
				TRW = rotation.w;
			}
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
			}
			if(d > 1.0) {
				d = 1.0;
			}
			double loss = 2.0 * System.Math.Acos(d);
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
			}
			if(d > 1.0) {
				d = 1.0;
			}
			return 2.0 * System.Math.Acos(d) <= MaximumError;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(d < 0.0) {
				d = -d;
			}
			if(d > 1.0) {
				d = 1.0;
			}
			return 2.0 * System.Math.Acos(d);
		}

		public void SetTarget(Transform target) {
			Target = target;
		}

		public void SetTarget(Quaternion rotation) {
			TRX = rotation.x;
			TRY = rotation.y;
			TRZ = rotation.z;
			TRW = rotation.w;
		}

		public void SetMaximumError(double value) {
			MaximumError = value;
		}

		public Quaternion GetTarget() {
			return new Quaternion((float)TRX, (float)TRY, (float)TRZ, (float)TRW);
		}

		void OnDrawGizmosSelected() {
			Quaternion rotation = new Quaternion((float)TRX, (float)TRY, (float)TRZ, (float)TRW);
			Vector3 right = rotation * Vector3.right;
			Vector3 up = rotation * Vector3.up;
			Vector3 forward = rotation * Vector3.forward;
			float length = 0.05f;
			
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position - length * right, transform.position + length * right);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position - length * up, transform.position + length * up);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position - length * forward, transform.position + length * forward);
		}

	}
}