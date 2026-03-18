using System;
using System.Collections.Generic;

using Necrogue.Enemy.Runtime;

namespace Necrogue.Player.Runtime
{
    public partial class NecromancerController
    {
        public event Action OnUndeadChanged;

        public int UndeadCount => undead.Count;
        public int ReservedCount => reserved.Count;
        public int ReanimCount => reanim.Count;

        public int SlotUsed => undead.Count + reserved.Count + reanim.Count;
        public int SlotMax => Max;

        public IReadOnlyList<EnemyContext> UndeadList => undead;

        void NotifyUndeadChanged() => OnUndeadChanged?.Invoke();
    }
}
