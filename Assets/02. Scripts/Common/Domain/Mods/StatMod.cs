namespace Necrogue.Core.Domain.Mods
{
    public readonly struct StatMod
    {
        public readonly StatId stat;
        public readonly ModType type;
        public readonly float value;

        public StatMod(StatId stat, ModType type, float value)
        {
            this.stat = stat;
            this.type = type;
            this.value = value;
        }
    }
}
