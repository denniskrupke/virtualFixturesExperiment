using UnityEngine;

namespace BioIK {
	public class LookAt : Objective {

		public Transform Target;
		public Vector3 Direction = Vector3.forward;
		public double MaximumError = 0.01;

		private double TPX, TPY, TPZ;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.LookAt;
		}

		public override void UpdateObjective() {
			if(Target != null) {
				Vector3 position = Target.position;
				TPX = position.x;
				TPY = position.y;
				TPZ = position.z;
			}
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
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
			double loss = System.Math.Acos(arg);
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
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
			return System.Math.Acos(arg) <= MaximumError;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
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

		public void SetTarget(Transform target) {
			Target = target;
		}

		public void SetTarget(Vector3 position) {
			TPX = position.x;
			TPY = position.y;
			TPZ = position.z;
		}

		public void SetDirection(Vector3 direction) {
			Direction = direction;
		}

		public void SetMaximumError(double value) {
			MaximumError = value;
		}
	}
}