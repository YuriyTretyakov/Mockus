using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class RequestIdentificationRule
    {
        public IdentificationSources Type { get; set; }
        public string SourceName { get; set; }

        public string Value { get; set; }

        public RequestIdentificationRule()
        {
        }

        public RequestIdentificationRule(IdentificationSources type, string name, string value)
        {
            Type = type;
            SourceName = name;
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            var other = (obj as RequestIdentificationRule);

            if (other == null)
                return false;

            return Type.Equals(other.Type)
                   && SourceName.Equals(other.SourceName)
                   && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + SourceName.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }
    }
}
