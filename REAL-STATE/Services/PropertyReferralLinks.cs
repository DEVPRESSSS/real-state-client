using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using REAL_STATE.Models;

namespace REAL_STATE.Services;

public sealed class PropertyReferralLinks(IDataProtectionProvider protection)
{
    private IDataProtector ForProperty(string propertyId) =>
        protection.CreateProtector("REAL_STATE.PropertyReferral.v1", propertyId);

    public string Create(string propertyId, string agentId) =>
        ForProperty(propertyId).Protect(agentId);

    public string? GetReferringAgent(Property property, string? token)
    {
        if (property.Agent is not { IsActive: true } agent ||
            !string.Equals(property.Status, "Available", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(token) || token.Length > 512)
            return null;

        try
        {
            var referrer = ForProperty(property.Id).Unprotect(token);
            return string.Equals(referrer, agent.Id, StringComparison.Ordinal) ? referrer : null;
        }
        catch (Exception exception) when (exception is CryptographicException or FormatException)
        {
            return null;
        }
    }
}
