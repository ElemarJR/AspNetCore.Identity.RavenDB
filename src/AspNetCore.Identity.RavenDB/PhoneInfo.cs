using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Identity.RavenDB
{
    public class PhoneInfo
    {
        public string Number { get; internal set; }

        public DateTime? ConfirmationTime { get; internal set; }
        public bool IsConfirmed => (ConfirmationTime != null);

        public static implicit operator PhoneInfo(string input)
            => new PhoneInfo { Number = input };
    }
}
