using System.Text;
using System.Text.Json;

using Anthropic;
using Anthropic.Models.Messages;

using pr.net.Services.Tokens;
using pr.net.Services.Chat.Generic;

using pr.net.Models.Incoming.Generic;
using pr.net.Models.Incoming.Anthropic;
using pr.net.Models.Outbound.Anthropic;

namespace pr.net.Services.Chat.Anthropic;

public class AnthropicApiClient(ITokenService tokenService, AnthropicClient client) : IChatApiClient { 

    // UPDATE THIS FUNCTION TO ENSURE THAT THE RETURNED LIST IS IN THE EXACT SAME ORDER AS THE INTAKE LIST 

}