using Discord.Commands;

namespace GLR.Core.Commands
{
    public class GLRModule<T> : ModuleBase<T> where T : class, ICommandContext
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
