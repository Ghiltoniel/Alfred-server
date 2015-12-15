using System.Collections.Generic;
using System.Linq;
using Microsoft.Speech.Recognition.SrgsGrammar;
using Alfred.Model.Core;

namespace Alfred.Utils.Speech
{
    public class GrammaireBuilder
    {
        public SrgsDocument document;

        public GrammaireBuilder(SrgsDocument doc)
        {
            document = doc;
        }

        public void CreateRule(string id, List<string> items)
        {
            if (items.Count == 0)
            {
                items = new List<string>();
                items.Add("test");
            }
            items.RemoveAll(s => s == null);
            var rule = new SrgsRule(id);
            rule.Scope = SrgsRuleScope.Public;
            var itemList = new SrgsOneOf(items.ToArray());
            var tag = new SrgsSemanticInterpretationTag("$._value = $recognized.text;");
            rule.Add(itemList);
            rule.Add(tag);
            document.Rules.Add(rule);
        }

        public void CreateRule(string id, IEnumerable<int> items)
        {
            var rule = new SrgsRule(id);
            rule.Scope = SrgsRuleScope.Public;
            var itemList = new SrgsOneOf(items.Select(o => o.ToString()).ToArray());
            var tag = new SrgsSemanticInterpretationTag("$._value = $recognized.text;");
            rule.Add(itemList);
            rule.Add(tag);
            document.Rules.Add(rule);
        }

        public void CreateRule(string id, IEnumerable<CommandModel> commands)
        {
            var rule = new SrgsRule(id);
            rule.Scope = SrgsRuleScope.Public;
            var oneOf = new SrgsOneOf();
            foreach (var command in commands)
            {
                if (command.Items.Count == 0)
                    continue;

                var item = new SrgsItem();
                var itemList = new SrgsOneOf(command.Items.Select(i => i.Term).ToArray());
                item.Add(itemList);


                string tagString = null;
                if (command.Ruleref == "")
                    tagString = "$.alfred={}; $.alfred._value=\"" + command.Name + "\";";
                else if (document.Rules.Contains(command.Ruleref))
                {
                    var ruleref = new SrgsRuleRef(document.Rules[command.Ruleref]);
                    item.Add(ruleref);
                    tagString = "$." + command.Ruleref + "=$" + command.Ruleref + "; $.alfred={}; $.alfred._value=\"" + command.Name + "\";";
                }

                if (tagString != null)
                {
                    var tag = new SrgsSemanticInterpretationTag(tagString);
                    item.Add(tag);
                    oneOf.Add(item);
                }
            }
            rule.Add(oneOf);
            document.Rules.Add(rule);
            document.Root = rule;
        }
    }
}
