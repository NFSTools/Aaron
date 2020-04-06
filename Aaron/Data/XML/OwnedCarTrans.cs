// Decompiled with JetBrains decompiler
// Type: Victory.DataLayer.Serialization.OwnedCarTrans
// Assembly: gameplay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C6DD75A0-4EC1-4B51-B7B1-BD2D8767A6AB
// Assembly location: E:\NFSW\gameplay.dll

using System;
using System.Runtime.Serialization;

namespace Aaron.Data.XML
{
    [DataContract(Namespace = "")]
    public class OwnedCarTrans
    {
        [DataMember]
        public DateTime ExpirationDate = DateTime.FromBinary(0L);
        [DataMember]
        public string OwnershipType = "";
        [DataMember]
        public CustomCarTrans CustomCar;
        [DataMember]
        public int Durability;
        [DataMember]
        public float Heat;
        [DataMember]
        public long Id;
    }
}
