using Microsoft.Agents.AI;

namespace PetWorld.Application.Interfaces;

public interface IWriterAgentFactory
{
    ChatClientAgent CreateWriterAgent();
    ChatClientAgent CreateCriticAgent();
}
