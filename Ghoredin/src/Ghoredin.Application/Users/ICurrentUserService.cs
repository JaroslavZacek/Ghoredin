using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Users
{
    public interface ICurrentUserService
    {
        string? UserId { get; }

        bool IsAuthenticated { get; }
    }
}
