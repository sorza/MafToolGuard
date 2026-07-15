namespace MafToolGuard
{
    public record UserProfile(string Name, string Role, IReadOnlyList<string> AllowedTools);
}
