using System.Text;

using Jeevan.ServiceCraftify.Implementations;

namespace Jeevan.ServiceCraftify;

public static class Craftify
{
    public static Task InitializeConfigAsync(TextWriter configWriter)
    {
        return Task.CompletedTask;
    }

    public static async Task InitializeConfigAsync(string configFilePath)
    {
        await using FileStream fs = new(configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        await using StreamWriter writer = new(fs, Encoding.UTF8);
        await InitializeConfigAsync(writer);
    }

    public static async Task<string> GenerateAsync(GenerateOptions options)
    {
        GenerateImplementation impl = await GenerateImplementation.Create(options);
        return await impl.GenerateAsync();
    }
}
