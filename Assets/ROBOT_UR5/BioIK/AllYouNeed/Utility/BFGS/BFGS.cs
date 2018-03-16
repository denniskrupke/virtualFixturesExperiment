namespace BioIK {
    using System;

    ///   Limited-memory Broyden–Fletcher–Goldfarb–Shanno (L-BFGS) optimization method.

    ///   The L-BFGS algorithm is a member of the broad family of quasi-Newton optimization
    ///   methods. L-BFGS stands for 'Limited memory BFGS'. Indeed, L-BFGS uses a limited
    ///   memory variation of the Broyden–Fletcher–Goldfarb–Shanno (BFGS) update to approximate
    ///   the inverse Hessian matrix (denoted by Hk). Unlike the original BFGS method which
    ///   stores a dense  approximation, L-BFGS stores only a few vectors that represent the
    ///   approximation implicitly. Due to its moderate memory requirement, L-BFGS method is
    ///   particularly well suited for optimization problems with a large number of variables.
    ///   L-BFGS never explicitly forms or stores Hk. Instead, it maintains a history of the past
    ///   m updates of the position x and gradient g, where generally the history
    ///   m can be short, often less than 10. These updates are used to implicitly do operations
    ///   requiring the Hk-vector product.

    ///   The framework implementation of this method is based on the original FORTRAN source code
    ///   by Jorge Nocedal (see references below). The original FORTRAN source code of L-BFGS (for
    ///   unconstrained problems) is available at http://www.netlib.org/opt/lbfgs_um.shar and had
    ///   been made available under the public domain.
    /// 
    ///   References:
    ///        http://www.netlib.org/opt/lbfgs_um.shar
    ///        Jorge Nocedal. Limited memory BFGS method for large scale optimization (Fortran source code). 1990.
    ///        Available in http://www.netlib.org/opt/lbfgs_um.shar
    ///        Jorge Nocedal. Updating Quasi-Newton Matrices with Limited Storage. Mathematics of Computation,
    ///        Vol. 35, No. 151, pp. 773--782, 1980.
    ///        Dong C. Liu, Jorge Nocedal. On the limited memory BFGS method for large scale optimization.

    public partial class BFGS {

        public enum Task{None, Start, New_X, FG, FG_LN, FG_ST, Abnormal, Convergence, Error, Restart_LN, Warning};

        public Func<double[], double> Function { get; set; }
        public int NumberOfVariables { get; protected set; }
        public double[] Solution;
        public double Value;
        public Func<double[], double[]> Gradient { get; set; }

        private const double stpmin = 1e-20;
        private const double stpmax = 1e20;
        private int evaluations;
        private int corrections = 1;
        private double[] lowerBound;
        private double[] upperBound;
        private double[] work;
        private double factr = 1e+5;
        private double pgtol = 0.0;
        public int Evaluations
        {
            get { return evaluations; }
        }
        public int Corrections
        {
            get { return corrections; }
            set
            {
                if (value <= 0) {
                    UnityEngine.Debug.Log("Number of corrections should be higher than zero.");
                    return;
                }

                corrections = value;
            }
        }
        public double[] UpperBounds
        {
            get { return upperBound; }
        }
        public double[] LowerBounds
        {
            get { return lowerBound; }
        }
        public double FunctionTolerance
        {
            get { return factr; }
            set
            {
                if (value < 0) {
                    UnityEngine.Debug.Log("Tolerance must be greater than or equal to zero.");
                    return;
                }

                factr = value;
            }
        }
        public double GradientTolerance
        {
            get { return pgtol; }
            set { pgtol = value; }
        }

        public BFGS(int numberOfVariables, Func<double[], double> function, Func<double[], double[]> gradient) {
            NumberOfVariables = numberOfVariables;
            Function = function;
            Gradient = gradient;
            upperBound = new double[numberOfVariables];
            lowerBound = new double[numberOfVariables];
            Solution = new double[numberOfVariables];
        }

        public void Minimize(double[] values, ref bool evolving) {
            for(int i=0; i<NumberOfVariables; i++) {
                Solution[i] = values[i];
            }
            Optimize(ref evolving);
            Value = Function(Solution);
        }

        private void Optimize(ref bool evolving) {
            int n = NumberOfVariables;
            int m = corrections;

            Task task = Task.None;
            Task csave = Task.None;
            bool[] lsave = new bool[4];
            int iprint = 101;
            int[] nbd = new int[n];
            int[] iwa = new int[3 * n];
            int[] isave = new int[44];
            double f = 0.0d;
            double[] l = new double[n];
            double[] u = new double[n];
            double[] g = new double[n];
            double[] dsave = new double[29];

            int totalSize = 2 * m * n + 11 * m * m + 5 * n + 8 * m;

            if(work == null || work.Length < totalSize) {
                work = new double[totalSize];
            }

            for(int i=0; i < NumberOfVariables; i++) {
                nbd[i] = 2;
                l[i] = LowerBounds[i];
                u[i] = UpperBounds[i];
            }

            double newF = 0;
            double[] newG = null;

            task = Task.Start;

            while(evolving) {
                setulb(n, m, Solution, 0, l, 0, u, 0, nbd, 0, ref f, g, 0,
                    factr, pgtol, work, 0, iwa, 0, ref task, iprint, ref csave,
                    lsave, 0, isave, 0, dsave, 0);

                if (task == Task.FG_LN || task == Task.FG_ST) {
                    evaluations++;
                    newF = Function(Solution);
                    newG = Gradient(Solution);
                    f = newF;
                    for (int j = 0; j < newG.Length; j++) {
                        g[j] = newG[j];
                    }
                }
            }
        }

        public void Minimize(double[] values, double timeout, Model model) {
            for(int i=0; i<NumberOfVariables; i++) {
                Solution[i] = values[i];
            }
            Optimize(timeout, model);
            Value = Function(Solution);
        }

        private void Optimize(double timeout, Model model) {
            System.DateTime then = System.DateTime.Now;
            
            int n = NumberOfVariables;
            int m = corrections;

            Task task = Task.None;
            Task csave = Task.None;
            bool[] lsave = new bool[4];
            int iprint = 101;
            int[] nbd = new int[n];
            int[] iwa = new int[3 * n];
            int[] isave = new int[44];
            double f = 0.0d;
            double[] l = new double[n];
            double[] u = new double[n];
            double[] g = new double[n];
            double[] dsave = new double[29];

            int totalSize = 2 * m * n + 11 * m * m + 5 * n + 8 * m;

            if(work == null || work.Length < totalSize) {
                work = new double[totalSize];
            }

            for(int i=0; i < NumberOfVariables; i++) {
                nbd[i] = 2;
                l[i] = LowerBounds[i];
                u[i] = UpperBounds[i];
            }

            double newF = 0;
            double[] newG = null;

            task = Task.Start;

            while((System.DateTime.Now-then).Duration().TotalSeconds < timeout && !IsConverged(Solution, model)) {
                setulb(n, m, Solution, 0, l, 0, u, 0, nbd, 0, ref f, g, 0,
                    factr, pgtol, work, 0, iwa, 0, ref task, iprint, ref csave,
                    lsave, 0, isave, 0, dsave, 0);

                if (task == Task.FG_LN || task == Task.FG_ST) {
                    evaluations++;
                    newF = Function(Solution);
                    newG = Gradient(Solution);
                    f = newF;
                    for (int j = 0; j < newG.Length; j++) {
                        g[j] = newG[j];
                    }
                }
            }
        }

        //Returns whether all objectives are satisfied by the evolution
        public bool IsConverged(double[] solution, Model model) {
            model.ApplyConfiguration(solution);
            for(int i=0; i<model.ObjectivePtrs.Length; i++) {
                Model.Node node = model.ObjectivePtrs[i].Node;
                if(!model.ObjectivePtrs[i].Objective.CheckConvergence(node.WPX, node.WPY, node.WPZ, node.WRX, node.WRY, node.WRZ, node.WRW, node, solution)) {
                    return false;
                }
            }
            return true;
        }
    }
}
