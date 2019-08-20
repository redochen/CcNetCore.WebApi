using System.Security.Cryptography;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 提供CRC32算法的实现
    /// </summary>
    public class CRC32 : HashAlgorithm {
        public const uint DefaultPolynomial = 0xedb88320;
        public const uint DefaultSeed = 0xffffffff;
        private uint hash;
        private uint seed;
        private uint[] table;
        private static uint[] defaultTable;

        protected CRC32 () {
            table = InitializeTable (DefaultPolynomial);
            seed = DefaultSeed;
            Initialize ();
        }

        protected CRC32 (uint polynomial, uint seed) {
            table = InitializeTable (polynomial);
            this.seed = seed;
            Initialize ();
        }

        public new static CRC32 Create () => new CRC32 ();

        public static CRC32 Create (uint polynomial, uint seed) => new CRC32 (polynomial, seed);

        public override void Initialize () {
            hash = seed;
        }

        protected override void HashCore (byte[] buffer, int start, int length) {
            hash = CalculateHash (table, hash, buffer, start, length);
        }

        protected override byte[] HashFinal () {
            byte[] hashBuffer = UInt32ToBigEndianBytes (~hash);
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        public static uint Compute (byte[] buffer) {
            return ~CalculateHash (InitializeTable (DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        public static uint Compute (uint seed, byte[] buffer) {
            return ~CalculateHash (InitializeTable (DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        public static uint Compute (uint polynomial, uint seed, byte[] buffer) {
            return ~CalculateHash (InitializeTable (polynomial), seed, buffer, 0, buffer.Length);
        }

        private static uint[] InitializeTable (uint polynomial) {
            if (polynomial == DefaultPolynomial && defaultTable != null) {
                return defaultTable;
            }

            uint[] createTable = new uint[256];
            for (int i = 0; i < 256; i++) {
                uint entry = (uint) i;
                for (int j = 0; j < 8; j++) {
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                }
                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial) {
                defaultTable = createTable;
            }

            return createTable;
        }

        private static uint CalculateHash (uint[] table, uint seed, byte[] buffer, int start, int size) {
            uint crc = seed;
            for (int i = start; i < size; i++) {
                unchecked {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            }
            return crc;
        }

        private byte[] UInt32ToBigEndianBytes (uint x) {
            return new byte[] {
                (byte) ((x >> 24) & 0xff),
                (byte) ((x >> 16) & 0xff),
                (byte) ((x >> 8) & 0xff),
                (byte) (x & 0xff)
            };
        }
    }
}