using UnityEngine;

//!!!!!!
//This objective type is still under development
//!!!!!!

namespace BioIK {
	public class JointValue : Objective {

		public IKSolver Solver;
		public KinematicJoint Joint;
		public double TargetValue = 0.0;
		public bool X,Y,Z = false;

		private int Index = -1;

		private const double Deg2Rad = 0.017453292;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Displacement;
		}

		public override void UpdateObjective() {
			if(Solver == null || Joint == null) {
				TargetValue = 0.0;
				X = Y = Z = false;
				Index = -1;
				return;
			}
			if(Solver.GetModel() == null || Solver.GetEvolution() == null) {
				return;
			}
			if(IsValid()) {
				if(Index == -1) {
					if(X) {
						Index = Solver.GetModel().FindMotionPtr(Joint.GetXMotion()).Index;
					}
					if(Y) {
						Index = Solver.GetModel().FindMotionPtr(Joint.GetYMotion()).Index;
					}
					if(Z) {
						Index = Solver.GetModel().FindMotionPtr(Joint.GetZMotion()).Index;
					}
				}
			} else {
				TargetValue = 0.0;
				X = Y = Z = false;
				Index = -1;
			}
		}

		public void SetSolver(IKSolver solver) {
			Solver = solver;
		}

		public void SetJoint(KinematicJoint joint) {
			Joint = joint;
		}

		public void SetAxis(bool x, bool y, bool z) {
			X = x; Y = y; Z = z;
		}

		private bool IsValid() {
			return 
			(X && Joint.GetXMotion().IsEnabled() && !Y && !Z) 
			||
			(!X && Y && Joint.GetYMotion().IsEnabled() && !Z)
			||
			(!X && !Y && Z && Joint.GetZMotion().IsEnabled()
			);
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			if(Index == -1) {
				return 0.0;
			}
			double loss = configuration[Index] - Deg2Rad*TargetValue;
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return true;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			if(Index == -1) {
				return 0.0;
			}
			return configuration[Index] - Deg2Rad*TargetValue;
		}
	}
}