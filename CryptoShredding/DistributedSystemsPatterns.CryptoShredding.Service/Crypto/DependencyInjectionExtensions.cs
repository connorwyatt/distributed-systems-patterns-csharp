namespace DistributedSystemsPatterns.CryptoShredding.Service.Crypto;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCrypto(this IServiceCollection services) =>
    services.AddTransient<CryptoService>();
}
