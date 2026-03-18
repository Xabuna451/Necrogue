using System;
using Necrogue.Perk.Data;

[Serializable]
public class PerkInstance
{
    public PerkDef def;
    public int stack;

    public PerkInstance(PerkDef def)
    {
        this.def = def;
        this.stack = 1;
    }
}
