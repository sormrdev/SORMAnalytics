namespace Application.DTOs.Common;
internal interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
