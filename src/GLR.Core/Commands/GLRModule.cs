using Discord.Commands;

namespace GLR.Core.Commands
{
    public class GLRModule : ModuleBase<SocketCommandContext>
    {
        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
        }
    }
}
