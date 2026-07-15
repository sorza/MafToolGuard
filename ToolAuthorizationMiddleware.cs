using Microsoft.Extensions.AI;

namespace MafToolGuard
{
    public class ToolAuthorizationMiddleware(IChatClient innerClient, UserProfile user)
    : DelegatingChatClient(innerClient)
    {
        private readonly UserProfile _user = user;

        public override async Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var response = await base.GetResponseAsync(messages, options, cancellationToken);

            // Extrai todas as tool calls solicitadas pelo modelo nesta resposta
            var toolCalls = response.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionCallContent>()
                .ToList();

            if (toolCalls.Count == 0)
                return response;

            // Verifica cada tool call contra a lista de permissões do usuário
            foreach (var call in toolCalls)
            {
                var isAllowed = _user.AllowedTools
                    .Any(t => t.Equals(call.Name, StringComparison.OrdinalIgnoreCase));

                if (!isAllowed)
                {
                    Console.WriteLine($"[AUTH] Acesso negado: usuário '{_user.Name}' ({_user.Role}) " +
                                      $"não tem permissão para invocar '{call.Name}'.");

                    // Interrompe o pipeline retornando resposta de acesso negado
                    return new ChatResponse(new ChatMessage(
                        ChatRole.Assistant,
                        $"Acesso negado. Seu perfil '{_user.Role}' não tem permissão para executar esta operação."));
                }

                Console.WriteLine($"[AUTH] Autorizado: '{call.Name}' para usuário '{_user.Name}' ({_user.Role}).");
            }

            return response;
        }
    }
}
