namespace Live.Log.Extractor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public static class ExtensionHelpers
    {
        /// <summary>
        /// Gets the enum for value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T GetEnumForValue<T>(this string value) 
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null && string.Equals(attribute.Description, value))
                {
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }

    }
}
