﻿using System;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.OvkTab.UI.Components;
using osu.Framework.Graphics;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.OvkTab.Tests.UI
{
    public class TestSceneUserPanel : OsuTestScene
    {
        private readonly FillFlowContainer container;

        public TestSceneUserPanel()
        {
            Add(container = new FillFlowContainer()
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new(60),
                Spacing = new(10),
            });
        }
        Random r;
        readonly string[] avatars = new[]
        {
            "https://sun9-59.userapi.com/impg/IFzOze1opVPaV1ZYfuPVRQg8fcY5z5f66okOwQ/UKZqRxDEA_M.jpg?size=750x750&quality=96&sign=243c006d2e4721876ef156b455ac5cbc&type=album",
            "https://sun9-49.userapi.com/impf/VjJVBon38R0wCdy8vw_5z1UE20t9G1-n5ko3Nw/m9IuvHSRKLo.jpg?size=320x227&quality=96&sign=7023cb765b48b7f537d997c5a9f3625a&type=album",
            "https://sun9-5.userapi.com/impf/c848528/v848528379/e393c/lK-34lneUl4.jpg?size=320x320&quality=96&sign=e4ffeedcd1ff1f8f5a2a20543f301abc&type=album",
            "https://sun1-84.userapi.com/impg/pzuFhdbKjDXINZr5P8iyAUhrIWc2wxu54uvNZw/9RV3YbERCzs.jpg?size=604x599&quality=96&sign=c3b9dc09d0f9942b183686d428834ef7&type=album"
        };
        readonly string[] t = ("Сегодня мы с Лёней займёмся чем? Правильно, поедем куда-то к черту на куличики. Зачем? Да потому что почему бы и нет. Дело было не помню " +
            "какого мая, мы решили поехать на аэродром. Ну потому что мы там толком не были, вот и поехали.").Split(' ');
        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Init random", () =>
            {
                r = new Random();
            });
            AddStep("Add some panels", () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    SimpleVkUser u = new()
                    {
                        full = null,
                        id = i+1,
                        name = PickRandom(t)+" "+PickRandom(t)+" "+ PickRandom(t) + " " + PickRandom(t),
                        avatarUrl = PickRandom(avatars),
                    };
                    container.Add(new UserPanel(u));
                }
            });
            AddStep("Clear", () => container.Clear(true));
        }

        string PickRandom(string[] array)
        {
            if (r == null) return array[0];
            return array[r.Next(array.Length)];
        }
    }
}
