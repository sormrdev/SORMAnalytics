using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

using Application.DTOs;

namespace Application.Common.Services.DataShaping;
public class DataShapingService
{
    private readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
    public ExpandoObject ShapeData<T>(T entity, string? fields)
    {
        if (entity == null)
        {
            return new ExpandoObject();
        }

        var filteredFields = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var properties = _propertiesCache.GetOrAdd(typeof(T), typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));

        if (filteredFields.Count != 0)
        {
            properties = properties
                .Where(f => filteredFields.Contains(f.Name))
                .ToArray();
        }

        IDictionary<string, object?> shapedObject = new ExpandoObject();

        foreach (var property in properties)
        {
            shapedObject[property.Name] = property.GetValue(entity);
        }

        return (ExpandoObject)shapedObject;
    }

    public List<ExpandoObject> ShapeDataCollection<T>(IEnumerable<T> entities, string? fields)
    {
        var filteredFields = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        var properties = _propertiesCache.GetOrAdd(typeof(T), typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));

        if (filteredFields.Count != 0)
        {
            properties = properties
                .Where(f => filteredFields.Contains(f.Name))
                .ToArray();
        }

        List<ExpandoObject> shapedObjects = [];

        foreach (T entity in entities)
        {
            IDictionary<string, object?> shapedObject = new ExpandoObject();

            foreach (var property in properties)
            {
                shapedObject[property.Name] = property.GetValue(entity);
            }

            shapedObjects.Add((ExpandoObject)shapedObject);
        }

        return shapedObjects;
    }

    public bool ValidateFields<T>(string fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        var filteredFields = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet();

        var properties = _propertiesCache.GetOrAdd(typeof(T), typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance));

        return filteredFields.All(f => properties.Any(p => p.Name.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}
