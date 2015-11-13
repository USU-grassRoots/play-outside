using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Diagnostics;

using OutsideEngine.Util;

namespace OutsideEngineTest.Util
{
    /// <summary>
    /// Unit tests for the OutsideEngine.Util.Dirtyable class
    /// </summary>
    [TestClass]
    public class DirtyableTest
    {
        /// <summary>
        /// Make sure regular construction of a Dirtyable object properly initializes all (publicly exposed) values
        /// </summary>
        [TestMethod]
        public void RegularConstruction()
        {
            Dirtyable<int> BoringIntDirtyable = new Dirtyable<int>(() => { return 2; });
            Assert.IsNotNull(BoringIntDirtyable, "Dirtyable<int> constructor should return a non-null reference to a new Dirtyable<int>");
            Assert.IsInstanceOfType(BoringIntDirtyable, typeof(Dirtyable<int>), "Value assigned from Dirtyable<int> constructor should be an instance of type Dirtyable<int>");
            Assert.IsTrue(BoringIntDirtyable.Dirty, "Upon construction, Dirtyable<int> should be dirty");
            Assert.IsNotNull(BoringIntDirtyable.ComputeValue, "Method which computes the value for a Dirtyable<int> should be set");
        }

        /// <summary>
        /// Make sure that improper construction of a Dirtyable causes and ArgumentNull exception to be thrown
        /// </summary>
        [TestMethod]
        public void BadConstruction()
        {
            try
            {
                Dirtyable<int> BadIntDirtyable = new Dirtyable<int>(null);
                Assert.Fail("Dirtyable<int> should throw exception when created with null for its computation function");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException), "Exception thrown on bad creation of Dirtyable<int> should be of type ArgumentNullException");
            }
        }

        /// <summary>
        /// Make sure that when an object is dirtied, the Value and Dirty properties are set to the expected values
        /// </summary>
        [TestMethod]
        public void Dirtying()
        {
            int k = 0;
            Dirtyable<int> BoringIntDirtyable = new Dirtyable<int>(() => { return ++k; });

            Assert.IsTrue(BoringIntDirtyable.Dirty, "Dirtyable should be dirty immediately after construction");
            Assert.AreEqual(1, BoringIntDirtyable.Value, "Dirtyable should compute value upon invoking Value property");
            Assert.IsFalse(BoringIntDirtyable.Dirty, "Dirtyable should be clean immediately after invoking Value property");

            BoringIntDirtyable.MakeDirty();
            Assert.IsTrue(BoringIntDirtyable.Dirty, "Dirtyable should be dirty immediately after doing the nasty");
            Assert.AreEqual(2, BoringIntDirtyable.Value, "Dirtyable should re-compute value upon invoking Value property after being dirtied");
            Assert.IsFalse(BoringIntDirtyable.Dirty, "Dirtyable should once again be clean after invoking Value property");
        }

        /// <summary>
        /// Test firing of events and chaining Dirtiables together
        /// </summary>
        [TestMethod]
        public void EventFiring()
        {
            int k = 5;
            bool FirstDependentChanged = false;
            bool SecondDependentChanged = false;
            Dirtyable<int> FirstDependentValue = new Dirtyable<int>(() => { return k * 2; });
            Dirtyable<int> SecondDependentValue = new Dirtyable<int>(() => { return FirstDependentValue.Value * 5; });

            FirstDependentValue.OnDirtied += (o, e) => { FirstDependentChanged = true; };
            SecondDependentValue.OnDirtied += (o, e) => { SecondDependentChanged = true; };

            FirstDependentValue.OnDirtied += (o, e) => { SecondDependentValue.MakeDirty(); };

            Assert.IsFalse(FirstDependentChanged, "OnUpdate event should not be fired on construction of an independent Dirtyable");
            Assert.IsFalse(SecondDependentChanged, "OnUpdate event should not be fired on construction of a dependent Dirtyable");

            Assert.AreEqual(5, k, "Integer value should equal what we told it to equal");
            Assert.AreEqual(10, FirstDependentValue.Value, "Independent Dirtyable should be equal to value expected on first invocation");
            Assert.AreEqual(50, SecondDependentValue.Value, "Dependent Dirtyable should be equal to value expected on first invocation");

            Assert.IsFalse(FirstDependentChanged, "Computation of value should not fire DependantChanged event on independent Dirtyable");
            Assert.IsFalse(SecondDependentChanged, "Computation of value should not fire DependantChanged event on dependant Dirtyable");

            k = 10;
            FirstDependentValue.MakeDirty();

            Assert.IsTrue(FirstDependentChanged, "Dependent Dirtyable should fire event on doing the nasty");
            Assert.IsTrue(SecondDependentChanged, "Independent Dirtyable should fire event when triggered from primary Dirtyable");

            Assert.AreEqual(10, k, "Integer value should equal what we told it to equal when re-assigning");
            Assert.AreEqual(20, FirstDependentValue.Value, "Independent Dirtyable should be equal to value expected on later invocation");
            Assert.AreEqual(100, SecondDependentValue.Value, "Dependent Dirtyable should be equal to value expected on later invocation");
        }

        /// <summary>
        /// Ensure thread safety - make sure that requests for Value cannot happen silmultaneously
        /// </summary>
        [TestMethod]
        public void ConcurrentThreadedValueRequest()
        {
            // Perform this test multiple times, to try and catch a bad case
            for (int i = 0; i < 10; i++)
            {
                // On dirtying, takes a full second to recompute value, which is just 2.
                int askCount = 0;
                Dirtyable<int> expensiveDirtyable = null;
                expensiveDirtyable = new Dirtyable<int>(() =>
                {
                    Thread.Sleep(1000);
                    ++askCount;
                    return 2;
                });

                // Ask for Value twice, concurrently
                Stopwatch sw = new Stopwatch();
                Thread t1 = new Thread(() => {
                    Assert.AreEqual(2, expensiveDirtyable.Value, "The expensive dirtying operation still should just return 2... (from thread 1)");
                });
                Thread t2 = new Thread(() => {
                    Assert.AreEqual(2, expensiveDirtyable.Value, "The expensive dirtying operation still should just return 2... (from thread 2)");
                    Assert.AreEqual(1, askCount, "The dirtyable should lock the ComputeValue method, and only invoke it once in case of interleave");
                    sw.Stop();
                    Assert.IsTrue(sw.ElapsedMilliseconds >= 1000, "The dirtying method for this test should take more than 1000 ms");
                });

                // This won't be precise, but since the time frame is 1000 ms, it doesn't have to be.
                sw.Start();
                t1.Start();
                Thread.Sleep(5);
                t2.Start();

                t1.Join();
                t2.Join();

                askCount = 0;
            }
        }

        /// <summary>
        /// Ensure thread safety - make sure that a request to MakeDirty only finishes after the value is computed
        /// </summary>
        [TestMethod]
        public void DirtyWhileValueComputing()
        {
            // Perform this test multiple times, to try and catch a bad case
            for (int i = 0; i < 10; i++)
            {
                // On dirtying, takes a full second to recompute value, which is just 2.
                int askCount = 0;
                Dirtyable<int> expensiveDirtyable = null;
                expensiveDirtyable = new Dirtyable<int>(() =>
                {
                    Thread.Sleep(1000);
                    ++askCount;
                    return 2;
                });

                // Test 1: Ask for Value twice, concurrently
                Stopwatch sw = new Stopwatch();
                Thread t1 = new Thread(() => {
                    Assert.AreEqual(2, expensiveDirtyable.Value, "The expensive dirtying operation still should just return 2... (from thread 1)");
                });
                Thread t2 = new Thread(() => {
                    expensiveDirtyable.MakeDirty();
                    sw.Stop();
                    Assert.IsTrue(sw.ElapsedMilliseconds >= 1000, "The dirtying method for this test should take more than 1000 ms");
                });

                // This won't be precise, but since the time frame is 1000 ms, it doesn't have to be.
                sw.Start();
                t1.Start();
                Thread.Sleep(5);
                t2.Start();

                t1.Join();
                t2.Join();

                askCount = 0;
            }
        }
    }
}
