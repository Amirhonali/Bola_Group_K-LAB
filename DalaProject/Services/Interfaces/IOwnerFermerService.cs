using DalaProject.DTOs;
using DalaProject.DTOs.OwnerFermer;

namespace DalaProject.Services.Interfaces;

public interface IOwnerFermerService
{
    Task<OwnerFermerDTO> CreateInvitationAsync(OwnerFermerCreateDTO dto); // create pending invite
    Task<IEnumerable<OwnerFermerDTO>> GetInvitationsForFermerAsync(int fermerId);
    Task AcceptInvitationAsync(int invitationId);
    Task RejectInvitationAsync(int invitationId);
    Task<IEnumerable<OwnerFermerDTO>> GetOwnersForFermerAsync(int fermerId);
}