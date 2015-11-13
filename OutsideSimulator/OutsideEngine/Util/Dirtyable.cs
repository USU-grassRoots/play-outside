using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutsideEngine.Util
{
    /// <summary>
    /// Encapsulates a computed value and handles caching thereof.
    /// Last valid computed value is used when value is queried, unless object is "dirty"
    /// Object must be marked "dirty" by code that uses the Dirtyable instance.
    /// Value is updated when next requested by calling code.
    /// 
    /// Thread safe, but synchronous.
    /// </summary>
    /// <typeparam name="T">The type of the encapsulated computed value</typeparam>
    public class Dirtyable <T>
    {
        /// <summary>
        /// Fired when the Dirtyable object is dirtied. Does not contain
        ///  the new value of the Dirtyable, as this would violate lazy computation.
        /// Useful for chaining together Dirtayble objects that may depend on each other,
        ///  so long as the graph is well defined and not cyclical. TAKE MUCH CARE DOING THIS.
        /// </summary>
        public event EventHandler OnDirtied;

        /// <summary>
        /// Lock used to prevent multiple silmultaneous locks - object may only be accessed by
        ///  one thread at a time (this includes dirtying as well - object may not be dirtied while
        ///  another thread is accessing the value - dirtying operation must wait for access to finish)
        /// </summary>
        private readonly object _lock;

        /// <summary>
        /// Method used to recompute the value of the Dirtyable object in case it is dirty at the
        ///  time of querying. Must return a value of the encapsulated type.
        /// This cannot be null - the constructor should enforce this.
        /// </summary>
        public Func<T> ComputeValue { get; protected set; }

        /// <summary>
        /// Locally cached value for our Dirtyable
        /// </summary>
        private T _value;

        /// <summary>
        /// True if the object in question is "dirty" and must be re-computed before
        ///  returning the value upon the next query.
        /// </summary>
        public bool Dirty { get; private set; }

        /// <summary>
        /// Publically exposed property for our Dirtyable value. If the object is dirty,
        ///  the updated value is computed using the ComputeValue method, and then returned.
        /// If the object is clean, the cached value is returned.
        /// </summary>
        public T Value
        {
            get
            {
                // Multiple queries cannot happen silmultaneously across threads -
                //  this prevents ComputeValue() from being called redundantly
                lock (_lock)
                {
                    if (Dirty)
                    {
                        _value = ComputeValue();
                        Dirty = false;
                    }
                    return _value;
                }
            }
        }

        /// <summary>
        /// Initialize a Dirtyable object that uses the given computation method to re-compute its
        ///  own value when queired for its value, after being notified about a possible change (optionally).
        /// </summary>
        /// <param name="ComputationMethod">Method used to compute encapsulated value</param>
        public Dirtyable(Func<T> ComputationMethod)
        {
            if (ComputationMethod == null)
            {
                throw new ArgumentNullException("ComputationMethod");
            }

            ComputeValue = ComputationMethod;
            Dirty = true;

            _lock = new object();
        }

        /// <summary>
        /// Notify the Dirtyable object that it is, in fact, dirty. This causes the next query of the
        ///  Value of this dirtyable to invoke the ComputationMethod defined at the creation of the Dirtyable
        /// </summary>
        public void MakeDirty()
        {
            // Must not dirty while value is being queried, and vice versa
            lock (_lock)
            {
                Dirty = true;
            }

            if (OnDirtied != null)
            {
                OnDirtied(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Implicit operator to convert a Dirtyable into its encapsulated value
        /// </summary>
        /// <param name="d">The dirtyable in question</param>
        public static implicit operator T(Dirtyable<T> d)
        {
            return d.Value;
        }
    }
}
