using Application.DTOs.Common;

namespace Web.Services.HATEOAS;

public class HypermediaResponse<T>
{
    public T Data { get; set; } = default!;
    public List<LinkDto> Links { get; set; } = [];

}
