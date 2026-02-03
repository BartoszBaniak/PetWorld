using PetWorld.Application.DTOs;

namespace PetWorld.Application.Interfaces;

public interface IWriterCriticService
{
    Task<WriterCriticResponse> ProcessQuestionAsync(string question);
}
