using UnityEngine;
using System.Collections;

namespace  GPCommon
{
    public class UnityPromiseUsageSample
    {
        public void UsePromise()
        {
            new UnityPromiseSample().LoadSprite("mySprite")
                .Then(x => Debug.Log("Sprite is loaded"))
                .Catch(exception => Debug.Log(exception.Message));
        }
    }

    public class UnityPromiseSample
    {
        private UnityPromise<Sprite> _promise;

        public IPromise<Sprite> LoadSprite(string imagePath)
        {
            _promise = new UnityPromise<Sprite>(LoadAsynchronousResource(imagePath));
            return _promise;
        }

        private IEnumerator LoadAsynchronousResource(string imagePath)
        {
            ResourceRequest request = Resources.LoadAsync(imagePath, typeof(Sprite));

            while (!request.isDone)
            {
                yield return null;
            }

            if (request.asset == null)
            {
                _promise.Error = "Could not find asset at path " + imagePath;
            }
            else
            {
                _promise.Result = (Sprite) request.asset;
            }
        }
    }
}
