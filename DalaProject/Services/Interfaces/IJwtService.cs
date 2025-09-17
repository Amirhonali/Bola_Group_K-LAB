using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DalaProject.Models;

namespace DalaProject.Services.Interfaces
{
    public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken();
}
}