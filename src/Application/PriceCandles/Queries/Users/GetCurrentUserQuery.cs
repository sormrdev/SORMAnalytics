using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

using Application.DTOs.Users;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using SORMAnalytics.Application.Common.Exceptions;
using SORMAnalytics.Application.Common.Interfaces;

namespace Application.PriceCandles.Queries.Users;
public record GetCurrentUserQuery(string userId) : IRequest<UserDto> { }

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetCurrentUserHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        UserDto? user = await _context.Users
            .Where(u => u.Id == request.userId)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new GenericException(404, "User not found");
        }

        return user;
    }
}
