using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using GLR.Core.Entities;

namespace GLR.Core.Services.DataStorage
{
    public class GuildAccountService
    {
        private LiteDBHandler _storage;
        private CommandService _commands;

        public GuildAccountService(LiteDBHandler storage, CommandService commands)
        {
            _storage = storage;
            _commands = commands;
        }

        internal GuildAccount GetOrCreateGuildAccount(ulong id)
        {
            if (!_storage.Exists<GuildAccount>(x => x.Id == id))
                CreateGuildAccount(id);
            var guildAcc = GetGuildAccount(id);
            return guildAcc;
        }

        private void CreateGuildAccount(ulong id)
            => SaveGuildAccount(new GuildAccount() { Id = id, Commands = GenerateSettingsForAllCommands(_commands.Commands)} );

        private GuildAccount GetGuildAccount(ulong id)
        {
            try
            {
                var test = _storage.RestoreSingle<GuildAccount>(x => x.Id == id);
                return test;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return null;
            }
            
        }

        internal void SaveGuildAccount(GuildAccount guild)
        {
            if (!_storage.Exists<GuildAccount>(x => x.Id == guild.Id))
                _storage.Store<GuildAccount>(guild);
            else _storage.Update<GuildAccount>(guild);
        }

        private List<CommandSettings> GenerateSettingsForAllCommands(IEnumerable<CommandInfo> cmds)
        {
            var commands = cmds.ToArray();
            var allCommandSettings = new List<CommandSettings>();

            for (int i = 0; i < commands.Count(); i++)
            {
                var extendedCommandName = $"{commands[i].Module.Name}_{commands[i].Name}";

                var commandSettings = new CommandSettings()
                {
                    Name = extendedCommandName.ToLower(),
                    IsEnabled = true,
                    ChannelListIsBlacklist = true,
                    RolesListIsBlacklist = true,
                    WhitelistedChannels = new List<ulong>(),
                    WhitelistedRoles = new List<ulong>()
                };

                allCommandSettings.Add(commandSettings);
            }

            return allCommandSettings;
        }
    }
}
