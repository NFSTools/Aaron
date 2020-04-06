using System;
using System.Diagnostics;
using System.IO;

namespace Aaron.Game
{
    /// <summary>
    /// Reads data chunks from a stream.
    /// </summary>
    public abstract class ChunkReader : IDisposable
    {
        protected Stream Stream;
        protected BinaryReader Reader;
        protected IProgress<string> Progress;

        protected ChunkReader(Stream stream)
        {
            SetStream(stream);
        }

        protected ChunkReader(string file)
        {
            SetStream(File.OpenRead(file));
        }

        /// <summary>
        /// Read all chunks from the stream.
        /// </summary>
        public void Read(IProgress<string> progress)
        {
            this.Progress = progress;
            PrepareStream();
            readChunks(Stream.Length);
        }

        /// <summary>
        /// Perform any necessary pre-read work.
        /// </summary>
        protected abstract void PrepareStream();

        /// <summary>
        /// Process a chunk that was read from the stream.
        /// </summary>
        /// <param name="chunk">The chunk that was read.</param>
        protected abstract void ProcessChunk(Chunk chunk);

        protected void SetStream(Stream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Reader = new BinaryReader(Stream);
        }

        private void readChunks(long length)
        {
            var endPos = Stream.Position + length;
            Debug.Assert(endPos >= Stream.Position && endPos <= Stream.Length);

            while (Stream.Position < endPos)
            {
                var chunk = new Chunk();
                chunk.Read(Reader);

                Debug.Assert(Stream.Position + chunk.Size <= endPos);

                if (chunk.Type != 0)
                {
                    if ((chunk.Type & 0x80000000) == 0x80000000)
                    {
                        this.readChunks(chunk.Size);
                    }
                    else
                    {
                        this.ProcessChunk(chunk);
                    }
                }

                Stream.Position = chunk.Offset + 8 + chunk.Size;
            }
        }

        public void Dispose()
        {
            Stream?.Dispose();
            Reader?.Dispose();
        }
    }
}