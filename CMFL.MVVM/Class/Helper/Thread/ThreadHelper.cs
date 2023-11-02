using System;

namespace CMFL.MVVM.Class.Helper.Thread
{
    public static class ThreadHelper
    {
        public static int GetMaxWorkThreadCount()
        {
            var processorCount = Environment.ProcessorCount;
            return processorCount * 4;
        }

        public static int GetMaxIOWorkThreadCount()
        {
            var processorCount = Environment.ProcessorCount;
            return processorCount * 2;
        }
    }
}