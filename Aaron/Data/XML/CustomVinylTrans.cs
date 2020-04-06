// Decompiled with JetBrains decompiler
// Type: Victory.DataLayer.Serialization.CustomVinylTrans
// Assembly: gameplay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C6DD75A0-4EC1-4B51-B7B1-BD2D8767A6AB
// Assembly location: E:\NFSW\gameplay.dll

using System.Runtime.Serialization;

namespace Aaron.Data.XML
{
    [DataContract(Namespace = "")]
    public class CustomVinylTrans
    {
        [DataMember]
        public int Hash;
        [DataMember]
        public int Hue1;
        [DataMember]
        public int Hue2;
        [DataMember]
        public int Hue3;
        [DataMember]
        public int Hue4;
        [DataMember]
        public int Layer;
        [DataMember]
        public bool Mir;
        [DataMember]
        public int Rot;
        [DataMember]
        public int Sat1;
        [DataMember]
        public int Sat2;
        [DataMember]
        public int Sat3;
        [DataMember]
        public int Sat4;
        [DataMember]
        public int ScaleX;
        [DataMember]
        public int ScaleY;
        [DataMember]
        public int Shear;
        [DataMember]
        public int TranX;
        [DataMember]
        public int TranY;
        [DataMember]
        public int Var1;
        [DataMember]
        public int Var2;
        [DataMember]
        public int Var3;
        [DataMember]
        public int Var4;
    }
}
