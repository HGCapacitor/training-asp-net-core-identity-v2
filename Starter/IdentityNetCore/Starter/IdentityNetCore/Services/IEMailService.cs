using System.Threading.Tasks;
using IdentityNetCore.Models;

namespace IdentityNetCore.Services;

public interface IEMailService
{
    Task SendEmailAsync(EMail mail);
}