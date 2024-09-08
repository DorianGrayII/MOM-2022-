namespace MOM
{
    using DBEnum;
    using ProtoBuf;
    using System;
    using UnityEngine;

    [ProtoContract]
    public class BattleFigure
    {
        [ProtoMember(1)]
        public int rangedAttack;
        [ProtoMember(2)]
        public int attack;
        [ProtoMember(3)]
        public int defence;
        [ProtoMember(4)]
        public int maxHitPoints;
        [ProtoMember(5)]
        public int resist;
        [ProtoMember(6)]
        public int rangedAmmo;
        [ProtoMember(7)]
        public int movementSpeed;
        [ProtoMember(8)]
        public float rangedAttackChance;
        [ProtoMember(9)]
        public float attackChance;
        [ProtoMember(10)]
        public float defenceChance;

        public BattleFigure()
        {
        }

        public BattleFigure(Unit source, Attributes attributes)
        {
            this.rangedAmmo = attributes.GetFinal(TAG.AMMUNITION).ToInt();
            this.UpdateFromAttributes(attributes);
        }

        public static void Copy(BattleFigure source, BattleFigure destination)
        {
            destination.rangedAttack = source.rangedAttack;
            destination.attack = source.attack;
            destination.defence = source.defence;
            destination.maxHitPoints = source.maxHitPoints;
            destination.resist = source.resist;
            destination.rangedAttackChance = source.rangedAttackChance;
            destination.attackChance = source.attackChance;
            destination.defenceChance = source.defenceChance;
            destination.rangedAmmo = source.rangedAmmo;
            destination.movementSpeed = source.movementSpeed;
        }

        public static BattleFigure From(BattleFigure bf)
        {
            BattleFigure destination = new BattleFigure();
            Copy(bf, destination);
            return destination;
        }

        public float GetFakePower()
        {
            float num = Mathf.Pow((Mathf.Pow((float) this.defence, 0.8f) - Mathf.Pow((float) this.defence, 0.6f)) + 1f, 2.2f);
            return (((((1.8f * this.attack) + (1.6f * this.rangedAttack)) + num) + (1.4f * this.maxHitPoints)) + (1f * this.resist));
        }

        public void UpdateFromAttributes(Attributes attributes)
        {
            this.rangedAttack = attributes.GetFinal(TAG.RANGE_ATTACK).ToInt();
            this.attack = attributes.GetFinal(TAG.MELEE_ATTACK).ToInt();
            this.defence = attributes.GetFinal(TAG.DEFENCE).ToInt();
            this.maxHitPoints = attributes.GetFinal(TAG.HIT_POINTS).ToInt();
            this.resist = attributes.GetFinal(TAG.RESIST).ToInt();
            this.movementSpeed = attributes.GetFinal(TAG.MOVEMENT_POINTS).ToInt();
            this.rangedAttackChance = attributes.GetFinal(TAG.RANGE_ATTACK_CHANCE).ToFloat();
            this.attackChance = attributes.GetFinal(TAG.MELEE_ATTACK_CHANCE).ToFloat();
            this.defenceChance = attributes.GetFinal(TAG.DEFENCE_CHANCE).ToFloat();
        }
    }
}

