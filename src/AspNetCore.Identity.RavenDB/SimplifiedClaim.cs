using System;
using System.Security.Claims;

namespace AspNetCore.Identity.RavenDB
{
    public class SimplifiedClaim : IEquatable<SimplifiedClaim>, IEquatable<Claim>
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public static implicit operator SimplifiedClaim(Claim original) =>
            new SimplifiedClaim {Type = original.Type, Value = original.Value};

        public static implicit operator Claim(SimplifiedClaim simplified) =>
            new Claim(simplified.Type, simplified.Value);

        public bool Equals(SimplifiedClaim other)
            => Type == other.Type && Value == other.Value;

        public bool Equals(Claim other)
            => Type == other.Type && Value == other.Value;
    }
}