using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GLR.Core.Entities;
using GLR.Core.Services.Commands;
using GLR.Core.Services.DataStorage;

namespace GLR.Core.Commands
{
    public class GLRModule : ModuleBase<SocketCommandContext>
    {
        public GuildAccountService Accounts { get; set; }
        public PaginatorService Paginator { get; set; }
        private CommandInfo _currentCommand;
        [DontInject]
        public string ExpandedCommandName => $"{_currentCommand.Module.Name}_{_currentCommand.Name}".ToLower();

        protected override void BeforeExecute(CommandInfo command)
        {            
            _currentCommand = command;
            var guild = Accounts.GetOrCreateGuildAccount(Context.Guild.Id);
            
            if (!CommandIsAllowedToRun(guild))
            {
                Context.Message.AddReactionAsync(new Emoji("⛔"));
                throw new Exception("User has insuffient permission to execute command.");
            }
        }

        protected override void AfterExecute(CommandInfo command)
            => base.AfterExecute(command);

        private bool CommandIsAllowedToRun(GuildAccount guild)
        {
            var currentCommand = guild.Commands.Find(x => x.Name == ExpandedCommandName);
            // If command doesn't exist in database, recreate it.
            if (currentCommand == null) guild.AddNewCommand(_currentCommand);
            Accounts.SaveGuildAccount(guild);

            currentCommand = guild.Commands.Find(x => x.Name == ExpandedCommandName);

            var userRoles = (Context.User as SocketGuildUser).Roles.ToList();
            
            if (!currentCommand.IsEnabled) return false;
            if (userRoles.Find(x => x.Id == guild.ModRoleId) != null) return true;

            var userHasRoleInList = UserHasRoleInList(currentCommand);
            if (currentCommand.RolesListIsBlacklist && userHasRoleInList
            || !currentCommand.RolesListIsBlacklist && !userHasRoleInList) 
                return false;
            
            var channelIsInList = currentCommand.WhitelistedChannels.Contains(Context.Channel.Id);
            if (currentCommand.ChannelListIsBlacklist && channelIsInList
            || !currentCommand.ChannelListIsBlacklist && !channelIsInList) 
                return false;

            return true;
        }

        private bool UserHasRoleInList(CommandSettings command)
        {
            var user = (Context.Message.Author as SocketGuildUser);
            var rolesInCommon = user.Roles.Select(x => x.Id).Intersect(command.WhitelistedRoles);

            if (!rolesInCommon.Any() || rolesInCommon == null) return false;
            return true;
        }
    
        public async Task<IUserMessage> SendPaginatedMessage(IEnumerable<string> displayTexts, EmbedBuilder templateEmbed)
        {
            templateEmbed.WithTitle($"{templateEmbed.Title} | Page 1");
            templateEmbed = templateEmbed.WithDescription(string.Join("\n", displayTexts.Take(10)));
            
            var message = await Paginator.HandleNewPaginatedMessageAsync(Context, displayTexts, templateEmbed.Build());
            return message;
        }        
    }
}
