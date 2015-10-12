using System;
using System.Linq;
using Ensage;
using Ensage.Common.Extensions;
using EnsageTest;
using SharpDX;

namespace EnsageTest
{
    internal class Program
    {
        const int WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101, WM_CHAR = 0x0102, WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105;

        private static DotaTexture _texture = Drawing.GetTexture(@"vgui\hud\minimap_creep.vmat");


        private static void Main(string[] args)
        {
            Drawing.OnDraw += Drawing_onDraw;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Orbwalker.init();

            
        }
        private static void Game_OnWndProc(WndEventArgs m)
        {
            try
            {

                if (m.Msg == WM_KEYDOWN)
                {
                   // Console.WriteLine("key: " + m.WParam + " state " + Game.GameState + " W " + Drawing.Width + " H " + Drawing.Height);
                    if (m.WParam == 32)
                    {
                        var me = ObjectMgr.LocalHero;

                        Console.WriteLine("debug: " + me.BaseAttackTime + " : " + Orbwalker.getBaseAttackTime() + " : " + Orbwalker.GetTurnTimeBypass(me, Game.MousePosition));

                        Console.WriteLine("spell R: " + me.Spellbook.SpellQ.Level + " : " + me.Spellbook.SpellQ.CanBeCasted());

                        foreach (
                         var hero in
                       ObjectMgr.GetEntities<Hero>()
                           .Where(hero => hero.IsValid && hero.IsAlive /*(&& CalculateDamage(hero) >= hero.Health)*/))
                        {
                            Console.WriteLine(" " + hero.Name + " : " + hero.Health + " rdmg: " + me.Spellbook.SpellR.CanBeCasted(hero));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            // Console.WriteLine("shgit");
        }

        private static bool HasScepter
        {
            get
            {
                return ObjectMgr.LocalHero.Inventory.Items.Any(item => item.ClassID == ClassID.CDOTA_Item_UltimateScepter);
            }
        }
        private static float CalculateDamage(Hero unit)
        {
            var me = ObjectMgr.LocalHero;
            var spell = me.Spellbook.SpellR;
            var damage = (HasScepter)?spell.AbilityData.First(x => x.Name == "damage_scepter").GetValue(spell.Level - 1) : spell.AbilityData.First(x => x.Name == "damage").GetValue(spell.Level - 1);

            Console.WriteLine(damage + " : " + unit.MagicDamageResist);
            return damage - ((damage * unit.MagicDamageResist) / 100);
        }

        private static void Drawing_onDraw(EventArgs args)
        {
            // if (Game.GameState != GameState.Started)
            //     return;
            
            var me = ObjectMgr.LocalHero;
           // Console.WriteLine("draw");
            //Drawing.DrawRect(new Vector2(0, 0), new Vector2(1000, 600), _texture);
           // Drawing.WorldToScreen()
            Drawing.DrawText("awefgwafwafa",Drawing.WorldToScreen(me.Position),Color.Red,FontFlags.Outline);
           // Drawing.DrawRect(new Vector2(0, 0), new Vector2(1000, 600), Color.Red);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //if (Game.GameState != GameState.Started)
                return;
            var me = ObjectMgr.LocalHero;
            if (me == null || me.Spellbook.SpellR.Level == 0 || me.Spellbook.SpellR.AbilityState != AbilityState.Ready)
                return;
            Console.WriteLine("zouess"); 
            foreach (
                var hero in
                    ObjectMgr.GetEntities<Hero>()
                        .Where(hero => hero.IsValid && hero.IsAlive && CalculateDamage(hero) >= hero.Health))
            {
                ObjectMgr.LocalHero.Spellbook.SpellR.UseAbility();
            }
        }
    }
}