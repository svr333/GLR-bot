using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace GLR.Core.Commands.Preconditions
{
    public class RequireGLRPermission : PreconditionAttribute
    {
        private GuildPermission _permission;

        public RequireGLRPermission(GuildPermission permission = default)
        {
            _permission = permission;
        }
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User.Id == 202095042372829184) return Task.FromResult(PreconditionResult.FromSuccess());

            var guildUser = context.User as SocketGuildUser;

            if (guildUser.GuildPermissions.Has(_permission) || guildUser.GuildPermissions.Has(GuildPermission.Administrator))
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError("Insufficient permissions."));
        }
    }
}
