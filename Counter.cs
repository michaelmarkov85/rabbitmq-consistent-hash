using System.Collections.Generic;

namespace RabbitMqConsistentHash
{
    public class Counter
    {
        public readonly List<KeyValuePair<int, int>> Result = new List<KeyValuePair<int, int>>();
    }
}
