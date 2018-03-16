using UnityEngine;

namespace BioIK {
	public class Position : Objective {

		public Transform Target;
		public double MaximumError = 0.001;

		private double TPX, TPY, TPZ;
		private const double PI = 3.14159265358979;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Position;
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
			double d = System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
			double s = System.Math.Sqrt((node.Chain.Length+d)*(System.Math.Sqrt((WPX-node.RootX)*(WPX-node.RootX) + (WPY-node.RootY)*(WPY-node.RootY) + (WPZ-node.RootZ)*(WPZ-node.RootZ))+d));
			double loss = PI * d / s;
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ)) <= MaximumError;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
		}

		public void SetTarget(Transform target) {
			Target = target;
		}

		public void SetTarget(Vector3 position) {
			TPX = position.x;
			TPY = position.y;
			TPZ = position.z;
		}
		
		public void SetMaximumError(double value) {
			MaximumError = value;
		}

		public Vector3 GetTarget() {
			return new Vector3((float)TPX, (float)TPY, (float)TPZ);
		}

		void OnDrawGizmosSelected() {
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(new Vector3((float)TPX, (float)TPY, (float)TPZ), 0.025f);
		}
	}
}