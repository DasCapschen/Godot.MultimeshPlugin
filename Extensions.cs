using System.Collections.Generic;
using System.Linq;

namespace MultimeshPlugin.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T, int)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((item, index) => (item, index));
    }
}

public static class StackExtensions
{
    public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            stack.Push(item);
        }
    }
}
