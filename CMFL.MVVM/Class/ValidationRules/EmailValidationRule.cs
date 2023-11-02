using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CMFL.MVVM.Class.ValidationRules
{
    public class EmailValidationRule : ValidationRule
    {
        private const string PATTERN = "^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var r = new Regex(PATTERN);
            var result = r.Match(value.ToString());

            return result.Success
                ? new ValidationResult(true, null)
                : new ValidationResult(false, "只允许英文字母、数字、下划线、英文句号及中划线");
        }
    }
}