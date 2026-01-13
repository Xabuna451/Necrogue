namespace Necrogue.Core.Domain.Necro
{
    public enum NecroParam
    {
        AllyDamage,
        AllyHp,
        AllyCap
    }

    public readonly struct NecroMod
    {
        public readonly NecroParam param;
        public readonly Necrogue.Core.Domain.Mods.ModType type;
        public readonly float value;

        public NecroMod(NecroParam param, Necrogue.Core.Domain.Mods.ModType type, float value)
        {
            this.param = param;
            this.type = type;
            this.value = value;
        }
    }
}
