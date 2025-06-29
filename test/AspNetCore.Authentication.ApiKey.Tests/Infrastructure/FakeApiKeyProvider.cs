﻿// Copyright (c) Mihir Dilip. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Security.Claims;

namespace AspNetCore.Authentication.ApiKey.Tests.Infrastructure
{
    class FakeApiKeyProvider : IApiKeyProvider
    {
        public Task<IApiKey?> ProvideAsync(string key)
        {
            var apiKey = FakeApiKeys.Keys.FirstOrDefault(k => k.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (apiKey != null)
            {
                if (apiKey.Key == FakeApiKeys.FakeKeyForLegacyIgnoreExtraValidatedApiKeyCheck)
                {
                    // replace the key
                    apiKey = new FakeApiKey(FakeApiKeys.FakeKey, apiKey.OwnerName, apiKey.Claims);
                }
                else if (apiKey.Key == FakeApiKeys.FakeKeyThrowsNotImplemented)
                {
                    throw new NotImplementedException();
                }
#if !(NET461 || NETSTANDARD2_0 || NETCOREAPP2_1)
                else if (apiKey.Key == FakeApiKeys.FakeKeyIgnoreAuthenticationIfAllowAnonymous)
                {
                    throw new InvalidOperationException(nameof(ApiKeyOptions.IgnoreAuthenticationIfAllowAnonymous));
                }
#endif
            }
            return Task.FromResult(apiKey);
        }
    }

    class FakeApiKey : IApiKey
    {
        public FakeApiKey(string key, string ownerName, IReadOnlyCollection<Claim>? claims = null)
        {
            Key = key;
            OwnerName = ownerName;
            Claims = claims ?? [];
        }

        public string Key { get; }

        public string OwnerName { get; }

        public IReadOnlyCollection<Claim> Claims { get; }
    }

    class FakeApiKeys
    {
        internal const string KeyName = "X-API-KEY";

        internal static string FakeInvalidKey = "<invalid-key>";
        internal static string FakeKey = "myrandomfakekey";
        internal static string FakeKeyThrowsNotImplemented = "myrandomfakekey-not-implemented";
        internal static string FakeKeyForLegacyIgnoreExtraValidatedApiKeyCheck = "ForLegacyIgnoreExtraValidatedApiKeyCheck";
        internal static string FakeKeyIgnoreAuthenticationIfAllowAnonymous = "IgnoreAuthenticationIfAllowAnonymous";
        internal static string FakeKeyOwner = "Fake Key";
        internal static Claim FakeNameClaim = new(ClaimTypes.Name, "FakeNameClaim", ClaimValueTypes.String);
        internal static Claim FakeNameIdentifierClaim = new(ClaimTypes.NameIdentifier, "FakeNameIdentifierClaim", ClaimValueTypes.String);
        internal static Claim FakeRoleClaim = new(ClaimTypes.Role, "FakeRoleClaim", ClaimValueTypes.String);

        internal static List<IApiKey> Keys =>
        [
            new FakeApiKey(FakeKey, FakeKeyOwner, [FakeNameClaim, FakeNameIdentifierClaim, FakeRoleClaim]),
            new FakeApiKey(FakeKeyThrowsNotImplemented, FakeKeyOwner, [FakeNameClaim, FakeNameIdentifierClaim, FakeRoleClaim]),
            new FakeApiKey(FakeKeyForLegacyIgnoreExtraValidatedApiKeyCheck, FakeKeyOwner, [FakeNameClaim, FakeNameIdentifierClaim, FakeRoleClaim]),
            new FakeApiKey(FakeKeyIgnoreAuthenticationIfAllowAnonymous, FakeKeyOwner, [FakeNameClaim, FakeNameIdentifierClaim, FakeRoleClaim])
        ];
    }
}
