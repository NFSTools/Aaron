using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Utils
{
    public static class BinaryHelpers
    {
        public static void WriteAlignedString(this BinaryWriter bw, string str)
        {
            var strBytes = Encoding.GetEncoding(1252).GetBytes(str);
            Array.Resize(ref strBytes, strBytes.Length + 1);
            strBytes[strBytes.Length - 1] = 0;

            bw.Write(strBytes);

            if (bw.BaseStream.Position % 4 != 0)
            {
                bw.BaseStream.Position += 4 - bw.BaseStream.Position % 4;
            }
        }

        public static string ReadNullTerminatedString(this BinaryReader br)
        {
            return br.BaseStream.ReadNullTerminatedString();
        }

        public static string ReadAlignedString(this BinaryReader br)
        {
            return br.BaseStream.ReadAlignedString();
        }

        public static string ReadNullTerminatedString(this Stream stream)
        {
            var str = "";
            char c;

            while ((c = (char)stream.ReadByte()) != '\0')
            {
                str += c;
            }

            return str;
        }

        public static string ReadFixedLengthString(this BinaryReader br, int length)
        {
            return new string(br.ReadChars(length)).Trim('\0');
        }

        public static string ReadAlignedString(this Stream stream)
        {
            var str = stream.ReadNullTerminatedString();

            if (stream.Position % 4 != 0)
            {
                stream.Position += 4 - stream.Position % 4;
            }

            return str;
        }


        /// <summary>
        /// Align the given binary data reader to the given boundary.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="align"></param>
        public static long AlignToBoundary(this BinaryReader br, long align)
        {
            if (br.BaseStream.Position % align != 0)
            {
                var diff = align - br.BaseStream.Position % align;
                br.BaseStream.Position += diff;
                return diff;
            }

            return 0;
        }

        /// <summary>
        /// Align the given binary data writer to the given boundary.
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="align"></param>
        public static long Align(this BinaryWriter bw, long align)
        {
            return bw.BaseStream.Align(align);
        }

        public static long Align(this Stream stream, long align)
        {
            if (stream.Position % align != 0)
            {
                //var buffer = new byte[align - stream.Position % align];
                //stream.Write(buffer, 0, buffer.Length);
                var result = align - stream.Position % align;
                stream.Position += result;

                if (stream.Length < stream.Position)
                {
                    stream.SetLength(stream.Length + (stream.Position - stream.Length));
                }

                return result;
            }

            return 0;
        }

        public static long SpecialAlign(this Stream stream, long boundary)
        {
            if ((stream.Position - boundary) % 0x10 != 0)
            {
                var align = 0x10 - ((stream.Position - boundary) % 0x10);

                stream.Position += align;

                return align;
            }

            return 0;
        }

        public static byte[] ReadBytesRequired(this BinaryReader br, int count)
        {
            byte[] b = br.ReadBytes(count);

            if (b.Length != count)
            {
                throw new InvalidDataException("Failed to read " + count + " bytes, got " + b.Length);
            }

            return b;
        }


        static byte[] GetBytes<T>(T obj) where T : struct
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Read a structure from a binary stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadStruct<T>(BinaryReader reader, int size = 0)
        {
            var bytes = reader.ReadBytesRequired(size == 0 ? Marshal.SizeOf(typeof(T)) : size);
            //var bytes = new byte[Marshal.SizeOf<T>()];
            //reader.Read(bytes, 0, size == 0 ? bytes.Length : size);

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        /// <summary>
        /// Read a structure from a  stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadStruct<T>(Stream stream, int size = 0)
        {
            byte[] data = new byte[size == 0 ? Marshal.SizeOf(typeof(T)) : size];

            if (stream.Read(data, 0, data.Length) != data.Length)
            {
                throw new InvalidDataException("not enough data in stream");
            }

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static object ReadStruct(Type type, BinaryReader reader, int size = 0)
        {
            //var bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            var bytes = new byte[Marshal.SizeOf(type)];
            reader.Read(bytes, 0, size == 0 ? bytes.Length : size);

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var theStructure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
            handle.Free();

            return theStructure;
        }

        /// <summary>
        /// Write a structure to a binary stream.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void WriteStruct<T>(BinaryWriter writer, T instance) where T : struct
        {
            writer.Write(GetBytes(instance));
        }
    }
}
