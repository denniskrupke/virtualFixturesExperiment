using UnityEngine;

namespace BioIK {
	//Kinematic joint to specify the 3D-motion of a segment.
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class KinematicJoint : MonoBehaviour {
		[SerializeField] private JointType JointType = JointType.Revolute;			//Type of the joint
		[SerializeField] private MotionType MotionType = MotionType.Instantaneous;	//Type of the applied motion
		[SerializeField] private Vector3 Anchor = Vector3.zero;						//Joint anchor
		[SerializeField] private Vector3 Orientation = Vector3.zero;				//Joint orientation
		[SerializeField] private Motion XMotion = new Motion(Vector3.right);		//X motion object
		[SerializeField] private Motion YMotion = new Motion(Vector3.up);			//Y motion object
		[SerializeField] private Motion ZMotion = new Motion(Vector3.forward);		//Z motion object
		[SerializeField] private float MaximumVelocity = 1f;						//Maximum joint velocity in radians / units
		[SerializeField] private float MaximumAcceleration = 1f;					//Maximum joint acceleration in radians / units
		[SerializeField] private float Smoothing = 0.5f;							//Factor to smooth Cartesian motion
		[SerializeField] private float AnimationWeight = 0.0f;						//Factor to weight animation into IK (note that this also shifts the joint limits)
		[SerializeField] private float AnimationBlend = 0.0f;						//Factor to blend between IK and animation

		[SerializeField] private bool Initialised = false;							//Whether the joint has already been initialised
		[SerializeField] private float DPX, DPY, DPZ, DRX, DRY, DRZ, DRW;			//Default frame

		private Vector3 AnimationPosition, AnimatedDefaultPosition;
		private Quaternion AnimationRotation, AnimatedDefaultRotation;

		private double R1, R2, R3, R4, R5, R6, R7, R8, R9;							//Precomputed rotation information
		private Vector3 LSA;														//LocalScaledAnchor
		private Vector3 ADPADRSA;													//AnimatedDefaultPosition + AnimatedDefaultRotation * LocalScaledAnchor

		private Vector3 LastPosition;
		private Quaternion LastRotation;
		private const float PI = 3.14159265358979f;

		private bool Controlled = false;

		void Awake() {
			transform.hideFlags = HideFlags.NotEditable;
			XMotion.Joint = this;
			YMotion.Joint = this;
			ZMotion.Joint = this;
			if(!Initialised) {
				Initialise();
			}
			LastPosition = GetDefaultPosition();
			LastRotation = GetDefaultRotation();

			Prepare();
		}

		public void Initialise() {
			SetDefaultFrame(transform.localPosition, transform.localRotation);
			SetJointType(JointType);
			SetMotionType(MotionType);
			SetAnchor(Anchor);
			SetOrientation(Orientation);
			Initialised = true;
		}

		void Update() {
			transform.hasChanged = false;
		}

		void LateUpdate() {
			if(Controlled) {
				return;
			}

			UpdateJoint();
		}

		public void Prepare() {
			if(transform.hasChanged) {
				AnimationPosition = transform.localPosition;
				AnimationRotation = transform.localRotation;
			} else {
				AnimationPosition = new Vector3(DPX, DPY, DPZ);
				AnimationRotation = new Quaternion(DRX, DRY, DRZ, DRW);
			}

			AnimatedDefaultPosition = (1f-AnimationWeight) * new Vector3(DPX, DPY, DPZ) + AnimationWeight * AnimationPosition;
			AnimatedDefaultRotation = Quaternion.Slerp(new Quaternion(DRX, DRY, DRZ, DRW), AnimationRotation, AnimationWeight);

			R1 = (1.0 - 2.0 * (AnimatedDefaultRotation.y * AnimatedDefaultRotation.y + AnimatedDefaultRotation.z * AnimatedDefaultRotation.z));
			R2 = 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.y + AnimatedDefaultRotation.w * AnimatedDefaultRotation.z);
			R3 = 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.z - AnimatedDefaultRotation.w * AnimatedDefaultRotation.y);
			R4 = 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.y - AnimatedDefaultRotation.w * AnimatedDefaultRotation.z);
			R5 = (1.0 - 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.x + AnimatedDefaultRotation.z * AnimatedDefaultRotation.z));
			R6 = 2.0 * (AnimatedDefaultRotation.y * AnimatedDefaultRotation.z + AnimatedDefaultRotation.w * AnimatedDefaultRotation.x);
			R7 = 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.z + AnimatedDefaultRotation.w * AnimatedDefaultRotation.y);
			R8 = 2.0 * (AnimatedDefaultRotation.y * AnimatedDefaultRotation.z - AnimatedDefaultRotation.w * AnimatedDefaultRotation.x);
			R9 = (1.0 - 2.0 * (AnimatedDefaultRotation.x * AnimatedDefaultRotation.x + AnimatedDefaultRotation.y * AnimatedDefaultRotation.y));
			LSA = Vector3.Scale(Anchor, transform.localScale);
			ADPADRSA = AnimatedDefaultPosition + AnimatedDefaultRotation * LSA;
		}

		public void UpdateJoint() {
			//Compute local transformation
			double lpX, lpY, lpZ, lrX, lrY, lrZ, lrW;
			ComputeLocalTransformation(XMotion.UpdateMotion(), YMotion.UpdateMotion(), ZMotion.UpdateMotion(), out lpX, out lpY, out lpZ, out lrX, out lrY, out lrZ, out lrW);

			//Apply local transformation
			if(Application.isPlaying) {
				//Assigning transformation
				transform.localPosition = (1f-Smoothing) * new Vector3((float)lpX, (float)lpY, (float)lpZ) + Smoothing * LastPosition;
				transform.localRotation = Quaternion.Slerp(new Quaternion((float)lrX, (float)lrY, (float)lrZ, (float)lrW), LastRotation, Smoothing);

				//Blending animation
				transform.localPosition = (1f-AnimationBlend) * transform.localPosition + (AnimationBlend) * AnimationPosition;
				transform.localRotation = Quaternion.Slerp(transform.localRotation, AnimationRotation, AnimationBlend);
			} else {
				transform.localPosition = new Vector3((float)lpX, (float)lpY, (float)lpZ);
				transform.localRotation = new Quaternion((float)lrX, (float)lrY, (float)lrZ, (float)lrW);
			}

			//Remember transformation
			LastPosition = transform.localPosition;
			LastRotation = transform.localRotation;

			transform.hasChanged = false;
		}

		public void SetControlled(bool value) {
			Controlled = value;
		}

		//Fast implementation to compute the local transform given the joint values
		public void ComputeLocalTransformation(double valueX, double valueY, double valueZ, out double lpX, out double lpY, out double lpZ, out double lrX, out double lrY, out double lrZ, out double lrW) {
			if(JointType == JointType.Prismatic) {
				//LocalPosition = DefaultLocalPosition + (Values . Axes)
				//LocalRotation = DefaultLocalRotation
				double x = valueX * XMotion.Axis.x + valueY * YMotion.Axis.x + valueZ * ZMotion.Axis.x;
				double y = valueX * XMotion.Axis.y + valueY * YMotion.Axis.y + valueZ * ZMotion.Axis.y;
				double z = valueX * XMotion.Axis.z + valueY * YMotion.Axis.z + valueZ * ZMotion.Axis.z;
				//Local position for translational motion
				lpX = AnimatedDefaultPosition.x + R1 * x + R4 * y + R7 * z;
				lpY = AnimatedDefaultPosition.y + R2 * x + R5 * y + R8 * z;
				lpZ = AnimatedDefaultPosition.z + R3 * x + R6 * y + R9 * z;
				//Local rotation for translational motion
				lrX = AnimatedDefaultRotation.x; lrY = AnimatedDefaultRotation.y; lrZ = AnimatedDefaultRotation.z; lrW = AnimatedDefaultRotation.w;
			} else {
				//LocalPosition = WorldAnchor + AngleAxis(roll) * AngleAxis(pitch) * AngleAxis(yaw) * (-LocalAnchor)
				//LocalRotation = DefaultLocalRotation * AngleAxis(roll) * AngleAxis(pitch) * AngleAxis(yaw)
				double sin, x1, y1, z1, w1, x2, y2, z2, w2, qx, qy, qz, qw = 0.0;
				if(valueZ != 0.0) {
					sin = System.Math.Sin(valueZ/2.0);
					qx = ZMotion.Axis.x * sin;
					qy = ZMotion.Axis.y * sin;
					qz = ZMotion.Axis.z * sin;
					qw = System.Math.Cos(valueZ/2.0);
					if(valueX != 0.0) {
						sin = System.Math.Sin(valueX/2.0);
						x1 = XMotion.Axis.x * sin;
						y1 = XMotion.Axis.y * sin;
						z1 = XMotion.Axis.z * sin;
						w1 = System.Math.Cos(valueX/2.0);
						x2 = qx; y2 = qy; z2 = qz; w2 = qw;
						qx = x1 * w2 + y1 * z2 - z1 * y2 + w1 * x2;
						qy = -x1 * z2 + y1 * w2 + z1 * x2 + w1 * y2;
						qz = x1 * y2 - y1 * x2 + z1 * w2 + w1 * z2;
						qw = -x1 * x2 - y1 * y2 - z1 * z2 + w1 * w2;
						if(valueY != 0.0) {
							sin = System.Math.Sin(valueY/2.0);
							x1 = YMotion.Axis.x * sin;
							y1 = YMotion.Axis.y * sin;
							z1 = YMotion.Axis.z * sin;
							w1 = System.Math.Cos(valueY/2.0);
							x2 = qx; y2 = qy; z2 = qz; w2 = qw;
							qx = x1 * w2 + y1 * z2 - z1 * y2 + w1 * x2;
							qy = -x1 * z2 + y1 * w2 + z1 * x2 + w1 * y2;
							qz = x1 * y2 - y1 * x2 + z1 * w2 + w1 * z2;
							qw = -x1 * x2 - y1 * y2 - z1 * z2 + w1 * w2;
						} else {
						}
					} else if(valueY != 0.0) {
						sin = System.Math.Sin(valueY/2.0);
						x1 = YMotion.Axis.x * sin;
						y1 = YMotion.Axis.y * sin;
						z1 = YMotion.Axis.z * sin;
						w1 = System.Math.Cos(valueY/2.0);
						x2 = qx; y2 = qy; z2 = qz; w2 = qw;
						qx = x1 * w2 + y1 * z2 - z1 * y2 + w1 * x2;
						qy = -x1 * z2 + y1 * w2 + z1 * x2 + w1 * y2;
						qz = x1 * y2 - y1 * x2 + z1 * w2 + w1 * z2;
						qw = -x1 * x2 - y1 * y2 - z1 * z2 + w1 * w2;
					} else {
					}
				} else if(valueX != 0.0) {
					sin = System.Math.Sin(valueX/2.0);
					qx = XMotion.Axis.x * sin;
					qy = XMotion.Axis.y * sin;
					qz = XMotion.Axis.z * sin;
					qw = System.Math.Cos(valueX/2.0);
					if(valueY != 0.0) {
						sin = System.Math.Sin(valueY/2.0);
						x1 = YMotion.Axis.x * sin;
						y1 = YMotion.Axis.y * sin;
						z1 = YMotion.Axis.z * sin;
						w1 = System.Math.Cos(valueY/2.0);
						x2 = qx; y2 = qy; z2 = qz; w2 = qw;
						qx = x1 * w2 + y1 * z2 - z1 * y2 + w1 * x2;
						qy = -x1 * z2 + y1 * w2 + z1 * x2 + w1 * y2;
						qz = x1 * y2 - y1 * x2 + z1 * w2 + w1 * z2;
						qw = -x1 * x2 - y1 * y2 - z1 * z2 + w1 * w2;
					} else {
					}
				} else if(valueY != 0.0) {
					sin = System.Math.Sin(valueY/2.0);
					qx = YMotion.Axis.x * sin;
					qy = YMotion.Axis.y * sin;
					qz = YMotion.Axis.z * sin;
					qw = System.Math.Cos(valueY/2.0);
				} else {
					lpX = AnimatedDefaultPosition.x;
					lpY = AnimatedDefaultPosition.y;
					lpZ = AnimatedDefaultPosition.z;
					lrX = AnimatedDefaultRotation.x;
					lrY = AnimatedDefaultRotation.y;
					lrZ = AnimatedDefaultRotation.z;
					lrW = AnimatedDefaultRotation.w;
					return;
				}

				//Local Rotation
				//R' = R*Q
				lrX = AnimatedDefaultRotation.x * qw + AnimatedDefaultRotation.y * qz - AnimatedDefaultRotation.z * qy + AnimatedDefaultRotation.w * qx;
				lrY = -AnimatedDefaultRotation.x * qz + AnimatedDefaultRotation.y * qw + AnimatedDefaultRotation.z * qx + AnimatedDefaultRotation.w * qy;
				lrZ = AnimatedDefaultRotation.x * qy - AnimatedDefaultRotation.y * qx + AnimatedDefaultRotation.z * qw + AnimatedDefaultRotation.w * qz;
				lrW = -AnimatedDefaultRotation.x * qx - AnimatedDefaultRotation.y * qy - AnimatedDefaultRotation.z * qz + AnimatedDefaultRotation.w * qw;

				//Local Position
				if(LSA.x == 0.0 && LSA.y == 0.0 && LSA.z == 0.0) {
					//P' = Pz
					lpX = AnimatedDefaultPosition.x;
					lpY = AnimatedDefaultPosition.y;
					lpZ = AnimatedDefaultPosition.z;
				} else {
					//P' = P + RA + R*Q*(-A)
					lpX = ADPADRSA.x + 2.0 * ((0.5 - lrY * lrY - lrZ * lrZ) * -LSA.x + (lrX * lrY - lrW * lrZ) * -LSA.y + (lrX * lrZ + lrW * lrY) * -LSA.z);
					lpY = ADPADRSA.y + 2.0 * ((lrX * lrY + lrW * lrZ) * -LSA.x + (0.5 - lrX * lrX - lrZ * lrZ) * -LSA.y + (lrY * lrZ - lrW * lrX) * -LSA.z);
					lpZ = ADPADRSA.z + 2.0 * ((lrX * lrZ - lrW * lrY) * -LSA.x + (lrY * lrZ + lrW * lrX) * -LSA.y + (0.5 - lrX * lrX - lrY * lrY) * -LSA.z);
				}
			}
		}

		public int GetDoF() {
			int dof = 0;
			if(XMotion.IsEnabled()) {
				dof += 1;
			}
			if(YMotion.IsEnabled()) {
				dof += 1;
			}
			if(ZMotion.IsEnabled()) {
				dof += 1;
			}
			return dof;
		}

		public void SetJointType(JointType type) {
			if(type == JointType.Continuous) {
				XMotion.SetLowerLimit(0f);
				XMotion.SetUpperLimit(0f);
				YMotion.SetLowerLimit(0f);
				YMotion.SetUpperLimit(0f);
				ZMotion.SetLowerLimit(0f);
				ZMotion.SetUpperLimit(0f);
			}
			JointType = type;
			if(type == JointType.Continuous) {
				XMotion.SetLowerLimit(-PI);
				XMotion.SetUpperLimit(PI);
				YMotion.SetLowerLimit(-PI);
				YMotion.SetUpperLimit(PI);
				ZMotion.SetLowerLimit(-PI);
				ZMotion.SetUpperLimit(PI);
			}
		}

		public JointType GetJointType() {
			return JointType;
		}


		public void SetMotionType(MotionType type) {
			MotionType = type;
		}

		public MotionType GetMotionType() {
			return MotionType;
		}

		public Motion GetXMotion() {
			return XMotion;
		}

		public Motion GetYMotion() {
			return YMotion;
		}

		public Motion GetZMotion() {
			return ZMotion;
		}
		
		public void SetSmoothing(float value) {
			Smoothing = Mathf.Clamp(value, 0f, 1f);
		}

		public float GetSmoothing() {
			return Smoothing;
		}

		public void SetAnimationWeight(float value) {
			AnimationWeight = Mathf.Clamp(value, 0f, 1f);
		}

		public float GetAnimationWeight() {
			return AnimationWeight;
		}

		public void SetAnimationBlend(float value) {
			AnimationBlend = Mathf.Clamp(value, 0f, 1f);
		}

		public float GetAnimationBlend() {
			return AnimationBlend;
		}

		public void SetMaximumVelocity(float value) {
			MaximumVelocity = Mathf.Max(0f, value);
		}

		public float GetMaximumVelocity() {
			return MaximumVelocity;
		}

		public void SetMaximumAcceleration(float value) {
			MaximumAcceleration = Mathf.Max(0f, value);
		}

		public float GetMaximumAcceleration() {
			return MaximumAcceleration;
		}

		public void SetUpperLimits(Vector3 upper) {
			SetUpperLimits(upper.x, upper.y, upper.z);
		}

		public void SetUpperLimits(float x, float y, float z) {
			XMotion.SetUpperLimit(x);
			YMotion.SetUpperLimit(y);
			ZMotion.SetUpperLimit(z);
		}

		public Vector3 GetUpperLimits() {
			return new Vector3(XMotion.GetUpperLimit(), YMotion.GetUpperLimit(), ZMotion.GetUpperLimit());
		}

		public void SetLowerLimits(Vector3 lower) {
			SetLowerLimits(lower.x, lower.y, lower.z);
		}

		public void SetLowerLimits(float x, float y, float z) {
			XMotion.SetLowerLimit(x);
			YMotion.SetLowerLimit(y);
			ZMotion.SetLowerLimit(z);
		}

		public Vector3 GetLowerLimits() {
			return new Vector3(XMotion.GetLowerLimit(), YMotion.GetLowerLimit(), ZMotion.GetLowerLimit());
		}

		public void SetTargetValues(Vector3 values) {
			SetTargetValues(values.x, values.y, values.z);
		}

		public void SetTargetValues(float x, float y, float z) {
			XMotion.SetTargetValue(x);
			YMotion.SetTargetValue(y);
			ZMotion.SetTargetValue(z);
		}

		public Vector3 GetDefaultPosition() {
			return new Vector3(DPX, DPY, DPZ);
		}

		public Quaternion GetDefaultRotation() {
			return new Quaternion(DRX, DRY, DRZ, DRW);
		}

		public void SetAnchor(Vector3 anchor) {
			Anchor = anchor;
			Prepare();
		}

		public Vector3 GetAnchor() {
			return Anchor;
		}

		public Vector3 GetAnchorInWorldSpace() {
			return transform.position + transform.rotation * Vector3.Scale(transform.lossyScale, Anchor);
		}

		public void SetOrientation(Vector3 orientation) {
			Orientation = orientation;
			Quaternion o = Quaternion.Euler(Orientation);
			XMotion.Axis = o * Vector3.right;
			YMotion.Axis = o * Vector3.up;
			ZMotion.Axis = o * Vector3.forward;
		}

		public Vector3 GetOrientation() {
			return Orientation;
		}

		//Precomputes the constant default transformation 
		public void SetDefaultFrame(Vector3 localPosition, Quaternion localRotation) {
			DPX = localPosition.x;
			DPY = localPosition.y;
			DPZ = localPosition.z;
			DRX = localRotation.x;
			DRY = localRotation.y;
			DRZ = localRotation.z;
			DRW = localRotation.w;

			Prepare();
		}

		void OnEnable() {
			Refresh();
		}

		void OnDisable() {
			Refresh();
		}

		void OnDestroy() {
			Refresh();
			transform.hideFlags = HideFlags.None;
			transform.localPosition = new Vector3(DPX, DPY, DPZ);
			transform.localRotation = new Quaternion(DRX, DRY, DRZ, DRW);
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