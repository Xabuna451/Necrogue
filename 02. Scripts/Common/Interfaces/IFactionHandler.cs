using Necrogue.Enemy.Runtime;

namespace Necrogue.Common.Interfaces
{
    public interface IFactionHandler
    {
        void OnFactionChanged(Faction newFaction);
    }
}