using System;

namespace GPCommon
{
    public class UnityPromiseException : Exception
    {
        private readonly string _error;

        public UnityPromiseException(string error)
        {
            _error = error;
        }

        public override string Message { get { return "An error happened while running the Unity Promise : " + _error; } }
    }
}