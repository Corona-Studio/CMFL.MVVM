using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CMFL.MVVM.Class.ValidationRules
{
    public class PasswordValidationRule : ValidationRule
    {
        private const string PATTERN =
            "^(?=.*?[A-Z])(?=(.*[a-z]){1,})(?=(.*[\\d]){1,})(?=(.*[\\W]){1,})(?!.*\\s).{8,}$";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return new ValidationResult(false, "该字段为必填项目");

            var r = new Regex(PATTERN);
            var result = r.Match(value.ToString());

            return result.Success
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "最少8位，需要包含大小写、数字以及特殊符号");
        }
    }
}