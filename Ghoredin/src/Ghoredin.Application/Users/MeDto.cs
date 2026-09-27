using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Users
{
    public record MeDto(
        string UserId, 
        string Email, 
        string? Nickname, 
        string DisplayName);
}
