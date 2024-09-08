using System.Collections.Generic;

namespace MOM
{
    public class EnchantmentRegister
    {
        private static EnchantmentRegister instance;

        private Dictionary<PlayerWizard, List<EnchantmentInstance>> registry = new Dictionary<PlayerWizard, List<EnchantmentInstance>>();

        private Dictionary<PlayerWizard, int> totalCostCache = new Dictionary<PlayerWizard, int>();

        public static bool IsInstantiated()
        {
            return EnchantmentRegister.instance != null;
        }

        private static EnchantmentRegister Get()
        {
            if (EnchantmentRegister.instance == null)
            {
                EnchantmentRegister.instance = new EnchantmentRegister();
            }
            return EnchantmentRegister.instance;
        }

        public static void EnchantmentAdded(EnchantmentInstance ei)
        {
            if (ei.upkeepMana > 0 && ei.owner != null)
            {
                PlayerWizard playerWizard = ei.owner.Get<PlayerWizard>();
                if (playerWizard != null)
                {
                    EnchantmentRegister.Get().Register(ei, playerWizard);
                }
            }
        }

        public static void EnchantmentAdded(List<EnchantmentInstance> eiList)
        {
            eiList?.ForEach(delegate(EnchantmentInstance o)
            {
                EnchantmentRegister.EnchantmentAdded(o);
            });
        }

        public static void EnchantmentRemoved(EnchantmentInstance ei)
        {
            if (ei.upkeepMana > 0 && ei.owner != null)
            {
                PlayerWizard playerWizard = ei.owner.Get<PlayerWizard>();
                if (playerWizard != null)
                {
                    EnchantmentRegister.Get().UnRegister(ei, playerWizard);
                }
            }
        }

        public static void EnchantmentRemoved(List<EnchantmentInstance> eiList)
        {
            eiList?.ForEach(delegate(EnchantmentInstance o)
            {
                EnchantmentRegister.EnchantmentRemoved(o);
            });
        }

        public static int GetTotalManaCost(PlayerWizard pw)
        {
            return EnchantmentRegister.Get().GetTMC(pw);
        }

        public static List<EnchantmentInstance> GetByWizard(PlayerWizard pw)
        {
            if (EnchantmentRegister.Get().registry.ContainsKey(pw))
            {
                return EnchantmentRegister.Get().registry[pw];
            }
            return new List<EnchantmentInstance>();
        }

        public static bool IsStillValid(EnchantmentInstance ei)
        {
            if (ei.upkeepMana > 0 && ei.owner != null)
            {
                PlayerWizard playerWizard = ei.owner.Get<PlayerWizard>();
                if (playerWizard != null)
                {
                    return EnchantmentRegister.Get().registry[playerWizard].Contains(ei);
                }
                return false;
            }
            return true;
        }

        public static void Clear()
        {
            EnchantmentRegister.instance = null;
        }

        private int GetTMC(PlayerWizard pw)
        {
            if (!this.totalCostCache.ContainsKey(pw) || this.totalCostCache[pw] < 0)
            {
                int cost = 0;
                if (this.registry.ContainsKey(pw))
                {
                    this.registry[pw].ForEach(delegate(EnchantmentInstance o)
                    {
                        cost += o.upkeepMana;
                    });
                }
                this.totalCostCache[pw] = cost;
            }
            return this.totalCostCache[pw];
        }

        private void Register(EnchantmentInstance ei, PlayerWizard pw)
        {
            if (!this.registry.ContainsKey(pw))
            {
                this.registry[pw] = new List<EnchantmentInstance>();
            }
            if (!this.registry[pw].Contains(ei))
            {
                this.registry[pw].Add(ei);
                this.totalCostCache[pw] = -1;
                if (HUD.Get() != null && pw == GameManager.GetHumanWizard())
                {
                    HUD.Get().UpdateHUD();
                }
            }
        }

        private void UnRegister(EnchantmentInstance ei, PlayerWizard pw)
        {
            if (this.registry.ContainsKey(pw) && this.registry[pw].Contains(ei))
            {
                this.registry[pw].Remove(ei);
                this.totalCostCache[pw] = -1;
                if (HUD.Get() != null && pw == GameManager.GetHumanWizard())
                {
                    HUD.Get().UpdateHUD();
                }
            }
        }
    }
}
