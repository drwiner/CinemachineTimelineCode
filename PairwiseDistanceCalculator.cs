using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PairwiseDistanceCalculator : MonoBehaviour {

    public GameObject LocationHost;
    public List<Transform> locations;
    //private Dictionary<string, float> distances;
    //private Dictionary<string, Dictionary<string, float>> distance_dict;
    private Transform this_location;
    private Transform other_location;
    private string source_name;
    private StreamWriter writer;

    // Use this for initialization
    void Start()
    {
        LocationHost = GameObject.FindGameObjectWithTag("LocationHost");
        writer = new StreamWriter("Assets/Resources/distances.txt", true);

        for (int i = 0; i < LocationHost.transform.childCount; i++)
        {
            this_location = LocationHost.transform.GetChild(i);
            locations.Add(this_location);
            source_name = this_location.gameObject.name;
            //distances = new Dictionary<string, float>();
            for (int j = 0; j < LocationHost.transform.childCount; j++)
            {
                if (i == j)
                {
                    continue;
                }
                other_location = LocationHost.transform.GetChild(j);
                float dist = getDistance(this_location, other_location);
                //distances.Add(other_location.gameObject.name, dist);
                writer.Write(source_name + " to " + other_location.gameObject.name + " = " + string.Format("{0:N2}", dist) + "\n");
            }
            //distance_dict.Add(source_name, distances);

        }
        writer.Close();
    }
    
    public static float getDistance(Transform a, Transform b)
    {
        return Vector3.Distance(a.position, b.position);
    }
	
	
}
