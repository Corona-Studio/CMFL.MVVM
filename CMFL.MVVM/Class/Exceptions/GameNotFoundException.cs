using System;
using System.Runtime.Serialization;
using CMFL.MVVM.Class.Helper.Launcher;

namespace CMFL.MVVM.Class.Exceptions
{
    [Serializable]
    public class GameNotFoundException : Exception
    {
        public GameNotFoundException() : base(LanguageHelper.GetField("GameNotFound"))
        {
        }

        public GameNotFoundException(string message) : base(message)
        {
        }

        public GameNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GameNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}