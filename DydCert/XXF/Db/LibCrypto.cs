using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace XXF.Db
{
    public static class LibCrypto
    {
        private static string mGetString_mStrs()
        {
            byte[] mStrs = new byte[7];
            mStrs[0] = 0x4a;
            mStrs[1] = 0x6a;
            mStrs[2] = 0x63;
            mStrs[3] = 0x64;
            mStrs[4] = 0x57;
            mStrs[5] = 0x78;
            mStrs[6] = 0x6c;

            string mString_mStrs = System.Text.Encoding.UTF8.GetString(mStrs);
            return mString_mStrs;
        }

        #region  MD5数组初始化
        private static string ConstKey = mGetString_mStrs();
        static byte[] SA1 = { 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1 };
        static byte[] SA2 = { 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1 };
        static byte[] SA3 = { 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1 };
        static byte[] SA4 = { 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1 };
        static byte[] SA5 = { 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0 };
        static byte[] SA6 = { 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1 };
        static byte[] SA7 = { 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1 };
        static byte[] SA8 = { 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 };
        static byte[] SB1 = { 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1 };
        static byte[] SB2 = { 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0 };
        static byte[] SB3 = { 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1 };
        static byte[] SB4 = { 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1 };
        static byte[] SB5 = { 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0 };
        static byte[] SB6 = { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1 };
        static byte[] SB7 = { 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1 };
        static byte[] SB8 = { 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0 };
        static byte[] SC1 = { 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 };
        static byte[] SC2 = { 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0 };
        static byte[] SC3 = { 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 1, 0 };
        static byte[] SC4 = { 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1 };
        static byte[] SC5 = { 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1 };
        static byte[] SC6 = { 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0 };
        static byte[] SC7 = { 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0 };
        static byte[] SC8 = { 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1 };
        static byte[] SD1 = { 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1 };
        static byte[] SD2 = { 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1 };
        static byte[] SD3 = { 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0 };
        static byte[] SD4 = { 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0 };
        static byte[] SD5 = { 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 1 };
        static byte[] SD6 = { 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1 };
        static byte[] SD7 = { 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0 };
        static byte[] SD8 = { 1, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1 };

        static byte[,] Sc ={
				{15, 18, 12, 25, 2, 6, 4, 1, 16, 7, 22, 11, 24, 20, 13, 5, 27, 9, 17, 8, 28, 21, 14, 3,42, 53, 32, 38, 48, 56, 31, 41, 52, 46, 34, 49, 45, 50, 40, 29, 35, 54, 47, 43, 51, 37, 30, 33},
				{16, 19, 13, 26, 3, 7, 5, 2, 17, 8, 23, 12, 25, 21, 14, 6, 28, 10, 18, 9, 1, 22, 15, 4,43, 54, 33, 39, 49, 29, 32, 42, 53, 47, 35, 50, 46, 51, 41, 30, 36, 55, 48, 44, 52, 38, 31, 34},
				{18, 21, 15, 28, 5, 9, 7, 4, 19, 10, 25, 14, 27, 23, 16, 8, 2, 12, 20, 11, 3, 24, 17, 6,45, 56, 35, 41, 51, 31, 34, 44, 55, 49, 37, 52, 48, 53, 43, 32, 38, 29, 50, 46, 54, 40, 33, 36},
				{20, 23, 17, 2, 7, 11, 9, 6, 21, 12, 27, 16, 1, 25, 18, 10, 4, 14, 22, 13, 5, 26, 19, 8,47, 30, 37, 43, 53, 33, 36, 46, 29, 51, 39, 54, 50, 55, 45, 34, 40, 31, 52, 48, 56, 42, 35, 38},
				{22, 25, 19, 4, 9, 13, 11, 8, 23, 14, 1, 18, 3, 27, 20, 12, 6, 16, 24, 15, 7, 28, 21, 10,49, 32, 39, 45, 55, 35, 38, 48, 31, 53, 41, 56, 52, 29, 47, 36, 42, 33, 54, 50, 30, 44, 37, 40},
				{24, 27, 21, 6, 11, 15, 13, 10, 25, 16, 3, 20, 5, 1, 22, 14, 8, 18, 26, 17, 9, 2, 23, 12,51, 34, 41, 47, 29, 37, 40, 50, 33, 55, 43, 30, 54, 31, 49, 38, 44, 35, 56, 52, 32, 46, 39, 42},
				{26, 1, 23, 8, 13, 17, 15, 12, 27, 18, 5, 22, 7, 3, 24, 16, 10, 20, 28, 19, 11, 4, 25, 14,53, 36, 43, 49, 31, 39, 42, 52, 35, 29, 45, 32, 56, 33, 51, 40, 46, 37, 30, 54, 34, 48, 41, 44},
				{28, 3, 25, 10, 15, 19, 17, 14, 1, 20, 7, 24, 9, 5, 26, 18, 12, 22, 2, 21, 13, 6, 27, 16,55, 38, 45, 51, 33, 41, 44, 54, 37, 31, 47, 34, 30, 35, 53, 42, 48, 39, 32, 56, 36, 50, 43, 46},
				{1, 4, 26, 11, 16, 20, 18, 15, 2, 21, 8, 25, 10, 6, 27, 19, 13, 23, 3, 22, 14, 7, 28, 17,56, 39, 46, 52, 34, 42, 45, 55, 38, 32, 48, 35, 31, 36, 54, 43, 49, 40, 33, 29, 37, 51, 44, 47},
				{3, 6, 28, 13, 18, 22, 20, 17, 4, 23, 10, 27, 12, 8, 1, 21, 15, 25, 5, 24, 16, 9, 2, 19,30, 41, 48, 54, 36, 44, 47, 29, 40, 34, 50, 37, 33, 38, 56, 45, 51, 42, 35, 31, 39, 53, 46, 49},
				{5, 8, 2, 15, 20, 24, 22, 19, 6, 25, 12, 1, 14, 10, 3, 23, 17, 27, 7, 26, 18, 11, 4, 21,32, 43, 50, 56, 38, 46, 49, 31, 42, 36, 52, 39, 35, 40, 30, 47, 53, 44, 37, 33, 41, 55, 48, 51},
				{7, 10, 4, 17, 22, 26, 24, 21, 8, 27, 14, 3, 16, 12, 5, 25, 19, 1, 9, 28, 20, 13, 6, 23,34, 45, 52, 30, 40, 48, 51, 33, 44, 38, 54, 41, 37, 42, 32, 49, 55, 46, 39, 35, 43, 29, 50, 53},
				{9, 12, 6, 19, 24, 28, 26, 23, 10, 1, 16, 5, 18, 14, 7, 27, 21, 3, 11, 2, 22, 15, 8, 25,36, 47, 54, 32, 42, 50, 53, 35, 46, 40, 56, 43, 39, 44, 34, 51, 29, 48, 41, 37, 45, 31, 52, 55},
				{11, 14, 8, 21, 26, 2, 28, 25, 12, 3, 18, 7, 20, 16, 9, 1, 23, 5, 13, 4, 24, 17, 10, 27,38, 49, 56, 34, 44, 52, 55, 37, 48, 42, 30, 45, 41, 46, 36, 53, 31, 50, 43, 39, 47, 33, 54, 29},
				{13, 16, 10, 23, 28, 4, 2, 27, 14, 5, 20, 9, 22, 18, 11, 3, 25, 7, 15, 6, 26, 19, 12, 1,40, 51, 30, 36, 46, 54, 29, 39, 50, 44, 32, 47, 43, 48, 38, 55, 33, 52, 45, 41, 49, 35, 56, 31},
				{14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2,41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32}};
        static byte[,] G ={
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};
        static byte[] L = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static byte[] R = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static byte[] F = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static byte[] C = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #endregion

        #region MD5辅助函数
        static byte[] StrToKey(string aKey)
        {
            byte[] Key = new byte[8];
            int i;
            for (i = 0; i <= Key.Length - 1; i++)
            {

                Key[i] = 0;
            }
            for (i = 1; i <= aKey.Length; i++)
            {
                Key[i % Key.Length] = (byte)(Key[i % Key.Length] + Convert.ToByte(aKey[i - 1]));
            }
            return Key;
        }

        static void Des_Init(string aKey, bool aFCode)
        {
            int n, h;
            if (aFCode)
            {
                for (n = 0; n <= 15; n++)
                {
                    for (h = 0; h <= 47; h++)
                    {
                        G[n, h] = C[Sc[n, h] - 1];
                    }
                }
            }
            else
            {

                /*    for n := 1 to 16 do
                    begin
                    for h := 1 to 48 do
                        begin
                        G[17 - n, h] := C[Sc[n, h]];
                        end;
                    end;*/

                for (n = 0; n <= 15; n++)
                {
                    for (h = 47; h >= 0; h--)
                    {
                        G[15 - n, h] = C[Sc[n, h] - 1];
                        G[15 - n, h] = C[Sc[n, h] - 1];
                    }
                }
                h = 1;
            }
        }

        static void Des_Code(byte[] aInput, ref byte[] aOutput)
        {
            byte n, h;
            ushort z;
            L[0] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 64) > 0 ? 1 : 0);
            L[1] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 64) > 0 ? 1 : 0);
            L[2] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 64) > 0 ? 1 : 0);
            L[3] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 64) > 0 ? 1 : 0);
            L[4] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 64) > 0 ? 1 : 0);
            L[5] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 64) > 0 ? 1 : 0);
            L[6] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 64) > 0 ? 1 : 0);
            L[7] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 64) > 0 ? 1 : 0);
            L[8] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 16) > 0 ? 1 : 0);
            L[9] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 16) > 0 ? 1 : 0);
            L[10] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 16) > 0 ? 1 : 0);
            L[11] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 16) > 0 ? 1 : 0);
            L[12] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 16) > 0 ? 1 : 0);
            L[13] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 16) > 0 ? 1 : 0);
            L[14] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 16) > 0 ? 1 : 0);
            L[15] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 16) > 0 ? 1 : 0);
            L[16] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 4) > 0 ? 1 : 0);
            L[17] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 4) > 0 ? 1 : 0);
            L[18] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 4) > 0 ? 1 : 0);
            L[19] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 4) > 0 ? 1 : 0);
            L[20] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 4) > 0 ? 1 : 0);
            L[21] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 4) > 0 ? 1 : 0);
            L[22] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 4) > 0 ? 1 : 0);
            L[23] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 4) > 0 ? 1 : 0);
            L[24] = (byte)(Convert.ToInt16(aInput[7].ToString()) & 1);
            L[25] = (byte)(Convert.ToInt16(aInput[6].ToString()) & 1);
            L[26] = (byte)(Convert.ToInt16(aInput[5].ToString()) & 1);
            L[27] = (byte)(Convert.ToInt16(aInput[4].ToString()) & 1);
            L[28] = (byte)(Convert.ToInt16(aInput[3].ToString()) & 1);
            L[29] = (byte)(Convert.ToInt16(aInput[2].ToString()) & 1);
            L[30] = (byte)(Convert.ToInt16(aInput[1].ToString()) & 1);
            L[31] = (byte)(Convert.ToInt16(aInput[0].ToString()) & 1);

            R[0] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 128) > 0 ? 1 : 0);
            R[1] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 128) > 0 ? 1 : 0);
            R[2] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 128) > 0 ? 1 : 0);
            R[3] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 128) > 0 ? 1 : 0);
            R[4] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 128) > 0 ? 1 : 0);
            R[5] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 128) > 0 ? 1 : 0);
            R[6] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 128) > 0 ? 1 : 0);
            R[7] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 128) > 0 ? 1 : 0);
            R[8] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 32) > 0 ? 1 : 0);
            R[9] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 32) > 0 ? 1 : 0);
            R[10] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 32) > 0 ? 1 : 0);
            R[11] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 32) > 0 ? 1 : 0);
            R[12] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 32) > 0 ? 1 : 0);
            R[13] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 32) > 0 ? 1 : 0);
            R[14] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 32) > 0 ? 1 : 0);
            R[15] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 32) > 0 ? 1 : 0);
            R[16] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 8) > 0 ? 1 : 0);
            R[17] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 8) > 0 ? 1 : 0);
            R[18] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 8) > 0 ? 1 : 0);
            R[19] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 8) > 0 ? 1 : 0);
            R[20] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 8) > 0 ? 1 : 0);
            R[21] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 8) > 0 ? 1 : 0);
            R[22] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 8) > 0 ? 1 : 0);
            R[23] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 8) > 0 ? 1 : 0);
            R[24] = (byte)((Convert.ToInt16(aInput[7].ToString()) & 2) > 0 ? 1 : 0);
            R[25] = (byte)((Convert.ToInt16(aInput[6].ToString()) & 2) > 0 ? 1 : 0);
            R[26] = (byte)((Convert.ToInt16(aInput[5].ToString()) & 2) > 0 ? 1 : 0);
            R[27] = (byte)((Convert.ToInt16(aInput[4].ToString()) & 2) > 0 ? 1 : 0);
            R[28] = (byte)((Convert.ToInt16(aInput[3].ToString()) & 2) > 0 ? 1 : 0);
            R[29] = (byte)((Convert.ToInt16(aInput[2].ToString()) & 2) > 0 ? 1 : 0);
            R[30] = (byte)((Convert.ToInt16(aInput[1].ToString()) & 2) > 0 ? 1 : 0);
            R[31] = (byte)((Convert.ToInt16(aInput[0].ToString()) & 2) > 0 ? 1 : 0);

            for (n = 0; n <= 15; n++)
            {
                z = (ushort)(((R[31] ^ G[n, 0]) << 5) | ((R[4] ^ G[n, 5]) << 4) | ((R[0] ^ G[n, 1]) << 3) | ((R[1] ^ G[n, 2]) << 2) | ((R[2] ^ G[n, 3]) << 1) | (R[3] ^ G[n, 4]));
                F[8] = (byte)(L[8] ^ SA1[z]);
                F[16] = (byte)(L[16] ^ SB1[z]);
                F[22] = (byte)(L[22] ^ SC1[z]);
                F[30] = (byte)(L[30] ^ SD1[z]);
                z = (ushort)(((R[3] ^ G[n, 6]) << 5) | ((R[8] ^ G[n, 11]) << 4) | ((R[4] ^ G[n, 7]) << 3) | ((R[5] ^ G[n, 8]) << 2) | ((R[6] ^ G[n, 9]) << 1) | (R[7] ^ G[n, 10]));
                F[12] = (byte)(L[12] ^ SA2[z]);
                F[27] = (byte)(L[27] ^ SB2[z]);
                F[1] = (byte)(L[1] ^ SC2[z]);
                F[17] = (byte)(L[17] ^ SD2[z]);

                z = (ushort)(((R[7] ^ G[n, 12]) << 5) | ((R[12] ^ G[n, 17]) << 4) | ((R[8] ^ G[n, 13]) << 3) | ((R[9] ^ G[n, 14]) << 2) | ((R[10] ^ G[n, 15]) << 1) | (R[11] ^ G[n, 16]));
                F[23] = (byte)(L[23] ^ SA3[z]);
                F[15] = (byte)(L[15] ^ SB3[z]);
                F[29] = (byte)(L[29] ^ SC3[z]);
                F[5] = (byte)(L[5] ^ SD3[z]);

                z = (ushort)(((R[11] ^ G[n, 18]) << 5) | ((R[16] ^ G[n, 23]) << 4) | ((R[12] ^ G[n, 19]) << 3) | ((R[13] ^ G[n, 20]) << 2) | ((R[14] ^ G[n, 21]) << 1) | (R[15] ^ G[n, 22]));
                F[25] = (byte)(L[25] ^ SA4[z]);
                F[19] = (byte)(L[19] ^ SB4[z]);
                F[9] = (byte)(L[9] ^ SC4[z]);
                F[0] = (byte)(L[0] ^ SD4[z]);



                z = (ushort)(((R[15] ^ G[n, 24]) << 5) | ((R[20] ^ G[n, 29]) << 4) | ((R[16] ^ G[n, 25]) << 3) | ((R[17] ^ G[n, 26]) << 2) | ((R[18] ^ G[n, 27]) << 1) | (R[19] ^ G[n, 28]));
                F[7] = (byte)(L[7] ^ SA5[z]);
                F[13] = (byte)(L[13] ^ SB5[z]);
                F[24] = (byte)(L[24] ^ SC5[z]);
                F[2] = (byte)(L[2] ^ SD5[z]);

                z = (ushort)(((R[19] ^ G[n, 30]) << 5) | ((R[24] ^ G[n, 35]) << 4) | ((R[20] ^ G[n, 31]) << 3) | ((R[21] ^ G[n, 32]) << 2) | ((R[22] ^ G[n, 33]) << 1) | (R[23] ^ G[n, 34]));
                F[3] = (byte)(L[3] ^ SA6[z]);
                F[28] = (byte)(L[28] ^ SB6[z]);
                F[10] = (byte)(L[10] ^ SC6[z]);
                F[18] = (byte)(L[18] ^ SD6[z]);


                z = (ushort)(((R[23] ^ G[n, 36]) << 5) | ((R[28] ^ G[n, 41]) << 4) | ((R[24] ^ G[n, 37]) << 3) | ((R[25] ^ G[n, 38]) << 2) | ((R[26] ^ G[n, 39]) << 1) | (R[27] ^ G[n, 40]));
                F[31] = (byte)(L[31] ^ SA7[z]);
                F[11] = (byte)(L[11] ^ SB7[z]);
                F[21] = (byte)(L[21] ^ SC7[z]);
                F[6] = (byte)(L[6] ^ SD7[z]);

                z = (ushort)(((R[27] ^ G[n, 42]) << 5) | ((R[0] ^ G[n, 47]) << 4) | ((R[28] ^ G[n, 43]) << 3) | ((R[29] ^ G[n, 44]) << 2) | ((R[30] ^ G[n, 45]) << 1) | (R[31] ^ G[n, 46]));
                F[4] = (byte)(L[4] ^ SA8[z]);
                F[26] = (byte)(L[26] ^ SB8[z]);
                F[14] = (byte)(L[14] ^ SC8[z]);
                F[20] = (byte)(L[20] ^ SD8[z]);

                for (h = 0; h < L.Length; h++)
                {
                    L[h] = R[h];
                }

                for (h = 0; h < R.Length; h++)
                {
                    R[h] = F[h];
                }
                //L = R;
                //R = F;
            }

            aOutput[0] = (byte)((L[7] << 7) | (R[7] << 6) | (L[15] << 5) | (R[15] << 4) | (L[23] << 3) | (R[23] << 2) | (L[31] << 1) | R[31]);
            aOutput[1] = (byte)((L[6] << 7) | (R[6] << 6) | (L[14] << 5) | (R[14] << 4) | (L[22] << 3) | (R[22] << 2) | (L[30] << 1) | R[30]);
            aOutput[2] = (byte)((L[5] << 7) | (R[5] << 6) | (L[13] << 5) | (R[13] << 4) | (L[21] << 3) | (R[21] << 2) | (L[29] << 1) | R[29]);
            aOutput[3] = (byte)((L[4] << 7) | (R[4] << 6) | (L[12] << 5) | (R[12] << 4) | (L[20] << 3) | (R[20] << 2) | (L[28] << 1) | R[28]);
            aOutput[4] = (byte)((L[3] << 7) | (R[3] << 6) | (L[11] << 5) | (R[11] << 4) | (L[19] << 3) | (R[19] << 2) | (L[27] << 1) | R[27]);
            aOutput[5] = (byte)((L[2] << 7) | (R[2] << 6) | (L[10] << 5) | (R[10] << 4) | (L[18] << 3) | (R[18] << 2) | (L[26] << 1) | R[26]);
            aOutput[6] = (byte)((L[1] << 7) | (R[1] << 6) | (L[9] << 5) | (R[9] << 4) | (L[17] << 3) | (R[17] << 2) | (L[25] << 1) | R[25]);
            aOutput[7] = (byte)((L[0] << 7) | (R[0] << 6) | (L[8] << 5) | (R[8] << 4) | (L[16] << 3) | (R[16] << 2) | (L[24] << 1) | R[24]);
        }
        #endregion

        /// <summary>MD5加密算法</summary>
        /// <param name="aStr"></param>
        /// <returns></returns>
        public static string Old_EnCrypt(string aStr)
        {
            return Old_EnCrypt(aStr, ConstKey);
        }

        /// <summary>MD5加密算法</summary>
        /// <param name="aStr"></param>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public static string Old_EnCrypt(string aStr, string aKey)
        {
            byte[] ReadBuf = new byte[8];
            byte[] WriteBuf = new byte[8];
            byte[] Key = new byte[8];
            int Count, Offset, I;
            string S, Tmp = "";
            Key = StrToKey(aKey);
            C[0] = (byte)((Convert.ToInt16(Key[7].ToString()) & 128) > 0 ? 1 : 0);
            C[1] = (byte)((Convert.ToInt16(Key[6].ToString()) & 128) > 0 ? 1 : 0);
            C[2] = (byte)((Convert.ToInt16(Key[5].ToString()) & 128) > 0 ? 1 : 0);
            C[3] = (byte)((Convert.ToInt16(Key[4].ToString()) & 128) > 0 ? 1 : 0);
            C[4] = (byte)((Convert.ToInt16(Key[3].ToString()) & 128) > 0 ? 1 : 0);
            C[5] = (byte)((Convert.ToInt16(Key[2].ToString()) & 128) > 0 ? 1 : 0);
            C[6] = (byte)((Convert.ToInt16(Key[1].ToString()) & 128) > 0 ? 1 : 0);
            C[7] = (byte)((Convert.ToInt16(Key[0].ToString()) & 128) > 0 ? 1 : 0);
            C[8] = (byte)((Convert.ToInt16(Key[7].ToString()) & 64) > 0 ? 1 : 0);
            C[9] = (byte)((Convert.ToInt16(Key[6].ToString()) & 64) > 0 ? 1 : 0);
            C[10] = (byte)((Convert.ToInt16(Key[5].ToString()) & 64) > 0 ? 1 : 0);
            C[11] = (byte)((Convert.ToInt16(Key[4].ToString()) & 64) > 0 ? 1 : 0);
            C[12] = (byte)((Convert.ToInt16(Key[3].ToString()) & 64) > 0 ? 1 : 0);
            C[13] = (byte)((Convert.ToInt16(Key[2].ToString()) & 64) > 0 ? 1 : 0);
            C[14] = (byte)((Convert.ToInt16(Key[1].ToString()) & 64) > 0 ? 1 : 0);
            C[15] = (byte)((Convert.ToInt16(Key[0].ToString()) & 64) > 0 ? 1 : 0);
            C[16] = (byte)((Convert.ToInt16(Key[7].ToString()) & 32) > 0 ? 1 : 0);
            C[17] = (byte)((Convert.ToInt16(Key[6].ToString()) & 32) > 0 ? 1 : 0);
            C[18] = (byte)((Convert.ToInt16(Key[5].ToString()) & 32) > 0 ? 1 : 0);
            C[19] = (byte)((Convert.ToInt16(Key[4].ToString()) & 32) > 0 ? 1 : 0);
            C[20] = (byte)((Convert.ToInt16(Key[3].ToString()) & 32) > 0 ? 1 : 0);
            C[21] = (byte)((Convert.ToInt16(Key[2].ToString()) & 32) > 0 ? 1 : 0);
            C[22] = (byte)((Convert.ToInt16(Key[1].ToString()) & 32) > 0 ? 1 : 0);
            C[23] = (byte)((Convert.ToInt16(Key[0].ToString()) & 32) > 0 ? 1 : 0);
            C[24] = (byte)((Convert.ToInt16(Key[7].ToString()) & 16) > 0 ? 1 : 0);
            C[25] = (byte)((Convert.ToInt16(Key[6].ToString()) & 16) > 0 ? 1 : 0);
            C[26] = (byte)((Convert.ToInt16(Key[5].ToString()) & 16) > 0 ? 1 : 0);
            C[27] = (byte)((Convert.ToInt16(Key[4].ToString()) & 16) > 0 ? 1 : 0);
            C[28] = (byte)((Convert.ToInt16(Key[7].ToString()) & 2) > 0 ? 1 : 0);
            C[29] = (byte)((Convert.ToInt16(Key[6].ToString()) & 2) > 0 ? 1 : 0);
            C[30] = (byte)((Convert.ToInt16(Key[5].ToString()) & 2) > 0 ? 1 : 0);
            C[31] = (byte)((Convert.ToInt16(Key[4].ToString()) & 2) > 0 ? 1 : 0);
            C[32] = (byte)((Convert.ToInt16(Key[3].ToString()) & 2) > 0 ? 1 : 0);
            C[33] = (byte)((Convert.ToInt16(Key[2].ToString()) & 2) > 0 ? 1 : 0);
            C[34] = (byte)((Convert.ToInt16(Key[1].ToString()) & 2) > 0 ? 1 : 0);
            C[35] = (byte)((Convert.ToInt16(Key[0].ToString()) & 2) > 0 ? 1 : 0);
            C[36] = (byte)((Convert.ToInt16(Key[7].ToString()) & 4) > 0 ? 1 : 0);
            C[37] = (byte)((Convert.ToInt16(Key[6].ToString()) & 4) > 0 ? 1 : 0);
            C[38] = (byte)((Convert.ToInt16(Key[5].ToString()) & 4) > 0 ? 1 : 0);
            C[39] = (byte)((Convert.ToInt16(Key[4].ToString()) & 4) > 0 ? 1 : 0);
            C[40] = (byte)((Convert.ToInt16(Key[3].ToString()) & 4) > 0 ? 1 : 0);
            C[41] = (byte)((Convert.ToInt16(Key[2].ToString()) & 4) > 0 ? 1 : 0);
            C[42] = (byte)((Convert.ToInt16(Key[1].ToString()) & 4) > 0 ? 1 : 0);
            C[43] = (byte)((Convert.ToInt16(Key[0].ToString()) & 4) > 0 ? 1 : 0);
            C[44] = (byte)((Convert.ToInt16(Key[7].ToString()) & 8) > 0 ? 1 : 0);
            C[45] = (byte)((Convert.ToInt16(Key[6].ToString()) & 8) > 0 ? 1 : 0);
            C[46] = (byte)((Convert.ToInt16(Key[5].ToString()) & 8) > 0 ? 1 : 0);
            C[47] = (byte)((Convert.ToInt16(Key[4].ToString()) & 8) > 0 ? 1 : 0);
            C[48] = (byte)((Convert.ToInt16(Key[3].ToString()) & 8) > 0 ? 1 : 0);
            C[49] = (byte)((Convert.ToInt16(Key[2].ToString()) & 8) > 0 ? 1 : 0);
            C[50] = (byte)((Convert.ToInt16(Key[1].ToString()) & 8) > 0 ? 1 : 0);
            C[51] = (byte)((Convert.ToInt16(Key[0].ToString()) & 8) > 0 ? 1 : 0);
            C[52] = (byte)((Convert.ToInt16(Key[3].ToString()) & 16) > 0 ? 1 : 0);
            C[53] = (byte)((Convert.ToInt16(Key[2].ToString()) & 16) > 0 ? 1 : 0);
            C[54] = (byte)((Convert.ToInt16(Key[1].ToString()) & 16) > 0 ? 1 : 0);
            C[55] = (byte)((Convert.ToInt16(Key[0].ToString()) & 16) > 0 ? 1 : 0);
            Des_Init(Key.ToString(), true);
            Offset = 0;
            Count = aStr.Length;
            do
            {
                S = "";
                for (I = 0; I <= 7; I++)
                {
                    if (Offset + I < aStr.Length)
                    {
                        S = S + aStr.Substring(Offset + I, 1);
                    }
                }
                for (I = 0; I <= ReadBuf.Length - 1; I++)
                {
                    ReadBuf[I] = 0;
                }
                for (I = 0; I <= S.Length - 1; I++)
                {
                    ReadBuf[I] = (byte)S[I];
                }
                Des_Code(ReadBuf, ref WriteBuf);
                for (I = 0; I <= 7; I++)
                {
                    Tmp = Tmp + WriteBuf[I].ToString("X").PadLeft(2, '0');
                }
                Offset = Offset + 8;
            }
            while ((Offset + 1) <= ((int)(Count + 7) / 8) * 8);
            return Tmp;
        }

        /// <summary>MD5解密算法</summary>
        /// <param name="aStr"></param>
        /// <returns></returns>
        public static string Old_DeCrypt(string aStr)
        {
            return Old_DeCrypt(aStr, ConstKey);
        }

        /// <summary>MD5解密算法</summary>
        /// <param name="aStr"></param>
        /// <param name="aKey"></param>
        /// <returns></returns>
        public static string Old_DeCrypt(string aStr, string aKey)
        {
            byte[] ReadBuf = new byte[8];
            byte[] WriteBuf = new byte[8];
            byte[] Key = new byte[8];
            int Count, Offset, I;
            string S, Tmp = "";
            Key = StrToKey(aKey);
            C[0] = (byte)((Convert.ToInt16(Key[7].ToString()) & 128) > 0 ? 1 : 0);
            C[1] = (byte)((Convert.ToInt16(Key[6].ToString()) & 128) > 0 ? 1 : 0);
            C[2] = (byte)((Convert.ToInt16(Key[5].ToString()) & 128) > 0 ? 1 : 0);
            C[3] = (byte)((Convert.ToInt16(Key[4].ToString()) & 128) > 0 ? 1 : 0);
            C[4] = (byte)((Convert.ToInt16(Key[3].ToString()) & 128) > 0 ? 1 : 0);
            C[5] = (byte)((Convert.ToInt16(Key[2].ToString()) & 128) > 0 ? 1 : 0);
            C[6] = (byte)((Convert.ToInt16(Key[1].ToString()) & 128) > 0 ? 1 : 0);
            C[7] = (byte)((Convert.ToInt16(Key[0].ToString()) & 128) > 0 ? 1 : 0);
            C[8] = (byte)((Convert.ToInt16(Key[7].ToString()) & 64) > 0 ? 1 : 0);
            C[9] = (byte)((Convert.ToInt16(Key[6].ToString()) & 64) > 0 ? 1 : 0);
            C[10] = (byte)((Convert.ToInt16(Key[5].ToString()) & 64) > 0 ? 1 : 0);
            C[11] = (byte)((Convert.ToInt16(Key[4].ToString()) & 64) > 0 ? 1 : 0);
            C[12] = (byte)((Convert.ToInt16(Key[3].ToString()) & 64) > 0 ? 1 : 0);
            C[13] = (byte)((Convert.ToInt16(Key[2].ToString()) & 64) > 0 ? 1 : 0);
            C[14] = (byte)((Convert.ToInt16(Key[1].ToString()) & 64) > 0 ? 1 : 0);
            C[15] = (byte)((Convert.ToInt16(Key[0].ToString()) & 64) > 0 ? 1 : 0);
            C[16] = (byte)((Convert.ToInt16(Key[7].ToString()) & 32) > 0 ? 1 : 0);
            C[17] = (byte)((Convert.ToInt16(Key[6].ToString()) & 32) > 0 ? 1 : 0);
            C[18] = (byte)((Convert.ToInt16(Key[5].ToString()) & 32) > 0 ? 1 : 0);
            C[19] = (byte)((Convert.ToInt16(Key[4].ToString()) & 32) > 0 ? 1 : 0);
            C[20] = (byte)((Convert.ToInt16(Key[3].ToString()) & 32) > 0 ? 1 : 0);
            C[21] = (byte)((Convert.ToInt16(Key[2].ToString()) & 32) > 0 ? 1 : 0);
            C[22] = (byte)((Convert.ToInt16(Key[1].ToString()) & 32) > 0 ? 1 : 0);
            C[23] = (byte)((Convert.ToInt16(Key[0].ToString()) & 32) > 0 ? 1 : 0);
            C[24] = (byte)((Convert.ToInt16(Key[7].ToString()) & 16) > 0 ? 1 : 0);
            C[25] = (byte)((Convert.ToInt16(Key[6].ToString()) & 16) > 0 ? 1 : 0);
            C[26] = (byte)((Convert.ToInt16(Key[5].ToString()) & 16) > 0 ? 1 : 0);
            C[27] = (byte)((Convert.ToInt16(Key[4].ToString()) & 16) > 0 ? 1 : 0);
            C[28] = (byte)((Convert.ToInt16(Key[7].ToString()) & 2) > 0 ? 1 : 0);
            C[29] = (byte)((Convert.ToInt16(Key[6].ToString()) & 2) > 0 ? 1 : 0);
            C[30] = (byte)((Convert.ToInt16(Key[5].ToString()) & 2) > 0 ? 1 : 0);
            C[31] = (byte)((Convert.ToInt16(Key[4].ToString()) & 2) > 0 ? 1 : 0);
            C[32] = (byte)((Convert.ToInt16(Key[3].ToString()) & 2) > 0 ? 1 : 0);
            C[33] = (byte)((Convert.ToInt16(Key[2].ToString()) & 2) > 0 ? 1 : 0);
            C[34] = (byte)((Convert.ToInt16(Key[1].ToString()) & 2) > 0 ? 1 : 0);
            C[35] = (byte)((Convert.ToInt16(Key[0].ToString()) & 2) > 0 ? 1 : 0);
            C[36] = (byte)((Convert.ToInt16(Key[7].ToString()) & 4) > 0 ? 1 : 0);
            C[37] = (byte)((Convert.ToInt16(Key[6].ToString()) & 4) > 0 ? 1 : 0);
            C[38] = (byte)((Convert.ToInt16(Key[5].ToString()) & 4) > 0 ? 1 : 0);
            C[39] = (byte)((Convert.ToInt16(Key[4].ToString()) & 4) > 0 ? 1 : 0);
            C[40] = (byte)((Convert.ToInt16(Key[3].ToString()) & 4) > 0 ? 1 : 0);
            C[41] = (byte)((Convert.ToInt16(Key[2].ToString()) & 4) > 0 ? 1 : 0);
            C[42] = (byte)((Convert.ToInt16(Key[1].ToString()) & 4) > 0 ? 1 : 0);
            C[43] = (byte)((Convert.ToInt16(Key[0].ToString()) & 4) > 0 ? 1 : 0);
            C[44] = (byte)((Convert.ToInt16(Key[7].ToString()) & 8) > 0 ? 1 : 0);
            C[45] = (byte)((Convert.ToInt16(Key[6].ToString()) & 8) > 0 ? 1 : 0);
            C[46] = (byte)((Convert.ToInt16(Key[5].ToString()) & 8) > 0 ? 1 : 0);
            C[47] = (byte)((Convert.ToInt16(Key[4].ToString()) & 8) > 0 ? 1 : 0);
            C[48] = (byte)((Convert.ToInt16(Key[3].ToString()) & 8) > 0 ? 1 : 0);
            C[49] = (byte)((Convert.ToInt16(Key[2].ToString()) & 8) > 0 ? 1 : 0);
            C[50] = (byte)((Convert.ToInt16(Key[1].ToString()) & 8) > 0 ? 1 : 0);
            C[51] = (byte)((Convert.ToInt16(Key[0].ToString()) & 8) > 0 ? 1 : 0);
            C[52] = (byte)((Convert.ToInt16(Key[3].ToString()) & 16) > 0 ? 1 : 0);
            C[53] = (byte)((Convert.ToInt16(Key[2].ToString()) & 16) > 0 ? 1 : 0);
            C[54] = (byte)((Convert.ToInt16(Key[1].ToString()) & 16) > 0 ? 1 : 0);
            C[55] = (byte)((Convert.ToInt16(Key[0].ToString()) & 16) > 0 ? 1 : 0);
            Des_Init(Key.ToString(), false);
            S = "";
            I = 0;
            do
            {
                S = S + Convert.ToChar(int.Parse(aStr.Substring(I, 2), System.Globalization.NumberStyles.HexNumber));
                I = I + 2;
            }
            while (I < aStr.Length);
            Offset = 0;
            Count = S.Length;
            while (Offset + 1 < ((int)((Count + 7) / 8) * 8))
            {
                for (I = 0; I <= ReadBuf.Length - 1; I++)
                {
                    ReadBuf[I] = 0;
                }
                for (I = 0; I <= 7; I++)
                {
                    ReadBuf[I] = (byte)S[I + Offset];
                }
                Des_Code(ReadBuf, ref WriteBuf);
                for (I = 0; I <= 7; I++)
                {
                    Tmp = Tmp + Convert.ToChar(WriteBuf[I]).ToString();
                }
                Offset = Offset + 8;
            }
            return Tmp.Replace("\0", "");
        }

        /// <summary>返回md5算法32位小写</summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string MD5(string Text)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(Text);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

        /// <summary>
        /// MD5加密 32位小写
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string En32MD5(string text)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            byte[] hs = md5.ComputeHash(bs);
            string str = "";
            for (int i = 0; i < hs.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                str += Convert.ToString(hs[i], 16).PadLeft(2, '0');

            }
            return str.PadLeft(32, '0').ToLower();
        }

        /// <summary>返回sha1算法小写字符串</summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string SHA1(string Text)
        {
            StringBuilder _result = new StringBuilder();
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(Text));
            foreach (var b in sha1Arr)
            {
                _result.AppendFormat("{0:x2}", b);
            }
            return _result.ToString().ToLower();
        }

        /// <summary>异或加密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string EnXor(string Source,char Key)   
        {
            Char[] sourceArray = Source.ToCharArray();   
            System.Text.StringBuilder temp = new System.Text.StringBuilder();   
            for (Int32 i = 0; i < sourceArray.Length; i++)
            {
                sourceArray[i] = (Char)(sourceArray[i] ^ Key);
                temp.Append(sourceArray[i].ToString());
            }
            return temp.ToString();
        }

        /// <summary>异或解密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string DeXor(string Source, char Key)   
        {
            Char[] ciphertextArray = Source.ToCharArray();   
            System.Text.StringBuilder temp = new System.Text.StringBuilder();   
            for (Int32 i = 0; i < ciphertextArray.Length; i++)   
            {   
                ciphertextArray[i] = (Char)(ciphertextArray[i] ^ Key);   
                temp.Append(ciphertextArray[i].ToString());   
            }   
            return temp.ToString();   
        }

        /// <summary>DES加密</summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string EnDES(string Source)
        {
            return EnDES(Source, "wSht8570");
        }

        /// <summary>DES加密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string EnDES(string Source,string Key)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(@"[^a-zA-Z]+", Key))
            {
                throw new Exception("EnDES函数的密码必须为英文字母!");
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中  
            //原来使用的UTF8编码，我改成Unicode编码了，不行  
            byte[] inputByteArray = Encoding.Default.GetBytes(Source);
            //byte[]  inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt);  

            //建立加密对象的密钥和偏移量  
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            //使得输入密码必须输入英文文本  
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //Write  the  byte  array  into  the  crypto  stream  
            //(It  will  end  up  in  the  memory  stream)  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //Get  the  data  back  from  the  memory  stream,  and  into  a  string  
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //Format  as  hex  
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        public static string DeDES(string Source)
        {
            return DeDES(Source, "wSht8570");
        }

        /// <summary>DES解密</summary>
        /// <param name="Source"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string DeDES(string Source,string Key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //Put  the  input  string  into  the  byte  array  
            byte[] inputByteArray = new byte[Source.Length / 2];
            for (int x = 0; x < Source.Length / 2; x++)
            {
                int i = (Convert.ToInt32(Source.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //建立加密对象的密钥和偏移量，此值重要，不能修改  
            des.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream  
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //Get  the  decrypted  data  back  from  the  memory  stream  
            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }  

        /// <summary>Rijndael加密</summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string EnRijndael(string Source)
        {
            Rijndael sm = new Rijndael();
            return sm.Encrypto(Source);
        }

        /// <summary>Rijndael解密</summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string DeRijndael(string Source)
        {
            Rijndael sm = new Rijndael();
            return sm.Decrypto(Source);
        }
    }

    #region 对称加密算法Rijndael
    /// <summary>对称加密算法</summary>
    public class Rijndael
    {
        private SymmetricAlgorithm mobjCryptoService;
        private string Key;
        /// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        public Rijndael()
        {
            mobjCryptoService = new RijndaelManaged();
            Key = "haitian85)(*&^%";
        }
        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = "ht";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string Encrypto(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string Decrypto(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
    #endregion 对称加密算法Rijndael
}
