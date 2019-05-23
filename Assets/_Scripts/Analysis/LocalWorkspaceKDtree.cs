using System.Collections.Generic;
using UnityEngine;
using Supercluster.KDTree;

using System;
using System.Linq;
using System.IO;

[RequireComponent(typeof(ParticleSystem))]

public class LocalWorkspaceKDtree : MonoBehaviour {
    [SerializeField]
    string filename = "default.csv";

    [SerializeField]
    float startSize = 0.01f;

    [SerializeField]
    float alphaValue = 0.05f;

    [SerializeField]
    float maxDistance = 0.05f;  //radius of the solution sphere

    [SerializeField]
    Transform goalPosition = null;  //center of the sphere containing the solutions

    [SerializeField]
    Transform referenceOrigin = null; //origin of cordinate frame of the data set

    [SerializeField]
    bool autoUpdateCenterOfSphere = true;   //recalculation on update

    [SerializeField]
    float delayBetweenUpdates = 0.1f;

    System.Diagnostics.Stopwatch stopwatch;
    Vector3 invPos;
    float timePassed;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    Color currentColor;

    Tuple<double[], string>[] radialTestResult;


    Func<double[], double[], double> L3Norm;
    Func<double[], double[], double> L3Rot;
    //Func<ConfigurationPose, Transform, double> L2Norm;

    KDTree<double,string> tree;
    KDTree<double,string> miniTree;

    private List<double[]> solutionList;


    void Start () {
        solutionList = new List<double[]>();

        //define distance function
        L3Norm = (a, b) =>
        {
            double dist = 0;
            dist += (a[0] - b[0]) * (a[0] - b[0]);
            dist += (a[1] - b[1]) * (a[1] - b[1]);
            dist += (a[2] - b[2]) * (a[2] - b[2]);

            return dist;
        };

        L3Rot = (a, b) =>
        {
            double dist = 0;
            dist += (a[3] - b[3]) * (a[3] - b[3]);
            dist += (a[4] - b[4]) * (a[4] - b[4]);
            dist += (a[5] - b[5]) * (a[5] - b[5]);

            return dist;
        };

        stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        ReadSolutionData();
        stopwatch.Stop();
        Debug.Log("Milliseconds for reading the CSV file: " + stopwatch.ElapsedMilliseconds);

        stopwatch.Restart();
        // generate the KD-Tree
        GenerateTree();
        stopwatch.Stop();
        Debug.Log("Milliseconds for generating the KD-Tree: " + stopwatch.ElapsedMilliseconds);

        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[solutionList.Count];

        // transforms the goal position to the coordinate frame of the solution datapoints
        invPos = referenceOrigin.transform.InverseTransformPoint(goalPosition.position);
        // Draws the whole workspace
        DrawParticles();
    }

		
	void Update () {
        // accumulating time since last particle update
        timePassed += Time.deltaTime;

        if (autoUpdateCenterOfSphere && timePassed>delayBetweenUpdates)
        {
            // transforms the goal position to the coordinate frame of the solution datapoints
            invPos = referenceOrigin.transform.InverseTransformPoint(goalPosition.position);

            //// Measure the time to search
            //stopwatch.Restart();
            //// Get the three closest points from the target
            //var nnTest = tree.NearestNeighbors(point: Vec3ToArray(invPos), neighbors: 3);
            //stopwatch.Stop();
            //Debug.Log("Milliseconds spent on searching the nearest neighbor: " + stopwatch.ElapsedMilliseconds + " solutions: " + nnTest.Length);
            ////Debug.Log("pointNN: " + nnTest[0].Item1[0] + "," + nnTest[0].Item1[1] + "," + nnTest[0].Item1[2]);
            ////Debug.Log("pointNNlabel: " + nnTest[0].Item2);

            // Measure the time to search
            //stopwatch.Restart();

            // Get all points with in a distance of 100 from the target.
            // Hint: The center has to match the number of dimensions with the tree itself
            radialTestResult = tree.RadialSearch(center: Vec3ToArray(invPos), radius: maxDistance);
            //stopwatch.Stop();
            //Debug.Log("Milliseconds spent on radial searching: " + stopwatch.ElapsedMilliseconds + " solutions: " + radialTest.Length);


            DrawParticles(radialTestResult);

            //sw.Stop();
            //Debug.Log("Time spent: " + sw.ElapsedMilliseconds);

            /*
            sw.Start();
            radialTest = tree.RadialSearch(center: Vec3ToArray(invPos), radius: maxDistance);
            double[][] treePoints = radialTest.Select(item => item.Item1).ToArray();
            var treeNodes = treePoints.Select(p => p.ToString()).ToArray();
            tree = new KDTree<double, string>(dimensions: 3, points: treePoints, nodes: treeNodes, metric: L3Rot);
            sw.Stop();
            Debug.Log("time elapsed for mini tree: " + sw.ElapsedMilliseconds);
            */
            timePassed = 0.0f;
        }
    }


    private double[] Vec3ToArray(Vector3 vec3)
    {
        return new double[] { vec3.x, vec3.y, vec3.z };
    }


    private void GenerateTree()
    {
        // Spatial points for the KDTree
        var treePoints = solutionList.ToArray() ;

        // Node objects associated with each point
        // These can be type, for this example we will just take the .ToString() of each point
        var treeNodes = treePoints.Select(p => p.ToString()).ToArray();

        // Build the KDTree.
        tree = new KDTree<double, string>(dimensions: 3, points: treePoints, nodes: treeNodes, metric: L3Norm);
    }


    private void GenerateTree(double[][] points)
    {
        var treeNodes = points.Select(p => p.ToString()).ToArray();
        miniTree = new KDTree<double, string>(dimensions: 3, points: points, nodes: treeNodes, metric: L3Norm);
    }


    // reads csv specified in field filename into solutionList
    private void ReadSolutionData()
    {
        foreach (string solution in File.ReadAllLines(filename))
        {
            var tempArray = solution.Split(';');
            solutionList.Add(new double[] {
                double.Parse(tempArray[0]),    //x
                double.Parse(tempArray[1]),    //y
                double.Parse(tempArray[2]),    //z
                double.Parse(tempArray[5]),    //quality
            });
        }
    }

    void DrawParticles(Tuple<double[], string>[] solution)
    {
        int count = 0;
        Vector3 position = new Vector3();
        for (int i = 0; i < solution.Length; i++)
        {
            position.x = (float)solution[i].Item1[0];
            position.y = (float)solution[i].Item1[1];
            position.z = (float)solution[i].Item1[2];
            float col = (float)solution[i].Item1[3];

            particles[count].position = position;
            particles[count].startSize = startSize;
            currentColor = Color.HSVToRGB(col / 3.0f, 1.0f, 1.0f);
            currentColor.a = alphaValue;
            particles[count].startColor = currentColor;
            //    //      Debug.Log (arr [i].quality * 0.3f);
            count++;
        }
        //      Debug.Log (count);
        ps.SetParticles(particles, count + 1);
    }

    void DrawParticles()
    {
        int count = 0;
        Vector3 position = new Vector3();
        for (int i = 0; i < solutionList.Count; i++)
        {
            position.x = (float) solutionList[i][0];
            position.y = (float) solutionList[i][1];
            position.z = (float) solutionList[i][2];
            float col = (float) solutionList[i][3];

            particles[count].position = position;
            particles[count].startSize = startSize;
            currentColor = Color.HSVToRGB(col / 3.0f, 1.0f, 1.0f);
            currentColor.a = alphaValue;
            particles[count].startColor = currentColor;
            //    //      Debug.Log (arr [i].quality * 0.3f);
            count++;
        }
        //      Debug.Log (count);
        ps.SetParticles(particles, count + 1);
    }
}
