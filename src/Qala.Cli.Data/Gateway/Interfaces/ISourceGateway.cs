using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface ISourceGateway
{
    Task<IEnumerable<Source>> ListSourcesAsync();
    Task<Source?> GetSourceAsync(string name);
    Task<Source?> CreateSourceAsync(string name, string description, SourceType sourceType, string configuration);
    Task<Source?> UpdateSourceAsync(string sourceName, string name, string description, SourceType sourceType, string configuration);
    Task DeleteSourceAsync(string name);
}