using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    public class UppercaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var upperCase = value.ToString()[0].ToString();

            if(upperCase != upperCase.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
