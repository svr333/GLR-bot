using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace GLR.Core.Commands.Preconditions
{
    public class RequireSvr : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User.Id == 202095042372829184) return Task.FromResult(PreconditionResult.FromSuccess());
            else return Task.FromResult(PreconditionResult.FromError("Requires bot owner to execute command."));
        }
    }
}
