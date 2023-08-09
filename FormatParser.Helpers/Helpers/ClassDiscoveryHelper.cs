namespace FormatParser.Helpers;

public static class ClassDiscoveryHelper
{
    public static IEnumerable<T> GetAllInstancesOf<T>()
    {
        var types = GetAllTypes<T>();

        foreach (var t in types.OrderBy(t => t.Name))
            yield return (T)Activator.CreateInstance(t)!;
    }
    
    public static IEnumerable<T> GetAllInstancesOf<T, TParam>(TParam param)
    {
        var types = GetAllTypes<T>();
        foreach (var t in types.OrderBy(t => t.Name))
            yield return (T)Activator.CreateInstance(t, param)!;
    }

    private static IEnumerable<Type> GetAllTypes<T>()
    {
        var type = typeof(T);
        
        return AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => type.IsAssignableFrom(t))
            .Where(t => t is { IsInterface: false, IsAbstract: false });
    }
}