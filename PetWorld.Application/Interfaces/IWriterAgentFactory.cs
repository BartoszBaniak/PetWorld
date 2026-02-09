using Microsoft.Extensions.AI;

namespace PetWorld.Application.Interfaces;

public interface IWriterAgentFactory
{
    IChatClient CreateWriterAgent();
    IChatClient CreateCriticAgent();
    IList<AITool> CreateProductTools();
}
