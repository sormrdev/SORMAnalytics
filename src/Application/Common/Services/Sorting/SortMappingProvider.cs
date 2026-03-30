using System.Linq.Dynamic.Core;

namespace Application.Common.Services.Sorting;

public class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
   public SortMapping[] GetMappings<TSource, TDestination>()
   {
        SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException($"No sort mapping definition found for source type {typeof(TSource)} and destination type {typeof(TDestination)}.");
        }

        return sortMappingDefinition.Mappings;
   }

    public bool ValidateMappings<TSource, TDestination>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }
        
        var sortFields = sort
            .Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        SortMapping[] mappings = GetMappings<TSource, TDestination>();

        return sortFields.All(f =>
            mappings.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));    
    }
}
