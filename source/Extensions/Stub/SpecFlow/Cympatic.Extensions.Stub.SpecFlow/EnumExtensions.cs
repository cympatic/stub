using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Cympatic.Extensions.Stub.SpecFlow
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum en)
        {
            var type = en.GetType();
            var memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                var descAttrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descAttrs.Length > 0)
                {
                    return ((DescriptionAttribute)descAttrs[0]).Description;
                }

                var enumAttrs = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);

                if (enumAttrs.Length > 0)
                {
                    var enumMember = (EnumMemberAttribute)enumAttrs[0];

                    if (!string.IsNullOrEmpty(enumMember.Value))
                    {
                        return enumMember.Value;
                    }
                }
            }

            return en.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string description) where TEnum : Enum
        {
            var type = typeof(TEnum);

            return
                (from object value in Enum.GetValues(type)
                 where ((TEnum)value).ToString().Equals(description, StringComparison.OrdinalIgnoreCase) ||
                       ((TEnum)value).ToDescription().Equals(description, StringComparison.OrdinalIgnoreCase)
                 select (TEnum)value)
                .FirstOrDefault();
        }

        public static IList ToList(this Enum en)
        {
            var list = new ArrayList();
            var enumValues = Enum.GetValues(en.GetType());

            foreach (Enum value in enumValues)
            {
                list.Add(new KeyValuePair<Enum, string>(value, value.ToDescription()));
            }

            return list;
        }
    }
}
