﻿namespace xxHash.Lib
{
    public static class xxHash64
    {
        private const ulong p1 = 11400714785074694791UL;
        private const ulong p2 = 14029467366897019727UL;
        private const ulong p3 =  1609587929392839161UL;
        private const ulong p4 =  9650029242287828579UL;
        private const ulong p5 =  2870177450012600261UL;

        public static unsafe ulong ComputeHash(byte[] data, int len, ulong seed = 0)
        {
            fixed (byte* pData = &data[0])
            {
                ulong* ptr = (ulong*) pData;
                byte* end = pData + len;
                ulong h64;

                if (len >= 32)
                {
                    byte* limit = end - 32;

                    ulong v1 = seed + p1 + p2;
                    ulong v2 = seed + p2;
                    ulong v3 = seed + 0;
                    ulong v4 = seed - p1;

                    do
                    {
                        v1 += ptr[0] * p2;
                        v1 = (v1 << 31) | (v1 >> (64 - 31)); // rotl 31
                        v1 *= p1;

                        v2 += ptr[1] * p2;
                        v2 = (v2 << 31) | (v2 >> (64 - 31)); // rotl 31
                        v2 *= p1;

                        v3 += ptr[2] * p2;
                        v3 = (v3 << 31) | (v3 >> (64 - 31)); // rotl 31
                        v3 *= p1;

                        v4 += ptr[3] * p2;
                        v4 = (v4 << 31) | (v4 >> (64 - 31)); // rotl 31
                        v4 *= p1;

                        ptr += 4;

                    } while (ptr <= limit);

                    h64 = ((v1 << 1) | (v1 >> (64 - 1))) +   // rotl 1
                          ((v2 << 7) | (v2 >> (64 - 7))) +   // rotl 7
                          ((v3 << 12) | (v3 >> (64 - 12))) + // rotl 12
                          ((v4 << 18) | (v4 >> (64 - 18)));  // rotl 18

                    // merge round
                    v1 *= p2;
                    v1 = (v1 << 31) | (v1 >> (64 - 31)); // rotl 31
                    v1 *= p1;
                    h64 ^= v1;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v2 *= p2;
                    v2 = (v2 << 31) | (v2 >> (64 - 31)); // rotl 31
                    v2 *= p1;
                    h64 ^= v2;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v3 *= p2;
                    v3 = (v3 << 31) | (v3 >> (64 - 31)); // rotl 31
                    v3 *= p1;
                    h64 ^= v3;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v4 *= p2;
                    v4 = (v4 << 31) | (v4 >> (64 - 31)); // rotl 31
                    v4 *= p1;
                    h64 ^= v4;
                    h64 = h64 * p1 + p4;
                }
                else
                {
                    h64 = seed + p5;
                }

                h64 += (ulong) len;

                // finalize
                while (ptr <= end - 8)
                {
                    ulong t1 = ptr[0] * p2;
                    t1 = (t1 << 31) | (t1 >> (64 - 31)); // rotl 31
                    t1 *= p1;
                    h64 ^= t1;
                    h64 = ((h64 << 27) | (h64 >> (64 - 27))) * p1 + p4; // (rotl 27) * p1 + p4
                    ptr += 1;
                }

                uint* l32 = (uint*) ptr;
                if (l32 <= end - 4)
                {
                    h64 ^= l32[0] * p1;
                    h64 = ((h64 << 23) | (h64 >> (64 - 23))) * p2 + p3; // (rotl 27) * p2 + p3
                    l32 += 1;
                }

                byte* lst = (byte*)l32;
                while (lst < end)
                {
                    h64 ^= lst[0] * p5;
                    h64 = ((h64 << 11) | (h64 >> (64 - 11))) * p1; // (rotl 11) * p1
                    lst += 1;
                }

                // avalanche
                h64 ^= h64 >> 33;
                h64 *= p2;
                h64 ^= h64 >> 29;
                h64 *= p3;
                h64 ^= h64 >> 32;

                return h64;
            }
        }
    }
}