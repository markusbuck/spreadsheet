using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
// Test file for the DependencyGraph
// Author: Markus Buckwalter
// Date: September 8, 2023
namespace DevelopmentTests;

/// <summary>
///This is a test class for DependencyGraphTest and is intended
///to contain all DependencyGraphTest Unit Tests (once completed by the student)
///</summary>
[TestClass()]
public class DependencyGraphTest
{

    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyTest()
    {
        DependencyGraph t = new DependencyGraph();
        Assert.AreEqual(0, t.NumDependencies);
    }

    /// <summary>
    /// Tests the add dependency method
    /// </summary>
    [TestMethod()]
    public void SimpleAddTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
    }

    /// <summary>
    /// Tests the add dependency with a duplicate 
    /// </summary>
    [TestMethod()]
    public void DuplicateAddTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
    }

    /// <summary>
    /// Tests the add depency with two of the same values just swapped places
    /// </summary>
    [TestMethod()]
    public void AddTestSwapped()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
        t.AddDependency("y", "x");
        Assert.AreEqual(2, t.NumDependencies);
    }

    /// <summary>
    /// Tests the add dependency method with the same starting node and different dependcenies
    /// </summary>
    [TestMethod()]
    public void AddTestSamefirstvalue()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
        t.AddDependency("x", "a");
        Assert.AreEqual(2, t.NumDependencies);
        t.AddDependency("x", "b");
        Assert.AreEqual(3, t.NumDependencies);
        t.AddDependency("x", "c");
        Assert.AreEqual(4, t.NumDependencies);
        t.AddDependency("x", "h");
        Assert.AreEqual(5, t.NumDependencies);
    }

    /// <summary>
    /// Tests the remove method when the two ordered pairs have the same dependee
    /// </summary>
    [TestMethod()]
    public void TestRemoveWithSameFirstValue()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        t.AddDependency("x", "z");

        Assert.AreEqual(2, t.NumDependencies);

        t.RemoveDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
    }

    /// <summary>
    /// Tests Remove Dependency on pair that has not been added. 
    /// </summary>
    [TestMethod()]
    public void TestRemoveWithInvalidPair()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        t.AddDependency("x", "z");

        Assert.AreEqual(2, t.NumDependencies);

        t.RemoveDependency("v", "y");
        Assert.AreEqual(2, t.NumDependencies);
    }

    /// <summary>
    /// Tests the remove method when the dependent is not in the set
    /// </summary>
    [TestMethod()]
    public void TestRemoveWithInvalidDependent()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        t.AddDependency("x", "z");

        Assert.AreEqual(2, t.NumDependencies);

        t.RemoveDependency("x", "a");
        Assert.AreEqual(2, t.NumDependencies);
    }



    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyRemoveTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.NumDependencies);
        t.RemoveDependency("x", "y");
        Assert.AreEqual(0, t.NumDependencies);
    }


    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void EmptyEnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
        Assert.IsTrue(e1.MoveNext());
        Assert.AreEqual("x", e1.Current);
        IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
        Assert.IsTrue(e2.MoveNext());
        Assert.AreEqual("y", e2.Current);
        t.RemoveDependency("x", "y");
        Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
        Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
    }


    /// <summary>
    ///Replace on an empty DG shouldn't fail
    ///</summary>
    [TestMethod()]
    public void SimpleReplaceTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(t.NumDependencies, 1);
        t.RemoveDependency("x", "y");
        Assert.AreEqual(t.NumDependencies, 0);
        t.ReplaceDependents("x", new HashSet<string>());
        t.ReplaceDependees("y", new HashSet<string>());
    }



    ///<summary>
    ///It should be possibe to have more than one DG at a time.
    ///</summary>
    [TestMethod()]
    public void StaticTest()
    {
        DependencyGraph t1 = new DependencyGraph();
        DependencyGraph t2 = new DependencyGraph();
        t1.AddDependency("x", "y");
        Assert.AreEqual(1, t1.NumDependencies);
        Assert.AreEqual(0, t2.NumDependencies);
    }




    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void SizeTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        Assert.AreEqual(4, t.NumDependencies);
    }


    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void EnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        // This is one of several ways of testing whether your IEnumerable
        // contains the right values. This does not require any particular
        // ordering of the elements returned.
        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }


    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void ReplaceThenEnumerate()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", new HashSet<string>());
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", new HashSet<string>() { "c" });
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
        t.ReplaceDependees("d", new HashSet<string>() { "b" });

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }



    /// <summary>
    ///Using lots of data
    ///</summary>
    [TestMethod()]
    public void StressTest()
    {
        // Dependency graph
        DependencyGraph t = new DependencyGraph();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = ("" + (char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = new HashSet<string>();
            dees[i] = new HashSet<string>();
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// Test the num dependees method works on a DG with valid values
    /// </summary>
    [TestMethod()]
    public void TestNumDependees()
    {
        DependencyGraph t = new DependencyGraph();

        t.AddDependency("m", "n");
        Assert.AreEqual(t.NumDependees("n"), 1);
    }

    /// <summary>
    /// Tests num dependees when called on a string that is not contained in the
    /// dependee set
    /// </summary>
    [TestMethod()]
    public void TestNumDependeesNoDependee()
    {
        DependencyGraph t = new DependencyGraph();

        t.AddDependency("m", "n");
        Assert.AreEqual(t.NumDependees("m"), 0);
    }

    // Test the has dependents method if the count of dependents is greater than 0
    [TestMethod()]
    public void TestHasDependents()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsTrue(t.HasDependents("m"));
    }

    /// <summary>
    /// Test the HasDependents method if the count of the dependents is zero
    /// </summary>
    [TestMethod()]
    public void TestHasDependentsLessThan1()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsFalse(t.HasDependents("n"));
    }

    /// <summary>
    /// Test the HasDependents method on string that hasn't been added to DG
    /// </summary>
    [TestMethod()]
    public void TestHasDependentsNoDependents()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsFalse(t.HasDependents("z"));
    }

    /// <summary>
    /// Test the has dependents method if the count of dependents is greater than 0
    /// </summary>
    [TestMethod()]
    public void TestHasDependeees()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsTrue(t.HasDependees("n"));
    }

    /// <summary>
    /// Test the HasDependees method if the count of the dependents is zero
    /// </summary>
    [TestMethod()]
    public void TestHasDependeesLessThan1()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsFalse(t.HasDependees("m"));
    }
    /// <summary>
    /// Test the HasDependents method on string that hasn't been added to DG
    /// </summary>
    [TestMethod()]
    public void TestHasDependentsNoDependeees()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("m", "n");

        Assert.IsFalse(t.HasDependees("z"));
    }

}