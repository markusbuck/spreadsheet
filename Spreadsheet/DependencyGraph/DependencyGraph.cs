using System.Collections.Generic;
// This class contains the library for the Dependency Graph Structure
// Author: Markus Buckwalter
// Date: September 8, 2023
namespace SpreadsheetUtilities;

/// <summary>
/// (s1,t1) is an ordered pair of strings
/// t1 depends on s1; s1 must be evaluated before t1
/// 
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        (The set of things that depend on s)    
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///        (The set of things that s depends on) 
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}
/// </summary>
public class DependencyGraph
{

    private Dictionary<string, HashSet<string>> dependents;
    private Dictionary<string, HashSet<string>> dependees;
    private int orderedPairs;

    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        dependents = new Dictionary<string, HashSet<string>>();
        dependees = new Dictionary<string, HashSet<string>>();
        orderedPairs = 0;
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// This is an example of a property.
    /// </summary>
    public int NumDependencies
    {
        get {
            return orderedPairs;
        }
    }


    /// <summary>
    /// Returns the size of dependees(s),
    /// that is, the number of things that s depends on.
    /// </summary>
    public int NumDependees(string s)
    {
        if (dependees.TryGetValue(s, out HashSet<string>? dependeeSet))
        {
            return dependeeSet.Count;
        }
        else
            return 0;
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        if (dependents.TryGetValue(s, out HashSet<string>? dependentSet))
            if (dependentSet.Count > 0)
            {
                return true;
            }
        return false;
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        if (dependees.TryGetValue(s, out HashSet<string>? dependeeSet))
            if (dependeeSet.Count > 0)
            {
                return true;
            }
        return false;
    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        if (dependents.ContainsKey(s))
        {
            return dependents[s];
        }
        return new HashSet<string>();
    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        if (dependees.ContainsKey(s))
        {
            return dependees[s];
        }
        return new HashSet<string>();
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>
    public void AddDependency(string s, string t)
    {
        if (dependents.ContainsKey(s))
        {
            if (dependents[s].Add(t))
            {
                orderedPairs++;
            }
        }
        else
        {
            HashSet<string> dependentSet = new HashSet<string>();
            dependentSet.Add(t);
            dependents.Add(s,dependentSet);
            orderedPairs++;
        }

        if (dependees.ContainsKey(t))
        {
            dependees[t].Add(s);
        }
        else
        {
            HashSet<string> dependeeSet = new HashSet<string>();
            dependeeSet.Add(s);
            dependees.Add(t, dependeeSet);
        }
    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
        if (dependents.ContainsKey(s))
        {
            if (dependents[s].Remove(t))
            {
                orderedPairs--;
            }
            if (dependents[s].Count == 0)
            {
                dependents.Remove(s);
            }
        }
        if (dependees.ContainsKey(t))
        {
            dependees[t].Remove(s);

            if (dependees[t].Count == 0)
            {
                dependees.Remove(t);
            }
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
        if (dependents.ContainsKey(s))
        {
            foreach (string r in dependents[s])
            {
                RemoveDependency(s, r);
            }
        }

        foreach(string t in newDependents)
        {
            AddDependency(s, t);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
        if (dependees.ContainsKey(s))
        {
            foreach (string r in dependees[s])
            {
                RemoveDependency(r, s);
            }
        }
        foreach (string t in newDependees)
        {
            AddDependency(t, s);
        }
    }
}