using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GLR.Core.Commands.Modules
{
    public class UserSpecificModule : GLRModule<SocketCommandContext>
    {
        [Command("don")][Alias("naitoo")]
        public async Task Don()
        {
            await ReplyAsync("🐝 <:AAAStarling_Happy:639094721536458752>");
        }

        [Command("maxim")]
        public async Task Maxim()
        {
            await ReplyAsync("", false, new EmbedBuilder() { Title = "Maxim is so fucking gay that he ..."}
            .WithFooter("Finish the sentence like a boss").Build());
        }

        [Command("marido")]
        public async Task Marido()
        {
            await ReplyAsync("**Marido:** Flash release will be tomorrow!");
        }

        [Command("tom")][Alias("inika")]
        public async Task Bg()
        {
            await ReplyAsync("Tom and Inika are the biggest bg (not big gay)");
        }
        
    }
}
