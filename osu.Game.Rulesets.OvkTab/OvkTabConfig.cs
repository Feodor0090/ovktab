using osu.Framework.Allocation;
using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab
{
    [Cached]
    public class OvkTabConfig : RulesetConfigManager<OvkTabRulesetSetting>
    {
        public OvkTabConfig(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(OvkTabRulesetSetting.Login, string.Empty);
            SetDefault(OvkTabRulesetSetting.Id, 0);
            SetDefault(OvkTabRulesetSetting.Token, string.Empty);
        }
    }

    public enum OvkTabRulesetSetting
    {
        Login,
        Id,
        Token
    }
}