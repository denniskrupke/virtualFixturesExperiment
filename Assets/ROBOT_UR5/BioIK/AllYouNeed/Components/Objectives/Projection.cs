using UnityEngine;

namespace BioIK {
	public class Projection : Objective {

		public Transform Target;
		public double MaximumError = 0.001;
		public Vector3 Normal;
		public float Offset;
		public float Length;

		private Vector3 Position;
		private Quaternion Rotation;

		private double TPX, TPY, TPZ;
		private double TRX, TRY, TRZ, TRW;
		private const double PI = 3.14159265358979;

		//public Correction[] Corrections = new Correction[0];

		private RaycastHit Hit;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Projection;
		}

		public override void UpdateObjective() {
			if(Target != null) {
				Position = Target.position;
				Rotation = Target.rotation;
			}

			Vector3 normal = Rotation * Normal;
			Vector3 start = Position + Offset * normal;
			Vector3 end = start + Length * normal;

			if(Physics.Raycast(start, end-start, out Hit, Length)) {
				if(Hit.collider.transform.root != transform.root) {
					Position = Hit.point;
					Rotation = Quaternion.FromToRotation(normal, -Hit.normal.normalized) * Rotation;
				}
			}

			/*
			Vector3 correctionPosition = Vector3.zero;
			Quaternion correctionRotation = Quaternion.identity;
			for(int i=0; i<Corrections.Length; i++) {
				if(Physics.Raycast(position + rotation*Corrections[i].Offset, rotation*Corrections[i].Direction.normalized, out Hit, Corrections[i].Length)) {
					if(Hit.collider.transform.root != transform.root) {
						float weight = (Corrections[i].Length-Hit.distance) / Corrections[i].Length;
						correctionPosition += weight * (Hit.point-position);
						correctionRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(rotation*Normal.normalized, -Hit.normal.normalized), weight) * correctionRotation;
					}
				}
			}

			position += correctionPosition;
			rotation = correctionRotation * rotation;
			*/

			TPX = Position.x;
			TPY = Position.y;
			TPZ = Position.z;
			TRX = Rotation.x;
			TRY = Rotation.y;
			TRZ = Rotation.z;
			TRW = Rotation.w;
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
			double s = System.Math.Sqrt((node.Chain.Length+d)*(System.Math.Sqrt((WPX-node.RootX)*(WPX-node.RootX) + (WPY-node.RootY)*(WPY-node.RootY) + (WPZ-node.RootZ)*(WPZ-node.RootZ))+d));
			d = PI * d / s;
			double o = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(o < 0.0) {
				o = -o;
			}
			if(o > 1.0) {
				o = 1.0;
			}
			o = 2.0 * System.Math.Acos(o);
			return Weight * 0.5 * (d*d + o*o);
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double d = System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
			double s = System.Math.Sqrt((node.Chain.Length+d)*(System.Math.Sqrt((WPX-node.RootX)*(WPX-node.RootX) + (WPY-node.RootY)*(WPY-node.RootY) + (WPZ-node.RootZ)*(WPZ-node.RootZ))+d));
			d = PI * d / s;
			double o = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(o < 0.0) {
				o = -o;
			}
			if(o > 1.0) {
				o = 1.0;
			}
			o = 2.0 * System.Math.Acos(o);
			return d <= MaximumError && o <= MaximumError;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return 0.0;
		}

		public void SetTarget(Transform target) {
			Target = target;
		}

		public void SetTarget(Vector3 position, Quaternion rotation) {
			Position = position;
			Rotation = rotation;
		}

		public void SetMaximumError(double value) {
			MaximumError = value;
		}

		public void SetNormal(Vector3 normal) {
			Normal = normal;
		}

		public void SetOffset(float offset) {
			Offset = offset;
		}

		public void SetLength(float length) {
			Length = length;
		}

		void OnDrawGizmosSelected() {
			Vector3 normal = transform.rotation * Normal;
			Vector3 start = transform.position + Offset * normal;
			Vector3 end = start + Length * normal;

			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(start, 0.025f);
			Gizmos.DrawLine(start, end);
			Gizmos.DrawSphere(end, 0.01f);

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(new Vector3((float)TPX, (float)TPY, (float)TPZ), 0.025f);

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

			/*
			for(int i=0; i<Corrections.Length; i++) {
				normal = transform.rotation * Corrections[i].Direction;
				start = transform.position + transform.rotation * Corrections[i].Offset;
				end = start + Corrections[i].Length * normal;

				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(start, 0.025f);
				Gizmos.DrawLine(start, end);
				Gizmos.DrawSphere(end, 0.025f);
			}
			*/
		}
	}
}