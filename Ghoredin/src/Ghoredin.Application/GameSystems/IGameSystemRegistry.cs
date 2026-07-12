using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Application.GameSystems
{
    public interface IGameSystemRegistry
    {
        IGameSystem Get(string gameSystemId);

        bool Exists(string gameSystemId);

        IReadOnlyList<IGameSystem> GetAll();
    }
}
