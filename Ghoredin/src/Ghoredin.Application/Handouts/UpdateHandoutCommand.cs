using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.Handouts
{
    public record UpdateHandoutCommand(
        Guid Id,
        string Title,
        string Content);
}
