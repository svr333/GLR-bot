using Discord.Commands;
using System.Threading.Tasks;

namespace GLR.Core.Commands.Modules
{
    public class TestModule : GLRModule<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string text = "Maxim gay af lol")
        {
            await ReplyAsync(text);
        }
    }
}
