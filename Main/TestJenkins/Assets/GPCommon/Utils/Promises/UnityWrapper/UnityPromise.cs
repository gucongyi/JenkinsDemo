using System.Collections;

namespace GPCommon
{
    public class UnityPromise : Promise
    {
        public readonly IEnumerator Coroutine;
        public string Error;

        public UnityPromise(IEnumerator coroutine)
        {
            Coroutine = coroutine;
            UnityPromiseRunner.Execute(this);
        }
    }

    public class UnityPromise<T> : Promise<T>
    {
        public readonly IEnumerator Coroutine;
        public string Error;
        public T Result;

        public UnityPromise(IEnumerator coroutine)
        {
            Coroutine = coroutine;
            UnityPromiseRunner.Execute(this);
        }
    }
}