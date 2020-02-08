using NUnit.Framework;
using System.Collections;

namespace Tests
{
    public static class AssertExtensions
    {
        public static void DoesNotContains(object expected, ICollection collection)
        {
            try
            {
                Assert.Contains(expected, collection);
            }
            catch(AssertionException)
            {
                return;
            }

            throw new AssertionException("Expected collection not to has " + expected);
        }
    }
}