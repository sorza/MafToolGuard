using MafToolGuard;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

// Perfil com acesso apenas a consulta — não pode reservar estoque
var operador = new UserProfile(
    Name: "Carlos",
    Role: "operador",
    AllowedTools: ["GetStockLevel"]);

// Perfil com acesso completo às duas ferramentas
var gerente = new UserProfile(
    Name: "Ana",
    Role: "gerente",
    AllowedTools: ["GetStockLevel", "ReserveStock"]);

var tools = new[]
{
    AIFunctionFactory.Create(WarehouseTools.GetStockLevel),
    AIFunctionFactory.Create(WarehouseTools.ReserveStock)
};

async Task RunScenario(string label, UserProfile profile, string question)
{
    Console.WriteLine($"\n{'='.ToString().PadRight(50, '=')}");
    Console.WriteLine($"Cenário: {label} | Usuário: {profile.Name} ({profile.Role})");
    Console.WriteLine($"Pergunta: {question}");
    Console.WriteLine('='.ToString().PadRight(50, '='));

    IChatClient ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");
    IChatClient clientWithAuth = new ToolAuthorizationMiddleware(ollama, profile);

    var agent = clientWithAuth
        .AsAIAgent(
            instructions: "Você é um assistente de estoque. Use as ferramentas disponíveis para responder.",
            tools: tools)
        .AsBuilder()
        .Build();

    var response = await agent.RunAsync(question);
    Console.WriteLine($"\nResposta final: {response.Messages.Last().Text}");
}

// Cenário 1: operador consulta estoque — deve ser autorizado
await RunScenario(
    "Consulta de estoque",
    operador,
    "Qual o nível de estoque do produto SKU-9821?");

// Cenário 2: operador tenta reservar — deve ser bloqueado
await RunScenario(
    "Tentativa de reserva por operador",
    operador,
    "Reserve 10 unidades do produto SKU-9821 para o pedido 445.");

// Cenário 3: gerente reserva estoque — deve ser autorizado
await RunScenario(
    "Reserva por gerente",
    gerente,
    "Reserve 5 unidades do produto SKU-3310 para o pedido 512.");