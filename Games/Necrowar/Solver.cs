using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Necrowar
{
    public static class Solver
    {

        public static void updateTowerRanges(Player Opponent, Dictionary<Tile, List<Tower>> towerRanges)
        {
            IEnumerable<Tile> tilesInRange;

            //Clear the lists for all tile paths
            foreach (var tile in Opponent.Side)
            {
                if (tile.IsPath)
                {
                    towerRanges[tile] = new List<Tower>();
                }
            }

            //Create a list of towers that are in range of the given path tile
            foreach (var Tower in Opponent.Towers)
            {
                tilesInRange = Tower.getTilesInRange();
                foreach (var tile in tilesInRange.Where(t => t != null && t.IsPath))
                {
                    towerRanges[tile].Add(Tower);
                }
            }
        }

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

        public static void MoveWorkersToMine(IEnumerable<Unit> workers, IEnumerable<Tile> mines)
        {
            var workerSet = new HashSet<Unit>(workers);
            var mineSet = new HashSet<Tile>(mines);

            while (workerSet.Count > 0 && mineSet.Count > 0)
            {
                var steps = FindPath(workerSet.Select(w => w.Tile), mineSet, t => t.IsGrass);
                if (steps.Count == 0)
                {
                    break;
                }
                var worker = steps.First.Value.Unit;
                var mine = steps.Last.Value;
                
                steps.RemoveFirst();
                while (worker.Moves > 0 && steps.Count > 0)
                {
                    worker.Move(steps.First());
                    steps.RemoveFirst();
                }

                if (worker.Tile == mine)
                {
                    worker.Mine(mine);
                }

                workerSet.Remove(worker);
                mineSet.Remove(mine);
            }
        }
    }
}
