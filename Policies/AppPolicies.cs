namespace dotnetMVP.Policies
{
    [Flags]
    public enum Access
    {
        // Access levels are defined as powers of 2 to allow for bitwise operations
        // Represents access to all ( even hiden ) platforms, challenges and writeups
        ModifyPlatforms = 1,
        ReadPlatforms = 2,
        ModifyChallenges = 4,
        ReadChallenges = 8,
        ModifyWriteups = 16,
        ReadWriteups = 32,
    }
    public static class AppPolicies
    {
        // High level definition
        // Moder -> { Policy1, Policy2, ..., PolicyN }. Can be easily changed at any time. Not the priority right now.
        public const Access AdminAccess = Access.ModifyChallenges | Access.ReadChallenges 
                                            | Access.ModifyPlatforms | Access.ReadPlatforms
                                            | Access.ModifyWriteups | Access.ReadWriteups;

        public const Access ModeratorAccess = Access.ReadChallenges | Access.ReadPlatforms | Access.ReadWriteups;

    }
    
}
