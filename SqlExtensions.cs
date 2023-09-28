using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DatabaseApp_Paul_Zaldea
{
    public static class SqlExtension
    {
        public static string ToSqlInsert<T>(this T toConvert, string tableName = "")
        {
            var sFields = new List<string>();
            var sValues = new List<string>();

            foreach (var pi in toConvert.GetType().GetProperties())
            {
                sFields.Add(string.Format("[{0}]", pi.Name));
                sValues.Add(string.Format("{0}{1}{2}",
                  IsNumericType(pi.PropertyType) ? "" : "'",
                  IsDateTimeType(pi.PropertyType) ? string.Format("{0:yyyyMMdd HH:mm:ss}", pi.GetValue(toConvert)) : pi.GetValue(toConvert),
                  IsNumericType(pi.PropertyType) ? "" : "'"));
            }

            return string.Format("INSERT INTO {0} ({1}) VALUES ({2}) {3}",
              tableName == string.Empty ? toConvert.GetType().Name : tableName,
              string.Join(", ", sFields),
              string.Join(", ", sValues),
              Environment.NewLine + "GO"
            );
        }

        public static string ToSqlUpdate<T>(this T toConvert, string tableName = "", List<string> keyFields = null)
        {
            var sFields = new List<string>();
            var sKeyFields = new List<string>();

            foreach (var pi in toConvert.GetType().GetProperties())
            {
                if (keyFields == null || !keyFields.Contains(pi.Name))
                {
                    sFields.Add(
                      string.Format(
                        "[{0}] = {1}{2}{3}",
                        pi.Name,
                        IsNumericType(pi.PropertyType) ? "" : "'",
                        IsDateTimeType(pi.PropertyType) ? string.Format("{0:yyyyMMdd HH:mm:ss}", pi.GetValue(toConvert)) : pi.GetValue(toConvert),
                        IsNumericType(pi.PropertyType) ? "" : "'"
                      )
                    );
                }
                else
                {
                    sKeyFields.Add(
                      string.Format(
                        "[{0}] = {1}{2}{3}",
                        pi.Name,
                        IsNumericType(pi.PropertyType) ? "" : "'",
                        IsDateTimeType(pi.PropertyType) ? string.Format("{0:yyyyMMdd HH:mm:ss}", pi.GetValue(toConvert)) : pi.GetValue(toConvert),
                        IsNumericType(pi.PropertyType) ? "" : "'"
                      )
                    );
                }
            }

            return string.Format(
              "UPDATE {0} SET {1} WHERE {2} {3}",
              tableName == string.Empty ? toConvert.GetType().Name : tableName,
              string.Join(", ", sFields),
              string.Join(" AND ", sKeyFields),
              Environment.NewLine + "GO"
            );
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static bool IsDateTimeType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.DateTime:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsDateTimeType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }

        public static T ToObject<T>(this IDataReader reader, object instance) where T : class, new()
        {
            T result;

            if (instance == null)
            {
                result = Activator.CreateInstance<T>();
            }
            else
            {
                result = instance as T;
            }

            Type typeT = result.GetType();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (!reader.IsDBNull(i))
                {
                    string fname = reader.GetName(i);
                    PropertyInfo pi = typeT.GetProperty(fname, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    if (pi != null)
                    {
                        if (reader.GetFieldType(i).Equals(typeof(byte[])) && pi.PropertyType.Equals(typeof(string)))
                        {
                            pi.SetValue(result, Convert.ToBase64String((byte[])reader[i]), null);
                        }
                        else
                        {
                            pi.SetValue(result, reader[i], null);
                        }
                    }
                }
            }

            return result;
        }

        public static T ToObject<T>(this IDataReader reader) where T : class, new()
        {
            return ToObject<T>(reader, Activator.CreateInstance<T>());
        }

        public static ICollection<T> FetchAll<T>(this IDataReader reader) where T : class, new()
        {
            List<T> result = new List<T>();

            while (reader.Read())
            {
                result.Add(reader.ToObject<T>());
            }

            return result;
        }

    }
}
