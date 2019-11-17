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

        public static void Move(Unit unit, IEnumerable<Tile> targets)
        {
            if (unit.Acted || unit.Moves == 0)
            {
                return;
            }

            var steps = FindPath(unit.Tile.ToEnumerable(), targets, t => CanPath(unit.Job, unit.Owner, t));
            if (steps.Count == 0)
            {
                return;
            }
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

        public static bool CanPath(UnitJob job, Player player, Tile tile)
        {
            if (job == AI.WORKER)
            {
                return tile.Unit == null && tile.IsGrass && tile.Owner != player.Opponent;
            }
            return tile.IsPath && (tile.Unit == null || (tile.Unit.Job == job && tile.NumUnits(job) < job.PerTile));
        }

        public static LinkedList<Tile> FindPath(IEnumerable<Tile> starts, IEnumerable<Tile> goals, Func<Tile, bool> isPathable)
        {
            var goalSet = new HashSet<Tile>(goals);
            var astar = new AStar<Tile>(starts, t => goalSet.Contains(t), (t1, t2) => 1, t => 0, t => t.GetNeighbors().Where(isPathable));
            return astar.Path;
        }

        public static void MoveAndMine(IEnumerable<Unit> workers, IEnumerable<Tile> mines, int count)
        {
            var workerSet = new HashSet<Unit>(workers);
            var mineSet = new HashSet<Tile>(mines);
            var remaining = count;

            while (workerSet.Count > 0 && mineSet.Count > 0 && remaining > 0)
            {
                var steps = FindPath(workerSet.Select(w => w.Tile), mineSet, t => Solver.CanPath(AI.WORKER, AI.US, t));
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
                remaining--;
            }
        }

        public static void MoveAndFish(IEnumerable<Unit> workers, int count)
        {
            var workerSet = new HashSet<Unit>(workers);
            var fishingSet = new HashSet<Tile>(AI.RIVER_NEIGHBORS);
            var remaining = count;

            while (workerSet.Count > 0 && fishingSet.Count > 0 && remaining > 0)
            {
                var steps = FindPath(workerSet.Select(w => w.Tile), fishingSet, t => Solver.CanPath(AI.WORKER, AI.US, t));
                if (steps.Count == 0)
                {
                    break;
                }
                var worker = steps.First.Value.Unit;
                var fishingSpot = steps.Last.Value;

                steps.RemoveFirst();
                while (worker.Moves > 0 && steps.Count > 0)
                {
                    worker.Move(steps.First());
                    steps.RemoveFirst();
                }

                if (fishingSet.Contains(worker.Tile))
                {
                    worker.Fish(worker.Tile.GetNeighbors().First(t => t.IsRiver));
                }

                workerSet.Remove(worker);
                fishingSet.Remove(fishingSpot);
                remaining--;
            }
        }

        public static float score(Unit unit)
        {
            float score = 0;
            // unit types are 'worker', 'zombie', 'ghoul', 'hound', 'abomination', 'wraith' or 'horseman'
            switch (unit.Job.Title)
            {
                case "worker":
                    score = 1;
                    break;
                case "zombie":
                    score = 5;
                    break;
                case "ghoul":
                    score = 5;
                    break;
                case "hound":
                    score = 5;
                    break;
                case "abomination":
                    score = 5;
                    break;
                case "wraith":
                    score = 15;
                    break;
                case "horseman":
                    score = 10;
                    break;
                default:
                    break;
            }
            //we increment the score of the target based off of missing health
            var currHealth = unit.Health;
            while(currHealth < unit.Job.Health)
            {
                score++;
                currHealth++;
            }

            return 0;
        }

        public static void attackUnits(this Tower tower, IEnumerable<Unit> units, Func<Unit, float> score)
        {
            //can this tower hit the supernatural?
            bool canHitSupernatural = tower.Job.Title == "cleansing" ? true : false;

            //find all the units within range of the tower
            IEnumerable<Unit> availableUnits = units.Where(t => ManhattanDistance(tower.Tile.X, t.Tile.X, tower.Tile.Y, t.Tile.Y) < 2 
                                                                && canHitSupernatural == (t.Job.Title == "wraith")
                                                                && t != null);
            if (availableUnits != null && availableUnits.Any())
            {
                //find the unit within range that has the highest "score"
                Unit unit = availableUnits.MaxByValue(score);

                //attack the unit if it exists
                if (unit != null)
                {
                    tower.Attack(unit.Tile);
                }
            }
        }

        public static int ManhattanDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
    }
}
