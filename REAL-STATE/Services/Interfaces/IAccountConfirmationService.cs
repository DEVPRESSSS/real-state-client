using REAL_STATE.Models;
namespace REAL_STATE.Services.Interfaces;
public interface IAccountConfirmationService { Task SendAsync(ApplicationUser user); }
