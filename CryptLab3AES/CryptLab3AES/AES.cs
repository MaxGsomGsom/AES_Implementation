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
        byte[,] key;

        int c1, c2, c3;

        public AES(byte[] key, int Nkey, int Nblock)
        {
            
            this.Nblock = Nblock;
            this.Nkey = Nkey;

            //выбираем кол-во раундов
            if (Nkey == 8 || Nblock == 8) rounds = 14;
            else if (Nkey == 6 || Nblock == 6) rounds = 12;
            else rounds = 10;

            this.key = new byte[Nkey, 4];

            //разбиваем ключ по 4 байта
            int keyPos = 0;
            for (int i = 0; i < Nkey; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.key[i, j] = key[keyPos];
                    keyPos++;
                }
            }

            //константы для ShiftRow
            if (this.Nblock == 4 || this.Nblock == 6)
            {
                c1 = 1;
                c2 = 2;
                c3 = 3;
            }
            else
            {
                c1 = 1;
                c2 = 3;
                c3 = 4;
            }
        }

        public byte[] Encode(byte[] data)
        {

            int dataPos = 0;
            byte[,] block = new byte[4, Nblock];

            for (int i = 0; i < rounds; i++)
            {

                //получаем блок кодируемых данных
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < Nblock; y++)
                    {
                        block[x, y] = data[dataPos];
                        dataPos++;
                    }
                }




            }


            return null;
        }


        public byte[] Decode(byte[] data)
        {
            return null;
        }



        byte[] ShiftRow(byte[] data)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < Nblock; y++)
                {

                }
            }

            return null;
        }
    }
}
