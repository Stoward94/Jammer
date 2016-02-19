using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Helpers
{
    public class PresentFutureDate : ValidationAttribute
    {
        public PresentFutureDate() : base("{0} must be a present or future date.")
        {}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var errorMessage = FormatErrorMessage(validationContext.DisplayName);

                DateTime srcDate;

                if (!DateTime.TryParse(value.ToString(), out srcDate))
                {
                    return new ValidationResult(errorMessage);
                }

                if (srcDate.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(errorMessage);
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("" + validationContext.DisplayName + " is required");
        }
    }
}
