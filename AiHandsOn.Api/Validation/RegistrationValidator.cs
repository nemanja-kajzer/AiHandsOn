using AiHandsOn.Api.Dtos;
using System.Text.RegularExpressions;

namespace AiHandsOn.Api.Validation
{
    // A focused validator class that encapsulates all validation logic for registration
    public class RegistrationValidator : IRegistrationValidator
    {
        private static readonly Regex EmailRegex = new("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.Compiled);
        private static readonly Regex LowerRegex = new("[a-z]");
        private static readonly Regex UpperRegex = new("[A-Z]");
        private static readonly Regex DigitRegex = new("[0-9]");
        private static readonly Regex SpecialRegex = new("[^A-Za-z0-9]");

        public List<string> Validate(RegisterUserDto dto)
        {
            var errors = new List<string>();

            if (dto == null)
            {
                errors.Add("Request body is required.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(dto.FirstName)) errors.Add("FirstName is required.");
            if (string.IsNullOrWhiteSpace(dto.LastName)) errors.Add("LastName is required.");
            if (string.IsNullOrWhiteSpace(dto.Email)) errors.Add("Email is required.");
            if (string.IsNullOrWhiteSpace(dto.Password)) errors.Add("Password is required.");
            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword)) errors.Add("ConfirmPassword is required.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && !EmailRegex.IsMatch(dto.Email))
            {
                errors.Add("Email is not a valid email address.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var pwd = dto.Password;
                if (pwd.Length < 8) errors.Add("Password must be at least 8 characters.");
                if (!LowerRegex.IsMatch(pwd)) errors.Add("Password must contain a lowercase letter.");
                if (!UpperRegex.IsMatch(pwd)) errors.Add("Password must contain an uppercase letter.");
                if (!DigitRegex.IsMatch(pwd)) errors.Add("Password must contain a digit.");
                if (!SpecialRegex.IsMatch(pwd)) errors.Add("Password must contain a special character.");
            }

            if (!string.Equals(dto.Password, dto.ConfirmPassword, StringComparison.Ordinal))
            {
                errors.Add("Password and ConfirmPassword do not match.");
            }

            return errors;
        }
    }
}