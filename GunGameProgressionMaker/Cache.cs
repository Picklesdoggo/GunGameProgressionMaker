using System.Collections.Generic;

namespace GunGameProgressionMaker
{
    public class Cache
    {
        public List<string> Firearms { get; set; }
        public List<string> Magazines { get; set; }
        public List<string> Clips { get; set; }
        public List<string> SpeedLoaders { get; set; }
        public List<string> Bullets { get; set; }
        public List<Entry> Entries { get; set; }
        public List<AmmoObject> AmmoObjects { get; set; }
        public List<Magazinedata> MagazineData { get; set; }
        public List<ClipData> ClipData { get; set; }
        public List<SpeedloaderData> SpeedLoaderData { get; set; }
        public List<BulletData> BulletData { get; set; }
    }

    public class Entry
    {
        public string FirearmID { get; set; }
        public int MagType { get; set; }
        public int ClipType { get; set; }
        public int BulletType { get; set; }
        public bool DoesUseSpeedloader { get; set; }
        public List<string> CompatibleMagazines { get; set; }
        public List<string> CompatibleClips { get; set; }
        public List<string> CompatibleSpeedLoaders { get; set; }
        public List<string> CompatibleBullets { get; set; }
    }

    public class AmmoObject
    {
        public string ObjectID { get; set; }
        public int Capacity { get; set; }
        public int MagType { get; set; }
        public int RoundType { get; set; }
    }

    public class Magazinedata
    {
        public string ObjectID { get; set; }
        public int Capacity { get; set; }
        public int MagType { get; set; }
        public int RoundType { get; set; }
    }

    public class ClipData
    {
        public string ObjectID { get; set; }
        public int Capacity { get; set; }
        public int MagType { get; set; }
        public int RoundType { get; set; }
    }

    public class SpeedloaderData
    {
        public string ObjectID { get; set; }
        public int Capacity { get; set; }
        public int MagType { get; set; }
        public int RoundType { get; set; }
    }

    public class BulletData
    {
        public string ObjectID { get; set; }
        public int Capacity { get; set; }
        public int MagType { get; set; }
        public int RoundType { get; set; }
    }
}

