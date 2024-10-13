using Jeevan.ServiceCraftify.Implementations;

NuGetHelper.LatestPackage? latestPackage = await NuGetHelper
    .GetLatestNuGetVersion("Collections.NET");
if (latestPackage is not null)
{
    Console.WriteLine(latestPackage.Version);
    Console.WriteLine(latestPackage.Repository.PackageSource);
}
