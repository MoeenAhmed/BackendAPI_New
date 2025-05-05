﻿namespace BackendAPI.Models
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
        public string[] Scopes { get; set; } = default!;

    }
}
