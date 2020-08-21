using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using static MercadoPago.Validation.ValidationError;

namespace MercadoPago.Validation
{
    internal static class Validator
    {
        internal static IEnumerable<ValidationError> GetValidationErrors(object instance)
        {
            var properties = instance.GetType().GetProperties();

            var instanceErrors =
                from property in properties
                from attr in property.GetCustomAttributes(typeof(ValidationAttribute), inherit: true)
                let validation = (ValidationAttribute) attr
                let propertyValue = property.GetValue(instance, BindingFlags.GetProperty, null, null, null)
                let validationError = GetValidationError(instance, validation, property.Name, propertyValue)
                where validationError != null
                select validationError;

            foreach (var e in instanceErrors)
                yield return e;

            if (instance is IEnumerable list)
            {
                var itemErrors =
                    from object item in list
                    from e in GetValidationErrors(item)
                    select e;

                foreach (var e in itemErrors)
                    yield return e;
            }
            else
            {
                var nestedErrors =
                    from property in properties
                    where property.PropertyType.IsSdkType()
                    let propertyValue = property.GetValue(instance, BindingFlags.GetProperty, null, null, null)
                    where propertyValue != null
                    from e in GetValidationErrors(propertyValue)
                    select e;

                foreach (var e in nestedErrors)
                    yield return e;
            }
        }

        public static ValidationResult GetValidationResult<T>(T instance) where T : ResourceBase
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var validationErrors = GetValidationErrors(instance);

            return new ValidationResult(validationErrors);
        }

        public static void Validate<T>(T instance) where T : ResourceBase
        {
            var result = GetValidationResult(instance);

            if (!result.IsOk)
            {
                var errorMessage = new StringBuilder($"There are errors in the object you're trying to create. Review them to continue: {Environment.NewLine}");

                foreach (var e in result.Errors)
                    errorMessage.AppendLine($" - {e.Message}{Environment.NewLine}");

                throw new MPException(errorMessage.ToString());
            }
        }

        private static ValidationError GetValidationError(object instance, ValidationAttribute attribute, string propertyName, object value)
        {
            try
            {
                if (attribute.IsValid(value))
                    return null;

                var msg = $"Error on property {propertyName}";

                switch (attribute)
                {
                    case RangeAttribute r:
                        return new ValidationError(OutOfRangeErrorCode, $"{msg}. The value you are trying to assign is not in the specified range: {r.Minimum}-{r.Maximum}.");
                    case RequiredAttribute _:
                        return new ValidationError(RequiredErrorCode, $"{msg}. There is no value for this required property.");
                    case RegularExpressionAttribute a:
                        return new ValidationError(RegExpErrorCode, $"{msg}. The specified value is not valid. RegExp: {a.Pattern}.");
                    case DataTypeAttribute _:
                        return new ValidationError(DataTypeErrorCode, $"{msg}. The value you are trying to assign has not the correct type.");
                    case StringLengthAttribute _:
                        return new ValidationError(StringLengthErrorCode, $"{msg}. The length of the string exceeds the maximum allowed length.");
                    default:
                        throw new InvalidOperationException($"Unknown Validation Attribute Type: {attribute.GetType().Name}");
                }
            }
            catch (Exception e)
            {
                throw new ValidationException(instance?.GetType().FullName, propertyName, e);
            }
        }

        private static bool IsSdkType(this Type type) =>
            type.Assembly.FullName == typeof(ResourceBase).Assembly.FullName ||
            type.IsGenericType && type.GetGenericArguments().Any(IsSdkType);
    }
}
