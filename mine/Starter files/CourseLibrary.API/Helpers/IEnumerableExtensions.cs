using System.Dynamic;
using System.Reflection;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Helpers;

public static class IEnumerableExtensions
{
    public static IEnumerable<ExpandoObject> ShapeData<TSource>(
        this IEnumerable<TSource> source,
        string? fields)
    {
        ArgumentNullException.ThrowIfNull(source);
        var expandoObjectList = new List<ExpandoObject>();
        var propertyInfoList = new List<PropertyInfo>();
        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(TSource)
                .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsAfterSplit = fields.Split(",");
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase
                                               | BindingFlags.Public
                                               | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    throw new Exception($"Property {propertyInfo} was not found on {typeof(TSource)}");
                }

                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (var sourceObject in source)
        {
            var dataShapedObject = new ExpandoObject();
            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(sourceObject);
                ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            expandoObjectList.Add(dataShapedObject);
        }

        return expandoObjectList;
    }
}