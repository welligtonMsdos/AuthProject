using System.Reflection;

namespace Auth10Api.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var suffixes = new[] { "Repository", "Service" };

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && suffixes.Any(s => t.Name.EndsWith(s)));

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.Name == "I" + type.Name)
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }
    }
}
