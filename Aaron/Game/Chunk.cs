using System.IO;

namespace Aaron.Game
{
    public class Chunk
    {
        public uint Type { get; set; }
        public uint Size { get;  set; }
        public long Offset { get;  set; }
        public long EndOffset => Offset + 8 + Size;

        public void Read(BinaryReader br)
        {
            Offset = br.BaseStream.Position;
            Type = br.ReadUInt32();
            Size = br.ReadUInt32();
        }

        public void Write(BinaryWriter bw)
        {
            Offset = bw.BaseStream.Position;
            bw.Write(Type);
            bw.Write(Size);
        }
    }
}