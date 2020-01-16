using System;
using System.ComponentModel;

namespace Dream.Common.ExtensionMethods
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gives the friendly name for a specific enum value based on the "Description" attribute used.
        /// http://stackoverflow.com/questions/1415140/can-my-enums-have-friendly-names
        /// </summary>
        public static string GetFriendlyDescription(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var enumName = Enum.GetName(type, enumValue);

            if (enumName != null)
            {
                var fieldInfo = type.GetField(enumName);
                if (fieldInfo != null)
                {
                    var descriptionAttributeType = typeof(DescriptionAttribute);
                    var descriptionAttribute = Attribute.GetCustomAttribute(fieldInfo, descriptionAttributeType);
                    var castedDescriptionAttribute = descriptionAttribute as DescriptionAttribute;

                    if (castedDescriptionAttribute != null)
                    {
                        return castedDescriptionAttribute.Description;
                    }
                }
            }

            return enumValue.ToString();
        }
    }
}
