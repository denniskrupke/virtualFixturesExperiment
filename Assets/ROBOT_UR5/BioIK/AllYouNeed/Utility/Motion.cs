using UnityEngine;

namespace BioIK {
	//Motion controller class to perform joint space movement for the joint axes.
	[System.Serializable]
	public class Motion {
		public KinematicJoint Joint;							//Reference to the joint
		[SerializeField] private bool Enabled = false;			//Is this joint enabled?
		[SerializeField] private float LowerLimit = 0f;			//Lower limit in radians / units
		[SerializeField] private float UpperLimit = 0f;			//Upper limit in radians / units
		[SerializeField] private float TargetValue = 0f;		//Target value to approach in radians / units
		[SerializeField] private float CurrentValue = 0f;		//Currently assigned value in radians / units
		public float CurrentError {get; private set;}			//Current error to the target value in radians / units
		public float CurrentAcceleration {get; private set;}	//Current acceleration of the joint in radians / units
		public float CurrentVelocity {get; private set;}		//Current velocity of the joint in radians / units
		public Vector3 Axis;									//Motion axis

		private float Speedup = 1f;
		private float Slowdown = 1f;

		public Motion(Vector3 axis) {
			Axis = axis;
		}

		//Runs one motion control cycle
		public double UpdateMotion() {
			if(!Enabled) {
				return CurrentValue;
			}

			if(!Application.isPlaying) {
				UpdateInstantaneous();
			} else {
				if(Joint.GetMotionType() == MotionType.Instantaneous) {
					UpdateInstantaneous();
				}
				if(Joint.GetMotionType() == MotionType.Realistic) {
					UpdateRealistic();
				}
			}

			return CurrentValue;
		}

		//Performs instantaneous motion control
		private void UpdateInstantaneous() {
			CurrentValue = TargetValue;
			CurrentError = 0f;
			CurrentVelocity = 0f;
			CurrentAcceleration = 0f;
		}

		//Performs realistic motion control
		//Input: TargetValue, CurrentValue, CurrentVelocity (initially 0), CurrentAcceleration (initially 0), MaximumVelocity, MaximumAcceleration
		//Output: CurrentValue, CurrentVelocity, CurrentAcceleration
		private void UpdateRealistic() {
			if(Time.deltaTime == 0f) {
				return;
			}
			
			//Compute current error
			CurrentError = TargetValue-CurrentValue;	

			//Minimum distance to stop: s = |(v^2)/(2a_max)| + |a/2*t^2| + |v*t|
			float stoppingDistance = 
				Mathf.Abs((CurrentVelocity*CurrentVelocity)/(2f*Joint.GetMaximumAcceleration()*Slowdown))
				+ Mathf.Abs(CurrentAcceleration)/2f*Time.deltaTime*Time.deltaTime
				+ Mathf.Abs(CurrentVelocity)*Time.deltaTime;

			if(Mathf.Abs(CurrentError) > stoppingDistance) {
				//Accelerate
				CurrentAcceleration = Mathf.Sign(CurrentError)*Mathf.Min(Mathf.Abs(CurrentError) / Time.deltaTime, Joint.GetMaximumAcceleration()*Speedup);
			} else {
				//Deccelerate
				CurrentAcceleration = -Mathf.Sign(CurrentVelocity)*Mathf.Min(Mathf.Abs(CurrentVelocity) / Time.deltaTime, Joint.GetMaximumAcceleration(), Mathf.Abs((CurrentVelocity*CurrentVelocity)/(2f*CurrentError)));
			}

			//Compute new velocity
			CurrentVelocity += CurrentAcceleration*Time.deltaTime;

			//Clamp velocity
			CurrentVelocity = Mathf.Clamp(CurrentVelocity, -Joint.GetMaximumVelocity(), Joint.GetMaximumVelocity());
			
			//Update Current Value
			CurrentValue += CurrentVelocity*Time.deltaTime;
		}

		public void Reset() {
			CurrentError = 0f;
			CurrentVelocity = 0f;
			CurrentValue = 0f;
			TargetValue = 0f;
		}

		public void Stop() {
			TargetValue = CurrentValue;
		}

		public void SetTargetValue(float value) {
			if(Joint.GetJointType() == JointType.Continuous) {
				TargetValue = value;
			} else {
				if(value > UpperLimit) {
					value = UpperLimit;
				}
				if(value < LowerLimit) {
					value = LowerLimit;
				}
				TargetValue = value;
			}
		}

		public float GetTargetValue() {
			return TargetValue;
		}

		public float GetCurrentValue() {
			return CurrentValue;
		}

		public void SetEnabled(bool enabled) {
			Enabled = enabled;
		}

		public bool IsEnabled() {
			return Enabled;
		}

		public void SetLowerLimit(float value) {
			LowerLimit = Mathf.Min(0f, value);
		}

		public float GetLowerLimit() {
			return LowerLimit;
		}

		public void SetUpperLimit(float value) {
			UpperLimit = Mathf.Max(0f, value);
		}

		public float GetUpperLimit() {
			return UpperLimit;
		}
	}
}