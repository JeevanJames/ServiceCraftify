using System.Diagnostics;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
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

    internal static async Task DownloadNuGetPackageAsync(SourceRepository repository, string packageId,
        NuGetVersion version, string targetFramework, string outputDirectory)
    {
        SourceCacheContext cache = new();

        string downloadDirectory = Path.Combine(Path.GetTempPath(), "ServiceCraftify", Guid.NewGuid().ToString("D"));
        DownloadResource downloadResource = await repository.GetResourceAsync<DownloadResource>();
        DownloadResourceResult downloadResult = await downloadResource.GetDownloadResourceResultAsync(
            new PackageIdentity(packageId, version),
            new PackageDownloadContext(cache),
            downloadDirectory, NullLogger.Instance, CancellationToken.None);

        using PackageReaderBase packageReader = downloadResult.PackageReader;
        NuspecReader nuspecReader = packageReader.NuspecReader;

        IEnumerable<NuGetFramework> packageFrameworks = nuspecReader.GetDependencyGroups().Select(dg => dg.TargetFramework);
        FrameworkReducer reducer = new();
        NuGetFramework bestMatchFramework = reducer.GetNearest(NuGetFramework.ParseFolder(targetFramework), packageFrameworks) ??
            throw new InvalidOperationException("Could not find matching framework.");

        IEnumerable<string> files = packageReader.GetLibItems()
            .Where(fsg => fsg.TargetFramework == bestMatchFramework)
            .SelectMany(fsg => fsg.Items);

        string packageDirectory = Path.Combine(outputDirectory, packageId.ToLowerInvariant(), version.ToString());
        if (!Directory.Exists(packageDirectory))
            Directory.CreateDirectory(packageDirectory);
        foreach (string file in files)
        {
            string outputFile = Path.Combine(packageDirectory, Path.GetFileName(file));
            await using FileStream fs = new(outputFile, FileMode.Create);
            Stream packageFileStream = packageReader.GetStream(file);
            await packageFileStream.CopyToAsync(fs);
        }
    }
}
