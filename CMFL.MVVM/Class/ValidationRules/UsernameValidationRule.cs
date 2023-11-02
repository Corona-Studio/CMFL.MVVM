using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CMFL.MVVM.Class.ValidationRules
{
    public class UsernameValidationRule : ValidationRule
    {
        private const string PATTERN = "^[a-zA-Z0-9_-]{4,16}$";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return new ValidationResult(false, "该字段为必填项目");

            var r = new Regex(PATTERN);
            var result = r.Match(value.ToString());

            return result.Success
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "长度4~16，只允许包含数字字母，下划线以及横杠");
        }
    }
}