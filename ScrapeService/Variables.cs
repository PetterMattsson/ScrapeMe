﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeService
{
    sealed class Variables
    {
        private static Variables variables = new Variables();
        public int ObjectId;
        public int NumberOfScrapes;
        public int NumberOfSaves;
        public bool Finished;

        private Variables()
        {
            ObjectId = 1;
            NumberOfSaves = 0;
            NumberOfScrapes = 0;
            Finished = false;
        }

        public static Variables GetVariables()
        {
            return variables;
        }

        public static void SetFinished(bool value)
        {
            variables.Finished = value;
        }

        public static bool GetFinished()
        {
            return variables.Finished;
        }

        public static void IncrementSaves()
        {
            variables.NumberOfSaves = variables.NumberOfSaves +1;
        }

        public int GetSaves()
        {
            return variables.NumberOfSaves;
        }

        public static void AddScrapes(int i)
        {
            variables.NumberOfScrapes = variables.NumberOfScrapes + i;
        }

        public static int GetObjectId()
        {
            return variables.ObjectId;
        }

        public static void IncrementObjectId()
        {
            variables.ObjectId = variables.ObjectId++;
        }


    }
}
