using UnityEngine.Serialization;

namespace BitBox.AI
{
    public enum ModuleType
    {
        [FormerlySerializedAs("None")] NONE = 0,
        [FormerlySerializedAs("Idle")] IDLE = 1,
        [FormerlySerializedAs("Die")] DIE = 2,
        [FormerlySerializedAs("Follow")] FOLLOW = 3,
        [FormerlySerializedAs("Retreat")] RETREAT = 4,
        [FormerlySerializedAs("Melee Attack")] MELEE_ATTACK = 5,
        [FormerlySerializedAs("Ranged Attack")] RANGED_ATTACK = 6,
        [FormerlySerializedAs("Follow Attack")] FOLLOW_ATTACK = 7,
        [FormerlySerializedAs("Parry")] PARRY = 8,
        [FormerlySerializedAs("Take Damage")] TAKE_DAMAGE = 9,
        [FormerlySerializedAs("Look At Player")] LOOK_AT_PLAYER = 10,

    }
    /*
    static class ModuleTypeExtension
    {
        public static Module GetModule(this ModuleType moduleType)
        {
            switch (moduleType)
            {
                case ModuleType.DIE: return new DieModule();
                case ModuleType.FOLLOW: return new FollowPlayerModule();
                case ModuleType.IDLE: return new IdleModule();
                case ModuleType.MELEE_ATTACK: return new MeleeAttackModule();
                case ModuleType.FOLLOW_ATTACK: return new FollowAttackModule();
                case ModuleType.PARRY: return new ParryModule();
                case ModuleType.RETREAT: return new RetreatModule();
                case ModuleType.TAKE_DAMAGE: return new TakeDamageModule();
                default: return null;
            }
        }
    }*/
}
