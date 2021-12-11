namespace EventBus.Redis;

public static class GenericTypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object o)
    {
        if (o == null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        return o.GetType().GetGenericTypeName();
    }
}
