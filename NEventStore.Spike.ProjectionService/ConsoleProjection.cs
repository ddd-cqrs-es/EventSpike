using System;
using Newtonsoft.Json;

namespace NEventStore.Spike.ProjectionService
{
    internal class ConsoleProjection : IObserver<ICommit>
    {
        public void OnNext(ICommit value)
        {
            Console.WriteLine(JsonConvert.SerializeObject(value));
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(JsonConvert.SerializeObject(error));
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }
    }
}