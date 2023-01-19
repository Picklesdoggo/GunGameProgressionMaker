using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GunGameProgressionMaker
{
    public enum EFirearmFiringMode
    {
        None,
        SemiAuto,
        Burst,
        FullAuto,
        SingleFire
    }

    public enum EObjectCategory
    {
        Uncategorized,
        Firearm,
        Magazine,
        Clip,
        Cartridge,
        Attachment,
        SpeedLoader,
        Thrown,
        MeleeWeapon = 10,
        Explosive = 20,
        Powerup = 25,
        Target = 30,
        Prop,
        Furniture,
        Tool = 40,
        Toy,
        Firework,
        Ornament,
        Loot = 50,
        VFX,
        SosigClothing = 60
    }

    public enum ETagEra
    {
        None,
        Colonial,
        WildWest,
        TurnOfTheCentury,
        WW1,
        WW2,
        PostWar,
        Modern,
        Futuristic,
        Medieval
    }

    public enum ETagSet
    {
        Real,
        GroundedFictional,
        SciFiFictional,
        Meme,
        MeatFortress,
        Holiday,
        TNH,
        NonCombat
    }

    public enum ETagFirearmSize
    {
        None,
        Pocket,
        Pistol,
        Compact,
        Carbine,
        FullSize,
        Bulky,
        Oversize
    }

    public enum ETagFirearmRoundPower
    {
        None,
        Tiny,
        Pistol,
        Shotgun,
        Intermediate,
        FullPower,
        AntiMaterial,
        Ordnance,
        Exotic,
        Fire
    }

    public enum ETagFirearmCountryOfOrigin
    {
        None,
        Fictional,
        UnitedStatesOfAmerica = 10,
        MuricanRemnants,
        BritishEmpire = 20,
        UnitedKingdom,
        CommonwealthOfAustralia,
        KingdomOfFrance = 30,
        FrenchSecondRepublic,
        SecondFrenchEmpire,
        FrenchThirdRepublic,
        VichyFrance,
        FrenchFourthRepublic,
        FrenchRepublic,
        GermanEmpire = 40,
        WeimarRepublic,
        GermanReich,
        WestGermany,
        GermanDemocraticRepublic,
        FederalRepublicOfGermany,
        TsardomOfRussia = 50,
        RussianEmpire,
        UnionOfSovietSocialistRepublics,
        RussianFederation,
        KingdomOfBelgium = 60,
        KingdomOfItaly = 70,
        ItalianRepublic,
        SwedishEmpire = 90,
        UnitedKingdomsOfSwedenAndNorway,
        KingdomOfSweden,
        KingdomOfNorway = 100,
        KingdomOfFinland = 110,
        RepublicOfFinland,
        Czechoslovakia = 120,
        CzechRepublic,
        Ukraine = 130,
        SwissConfederation = 140,
        FirstSpanishRepublic = 150,
        SecondSpanishRepublic,
        SpanishState,
        KingdomOfSpain,
        AustrianEmpire = 160,
        AustroHungarianEmpire,
        RepublicOfAustria,
        FirstHungarianRepublic = 170,
        HungarianRepublic,
        KingdomOfHungary,
        HungarianPeoplesRepublic,
        RepublicOfCroatia = 190,
        RepublicOfKorea = 200,
        DemocraticRepublicOfVietnam = 210,
        StateOfIsrael = 220,
        FederativeRepublicOfBrazil = 230,
        EmpireOfJapan = 240,
        RepublicOfSouthAfrica = 250,
        GovernmentOfTheRepublicOfPolandInExile = 262,
        RepublicOfPoland,
        PeoplesRepublicOfChina = 270,
        FormerYugoslavicRepublicOfMacedonia = 280,
        Yugoslavia
    }

    public enum ETagFirearmAction
    {
        None,
        BreakAction,
        BoltAction,
        Revolver,
        PumpAction,
        LeverAction,
        Automatic,
        RollingBlock,
        OpenBreach,
        Preloaded,
        SingleActionRevolver
    }

    public class ObjectID
    {
        public string SpawnFromID { get; set; }

        public int MagazineType { get; set; }

        public int ClipType { get; set; }

        public int RoundType { get; set; }

        public string ItemID { get; set; }

        public List<EFirearmFiringMode> FiringModes { get; set; }

        public List<string> FiringModesDisplay
        {
            get
            {
                List<string> displayNames = new List<string>();

                foreach (EFirearmFiringMode mode in FiringModes)
                {
                    string modeString = Regex.Replace(mode.ToString(), "([A-Z])", " $1").Trim();
                    displayNames.Add(modeString);
                }

                return displayNames;
            }

            private set
            {
                FiringModesDisplay = value;
            }
        }

        public EObjectCategory Category { get; set; }

        public ETagEra Era { get; set; }

        public string EraDisplay
        {
            get
            {
                if (Era == ETagEra.WW1)
                {
                    return "WW1";
                }
                else if (Era == ETagEra.WW2)
                {
                    return "WW2";
                }
                else
                {
                    return Regex.Replace(Era.ToString(), "([A-Z])", " $1").Trim();
                }
            }
            private set
            {
                EraDisplay = value;
            }
        }

        public ETagSet Set { get; set; }

        public string SetDisplay
        {
            get
            {
                return Regex.Replace(Set.ToString(), "([A-Z])", " $1").Trim();
            }
            private set
            {
                SetDisplay = value;
            }
        }

        public ETagFirearmSize FirearmSize { get; set; }

        public string FirearmSizeDisplay
        {
            get
            {
                return Regex.Replace(FirearmSize.ToString(), "([A-Z])", " $1").Trim();
            }
            private set
            {
                FirearmSizeDisplay = value;
            }
        }

        public ETagFirearmRoundPower FirearmRoundPower { get; set; }

        public string FirearmRoundPowerDisplay
        {
            get
            {
                if (FirearmRoundPower == ETagFirearmRoundPower.AntiMaterial)
                {
                    return "Anti-Material";
                }
                else
                {
                    return Regex.Replace(FirearmRoundPower.ToString(), "([A-Z])", " $1").Trim();
                }
            }
            private set
            {
                FirearmRoundPowerDisplay = value;
            }
        }

        public ETagFirearmCountryOfOrigin CountryOfOrigin { get; set; }

        public string CountryOfOriginDisplay
        {
            get
            {
                return Regex.Replace(CountryOfOrigin.ToString(), "([A-Z])", " $1").Trim();
            }
            private set
            {
                CountryOfOriginDisplay = value;
            }
        }

        public ETagFirearmAction FirearmAction { get; set; }

        public string FirearmActionDisplay
        {
            get
            {
                return Regex.Replace(FirearmAction.ToString(), "([A-Z])", " $1").Trim();
            }
            private set
            {
                FirearmActionDisplay = value;
            }
        }

        public bool UsesSpeedloader { get; set; }

    }
}
