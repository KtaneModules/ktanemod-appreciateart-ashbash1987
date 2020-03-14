public static class IEnumerableExtensions
{
    public static T RandomPick<T>(this T[] array)
    {
        int lookupIndex = UnityEngine.Random.Range(0, array.Length);
        return array[lookupIndex];
    }
}
