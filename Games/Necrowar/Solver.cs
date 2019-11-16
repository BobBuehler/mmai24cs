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

            var steps = FindPath(unit.Tile.ToEnumerable(), targets, t => t.IsPath);
            steps.RemoveFirst();

            while (unit.Moves > 0 && steps.Count > 0)
            {
                unit.Move(steps.First());
                steps.RemoveFirst();
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

        public static LinkedList<Tile> FindPath(IEnumerable<Tile> starts, IEnumerable<Tile> goals, Func<Tile, bool> isPathable)
        {
            var goalSet = new HashSet<Tile>(goals);
            var astar = new AStar<Tile>(starts, t => goalSet.Contains(t), (t1, t2) => 1, t => 0, t => t.GetNeighbors().Where(isPathable));
            return astar.Path;
        }
    }
}
