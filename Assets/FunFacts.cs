using UnityEngine;
using TMPro;

public class FunFacts : MonoBehaviour
{

    private string[] funFacts = {
        "The MSP personnel most visible to the public are the uniform troopers of the Field Services " +
        "Bureau whose primary responsibilities include investigating crimes, deterring criminal activity, " +
        "apprehending criminals and fugitives, conducting traffic enforcement to increase traffic safety, " + 
        "and participating in community outreach and prevention services activities.",
        
        "The Michigan State Police divide the state of Mi into 7 districts; the entire Upper Peninsula is the Eighth District.",
        
        "The Michigan State Police (MSP) use a single red \"gumball\" light on their patrol cars primarily for high visibility, instant identification, and tradition.",
        
        "The Training Division's Precision Driving Unit provides regularly scheduled precision driving programs " +
        "to law enforcement personnel from throughout the United States and Canada. These programs include basic " +
        "driving programs, instructor development programs, pursuit schools, and recruit driver training.",

        "The Precision Driving Unit is internationally recognized for the testing of pursuit-rated patrol cars through the Police Vehicle Evaluation and Purchasing Program.",

        "Michigan State Police have used a variety of patrol cars for traffic enforcement including but not limited to: Ford Crown Victoria, Dodge Charger, Dodge Durango, Chevrolet Tahoe.",

        "All individuals interested in becoming a Michigan state police trooper will attend schooling at the Michigan State Police Training Academy in Lansing, MI.",

        "A high school diploma is the minimum education requirement to be a Michigan state trooper.  If interested in becoming a trooper, contact your local state police post.",
        
        "After 10 years of service, troopers are eligible for a competitive salary of $100,500.00. This competitive pay " +
        "structure is part of the Michigan State Police effort to retain experienced law enforcement officers in the state.",
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.SetText(funFacts[Random.Range(0, funFacts.Length)]);
    }

}
