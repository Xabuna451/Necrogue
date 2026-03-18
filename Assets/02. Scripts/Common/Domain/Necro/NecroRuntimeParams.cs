namespace Necrogue.Core.Domain.Necro
{
    public class NecroRuntimeParams
    {
        // Damage
        public float allyDamageAdd = 0f;
        public float allyDamageMul = 0f;
        public bool hasAllyDamageOv = false;
        public float allyDamageOv = 0f;

        // HP
        public float allyHpAdd = 0f;
        public float allyHpMul = 0f;
        public bool hasAllyHpOv = false;
        public float allyHpOv = 0f;

        // Cap
        public int allyCapAdd = 0;
        public float allyCapMul = 0f;
        public bool hasAllyCapOv = false;
        public int allyCapOv = 0;

        public void Reset()
        {
            allyDamageAdd = 0f; allyDamageMul = 0f; hasAllyDamageOv = false; allyDamageOv = 0f;
            allyHpAdd = 0f; allyHpMul = 0f; hasAllyHpOv = false; allyHpOv = 0f;
            allyCapAdd = 0; allyCapMul = 0f; hasAllyCapOv = false; allyCapOv = 0;
        }
    }
}
