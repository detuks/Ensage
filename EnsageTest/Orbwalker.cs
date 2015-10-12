using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;

namespace EnsageTest
{
    class Orbwalker
    {
        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        private static int lastAutoAttack = 0;
        private static int lastmove = 0;

        private static int baseAttackTime = -1;

        private static Player player;
        private static Hero locHero;

        public static void init()
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            player = ObjectMgr.LocalPlayer;
            locHero = ObjectMgr.LocalHero;
        }

        private static void Game_OnWndProc(WndEventArgs m)
        {

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Game.IsKeyDown(System.Windows.Input.Key.V))
            {
                orbwalk(Game.MousePosition);
            }
        }

        public static void orbwalk(Vector3 position)
        {
            if (canAttack())
            {
                var targ = getBestTarget((int) (locHero.AttackRange));
                if (targ != null)
                {
                    lastAutoAttack = now + (int)(GetTurnTimeBypass(locHero, targ.Position) * 1000);
                    Console.WriteLine("Ataackk!!! " + GetTurnTimeBypass(locHero, targ.Position));
                    locHero.Attack(targ);
                    return;
                }
            }
            if (canMove())
            {
                moveTo(position);
            }
        }

        public static void moveTo(Vector3 position)
        {
            if(now - lastmove < 250)
                return;
            Console.WriteLine("Move");
            lastmove = now;
            locHero.Move(position);
        }

        public static bool canAttack()
        {
            return canAttackAfter() == 0;
        }

        public static int canAttackAfter()
        {
            int raw = (int)(lastAutoAttack + locHero.SecondsPerAttack*1000+100 - now);
            return (raw > 0) ? raw : 0;
        }

        public static bool canMove()
        {
            return canMoveAfter() == 0;
        }

        public static int canMoveAfter()
        {
            int raw = (int)(lastAutoAttack + getBaseAttackTime()+50 - now);
            return (raw > 0) ? raw : 0;
        }

        public static Unit getBestTarget(int range)
        {
            var unitsAround = ObjectMgr.GetEntities<Unit>().Where(uni => uni.Team != locHero.Team && uni.Health>0 && locHero.Distance2D(uni) < range+uni.CollisionPadding && !uni.IsAttackImmune());

            return unitsAround.OrderByDescending(uni => uni.Health).FirstOrDefault();
        }

        public static int getBaseAttackTime()
        {
            if (baseAttackTime != -1)
                return baseAttackTime;
            baseAttackTime = (int)(Game.FindKeyValues("npc_dota_hero_lycan/AttackAnimationPoint", KeyValueSource.Hero).FloatValue*1000);
            return baseAttackTime;
        }

        public static double GetTurnTimeBypass(Entity unit, Vector3 position)
        {
            return
                (Math.Max(
                    Math.Abs(FindAngleR(unit) - Utils.DegreeToRadian(unit.FindAngleBetween(position))) - 0.69,
                    0) / (0.5 * (1 / 0.03)));
        }

        public static float FindAngleR(Entity ent)
        {
            return (float)(ent.RotationRad < 0 ? Math.Abs(ent.RotationRad) : 2 * Math.PI - ent.RotationRad);
        }
    }
}
