using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLab3AES
{
    public class AES
    {

        int Nblock, Nkey;
        int rounds;
        byte[,] key, workKey, antiWorkKey;

        byte[] temp = new byte[4];
        byte tempByte;

        int[] C;

        public AES(byte[] key, int Nkey, int Nblock)
        {
            this.Nblock = Nblock;
            this.Nkey = Nkey;

            //выбираем кол-во раундов
            if (Nkey == 8 || Nblock == 8) rounds = 14;
            else if (Nkey == 6 || Nblock == 6) rounds = 12;
            else rounds = 10;

            this.key = new byte[4, Nkey];

            //разбиваем ключ по 4 байта
            for (int y = 0; y < Nkey; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    this.key[x, y] = key[x + y * 4];
                }
            }

            //выбираем константы для ShiftRow
            C = new int[3];

            if (this.Nblock == 4 || this.Nblock == 6)
            {
                C[0] = 1;
                C[1] = 2;
                C[2] = 3;
            }
            else
            {
                C[0] = 1;
                C[1] = 3;
                C[2] = 4;
            }




            //получаем рабочий ключ
            workKey = KeySchedule(this.key);

            //получаем рабочий ключ расшифрования
            antiWorkKey = workKey;
        }


        //шифрование
        public byte[] Encode(byte[] data)
        {
            //дополняет до кратного числа байт
            int addLength = Nblock * 4 - data.Length % (Nblock * 4);
            Array.Resize(ref data, data.Length + addLength);


            int dataPos = 0;
            byte[,] block = new byte[4, Nblock];

            byte[] result = new byte[data.Length];

            while (dataPos < data.Length)
            {
                GetDataBlock(data, dataPos, block);

                for (int i = 0; i < rounds + 1; i++)
                {
                    if (i == 0)
                    {
                        AddRoundKey(block, i);
                    }
                    else if (i < rounds)
                    {
                        ByteSub(block);
                        ShiftRow(block);
                        MixColumn(block);
                        AddRoundKey(block, i);
                    }
                    else if (i == rounds)
                    {
                        ByteSub(block);
                        ShiftRow(block);
                        AddRoundKey(block, i);
                    }
                }

                dataPos = SetDataBlock(block, dataPos, result);

            }


            return result;
        }



        //расшифрование
        public byte[] Decode(byte[] data)
        {
            int dataPos = 0;
            byte[,] block = new byte[4, Nblock];

            byte[] result = new byte[data.Length];


            while (dataPos < data.Length)
            {
                GetDataBlock(data, dataPos, block);

                for (int i = rounds; i >= 0; i--)
                {
                    if (i == rounds)
                    {
                        AntiAddRoundKey(block, i);
                    }
                    else if (i == 0)
                    {
                        AntiShiftRow(block);
                        AntiByteSub(block);
                        AntiAddRoundKey(block, i);
                    }
                    else if (i < rounds)
                    {
                        AntiShiftRow(block);
                        AntiByteSub(block);
                        AntiAddRoundKey(block, i);
                        AntiMixColumn(block);
                    }
                }

                dataPos = SetDataBlock(block, dataPos, result);
            }

            //удаляем пустые байты в конце
            while (result[result.Length - 1] == 0x00)
            {
                Array.Resize(ref result, result.Length - 1);
            }


            return result;
        }

        //получение блока для кодирования
        int GetDataBlock(byte[] data, int dataPos, byte[,] block)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    block[x, y] = data[dataPos];
                    dataPos++;
                }
            }

            return dataPos;
        }


        int SetDataBlock(byte[,] block, int dataPos, byte[] data)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    data[dataPos] = block[x, y];
                    dataPos++;
                }
            }

            return dataPos;
        }



        //замена байт на байты из sbox
        void ByteSub(byte[,] data)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    data[x, y] = sbox[data[x, y]];
                }
            }
        }


        //замена байт на байты из sbox
        void AntiByteSub(byte[,] data)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    data[x, y] = antiSbox[data[x, y]];
                }
            }
        }


        //сдвиг в строках
        void ShiftRow(byte[,] data)
        {
            //для каждой строки
            for (int row = 1; row < 4; row++)
            {
                //копируем первые С байт в буфер
                for (int i = 0; i < C[row - 1]; i++)
                {
                    temp[i] = data[row, i];
                }

                //сдвигаем байты
                for (int i = 0; i < Nblock - C[row - 1]; i++)
                {
                    data[row, i] = data[row, i + C[row - 1]];
                }


                //вставляем байты из буфера в конец
                for (int i = 0; i < C[row - 1]; i++)
                {
                    data[row, i + Nblock - C[row - 1]] = temp[i];
                }
            }

        }


        //сдвиг в строках
        void AntiShiftRow(byte[,] data)
        {
            //для каждой строки
            for (int row = 1; row < 4; row++)
            {
                //копируем последние С байт в буфер
                for (int i = 0; i < C[row - 1]; i++)
                {
                    temp[i] = data[row, Nblock - C[row - 1] + i];
                }

                //сдвигаем байты
                for (int i = Nblock - 1; i >= C[row - 1]; i--)
                {
                    data[row, i] = data[row, i - C[row - 1]];
                }

                //вставляем байты из буфера в начало
                for (int i = 0; i < C[row - 1]; i++)
                {
                    data[row, i] = temp[i];
                }
            }

        }

        //перемешивание столбцов
        void MixColumn(byte[,] data)
        {
            for (int i = 0; i < Nblock; i++)
            {
                //копируем столбец в буфер
                for (int k = 0; k < 4; k++)
                {
                    temp[k] = data[k, i];
                }

                //умножаем столбец на матрицу - перемешиваем
                data[0, i] = (byte)(GMul(temp[0], 0x02) ^ GMul(temp[1], 0x03) ^ GMul(temp[2], 0x01) ^ GMul(temp[3], 0x01));
                data[1, i] = (byte)(GMul(temp[0], 0x01) ^ GMul(temp[1], 0x02) ^ GMul(temp[2], 0x03) ^ GMul(temp[3], 0x01));
                data[2, i] = (byte)(GMul(temp[0], 0x01) ^ GMul(temp[1], 0x01) ^ GMul(temp[2], 0x02) ^ GMul(temp[3], 0x03));
                data[3, i] = (byte)(GMul(temp[0], 0x03) ^ GMul(temp[1], 0x01) ^ GMul(temp[2], 0x01) ^ GMul(temp[3], 0x02));
            }
        }



        /// <summary>
        /// Операция умножения в Galois Field
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        byte GMul(byte a, byte b)
        {
            byte p = 0;
            byte counter;
            byte hi_bit_set;

            for (counter = 0; counter < 8; counter++)
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }
                hi_bit_set = (byte)(a & 0x80);
                a <<= 1;
                if (hi_bit_set != 0)
                {
                    a ^= 0x1b;
                }
                b >>= 1;
            }
            return p;
        }

        //перемешивание столбцов
        void AntiMixColumn(byte[,] data)
        {
            for (int i = 0; i < Nblock; i++)
            {
                //копируем столбец в буфер
                for (int k = 0; k < 4; k++)
                {
                    temp[k] = data[k, i];
                }

                //умножаем столбец на матрицу - перемешиваем
                data[0, i] = (byte)(GMul(temp[0], 0x0e) ^ GMul(temp[1], 0x0b) ^ GMul(temp[2], 0x0d) ^ GMul(temp[3], 0x09));
                data[1, i] = (byte)(GMul(temp[0], 0x09) ^ GMul(temp[1], 0x0e) ^ GMul(temp[2], 0x0b) ^ GMul(temp[3], 0x0d));
                data[2, i] = (byte)(GMul(temp[0], 0x0d) ^ GMul(temp[1], 0x09) ^ GMul(temp[2], 0x0e) ^ GMul(temp[3], 0x0b));
                data[3, i] = (byte)(GMul(temp[0], 0x0b) ^ GMul(temp[1], 0x0d) ^ GMul(temp[2], 0x09) ^ GMul(temp[3], 0x0e));
            }
        }


        byte[,] KeySchedule(byte[,] key)
        {
            byte[,] result = new byte[4, Nblock * (rounds + 1)];

            //копируем основной ключ в первый раундовый ключ
            for (int y = 0; y < Nkey; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    result[x, y] = key[x, y];
                }
            }


            //генерируем остальные раундовые ключи
            for (int r = Nkey; r < (rounds + 1) * Nkey; r++)
            {

                //копируем последние 4 байта ключа в буфер
                for (int i = 0; i < 4; i++)
                {
                    temp[i] = result[i, r - 1];
                }


                //для каждого первого слова раундового ключа
                if (r % Nkey == 0)
                {

                    //вращаем байты слова
                    RotateWord(temp);
                    //заменяем слово на байты из sbox
                    SboxWord(temp);

                    //слово XOR rcon
                    temp[0] = (byte)(temp[0] ^ rcon[r / Nkey]);
                    temp[1] = (byte)(temp[1] ^ 0x00);
                    temp[2] = (byte)(temp[2] ^ 0x00);
                    temp[3] = (byte)(temp[3] ^ 0x00);

                }
                //если ключ 8 байт и генерируем 5-е слово ключа, то заменяем слово на байты из sbox
                else if (Nkey == 8 && r % Nkey == 4)
                {
                    SboxWord(temp);
                }

                //новое слова ключа получается как "последнее слово ключа XOR полученное в temp слово"
                for (int i = 0; i < 4; i++)
                {
                    result[i, r] = (byte)(result[i, r - Nkey] ^ temp[i]);
                }
            }


            return result;

        }



        //замена байтов ключа на байты из sbox
        void SboxWord(byte[] word)
        {
            for (int i = 0; i < 4; i++)
            {
                word[i] = sbox[word[i]];
            }
        }

        //циклический поворот байтов ключа
        void RotateWord(byte[] word)
        {
            tempByte = word[0];
            word[0] = word[1];
            word[1] = word[2];
            word[2] = word[3];
            word[3] = tempByte;
        }


        //суммирование с ключем
        void AddRoundKey(byte[,] data, int round)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    data[x, y] = (byte)(data[x, y] ^ workKey[x, y + round * Nblock]);
                }
            }
        }



        //суммирование с ключем
        void AntiAddRoundKey(byte[,] data, int round)
        {
            for (int y = 0; y < Nblock; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    data[x, y] = (byte)(data[x, y] ^ antiWorkKey[x, y + round * Nblock]);
                }
            }
        }



        byte[] sbox = new byte[256] {
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16};


        byte[] antiSbox = new byte[256] {
            0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
            0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
            0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
            0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
            0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
            0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
            0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
            0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
            0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
            0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
            0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
            0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
            0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
            0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
            0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d};



        byte[] rcon = {
            0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a,
            0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39,
            0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a,
            0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8,
            0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef,
            0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc,
            0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b,
            0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3,
            0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94,
            0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20,
            0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35,
            0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f,
            0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04,
            0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63,
            0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd,
            0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d};


    }

}
