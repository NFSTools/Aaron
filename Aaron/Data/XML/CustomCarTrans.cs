// Decompiled with JetBrains decompiler
// Type: Victory.DataLayer.Serialization.CustomCarTrans
// Assembly: gameplay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C6DD75A0-4EC1-4B51-B7B1-BD2D8767A6AB
// Assembly location: E:\NFSW\gameplay.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aaron.Data.XML
{
    [DataContract(Namespace = "")]
    public class CustomCarTrans
    {
        [DataMember]
        public string Name = "";
        [DataMember]
        public int BaseCar;
        [DataMember]
        public int CarClassHash;
        [DataMember]
        public int Id;
        [DataMember]
        public bool IsPreset;
        [DataMember]
        public int Level;
        [DataMember]
        public List<CustomPaintTrans> Paints;
        [DataMember]
        public List<PerformancePartTrans> PerformanceParts;
        [DataMember]
        public int PhysicsProfileHash;
        [DataMember]
        public int Rating;
        [DataMember]
        public float ResalePrice;
        [DataMember]
        public float RideHeightDrop;
        [DataMember]
        public List<SkillModPartTrans> SkillModParts;
        [DataMember]
        public int SkillModSlotCount;
        [DataMember]
        public int Version;
        [DataMember]
        public List<CustomVinylTrans> Vinyls;
        [DataMember]
        public List<VisualPartTrans> VisualParts;
    }
}
