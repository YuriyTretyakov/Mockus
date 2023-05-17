using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class RequestIdentifier : IRequestIdentifier
    {
        public RequestIdentifier(string path, string method):this()
        {
            Path = path;
            Method = method;
        }

        public RequestIdentifier()
        {
            CustomIdentification = new IdentificationRules();
        }

        public RequestIdentifier(string path, HttpMethod method):this()
        {
            Path = path;
            Method = method.ToString();
        }

        public string Path { get; set; }
        public string Method { get; set; }

        public IdentificationRules CustomIdentification { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Path.GetHashCode();
                hash = hash * 23 + Method.GetHashCode();
                hash = hash * 23 + CustomIdentification.GetHashCode();
                return hash;
            }
        }

        public bool EqualsByPathAndMethod(RequestIdentifier identifier)
        {
            if (identifier == null)
                return false;

            return Path.Equals(identifier.Path)
                   && Method.Equals(identifier.Method);
        }

        public override bool Equals(object? obj)
        {
            var other = (obj as RequestIdentifier);

            if (other == null)
                return false;

            return Path.Equals(other.Path)
                           && Method.Equals(other.Method)
                           && CustomIdentification.Equals(other.CustomIdentification);
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Path))
            {
                throw new ValidationException($"{nameof(Path)} should not be null or empty");
            }

            if (Enum.TryParse(typeof(HttpMethod), Method, out _))
            {
                throw new ValidationException($"{nameof(Method)} is invalid : '{Method}'");
            }
        }

        public override string ToString()
        {
            return $"Path: {Path} Method: {Method}";
        }
    }
}