using Alfred.Model.Core;
using Alfred.Model.Core.Light;
using Alfred.Model.Core.Scenario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Model.Db.Repositories
{
    public class CommandRepository
    {
        public int GetIdFromName(string command)
        {
            using (var db = new AlfredContext())
            {
                return db.Commands.Single(c => c.Name == command).Id;
            }
        }

        public void UpdateItemsCommand(CommandModel command)
        {
            using (var db = new AlfredContext())
            {
                var existingCommand = db.Commands.SingleOrDefault(c => c.Id == command.Id);
                if (existingCommand != null)
                {
                    existingCommand.Items = new HashSet<CommandItem>();
                    foreach (var item in command.Items)
                    {
                        existingCommand.Items.Add(new CommandItem() { Term = item.Term });
                    }
                    existingCommand.Ruleref = command.Ruleref;
                    db.SaveChanges();
                }
            }
        }

        public IEnumerable<CommandModel> GetCommands()
        {
            using (var db = new AlfredContext())
            {
                return db.Commands.Include(c => c.Items).Select(c => new CommandModel()
                {
                    Items = c.Items.Select(i => new CommandItemModel() { Id = i.Id, Term = i.Term, CommandId = i.CommandId }).ToArray(),
                    Name = c.Name,
                    Ruleref = c.Ruleref
                }).ToList();
            }
        }
    }
}
