using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Necrowar
{
    public static class Solver
    {
        public static void MoveAttacker(Unit unit, IEnumerable<Tile> targets)
        {
            if (unit.Acted || unit.Moves == 0)
            {
                return;
            }

            var goals = new HashSet<Tile>(targets);

            var astar = new AStar<Tile>(unit.Tile.ToEnumerable(), t => goals.Contains(t), (t1, t2) => 1, t => 0, t => t.GetNeighbors().Where(n => n.IsPath));
            var steps = astar.Path.Skip(1).ToList();

            while (unit.Moves > 0 && steps.Count > 0)
            {
                unit.Move(steps[0]);
                steps.RemoveAt(0);
            }
        }

        public static void Attack(Unit unit, Tower tower)
        {
            if (unit.Acted || !tower.Tile.HasNeighbor(unit.Tile))
            {
                return;
            }

            unit.Attack(tower.Tile);
        }

        public static bool CanAfford(Player player, UnitJob job)
        {
            return player.Gold >= job.GoldCost && player.Mana >= job.ManaCost;
        }
    }
}
