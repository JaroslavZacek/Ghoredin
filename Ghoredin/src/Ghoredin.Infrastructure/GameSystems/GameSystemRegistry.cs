using Ghoredin.Application.GameSystems;

using System;
using System.Collections.Generic;
using System.Text;

namespace Ghoredin.Infrastructure.GameSystems
{
    public class GameSystemRegistry : IGameSystemRegistry
    {
        private readonly Dictionary<string, IGameSystem> _systems;

        public GameSystemRegistry(IEnumerable<IGameSystem> systems)
        {
            _systems = systems.ToDictionary(s => s.Id, s => s);
        }

        public IGameSystem Get(string gameSystemId)
        {
            if (_systems.TryGetValue(gameSystemId, out var system))
                return system;

            throw new InvalidOperationException($"Neznámý herní systém: {gameSystemId}");
        }

        public bool Exists(string gameSystemId)
            => _systems.ContainsKey(gameSystemId);

        public IReadOnlyList<IGameSystem> GetAll()
            => _systems.Values.ToList();
    }
}
