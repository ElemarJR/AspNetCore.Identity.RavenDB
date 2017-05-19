using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Raven.Imports.Newtonsoft.Json;

namespace AspNetCore.Identity.RavenDB
{
    public class LockoutInfo
    {
        public DateTimeOffset? EndDate { get; internal set; }
        public bool Enabled { get; internal set; }
        public int AccessFailedCount { get; internal set; }

        [JsonIgnore]
        public bool AllPropertiesAreSetToDefaults =>
            EndDate == null &&
            Enabled == false &&
            AccessFailedCount == 0;
    }
}
