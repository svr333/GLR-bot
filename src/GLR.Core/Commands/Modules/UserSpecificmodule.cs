using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GLR.Core.Commands.Modules
{
    public class UserSpecificModule : GLRModule<SocketCommandContext>
    {
        [Command("don")][Alias("naitoo", "chubi")]
        public async Task Don()
        {
            await ReplyAsync("🐝 <:AAAStarling_Happy:700370912486359141>");
        }

        [Command("maxim")]
        public async Task Maxim()
        {
            await ReplyAsync("", false, new EmbedBuilder() { Title = "Maxim is so fucking gay that he ..."}
            .WithFooter("Finish the sentence like a boss").Build());
            await Context.Channel.SendFileAsync("assets/maxim.mp4");
        }

        [Command("marido")]
        public async Task Marido()
        {
            await ReplyAsync("**Marido:** Flash release will be tomorrow!");
        }

        [Command("tom")][Alias("inika", "bg", "svr333", "svr")]
        public async Task Bg()
        {
            await ReplyAsync("Tom and Inika are the biggest bg (not big gay)");
        }
        
        [Command("nudes")]
        public async Task Nudes()
        {
            var embed = new EmbedBuilder().WithTitle("The nudes command.").WithColor(Color.Blue)
            .WithDescription("Nudes wanted me to do more to do this command so here I am trying to please this fucktard. If you think this is unfair well then you are correct. Nudes demands too much and stinks. I started a fundraiser to buy nudes a brain that works properly.\n\n**Link:** [click me](https://www.gofundme.com/f/8pggu?utm_source=customer&utm_medium=copy_link&utm_campaign=p_cf+share-flow-1)")
            .WithThumbnailUrl("https://cdn.discordapp.com/avatars/214049777183096833/4163f0aeb558af7df4bb472141788e31.png?size=1024").Build();
            await ReplyAsync("<a:miro2:696692388218798140> <a:miro:696692388130717727>", false, embed);
        }

        [Command("aniela")]
        public async Task Aniela()
        {
            await ReplyAsync("Idk some yiff furry shit, #FurrysAreGood");
        }

        [Command("whirl")]
        public async Task Whirl()
            => await Context.Channel.SendFileAsync("assets/whirl.mp4");
    }
}
