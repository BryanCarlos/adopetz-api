using Adopetz.Api.Application.Common.Models;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Common.Interfaces
{
    public interface ITokenGenerator
    {
        TokenResult GenerateAccessToken(User user, DateTime now);
        TokenResult GenerateRefreshToken(DateTime now);
    }
}
