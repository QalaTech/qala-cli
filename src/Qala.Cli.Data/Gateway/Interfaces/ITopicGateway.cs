using Qala.Cli.Data.Models;

namespace Qala.Cli.Data.Gateway.Interfaces;

public interface ITopicGateway
{
    Task<IEnumerable<Topic>> ListTopicsAsync();
}
