using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.GameSystems
{
    public record ValidationResult(bool IsValid, List<string> Errors)
    {
        public static ValidationResult Success() => new ValidationResult(true, new List<string>());
        public static ValidationResult Failure(params string[] errors) => new(false, errors.ToList());
    }
}
