using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutsideEngine
{
    /// <summary>
    /// Static class that holds data about build flags that may be important
    ///  to someone implementing a game (or unit tests)
    /// Each build flag will have an appropriate description
    /// </summary>
    public static class BuildFlags
    {
        /// <summary>
        /// If this flag is set to true, then nullchecks are being performed when constructing
        ///  various internally in the engine. When a parameter is passed null that is not expected
        ///  to be null, an exception is raised (usually ArgumentNullException)
        /// This may be disabled to improve performance (via reducing branching on highly performance
        ///  sensitive object construction), or enabled if these exceptions are being consumed.
        /// </summary>
#if (NULLCHECKS)
        public static bool NullChecks = true;
#else
        public static bool NullChecks = false;
#endif
    }
}
