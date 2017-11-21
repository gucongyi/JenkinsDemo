using System;
using System.Collections.Generic;

namespace GPCommon
{
    public class ParallelProgressFlow
    {
        public interface IProgress
        {
            bool IsCompleted { get; }
            void StartProgress();
        }

        private readonly List<IProgress> _progress;

        public ParallelProgressFlow()
        {
            _progress = new List<IProgress>();
        }

        public void AddProgress(IProgress progress)
        {
            _progress.Add(progress);
        }

        public void Start(Action onComplete)
        {
            _progress.ForEach(x => x.StartProgress());

            Timer.UpdateUntilReturnTrue(() =>
            {
                if (_progress.TrueForAll(x => x.IsCompleted))
                {
                    onComplete();
                    return true;
                }
                return false;
            });
        }

        public void LogNotCompleteProgress()
        {
            _progress.ForEach((x) =>
            {
                if (!x.IsCompleted) Watchdog.Log("ParallelProgressFlow", x.ToString());
            });
        }
    }
}