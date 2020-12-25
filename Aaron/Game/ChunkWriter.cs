using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Aaron.Utils;

namespace Aaron.Game
{
    /// <summary>
    /// Writes data chunks to a stream.
    /// </summary>
    public abstract class ChunkWriter : IDisposable
    {
        private readonly bool _compress;
        protected Stream Stream;
        protected BinaryWriter Writer;

        private readonly Stack<Chunk> _chunks = new Stack<Chunk>();

        protected ChunkWriter(Stream stream, bool compress = true)
        {
            _compress = compress;
            SetStream(stream);
        }

        protected ChunkWriter(string file)
        {
            SetStream(File.Open(file, FileMode.Create, FileAccess.ReadWrite));
        }

        public void DoWrite(IProgress<string> progress = null)
        {
            var origStream = Stream;

            SetStream(new MemoryStream());
            Write(progress);

            progress?.Report("Post processing: Compressing file...");
            Stream.Position = 0;
            //Stream.CopyTo(origStream);

            MemoryStream ms = (MemoryStream)this.Stream;

            if (_compress)
            {
                BlockCompression.WriteBlockFile(origStream, ms.ToArray());
            }
            else
            {
                ms.CopyTo(origStream);
            }

            this.Stream = origStream;

            //File.WriteAllBytes("GlobalC_gen.bin", ms.ToArray());
            //BlockCompression.WriteBlockFile(origStream, ms.ToArray());
        }

        protected abstract void Write(IProgress<string> progress = null);

        protected Chunk BeginChunk(uint type)
        {
            var chunk = new Chunk();
            chunk.Offset = Writer.BaseStream.Position;
            chunk.Type = type;

            chunk.Write(Writer);

            _chunks.Push(chunk);

            return chunk;
        }

        protected void EndChunk()
        {
            Chunk chunk = _chunks.Pop();

            chunk.Size = (uint)(Writer.BaseStream.Position - (chunk.Offset + 8));

            Writer.BaseStream.Position = chunk.Offset;
            chunk.Write(Writer);
            Writer.BaseStream.Position = chunk.EndOffset;
        }

        protected void SetStream(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Writer = new BinaryWriter(Stream);
        }

        protected void NextAlignment(int align)
        {
            if (Stream.Position % align != 0)
            {
                var newPos = Stream.Position + (align - (Stream.Position % align));

                Stream.Position = newPos;
                Stream.SetLength(Math.Max(Stream.Length, newPos));
            }
        }


        protected void DifferenceAlignment(int alignment)
        {
            if ((Stream.Position - alignment) % 0x10 != 0)
            {
                BeginChunk(0);

                if ((Stream.Position - alignment) % 0x10 != 0)
                {
                    Stream.Write(
                        new byte[(Stream.Position - alignment) % 0x10],
                        0,
                        (int)((Stream.Position - alignment) % 0x10));
                }

                EndChunk();
            }
        }

        protected void PaddingAlignment(int alignment)
        {
            if (Stream.Position % alignment != 0)
            {
                //Debug.WriteLine("stream @ {0:X}, requested alignment {1}, mod = {2}",
                //    _stream.Position, alignment, _stream.Position % alignment);

                BeginChunk(0);

                if (Stream.Position % alignment != 0)
                {
                    //Debug.WriteLine("\tseek: {0}", alignment - _stream.Position % alignment);

                    Stream.Write(
                        new byte[alignment - Stream.Position % alignment],
                        0,
                        (int)(alignment - Stream.Position % alignment));
                    //_stream.Seek(alignment - _stream.Position % alignment, SeekOrigin.Current);
                }

                EndChunk();
            }
        }

        public void Dispose()
        {
            Stream?.Dispose();
            Writer?.Dispose();
        }
    }
}