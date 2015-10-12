using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;

namespace EnsageTest
{
    class HealthPrediction
    {
        static HealthPrediction()
        {
            ObjectMgr.OnAddEntity += onCreate;
            ObjectMgr.OnRemoveEntity += onDelete;
        }

        private static void onDelete(EntityEventArgs args)
        {

        }

        private static void onCreate(EntityEventArgs args)
        {
            var ent = args.Entity;
            if (!(ent is Projectile))
                return;
            //Projectile proj = ent as Projectile;
        }

        public static int predHealth(Unit unit, int time)
        {

            return (int) unit.Health;
        }
    }
}
