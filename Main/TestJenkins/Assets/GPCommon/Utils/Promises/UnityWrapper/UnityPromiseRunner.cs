using System.Collections;

namespace GPCommon
{
	public static class UnityPromiseRunner
	{
	    public static void Execute(UnityPromise unityPromise)
	    {
	        // ReSharper disable once ObjectCreationAsStatement
	        new Task(PromiseCoroutine(unityPromise));
	    }

	    private static IEnumerator PromiseCoroutine(UnityPromise unityPromise)
	    {
	        yield return null; // To ensure that unityPromise is instantiated when accessing it in the unityPromise Coroutine.
	        yield return unityPromise.Coroutine;

	        if (string.IsNullOrEmpty(unityPromise.Error))
	        {
	            unityPromise.Resolve();
	        }
	        else
	        {
	            unityPromise.Reject(new UnityPromiseException(unityPromise.Error));
	        }
	    }

	    public static void Execute<T>(UnityPromise<T> unityPromise)
	    {
	        // ReSharper disable once ObjectCreationAsStatement
	        new Task(PromiseCoroutine(unityPromise));
	    }

	    private static IEnumerator PromiseCoroutine<T>(UnityPromise<T> unityPromise)
	    {
	        yield return null; // To ensure that unityPromise is instantiated when accessing it in the unityPromise Coroutine.
	        yield return unityPromise.Coroutine;

	        if (string.IsNullOrEmpty(unityPromise.Error))
	        {
	            unityPromise.Resolve(unityPromise.Result);
	        }
	        else
	        {
	            unityPromise.Reject(new UnityPromiseException(unityPromise.Error));
	        }
	    }
	}
}