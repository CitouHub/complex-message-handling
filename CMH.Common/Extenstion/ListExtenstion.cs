namespace CMH.Common.Extenstion
{
    public static class ListExtensions
    {
        public static void Split<T>(this List<T> source, int index, out List<T> part1, out List<T> part2)
        {
            part1 = source.Take(index).ToList();
            part2 = source.TakeLast(source.Count - index).ToList();
        }
    }
}
