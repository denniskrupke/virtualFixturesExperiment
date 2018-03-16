using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace BioIK {
	//----------------------------------------------------------------------------------------------------
	//====================================================================================================
	//Memetic Evolutionary Optimisation
	//====================================================================================================
	//----------------------------------------------------------------------------------------------------
	public class Evolution {		
		private Model Model;									//Reference to the kinematic model
		private Motion[] Motions;								//Reference to all joint motions
		private int Size;										//Number of individuals (population size)
		private int Elites;										//Number of elite individuals
		private int Dimensionality;								//Search space dimensionality

		private double[] Solution;								//Global evolutionary solution
		private double Fitness;									//Global evolutionary fitness

		private Individual[] Population;						//Array for current individuals
		private Individual[] Offspring;							//Array for offspring individuals

		private List<Individual> Pool = new List<Individual>();	//Selection pool for recombination
		private int PoolCount;									//Current size of the selection pool
		private double[] Probabilities;							//Current probabilities for selection
		private double Storage;									//Simple storage variable
		private double[] LowerBounds;							//Constraints for the lower bounds
		private double[] UpperBounds;							//Constraints for the upper bounds
		private bool[] UseBounds;								//Whether bounds can be exceeded or not
		private double[] Spans;

		private const double PI = 3.14159265358979;

		private BFGS[] EliteOptimisers;
		private bool Running;
		private readonly object Mutex = new object();
		private int Jobs;
		private bool[] Work;
		private Thread[] EliteThreads;
		private Model[] EliteModels;
		private bool[] EliteImproved;
		
		private int RandomCount;
		private int RandomIndex;
		private double[] RandomValues;

		private bool Evolving;

		//Initialises the algorithm
		public Evolution(Model model, int size, int elites, double[] seed) {
			Model = model;
			Size = size;
			Elites = elites;
			Dimensionality = Model.MotionPtrs.Length;
			Motions = new Motion[Dimensionality];
			for(int i=0; i<Dimensionality; i++) {
				Motions[i] = model.MotionPtrs[i].Motion;
			}

			Storage = 0.0;
			LowerBounds = new double[Dimensionality];
			UpperBounds = new double[Dimensionality];
			UseBounds = new bool[Dimensionality];
			Spans = new double[Dimensionality];
			UpdateBounds();

			Population = new Individual[Size];
			Offspring = new Individual[Size];

			for(int i=0; i<Size; i++) {
				Population[i] = new Individual(Dimensionality);
				Offspring[i] = new Individual(Dimensionality);
			}

			Solution = seed;
			Probabilities = new double[Size];

			EliteThreads = new Thread[Elites];
			Running = false;
			Jobs = 0;
			Work = new bool[Elites];
			EliteOptimisers = new BFGS[Elites];
			EliteModels = new Model[Elites];
			EliteImproved = new bool[Elites];
			for(int i=0; i<Elites; i++) {
				int index = i;
				EliteModels[index] = new Model(Model.GetRoot());
				EliteOptimisers[index] = new BFGS(Dimensionality, x => ComputeFitness(x, EliteModels[index]), y => ComputeGradient(y, EliteModels[index]));
			}

			RandomCount = 10000;
			RandomIndex = 0;
			RandomValues = new double[RandomCount];
			for(int i=0; i<RandomCount; i++) {
				RandomValues[i] = Random.value;
			}

			Evolving = false;

			Initialise();
		}

		private double GetRandom() {
			double value = RandomValues[RandomIndex];
			RandomIndex += 1;
			if(RandomIndex == RandomCount) {
				RandomIndex = 0;
			}
			return value;
		}

		public void StartThreads() {
			if(Running) {
				return;
			}
			Running = true;
			for(int i=0; i<Elites; i++) {
				int index = i;
				EliteThreads[index] = new Thread(x => Survive(index));
				EliteThreads[index].Start();
			}
			for(int i=0; i<Elites; i++) {
				Work[i] = false;
			}
			Jobs = 0;
		}

		public void StopThreads() {
			if(!Running) {
				return;
			}
			Running = false;
			for(int i=0; i<Elites; i++) {
				if(EliteThreads[i] != null) {
					EliteThreads[i].Join();
				}
			}
			for(int i=0; i<Elites; i++) {
				Work[i] = false;
			}
			Jobs = 0;
		}

		//Initialises a new population and integrates the evolutionary solution (seed)
		private bool Initialise() {
			for(int i=0; i<Dimensionality; i++) {
				Population[0].Genes[i] = Solution[i];
				Population[0].Gradient[i] = 0.0;
			}
			Population[0].Fitness = ComputeFitness(Population[0].Genes, Model);

			for(int i=1; i<Size; i++) {
				for(int j=0; j<Dimensionality; j++) {
					Population[i].Genes[j] = (float)Random.Range((float)LowerBounds[j], (float)UpperBounds[j]);
					Population[i].Gradient[j] = (float)Random.Range((float)LowerBounds[j], (float)UpperBounds[j]);
				}
				Population[i].Fitness = ComputeFitness(Population[i].Genes, Model);
			}

			SortByFitness();
			ComputeExtinctions();

			return TryUpdateSolution();
		}

		public void Prepare() {
			UpdateBounds();
			for(int i=0; i<Elites; i++) {
				//EliteModels[i].UpdateConfiguration();
				EliteModels[i].CopyFrom(Model);
			}
		}

		//Returns whether the solution could be improved
		public bool Evolve() {
			//Create mating pool
			Pool.Clear();
			Pool.AddRange(Population);
			PoolCount = Size;

			//Start evolutionary cycle
			Jobs = Elites;
			if(!Running) {
				StartThreads();
			}

			//Elitism exploitation
			for(int i=0; i<Elites; i++) {
				Work[i] = true;
			}

			//Evolve offspring
			Evolving = true;
			for(int i=Elites; i<Size; i++) {
				if(PoolCount > 0) {
					Individual parentA = Select(Pool);
					Individual parentB = Select(Pool);
					Individual prototype = Select(Pool);

					//Recombination, Mutation, Adoption
					Reproduce(i, parentA, parentB, prototype);

					//Pre-Selection Niching
					if(Offspring[i].Fitness < parentA.Fitness) {
						Pool.Remove(parentA);
						PoolCount -= 1;
					}
					if(Offspring[i].Fitness < parentB.Fitness) {
						Pool.Remove(parentB);
						PoolCount -= 1;
					}
				} else {
					//Fill the population
					Reroll(i);
				}
			}
			Evolving = false;

			//Wait for threads to finish
			while(Jobs > 0) {
				
			}

			//Reroll elite if exploitation was not successful
			for(int i=0; i<Elites; i++) {
				if(!EliteImproved[i]) {
					Reroll(i);
				}
			}

			//Swap population and offspring
			Swap(ref Population, ref Offspring);

			//Finalise
			SortByFitness();

			//Check improvement and wipeout criterion
			if(!TryUpdateSolution() && !HasAnyEliteImproved()) {
				return Initialise();
			} else {
				ComputeExtinctions();
				return true;
			}
		}

		//Returns whether all objectives are satisfied by the evolution
		public bool IsConverged() {
			Model.ApplyConfiguration(Solution);
			for(int i=0; i<Model.ObjectivePtrs.Length; i++) {
				Model.Node node = Model.ObjectivePtrs[i].Node;
				if(!Model.ObjectivePtrs[i].Objective.CheckConvergence(node.WPX, node.WPY, node.WPZ, node.WRX, node.WRY, node.WRZ, node.WRW, node, Solution)) {
					return false;
				}
			}
			return true;
		}

		private bool HasAnyEliteImproved() {
			for(int i=0; i<Elites; i++) {
				if(EliteImproved[i]){
					return true;
				}
			}
			return false;
		}

		//Tries to improve the evolutionary solution by the population, and returns whether it was successful
		private bool TryUpdateSolution() {
			Fitness = ComputeFitness(Solution, Model);
			double candidateFitness = Population[0].Fitness;
			if(candidateFitness < Fitness) {
				for(int i=0; i<Dimensionality; i++) {
					Solution[i] = Population[0].Genes[i];
				}
				Fitness = candidateFitness;
				return true;
			} else {
				return false;
			}
		}

		//Lets elite individuals survive and performs a memetic exploitation
		private void Survive(int index) {
			while(Running) {
				if(Work[index]) {
					for(int i=0; i<Dimensionality; i++) {
						Offspring[index].Genes[i] = Population[index].Genes[i];
						Offspring[index].Gradient[i] = Population[index].Gradient[i];
					}

					EliteImproved[index] = Exploit(Offspring[index], index);

					Work[index] = false;
					lock(Mutex) {
						Jobs -= 1;
					}
				}
			}
		}

		//Evolves a new individual
		private void Reproduce(int index, Individual parentA, Individual parentB, Individual prototype) {
			Individual offspring = Offspring[index];
			double weight;
			double mutationProbability = GetMutationProbability(parentA, parentB);
			double mutationStrength = GetMutationStrength(parentA, parentB);

			for(int i=0; i<Dimensionality; i++) {
				//Recombination
				weight = GetRandom();
				double gradient = GetRandom()*parentA.Gradient[i] + GetRandom()*parentB.Gradient[i];
				offspring.Genes[i] = weight*parentA.Genes[i] + (1.0-weight)*parentB.Genes[i] + gradient;

				//Store
				Storage = offspring.Genes[i];

				//Mutation
				if(GetRandom() < mutationProbability) {
					offspring.Genes[i] += (GetRandom() * Spans[i] + LowerBounds[i]) * mutationStrength;
				}

				//Adoption
				weight = GetRandom();
				offspring.Genes[i] += 
					weight * GetRandom() * (0.5 * (parentA.Genes[i] + parentB.Genes[i]) - offspring.Genes[i])
					+ (1.0-weight) * GetRandom() * (prototype.Genes[i] - offspring.Genes[i]);

				//Clip
				offspring.Genes[i] = Project(offspring.Genes[i], LowerBounds[i], UpperBounds[i], UseBounds[i]);

				//Evolutionary Gradient
				offspring.Gradient[i] = GetRandom()*gradient + (offspring.Genes[i] - Storage);
			}

			//Fitness
			offspring.Fitness = ComputeFitness(offspring.Genes, Model);
		}

		//Generates a random individual
		private void Reroll(int index) {
			Individual offspring = Offspring[index];
			for(int i=0; i<Dimensionality; i++) {
				offspring.Genes[i] = GetRandom() * Spans[i] + LowerBounds[i];
				offspring.Gradient[i] = 0.0;
			}
			offspring.Fitness = ComputeFitness(offspring.Genes, Model);
		}

		//Rank-based selection of an individual
		private Individual Select(List<Individual> pool) {
			double rankSum = PoolCount*(PoolCount+1) / 2.0;
			for(int i=0; i<PoolCount; i++) {
				Probabilities[i] = (PoolCount-i)/rankSum;
			}
			return pool[GetRandomWeightedIndex(Probabilities, PoolCount)];
		}
		
		//Returns a random index with respect to the probability weights
		private int GetRandomWeightedIndex(double[] probabilities, int count) {
			double weightSum = 0.0;
			for(int i=0; i<count; i++) {
				weightSum += probabilities[i];
			}
			double rVal = GetRandom()*weightSum;
			for(int i=0; i<count; i++) {
				rVal -= probabilities[i];
				if(rVal <= 0.0) {
					return i;
				}
			}
			return count-1;
		}

		//Returns the mutation probability from two parents
		private double GetMutationProbability(Individual parentA, Individual parentB) {
			double extinction = 0.5 * (parentA.Extinction + parentB.Extinction);
			double inverse = 1.0/(double)Dimensionality;
			return extinction * (1.0-inverse) + inverse;
		}

		//Returns the mutation strength from two parents
		private double GetMutationStrength(Individual parentA, Individual parentB) {
			return 0.5 * (parentA.Extinction + parentB.Extinction);
		}

		//Computes the extinction factors for all individuals
		private void ComputeExtinctions() {
			double min = Population[0].Fitness;
			double max = Population[Size-1].Fitness;
			for(int i=0; i<Size; i++) {
				double grading = (double)i/((double)Size-1);
				Population[i].Extinction = (Population[i].Fitness + min*(grading-1.0)) / max;
			}
		}

		//Performs the memetic exploitation
		private bool Exploit(Individual individual, int index) {
			double fitness = ComputeFitness(individual.Genes, EliteModels[index]);
			for(int i=0; i<Dimensionality; i++) {
				EliteOptimisers[index].LowerBounds[i] = LowerBounds[i];
				EliteOptimisers[index].UpperBounds[i] = UpperBounds[i];
			}
			EliteOptimisers[index].Minimize(individual.Genes, ref Evolving);
			if(EliteOptimisers[index].Value < fitness) {
				for(int i=0; i<Dimensionality; i++) {
					individual.Gradient[i] = EliteOptimisers[index].Solution[i] - individual.Genes[i];
					individual.Genes[i] = EliteOptimisers[index].Solution[i];
				}
				individual.Fitness = EliteOptimisers[index].Value;
				return true;
			} else {
				individual.Fitness = fitness;
				return false;
			}

			/*
			double startFitness = ComputeFitness(individual.Genes, EliteModels[index]);
			double newFitness = startFitness;
			for(int k=0; k<100; k++) {
				double[] genes = (double[])individual.Genes.Clone();
				double fitness = ComputeFitness(genes, EliteModels[index]);
				double[] gradient = ComputeGradient(genes, EliteModels[index]);
				// normalize gradient
				double dp = 0.00001;
				double sum = dp*dp;
				for(int i=0; i<Dimensionality; i++) {
					sum += System.Math.Abs(gradient[i]);
				}
				double f = 1.0 / sum * dp;
				for(int i=0; i<Dimensionality; i++) {
					gradient[i] *= f;
				}

				double[] before = new double[Dimensionality];
				double[] after = new double[Dimensionality];
				for(int i=0; i<Dimensionality; i++) {
					before[i] = genes[i] - gradient[i];
					after[i] = genes[i] + gradient[i];
				}

				double f1 = ComputeFitness(before, EliteModels[index]);
				double f2 = fitness;
				double f3 = ComputeFitness(after, EliteModels[index]);

				double v1 = (f2 - f1); // f / j
				double v2 = (f3 - f2); // f / j
				double v = (v1 + v2) * 0.5; // f / j
				double a = (v1 - v2); // f / j^2
				double step_size = v / a; // (f / j) / (f / j^2) = f / j / f * j * j = j

				for(int i=0; i<Dimensionality; i++) {
					genes[i] = Project(genes[i] + gradient[i] * step_size, LowerBounds[i], UpperBounds[i], UseBounds[i]);
				}

				newFitness = ComputeFitness(genes, EliteModels[index]);
				
				if(newFitness < f2) {
					for(int i=0; i<Dimensionality; i++) {
						individual.Genes[i] = genes[i];
						individual.Fitness = newFitness;
					}
				}
			}
			return newFitness < startFitness;
			*/
		}

		//Updates the lower and upper search space bounds
		private void UpdateBounds() {
			for(int i=0; i<Dimensionality; i++) {
				LowerBounds[i] = Motions[i].GetLowerLimit();
				UpperBounds[i] = Motions[i].GetUpperLimit();
				UseBounds[i] = Motions[i].Joint.GetJointType() == JointType.Continuous;
				Spans[i] = UpperBounds[i] - LowerBounds[i];
			}
		}

		//Projects a single gene to stay within search space
		private double Project(double gene, double min, double max, bool ignoreBounds) {
			if(max - min == 0.0) {
				return min;
			}
			if(ignoreBounds) {
				//Overflow
				while(gene < -PI || gene > PI) {
					if(gene > PI) {
						gene -= PI + PI;
					}
					if(gene < -PI) {
						gene += PI + PI;
					}
				}
			} else {
				//Bounce
				while(gene < min || gene > max) {
					if(gene > max) {
						gene = max + max - gene;
					}
					if(gene < min) {
						gene = min + min - gene;
					}
				}
			}
			return gene;
		}

		//Evaluates the fitness of an individual after updating the kinematic tree
		private double ComputeFitness(double[] genes, Model model) {
			model.ApplyConfiguration(genes);
			double fitness = 0.0;
			for(int i=0; i<model.ObjectivePtrs.Length; i++) {
				fitness += model.Losses[i];
			}
			return System.Math.Sqrt(fitness / (double)model.ObjectivePtrs.Length);
		}

		//Evaluates the fitness of an individual after simulating modification in the kinematic tree
		private double ComputeFitness(double[] genes, Model.MotionPtr modifiedMotion) {
			modifiedMotion.Node.SimulateModification(genes);
			double fitness = 0.0;
			for(int i=0; i<Model.ObjectivePtrs.Length; i++) {
				fitness += Model.ModificationLosses[i];
			}
			return System.Math.Sqrt(fitness / (double)Model.ObjectivePtrs.Length);
		}

		//Approximates the gradient of a position on the fitness function
		private double[] ComputeGradient(double[] values, Model model) {
			double[] genes = (double[])values.Clone();
			double[] gradient = new double[genes.Length];
			double oldFitness = ComputeFitness(genes, model);
			double step = 0.00001;
			for(int j=0; j<Dimensionality; j++) {
				//Approximate Derivative
				genes[j] += step;
				model.MotionPtrs[j].Node.SimulateModification(genes);
				genes[j] -= step;

				double newFitness = 0.0;
				for(int i=0; i<model.ObjectivePtrs.Length; i++) {
					newFitness += model.ModificationLosses[i];
				}
				newFitness = System.Math.Sqrt(newFitness / (double)model.ObjectivePtrs.Length);
				gradient[j] = (newFitness - oldFitness) / step;
			}
			return gradient;
		}

		/*
		private double ComputeDisplacement(double[] configuration)  {
			double weight = 0.99;
			double loss = 0.0;
			for(int i=0; i<Dimensionality; i++) {
				double diff = System.Math.Abs(Solution[i] - configuration[i]) / (GetModel().MotionPtrs[i].Motion.GetUpperLimit() - GetModel().MotionPtrs[i].Motion.GetLowerLimit());
				loss += diff;
			}
			loss /= (double)Dimensionality;
			loss = (1.0-weight) * System.Math.Exp(weight*loss);
			return loss;
		}
		*/

		//Sorts the population by their fitness values (descending)
		private void SortByFitness() {
			System.Array.Sort(Population,
				delegate(Individual a, Individual b) {
					return a.Fitness.CompareTo(b.Fitness);
				}
			);
		}

		//In-place swap by references
		private void Swap(ref Individual[] a, ref Individual[] b) {
			Individual[] tmp = a;
			a = b;
			b = tmp;
		}

		public Model GetModel() {
			return Model;
		}

		public Individual[] GetPopulation() {
			return Population;
		}

		public int GetDimensionality() {
			return Dimensionality;
		}

		public int GetSize() {
			return Size;
		}

		public int GetElites() {
			return Elites;
		}

		public double[] GetSolution() {
			return Solution;
		}

		public double GetFitness() {
			return Fitness;
		}

		public double[,] GetGeneLandscape() {
			double[,] values = new double[Size,Dimensionality];
			for(int i=0; i<Size; i++) {
				for(int j=0; j<Dimensionality; j++) {
					values[i,j] = Population[i].Genes[j];
				}
			}
			return values;
		}

		public double[] GetFitnessLandscape() {
			double[] values = new double[Size];
			for(int i=0; i<Size; i++) {
				values[i] = Population[i].Fitness;
			}
			return values;
		}

		public double[] GetExtinctionLandscape() {
			double[] values = new double[Size];
			for(int i=0; i<Size; i++) {
				values[i] = Population[i].Extinction;
			}
			return values;
		}

		//Data class for the individuals
		public class Individual {
			public double[] Genes;
			public double[] Gradient;
			public double Extinction;
			public double Fitness;

			public Individual(int dimensionality) {
				Genes = new double[dimensionality];
				Gradient = new double[dimensionality];
				Extinction = 0f;
				Fitness = 0f;
			}
		}
	}
}