using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Quiz.Domain.Extensions {

    [DebuggerStepThrough]
    public static class Check {

        public static T NotNull<T>(
            T value,
            [NotNull] string parameterName) {
            if (value == null) {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T NotNull<T>(
            T value,
            [NotNull] string parameterName,
            string message) {
            if (value == null) {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }

        public static string NotNull(
            string value,
            [NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0) {
            if (value == null) {
                throw new ArgumentException($"{parameterName} can not be null!", parameterName);
            }

            if (value.Length > maxLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }

        public static string NotNullOrWhiteSpace(
            string value,
            [NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            if (value.Length > maxLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }

        public static string NotNullOrEmpty(
            string value,
            [NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0) {
            if (string.IsNullOrEmpty(value)) {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            if (value.Length > maxLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
            }

            return value;
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, [NotNull] string parameterName) {
            if ((value?.Count ?? 0) == 0) {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }

        public static string Length(
            string? value,
            [NotNull] string parameterName,
            int maxLength,
            int minLength = 0) {
            if (minLength > 0) {
                if (string.IsNullOrEmpty(value)) {
                    throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
                }

                if (value.Length < minLength) {
                    throw new ArgumentException($"{parameterName} length must be equal to or bigger than {minLength}!", parameterName);
                }
            }

            if (value != null && value.Length > maxLength) {
                throw new ArgumentException($"{parameterName} length must be equal to or lower than {maxLength}!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        public static Type AssignableTo<TBaseType>(
            Type type,
            [NotNull] string parameterName) {
            NotNull(type, parameterName);

            if (!type.IsAssignableTo<TBaseType>()) {
                throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
            }

            return type;
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <typeparamref name="TTarget"></typeparamref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/>.
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
        public static bool IsAssignableTo<TTarget>([NotNull] this Type type) {
            Check.NotNull(type, nameof(type));

            return type.IsAssignableTo(typeof(TTarget));
        }

        /// <summary>
        /// Determines whether an instance of this type can be assigned to
        /// an instance of the <paramref name="targetType"></paramref>.
        ///
        /// Internally uses <see cref="Type.IsAssignableFrom"/> (as reverse).
        /// </summary>
        /// <param name="type">this type</param>
        /// <param name="targetType">Target type</param>
        public static bool IsAssignableTo([NotNull] this Type type, [NotNull] Type targetType) {
            Check.NotNull(type, nameof(type));
            Check.NotNull(targetType, nameof(targetType));

            return targetType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string GetFullNameWithAssemblyName(this Type type) {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

    }
}
