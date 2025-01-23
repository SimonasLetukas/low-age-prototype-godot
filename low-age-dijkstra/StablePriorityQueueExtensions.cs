using Priority_Queue;

namespace low_age_dijkstra
{
    public static class StablePriorityQueueExtensions
    {
        public static void EnqueueOrUpdate<T>(this StablePriorityQueue<T> queue, T node, float priority)
            where T : StablePriorityQueueNode
        {
            if (queue.Contains(node))
                queue.UpdatePriority(node, priority);
            else
                queue.Enqueue(node, priority);
        }
    }
}