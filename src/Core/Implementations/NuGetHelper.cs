using System.Diagnostics;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Jeevan.ServiceCraftify.Implementations;

internal static class NuGetHelper
{
    internal static async Task<LatestPackage?> GetLatestNuGetVersion(string packageId, string? packageSource = null)
    {
        // Set up NuGet settings to load from nuget.config files
        ISettings settings = Settings.LoadDefaultSettings(root: null);

        // Define package source provider
        PackageSourceProvider packageSourceProvider = new(settings);
        SourceRepositoryProvider sourceRepositoryProvider = new(packageSourceProvider, Repository.Provider.GetCoreV3());

        // Use configured sources if packageSource is not specified
        List<SourceRepository> repositories = string.IsNullOrWhiteSpace(packageSource)
            ? [..sourceRepositoryProvider.GetRepositories()]
            : [sourceRepositoryProvider.CreateRepository(new PackageSource(packageSource))];

        // Retrieve package metadata
        SourceCacheContext cache = new();
        NuGetVersion? maxVersion = null;
        SourceRepository? maxVersionRepo = null;
        foreach (SourceRepository repository in repositories)
        {
            // Get all matching packages from the repository
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>();
            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(packageId,
                includePrerelease: false, includeUnlisted: false,
                cache, NullLogger.Instance, default);

            // If there are matching packages, get the latest version and see if it is greater than
            // the current value.
            if (packages.Any())
            {
                IPackageSearchMetadata latestPackage = packages.OrderByDescending(p => p.Identity.Version).First();
                if (maxVersion is null || latestPackage.Identity.Version > maxVersion)
                {
                    maxVersion = latestPackage.Identity.Version;
                    maxVersionRepo = repository;
                }
            }
        }

        if (maxVersion is null)
            return null;

        Debug.Assert(maxVersionRepo is not null);
        return (LatestPackage?)new LatestPackage(maxVersion, maxVersionRepo);
    }

    internal sealed record LatestPackage(NuGetVersion Version, SourceRepository Repository);
}
