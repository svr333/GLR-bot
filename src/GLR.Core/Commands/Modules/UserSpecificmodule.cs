using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GLR.Core.Commands.Modules
{
    public class UserSpecificModule : GLRModule
    {
        [Command("don")][Alias("naitoo", "chubi")]
        public async Task Don()
        {
            await ReplyAsync("🐝 <:AAAStarling_Happy:700370912486359141>");
        }

        [Command("maxim")]
        public async Task Maxim()
        {
            await ReplyAsync("", false, new EmbedBuilder()
            .WithTitle("Maxim is such a horny boy")
            .WithImageUrl("https://media.discordapp.net/attachments/638859930736132097/700444938701111336/image0-35-1.gif")
            .Build());
            //await Context.Channel.SendFileAsync("assets/maxim.mp4");
        }

        [Command("marido")]
        public async Task Marido()
            => await Context.Channel.SendFileAsync("assets/marido.mp4");

        [Command("marido2")]
        public async Task Marido2()
            => await Context.Channel.SendFileAsync("assets/marido2.mp4");

        [Command("tom")][Alias("inika", "bg", "jerry")]
        public async Task Bg()
            => await ReplyAsync("Tom and Jerry are the biggest bg (not big gay)");
        
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
            => await ReplyAsync("Idk some yiff furry shit, #FurrysAreGood");

        [Command("whirl")]
        public async Task WhirlDies()
            => await Context.Channel.SendFileAsync("assets/whirl.mp4");

        [Command("whirl2")]
        public async Task WhirlLives()
            => await Context.Channel.SendFileAsync("assets/whirl2.mp4");

        [Command("whirl3")]
        public async Task WhirlOriginal()
            => await Context.Channel.SendFileAsync("assets/whirl3.mp4");

        [Command("svr")][Alias("svr333")]
        public async Task Svr()
            => await Context.Channel.SendFileAsync("assets/svr.mp4");

        [Command("acer")]
        public async Task Acer()
            => await ReplyAsync("Acer be like:\nhttps://www.youtube.com/watch?v=cpZma0JjwIo");

        [Command("acer2")][Alias("fish")]
        public async Task Acer2()
            => await Context.Channel.SendFileAsync("assets/acer2.mp4");

        [Command("teren")]
        public async Task Teren()
            =>  await ReplyAsync("", false, new EmbedBuilder()
            .WithImageUrl("https://media.discordapp.net/attachments/638859930736132097/702900229044699216/terenwhen18yr.gif")
            .Build());

        [Command("teren2")]
        public async Task Teren2()
            => await ReplyAsync("", false, new EmbedBuilder()
            .WithImageUrl("https://media.discordapp.net/attachments/621747527724695595/696098296623661094/tukey_man.gif")
            .Build());

        [Command("major")][Alias("majorworm")]
        public async Task Majorworm()
            => await ReplyAsync("🤏🏿<:AFCMajor_Wor:700368330246193152>");

        [Command("rickert")]
        public async Task Rickert()
            => await Context.Channel.SendFileAsync("assets/rickert.mp4");

        [Command("rickert2")]
        public async Task Rickert2()
            => await Context.Channel.SendFileAsync("assets/rickert2.mp4");

        [Command("rickert3")]
        public async Task Rickert3()
            => await Context.Channel.SendFileAsync("assets/rickert3.mp4");

        [Command("pyha")]
        public async Task Pyha()
            => await ReplyAsync("", false, new EmbedBuilder()
            .WithImageUrl("https://cdn.discordapp.com/attachments/694261545840017449/704097544967553104/20181229_091342.gif")
            .Build());
    }
}
