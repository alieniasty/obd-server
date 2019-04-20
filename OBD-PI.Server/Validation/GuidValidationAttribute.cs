using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDPI.Server.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GuidValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value is Guid ? ValidationResult.Success : null;
        }
    }
}
