using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GunGameProgressionMaker
{
    public enum EItemCategory
    {
        Pistol,
        Shotgun,
        SMG_Rifle,
        Support,
        Attachment,
        Melee,
        Misc,
        Magazine,
        Clip,
        MeatFortress,
        Speedloader,
        Cartridge,
        Props,
        Attachment2Temp = 40
    }

    public enum EItemSubCategory
    {
        None,
        AutomaticPistol,
        Revolver,
        MachinePistol,
        BreakActionShotgun,
        TubeFedShotgun,
        MagazineFedShotgun,
        SMG,
        PDW,
        LeverActionRifle,
        Carbine,
        AssaultRifle,
        BattleRifle,
        BoltActionRifle,
        AntiMaterial,
        Grenade,
        Machinegun,
        Ordnance,
        IronSight,
        ReflexSight,
        Magnifier_Scope,
        Suppressor,
        Laser_Light,
        RailAdapter,
        Decorative,
        Tactical,
        Improvised,
        Thrown,
        Utility,
        Target,
        Horseshoe,
        TippyToy,
        Firework,
        BreechloadingHandgun,
        LeverActionHandgun,
        LeverActionShotgun,
        Garage,
        PowerTools,
        Medieval,
        GoofyMelee,
        FarmTools,
        Shields,
        BoltActionPistol,
        Foregrip,
        Stock,
        Backpack,
        Bayonet,
        UnderBarrelWeapon,
        BangerJunk,
        RailHat,
        MuzzleLoadedPistol,
        Derringer,
        BreechloadingRifle,
        MuzzleLoadedRifle,
        Bipods,
        MuzzleBrakes,
        MuzzleAdapters,
        Sosigguns,
        GrenadeLaunchers,
        RocketMissileLaunchers,
        LocomotionGuns,
        RemoteExplosives,
        Exotic,
        Barrier,
        SteelTarget,
        DestructibleTarget,
        Prop,
        Furniture,
        MF_Scout = 100,
        MF_Soldier,
        MF_Pyro,
        MF_Demo,
        MF_Heavy,
        MF_Engineer,
        MF_Medic,
        MF_Sniper,
        MF_Spy,
        MF_Generic
    }

    public class ItemSpawnerID
    {
        public string Caliber { get; set; }

        public string SpawnFromID { get; set; }

        public EItemCategory Category { get; set; }

        public string CategoryDisplay
        {
            get
            {
                if (Category == EItemCategory.SMG_Rifle)
                {
                    if (SubCategory == EItemSubCategory.SMG || SubCategory == EItemSubCategory.PDW)
                    {
                        return "SMG";
                    }
                    else
                    {
                        return "Rifle";
                    }

                }
                else
                {
                    return Regex.Replace(Category.ToString(), "([A-Z])", " $1").Trim();
                }
            }
            private set
            {
                CategoryDisplay = value;
            }
        }

        public EItemSubCategory SubCategory { get; set; }

        public string SubCategoryDisplay
        {
            get
            {
                if ((int)SubCategory >= 100)
                {
                    return SubCategory.ToString();
                }
                else
                {
                    if (SubCategory == EItemSubCategory.PDW)
                    {
                        return "PDW";

                    }
                    if (SubCategory == EItemSubCategory.SMG)
                    {
                        return "SMG";
                    }

                    return Regex.Replace(SubCategory.ToString(), "([A-Z])", " $1").Trim();
                }
            }
            private set
            {
                SubCategoryDisplay = value;
            }
        }
    }
}
