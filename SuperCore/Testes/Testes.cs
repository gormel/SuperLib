﻿namespace Testes
{
    public class Testes : ITestes
    {
        private int counter = 0;
        public int add(int count)
        {
            return counter += count;
        }

        public void hren(int count)
        {
            counter += count;
        }

        public string get()
        {
            return counter.ToString();
        }
    }
}