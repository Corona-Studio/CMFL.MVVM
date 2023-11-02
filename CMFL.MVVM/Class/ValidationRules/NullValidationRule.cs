using System.Globalization;
using System.Windows.Controls;

namespace CMFL.MVVM.Class.ValidationRules
{
    public class NullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(!string.IsNullOrWhiteSpace(value?.ToString()), "该字段为必填项目");
        }
    }
}