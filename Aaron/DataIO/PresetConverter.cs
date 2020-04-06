using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Data.XML;
using Aaron.Structures;
using Aaron.Utils;

namespace Aaron.DataIO
{
    /// <summary>
    /// Provides functions to convert between various preset car data formats.
    /// </summary>
    public static class PresetConverter
    {
        private static readonly Dictionary<int, uint> indexToSlotDictionary = new Dictionary<int, uint>()
        {
            { 49, 1469703691 },
            { 52, 2385239724 }, // SPECIAL_EFFECT
            { 53, 1403499614 }, // SPECIAL_EFFECT2
            { 54, 1403499615 }, // SPECIAL_EFFECT3
            { 55, 1403499616 }, // SPECIAL_EFFECT4
            { 56, 1403499617 }, // SPECIAL_EFFECT5
            { 57, 1403499618 }, // SPECIAL_EFFECT6
            { 58, 2645378418 },
            { 59, 1398141924 },
            { 51, 2779938817 },
            { 60, 3376116733 },
            { 61, 2168223373 },
            { 91, 1490249 },
            { 97, 2789436348 },
            { 99, 0x029A22F7 },
            { 100, 2106784967 },
            { 101, 1694991 },
            { 118, 453545749 },
            { 122, 3328879149 },
        };

        private static readonly Dictionary<uint, int> slotToIndexDictionary = new Dictionary<uint, int>()
        {
            { 0x5799E60B, 49 },
            { 0x8E2BDEAC, 52 },
            { 0x53A7B45E, 53 },
            { 0x53A7B45F, 54 },
            { 0x53A7B460, 55 },
            { 0x53A7B461, 56 },
            { 0x53A7B462, 57 },
            { 0x9DAD4572, 58 },
            { 0x5355F3E4, 59 },
            { 0xA5B28001, 51 },
            { 0xC93B73FD, 60 },
            { 0x813C768D, 61 },
            { 0x16BD49, 91 },
            { 0xA6436BBC, 97 },
            { 0x029A22F7, 99 },
            { 0x7D92FCC7, 100 },
            { 0x19DD0F, 101 },
            { 0x1B088F15, 118 },
            { 0xC66AAA2D, 122 }
        };

        /// <summary>
        /// Converts a <see cref="FEPresetCar"/> to a <see cref="AaronPresetCar"/>.
        /// </summary>
        /// <param name="fePresetCar">The <see cref="FEPresetCar"/> to be converted.</param>
        /// <returns></returns>
        public static AaronPresetCar ConvertFeToAaronPresetCar(FEPresetCar fePresetCar)
        {
            AaronPresetCar aaronPresetCar = new AaronPresetCar();
            aaronPresetCar.CarName = fePresetCar.CarCollectionName;
            aaronPresetCar.PresetName = fePresetCar.PresetName;

            aaronPresetCar.Paints = new SynchronizedObservableCollection<AaronPresetCarPaint>();
            aaronPresetCar.PerformanceParts = new SynchronizedObservableCollection<AaronPresetCarPerfPart>();
            aaronPresetCar.SkillModParts = new SynchronizedObservableCollection<AaronPresetCarSkill>();
            aaronPresetCar.Vinyls = new SynchronizedObservableCollection<AaronPresetCarVinyl>();
            aaronPresetCar.VisualParts = new SynchronizedObservableCollection<AaronPresetCarVisualPart>();

            aaronPresetCar.SkillModSlotCount = fePresetCar.SkillModSlotCount;

            for (int i = 0; i < fePresetCar.PaintsSet.Length; i++)
            {
                aaronPresetCar.Paints.Add(fePresetCar.PaintsSet[i]
                    ? convertFePaintToAaronPaint(fePresetCar.Paints[i])
                    : new AaronPresetCarPaint());
            }

            for (int i = 0; i < fePresetCar.PerformanceParts.Length && fePresetCar.PerformanceParts[i] != 0; i++)
            {
                aaronPresetCar.PerformanceParts.Add(new AaronPresetCarPerfPart
                {
                    Hash = fePresetCar.PerformanceParts[i]
                });
            }

            for (int i = 0; i < fePresetCar.SkillModHashes.Length && fePresetCar.SkillModHashes[i] != 0; i++)
            {
                aaronPresetCar.SkillModParts.Add(new AaronPresetCarSkill
                {
                    Hash = fePresetCar.SkillModHashes[i],
                    IsFixed = fePresetCar.SkillModsFixed[i]
                });
            }

            for (var index = 0; index < fePresetCar.VisualPartHashes.Length; index++)
            {
                var visualPartHash = fePresetCar.VisualPartHashes[index];
                if (visualPartHash != 0xFFFFFFFF)
                {
                    aaronPresetCar.VisualParts.Add(new AaronPresetCarVisualPart
                    {
                        PartHash = visualPartHash,
                        SlotHash = indexToSlotDictionary[index]
                    });
                }
            }

            foreach (var fePresetVinyl in fePresetCar.Vinyls)
            {
                if (fePresetVinyl.Hash == 0) break;
                aaronPresetCar.Vinyls.Add(convertFeVinylToAaronVinyl(fePresetVinyl));
            }

            return aaronPresetCar;
        }

        public static FEPresetCar ConvertAaronPresetToFEPreset(AaronPresetCar aaronPresetCar)
        {
            FEPresetCar presetCar = new FEPresetCar();
            presetCar.InheritedFields = new byte[8];
            presetCar.CarCollectionName = aaronPresetCar.CarName;
            presetCar.PresetName = aaronPresetCar.PresetName;
            presetCar.VehicleKey = ~Hashing.JenkinsHash(aaronPresetCar.CarName) * 0x57A5DEEB;
            presetCar.InverseThing = 0xA85A2115;
            presetCar.VisualPartHashes = new uint[123];
            presetCar.PaintsSet = new bool[8];
            presetCar.Paints = new FEPresetPaint[8];
            presetCar.Blank = new byte[0x64];
            presetCar.PerformanceParts = new uint[6];
            presetCar.SkillModSlotCount = aaronPresetCar.SkillModSlotCount;
            presetCar.SkillModHashes = new uint[6];
            presetCar.SkillModsFixed = new bool[8];
            presetCar.Vinyls = new FEPresetVinyl[31];
            presetCar.Blank4 = new byte[8];

            for (int i = 0; i < 123; i++)
            {
                presetCar.VisualPartHashes[i] = 0xFFFFFFFF;
            }

            foreach (var aaronPresetCarVisualPart in aaronPresetCar.VisualParts)
            {
                presetCar.VisualPartHashes[slotToIndexDictionary[aaronPresetCarVisualPart.SlotHash]] =
                    aaronPresetCarVisualPart.PartHash;
            }

            for (var index = 0; index < aaronPresetCar.Paints.Count; index++)
            {
                var aaronPresetCarPaint = aaronPresetCar.Paints[index];
                if (aaronPresetCarPaint == null || aaronPresetCarPaint.Group == 0)
                {
                    continue;
                }

                presetCar.PaintsSet[index] = true;
            }

            for (int i = 0; i < aaronPresetCar.Paints.Count; i++)
            {
                if (!presetCar.PaintsSet[i])
                {
                    presetCar.Paints[i] = new FEPresetPaint();
                }
                else
                {
                    presetCar.Paints[i] = new FEPresetPaint
                    {
                        Group = aaronPresetCar.Paints[i].Group,
                        Hue = aaronPresetCar.Paints[i].Hue,
                        Saturation = aaronPresetCar.Paints[i].Saturation,
                        Variance = aaronPresetCar.Paints[i].Variance,
                    };
                }
            }

            for (var index = 0; index < aaronPresetCar.PerformanceParts.Count; index++)
            {
                var aaronPresetCarPerfPart = aaronPresetCar.PerformanceParts[index];

                presetCar.PerformanceParts[index] = aaronPresetCarPerfPart.Hash;
            }

            for (var index = 0; index < aaronPresetCar.SkillModParts.Count; index++)
            {
                var aaronPresetCarSkill = aaronPresetCar.SkillModParts[index];

                presetCar.SkillModHashes[index] = aaronPresetCarSkill.Hash;
                presetCar.SkillModsFixed[index] = aaronPresetCarSkill.IsFixed;
            }

            for (var index = 0; index < aaronPresetCar.Vinyls.Count; index++)
            {
                var aaronPresetCarVinyl = aaronPresetCar.Vinyls[index];

                if (aaronPresetCarVinyl.Hash == 0)
                {
                    presetCar.Vinyls[index] = new FEPresetVinyl();
                }
                else
                {
                    presetCar.Vinyls[index] = convertAaronVinylToFE(aaronPresetCarVinyl);
                }
            }

            return presetCar;
        }

        private static FEPresetVinyl convertAaronVinylToFE(AaronPresetCarVinyl aaronPresetCarVinyl)
        {
            FEPresetVinyl fePresetVinyl = new FEPresetVinyl();
            fePresetVinyl.Hash = unchecked((int)aaronPresetCarVinyl.Hash);
            fePresetVinyl.Hues = new FEPresetVinylHue[4];
            fePresetVinyl.IsMirrored = aaronPresetCarVinyl.IsMirrored;
            fePresetVinyl.Rotation = aaronPresetCarVinyl.Rotation;

            if (aaronPresetCarVinyl.Shear * 2 > byte.MaxValue)
            {
                throw new InvalidDataException("invalid shear");
            }

            fePresetVinyl.Shear = (byte)(aaronPresetCarVinyl.Shear * 2);
            fePresetVinyl.ScaleX = aaronPresetCarVinyl.ScaleX;
            fePresetVinyl.ScaleY = aaronPresetCarVinyl.ScaleY;
            fePresetVinyl.TranX = aaronPresetCarVinyl.TranX;
            fePresetVinyl.TranY = aaronPresetCarVinyl.TranY;

            fePresetVinyl.Hues[0] = new FEPresetVinylHue();
            fePresetVinyl.Hues[1] = new FEPresetVinylHue();
            fePresetVinyl.Hues[2] = new FEPresetVinylHue();
            fePresetVinyl.Hues[3] = new FEPresetVinylHue();

            for (int i = 0; i < 4; i++)
            {
                fePresetVinyl.Hues[i] = new FEPresetVinylHue();

                if (aaronPresetCarVinyl.Hues[i] != null)
                {
                    fePresetVinyl.Hues[i].Hue = unchecked((int)aaronPresetCarVinyl.Hues[i].Hue);
                    fePresetVinyl.Hues[i].Saturation = aaronPresetCarVinyl.Hues[i].Saturation;
                    fePresetVinyl.Hues[i].Variance = aaronPresetCarVinyl.Hues[i].Variance;
                }
            }

            return fePresetVinyl;
        }

        /// <summary>
        /// Converts a <see cref="AaronPresetCar"/> to a <see cref="OwnedCarTrans"/>
        /// </summary>
        /// <param name="aaronPresetCar"></param>
        /// <returns></returns>
        public static OwnedCarTrans ConvertAaronPresetToServerXML(AaronPresetCar aaronPresetCar)
        {
            OwnedCarTrans ownedCarTrans = new OwnedCarTrans();
            ownedCarTrans.ExpirationDate = DateTime.FromBinary(0);
            ownedCarTrans.OwnershipType = "PresetCar";
            ownedCarTrans.Id = 0;
            ownedCarTrans.Durability = 0;
            ownedCarTrans.CustomCar = new CustomCarTrans();

            // setup customcar
            ownedCarTrans.CustomCar.Name = aaronPresetCar.CarName;
            ownedCarTrans.CustomCar.BaseCar = unchecked((int)Hashing.BinHash(aaronPresetCar.CarName.ToUpperInvariant()));
            ownedCarTrans.CustomCar.IsPreset = true;
            ownedCarTrans.CustomCar.PhysicsProfileHash = unchecked((int)Hashing.JenkinsHash(aaronPresetCar.CarName));

            ownedCarTrans.CustomCar.Paints = new List<CustomPaintTrans>();
            ownedCarTrans.CustomCar.PerformanceParts = new List<PerformancePartTrans>();
            ownedCarTrans.CustomCar.SkillModParts = new List<SkillModPartTrans>();
            ownedCarTrans.CustomCar.Vinyls = new List<CustomVinylTrans>();
            ownedCarTrans.CustomCar.VisualParts = new List<VisualPartTrans>();

            ownedCarTrans.CustomCar.SkillModSlotCount = (int)aaronPresetCar.SkillModSlotCount;

            for (var index = 0; index < aaronPresetCar.Paints.Count; index++)
            {
                var aaronPresetCarPaint = aaronPresetCar.Paints[index];

                if (aaronPresetCarPaint == null) continue;

                var paint = convertAaronPaintToXML(aaronPresetCarPaint, index);

                ownedCarTrans.CustomCar.Paints.Add(paint);
            }

            foreach (var aaronPresetCarPart in aaronPresetCar.SkillModParts)
            {
                ownedCarTrans.CustomCar.SkillModParts.Add(new SkillModPartTrans
                {
                    IsFixed = false,
                    SkillModPartAttribHash = unchecked((int)aaronPresetCarPart.Hash)
                });
            }

            foreach (var aaronPresetCarVisualPart in aaronPresetCar.VisualParts)
            {
                ownedCarTrans.CustomCar.VisualParts.Add(new VisualPartTrans
                {
                    PartHash = unchecked((int)aaronPresetCarVisualPart.PartHash),
                    SlotHash = unchecked((int)aaronPresetCarVisualPart.SlotHash),
                });
            }

            foreach (var aaronPresetCarPerfPart in aaronPresetCar.PerformanceParts)
            {
                ownedCarTrans.CustomCar.PerformanceParts.Add(new PerformancePartTrans
                {
                    PerformancePartAttribHash = unchecked((int)aaronPresetCarPerfPart.Hash)
                });
            }

            for (var index = 0; index < aaronPresetCar.Vinyls.Count; index++)
            {
                var aaronPresetCarVinyl = aaronPresetCar.Vinyls[index];

                if (aaronPresetCarVinyl == null) continue;

                var convertAaronVinylToXml = convertAaronVinylToXML(aaronPresetCarVinyl);
                convertAaronVinylToXml.Layer = index;

                ownedCarTrans.CustomCar.Vinyls.Add(convertAaronVinylToXml);
            }

            return ownedCarTrans;
        }

        public static AaronPresetCar ConvertServerXMLToAaronPreset(OwnedCarTrans ownedCarTrans)
        {
            AaronPresetCar aaronPresetCar = new AaronPresetCar();

            aaronPresetCar.CarName = ownedCarTrans.CustomCar.Name;
            aaronPresetCar.Paints = new SynchronizedObservableCollection<AaronPresetCarPaint>();
            aaronPresetCar.PerformanceParts = new SynchronizedObservableCollection<AaronPresetCarPerfPart>();
            aaronPresetCar.SkillModParts = new SynchronizedObservableCollection<AaronPresetCarSkill>();
            aaronPresetCar.Vinyls = new SynchronizedObservableCollection<AaronPresetCarVinyl>();
            aaronPresetCar.VisualParts = new SynchronizedObservableCollection<AaronPresetCarVisualPart>();
            aaronPresetCar.SkillModSlotCount = (uint)ownedCarTrans.CustomCar.SkillModSlotCount;

            for (int i = 0; i < 8; i++)
            {
                aaronPresetCar.Paints.Add(new AaronPresetCarPaint());
            }

            for (int i = 0; i < 31; i++)
            {
                aaronPresetCar.Vinyls.Add(new AaronPresetCarVinyl());
            }

            foreach (var paint in ownedCarTrans.CustomCar.Paints)
            {
                aaronPresetCar.Paints[paint.Slot] =
                    convertXMLPaintToAaron(paint);
            }

            foreach (var skillModPart in ownedCarTrans.CustomCar.SkillModParts)
            {
                aaronPresetCar.SkillModParts.Add(new AaronPresetCarSkill
                {
                    Hash = unchecked((uint)skillModPart.SkillModPartAttribHash),
                    IsFixed = skillModPart.IsFixed
                });
            }

            foreach (var performancePart in ownedCarTrans.CustomCar.PerformanceParts)
            {
                aaronPresetCar.PerformanceParts.Add(new AaronPresetCarPerfPart
                {
                    Hash = unchecked((uint)performancePart.PerformancePartAttribHash)
                });
            }

            foreach (var customCarVisualPart in ownedCarTrans.CustomCar.VisualParts)
            {
                aaronPresetCar.VisualParts.Add(new AaronPresetCarVisualPart
                {
                    PartHash = unchecked((uint)customCarVisualPart.PartHash),
                    SlotHash = unchecked((uint)customCarVisualPart.SlotHash)
                });
            }

            foreach (var customVinyl in ownedCarTrans.CustomCar.Vinyls)
            {
                aaronPresetCar.Vinyls[customVinyl.Layer] = convertXMLVinylToAaron(customVinyl);
            }

            return aaronPresetCar;
        }

        private static AaronPresetCarVinyl convertXMLVinylToAaron(CustomVinylTrans customVinyl)
        {
            return new AaronPresetCarVinyl
            {
                Hash = unchecked((uint)customVinyl.Hash),
                IsMirrored = customVinyl.Mir,
                Rotation = (byte)customVinyl.Rot,
                ScaleX = (short)customVinyl.ScaleX,
                ScaleY = (short)customVinyl.ScaleY,
                Shear = (byte)customVinyl.Shear,
                TranX = (short)customVinyl.TranX,
                TranY = (short)customVinyl.TranY,
                Hues = new AaronPresetCarVinylHue[]
                {
                    new AaronPresetCarVinylHue
                    {
                        Hue = unchecked((uint) customVinyl.Hue1), Saturation = (byte) customVinyl.Sat1,
                        Variance = (byte) customVinyl.Var1
                    },
                    new AaronPresetCarVinylHue
                    {
                        Hue = unchecked((uint) customVinyl.Hue2), Saturation = (byte) customVinyl.Sat2,
                        Variance = (byte) customVinyl.Var2
                    },
                    new AaronPresetCarVinylHue
                    {
                        Hue = unchecked((uint) customVinyl.Hue3), Saturation = (byte) customVinyl.Sat3,
                        Variance = (byte) customVinyl.Var3
                    },
                    new AaronPresetCarVinylHue
                    {
                        Hue = unchecked((uint) customVinyl.Hue4), Saturation = (byte) customVinyl.Sat4,
                        Variance = (byte) customVinyl.Var4
                    }
                }
            };
        }

        private static AaronPresetCarPaint convertXMLPaintToAaron(CustomPaintTrans customCarPaint)
        {
            var paint = new AaronPresetCarPaint
            {
                Group = unchecked((uint)customCarPaint.Group),
                Hue = unchecked((uint)customCarPaint.Hue),
                Saturation = (byte)customCarPaint.Sat,
                Variance = (byte)customCarPaint.Var
            };

            return paint;
        }

        private static CustomVinylTrans convertAaronVinylToXML(AaronPresetCarVinyl aaronPresetCarVinyl)
        {
            CustomVinylTrans customVinylTrans = new CustomVinylTrans();
            customVinylTrans.Hash = unchecked((int)aaronPresetCarVinyl.Hash);

            customVinylTrans.Hue1 = unchecked((int)aaronPresetCarVinyl.Hues[0].Hue);
            customVinylTrans.Hue2 = unchecked((int)aaronPresetCarVinyl.Hues[1].Hue);
            customVinylTrans.Hue3 = unchecked((int)aaronPresetCarVinyl.Hues[2].Hue);
            customVinylTrans.Hue4 = unchecked((int)aaronPresetCarVinyl.Hues[3].Hue);

            customVinylTrans.Sat1 = aaronPresetCarVinyl.Hues[0].Saturation;
            customVinylTrans.Sat2 = aaronPresetCarVinyl.Hues[1].Saturation;
            customVinylTrans.Sat3 = aaronPresetCarVinyl.Hues[2].Saturation;
            customVinylTrans.Sat4 = aaronPresetCarVinyl.Hues[3].Saturation;

            customVinylTrans.Var1 = aaronPresetCarVinyl.Hues[0].Variance;
            customVinylTrans.Var2 = aaronPresetCarVinyl.Hues[1].Variance;
            customVinylTrans.Var3 = aaronPresetCarVinyl.Hues[2].Variance;
            customVinylTrans.Var4 = aaronPresetCarVinyl.Hues[3].Variance;

            customVinylTrans.Shear = aaronPresetCarVinyl.Shear;
            customVinylTrans.Rot = aaronPresetCarVinyl.Rotation;
            customVinylTrans.Mir = aaronPresetCarVinyl.IsMirrored;
            customVinylTrans.ScaleX = aaronPresetCarVinyl.ScaleX;
            customVinylTrans.ScaleY = aaronPresetCarVinyl.ScaleY;
            customVinylTrans.TranX = aaronPresetCarVinyl.TranX;
            customVinylTrans.TranY = aaronPresetCarVinyl.TranY;

            return customVinylTrans;
        }

        private static CustomPaintTrans convertAaronPaintToXML(AaronPresetCarPaint aaronPresetCarPaint, int index)
        {
            var paint = new CustomPaintTrans();
            paint.Group = unchecked((int)aaronPresetCarPaint.Group);
            paint.Hue = unchecked((int)aaronPresetCarPaint.Hue);
            paint.Slot = index;
            paint.Sat = aaronPresetCarPaint.Saturation;
            paint.Var = aaronPresetCarPaint.Variance;
            return paint;
        }

        private static AaronPresetCarVinyl convertFeVinylToAaronVinyl(FEPresetVinyl fePresetVinyl)
        {
            AaronPresetCarVinyl vinyl = new AaronPresetCarVinyl();
            vinyl.Shear = (byte)(fePresetVinyl.Shear / 2);
            vinyl.Hash = unchecked((uint)fePresetVinyl.Hash);
            vinyl.IsMirrored = fePresetVinyl.IsMirrored;
            vinyl.Rotation = fePresetVinyl.Rotation;
            vinyl.ScaleX = fePresetVinyl.ScaleX;
            vinyl.ScaleY = fePresetVinyl.ScaleY;
            vinyl.TranX = fePresetVinyl.TranX;
            vinyl.TranY = fePresetVinyl.TranY;
            vinyl.Hues = new AaronPresetCarVinylHue[fePresetVinyl.Hues.Length];

            for (var index = 0; index < fePresetVinyl.Hues.Length; index++)
            {
                var fePresetVinylHue = fePresetVinyl.Hues[index];

                vinyl.Hues[index] = new AaronPresetCarVinylHue();
                vinyl.Hues[index].Hue = unchecked((uint)fePresetVinylHue.Hue);
                vinyl.Hues[index].Saturation = fePresetVinylHue.Saturation;
                vinyl.Hues[index].Variance = fePresetVinylHue.Variance;
            }

            return vinyl;
        }

        private static AaronPresetCarPaint convertFePaintToAaronPaint(FEPresetPaint fePresetPaint)
        {
            return new AaronPresetCarPaint
            {
                Group = fePresetPaint.Group,
                Hue = fePresetPaint.Hue,
                Saturation = fePresetPaint.Saturation,
                Variance = fePresetPaint.Variance
            };
        }
    }
}
