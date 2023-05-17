using System.Collections.Generic;
using System.Linq;

namespace Mockus.Contracts.Request
{
    public class IdentificationRules: List<RequestIdentificationRule>
    {
        public override int GetHashCode()
        {
            int hashValues = 0;
            var hash = 17;
            unchecked
            {
                foreach (var value in this)
                {
                    hashValues = hash * 23 + value.GetHashCode();
                }
            }

            return hashValues;
        }

        public override bool Equals(object? obj)
        {
            var other = (obj as IdentificationRules);

            if (other == null)
                return false;

            return this.Intersect(other).Count() == this.Count;
        }
    }
}
