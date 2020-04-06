// Decompiled with JetBrains decompiler
// Type: Victory.DataLayer.Serialization.CustomPaintTrans
// Assembly: gameplay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C6DD75A0-4EC1-4B51-B7B1-BD2D8767A6AB
// Assembly location: E:\NFSW\gameplay.dll

using System.Runtime.Serialization;

namespace Aaron.Data.XML
{
    [DataContract(Namespace = "")]
    public class CustomPaintTrans
    {
        [DataMember]
        public int Group;
        [DataMember]
        public int Hue;
        [DataMember]
        public int Sat;
        [DataMember]
        public int Slot;
        [DataMember]
        public int Var;
    }
}
