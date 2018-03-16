using UnityEngine;

//!!!!!!
//This objective type is still under development
//!!!!!!

namespace BioIK {
	public class Displacement : Objective {

		public IKSolver Solver;

		private double[] Configuration;

		public override ObjectiveType GetObjectiveType() {
			return ObjectiveType.Displacement;
		}

		public override void UpdateObjective() {
			if(Solver != null) {
				if(Solver.GetModel() != null && Solver.GetEvolution() != null) {
					Configuration = Solver.GetEvolution().GetSolution();
				}
			}
		}

		public override double ComputeLoss(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			if(Configuration == null || Solver.GetModel() == null) {
				return 0.0;
			} else if(Configuration.Length != configuration.Length) {
				return 0.0;
			}
			double loss = 0.0;
			for(int i=0; i<Configuration.Length; i++) {
				double diff = System.Math.Abs(Configuration[i] - configuration[i]) / (Solver.GetModel().MotionPtrs[i].Motion.GetUpperLimit() - Solver.GetModel().MotionPtrs[i].Motion.GetLowerLimit());
				loss += diff;
			}
			loss /= Configuration.Length;
			return Weight * loss * loss;
		}

		public override bool CheckConvergence(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			return true;
		}

		public override double ComputeValue(double WPX, double WPY, double WPZ, double WRX, double WRY, double WRZ, double WRW, Model.Node node, double[] configuration) {
			double value = 0.0;
			for(int i=0; i<Configuration.Length; i++) {
				double diff = System.Math.Abs(Configuration[i] - configuration[i]) / (Solver.GetModel().MotionPtrs[i].Motion.GetUpperLimit() - Solver.GetModel().MotionPtrs[i].Motion.GetLowerLimit());
				value += diff*diff;
			}
			value /= configuration.Length;
			return System.Math.Sqrt(value);
		}

		public void SetSolver(IKSolver solver) {
			Solver = solver;
		}
	}
}