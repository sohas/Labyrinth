using System;
using System.ComponentModel;

namespace Game
{
    [Serializable]
    public static class EnumExtensions
    {
        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            Type type = value.GetType();
            System.Reflection.MemberInfo[] memberInfo = type.GetMember(value.ToString());
            object[] attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            DescriptionAttribute attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}