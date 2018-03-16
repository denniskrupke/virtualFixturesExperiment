/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BioIK {
		[System.Serializable]
		public class Projection {

		public Vector3 Normal;
		public float Offset;
		public float Length;

		private RaycastHit Hit;

		public void Project(ref Vector3 position, ref Quaternion rotation, Transform t) {
			Vector3 normal = rotation * Normal;
			Vector3 start = position + Offset * normal;
			Vector3 end = start + Length * normal;

			if(Physics.Raycast(start, end-start, out Hit, Length)) {
				if(Hit.collider.transform.root != t.root) {
					position = Hit.point;
					rotation = Quaternion.FromToRotation(normal, -Hit.normal.normalized) * rotation;
				}
			}
		}

		public void DrawGizmos(Vector3 position, Quaternion rotation, Transform t) {
			Vector3 normal = rotation * Normal;
			Vector3 start = position + Offset * normal;
			Vector3 end = start + Length * normal;

			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(start, 0.025f);
			Gizmos.DrawLine(start, end);
			Gizmos.DrawSphere(end, 0.01f);

			Gizmos.color = Color.red;
			Vector3 pos = new Vector3(position.x, position.y, position.z);
			Quaternion rot = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
			Project(ref pos, ref rot, t.root);
			Gizmos.DrawSphere(pos, 0.025f);
		}
	}
}
*/

//!!!!!!
//This objective type is still under development
//!!!!!!

/*
namespace BioIK {
	public class Projection : Objective {

		public Vector3 Normal;
		public float Offset;
		public float Length;

		public Correction[] Corrections = new Correction[0];

		private double TPX, TPY, TPZ;
		private double TRX, TRY, TRZ, TRW;
		private const double PI = 3.14159265358979;

		private RaycastHit Hit;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Projection;
		}

		public override void UpdateObjective() {
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;

			Vector3 normal = rotation * Normal;
			Vector3 start = position + Offset * normal;
			Vector3 end = start + Length * normal;

			if(Physics.Raycast(start, end-start, out Hit, Length)) {
				if(Hit.collider.transform.root != transform.root) {
					position = Hit.point;
					rotation = Quaternion.FromToRotation(normal, -Hit.normal.normalized) * rotation;
				}
			}

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

			TPX = position.x;
			TPY = position.y;
			TPZ = position.z;
			TRX = rotation.x;
			TRY = rotation.y;
			TRZ = rotation.z;
			TRW = rotation.w;
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double p = System.Math.Sqrt((TPX-WPX)*(TPX-WPX) + (TPY-WPY)*(TPY-WPY) + (TPZ-WPZ)*(TPZ-WPZ));
			double s = System.Math.Sqrt((node.Chain.Length+p)*(System.Math.Sqrt((WPX-node.RootX)*(WPX-node.RootX) + (WPY-node.RootY)*(WPY-node.RootY) + (WPZ-node.RootZ)*(WPZ-node.RootZ))+p));
			p = PI * p / s;
			double o = WRX*TRX + WRY*TRY + WRZ*TRZ + WRW*TRW;
			if(o < 0.0) {
				o = -o;
			}
			if(o > 1.0) {
				o = 1.0;
			}
			o = 2.0 * System.Math.Acos(o);
			p *= 0.5;
			o *= 0.5;
			double loss = p*p + o*o;
			return Weight * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return false;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return 0.0;
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
			Gizmos.DrawSphere(end, 0.025f);

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(new Vector3((float)TPX, (float)TPY, (float)TPZ), 0.025f);

			for(int i=0; i<Corrections.Length; i++) {
				normal = transform.rotation * Corrections[i].Direction;
				start = transform.position + transform.rotation * Corrections[i].Offset;
				end = start + Corrections[i].Length * normal;

				Gizmos.color = Color.cyan;
				Gizmos.DrawSphere(start, 0.025f);
				Gizmos.DrawLine(start, end);
				Gizmos.DrawSphere(end, 0.025f);
			}
		}
	}

	[System.Serializable]
	public class Correction {
		public Vector3 Offset;
		public Vector3 Direction;
		public float Length;
	}
}
*/