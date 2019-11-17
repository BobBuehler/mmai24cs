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

        public static Tuple<Unit, Tile> MoveNearest(IEnumerable<Unit> units, IEnumerable<Tile> targets, UnitJob job)
        {
            var movable = units.Where(u => !u.Acted && u.Moves > 0);
            if (!movable.Any())
            {
                return null;
            }

            var steps = FindPath(movable.Select(u => u.Tile), targets, t => CanPath(job, AI.US, t));
            if (steps.Count == 0)
            {
                steps = FindPath(movable.Select(u => u.Tile), targets, t => CanPath(job, AI.US, t, true));
                if (steps.Count == 0)
                {
                    return null;
                }
            }

            var moverTile = steps.First();
            var mover = units.First(u => u.Tile == moverTile);
            var target = steps.Last();
            steps.RemoveFirst();
            while (mover.Moves > 0 && steps.Count > 0 && CanPath(job, AI.US, steps.First()))
            {
                mover.Move(steps.First());
                steps.RemoveFirst();
            }

            return Tuple.Create(mover, target);
        }

        public static void MoveAndSpreadAndAttack(Unit unit, IEnumerable<Tower> targets, UnitJob job)
        {
            if (unit.Acted || unit.Moves == 0)
            {
                return;
            }

            var targetNeighbors = new HashSet<Tile>(targets.SelectMany(t => t.Tile.GetNeighbors()));

            var pathsToNeighbors = FindPaths(unit.Tile.ToEnumerable(), targetNeighbors, t => CanPath(job, AI.US, t));
            var reachableTargetNeighbors = pathsToNeighbors.Keys.Where(t => pathsToNeighbors[t].Count <= unit.Moves + 1);

            LinkedList<Tile> path = null;
            if (reachableTargetNeighbors.Any())
            {
                path = pathsToNeighbors[reachableTargetNeighbors.MinByValue(t => t.NumUnits(job))];
            }
            else
            {
                var pathsIgnore = FindPaths(unit.Tile.ToEnumerable(), targetNeighbors, t => CanPath(job, AI.US, t, true));
                if (pathsIgnore.Count > 0)
                {
                    path = pathsIgnore.MinByValue(kvp => kvp.Value.Count).Value;
                }
            }

            if (path == null)
            {
                return;
            }

            path.RemoveFirst();
            while (unit.Moves > 0 && path.Count > 0 && CanPath(job, AI.US, path.First()))
            {
                unit.Move(path.First());
                path.RemoveFirst();
            }

            var target = targets.FirstOrDefault(t => t.Tile.HasNeighbor(unit.Tile));
            if (target != null)
            {
                unit.Attack(target.Tile);
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

        public static bool CanAfford(Player player, UnitJob job, int count = 1)
        {
            return player.Gold >= job.GoldCost * count && player.Mana >= job.ManaCost * count;
        }

        public static bool CanAfford(Player player, TowerJob job)
        {
            return player.Gold >= job.GoldCost && player.Mana >= job.ManaCost;
        }

        public static bool CanPath(UnitJob job, Player player, Tile tile, bool ignoreUnits = false)
        {
            if (job == AI.WORKER)
            {
                return (ignoreUnits || tile.Unit == null) && tile.IsGrass && tile.Owner != player.Opponent && tile.Tower == null;
            }
            return tile.IsPath && (ignoreUnits || tile.Unit == null || (tile.Unit.Job == job && tile.NumUnits(job) < job.PerTile));
        }

        public static LinkedList<Tile> FindPath(IEnumerable<Tile> starts, IEnumerable<Tile> goals, Func<Tile, bool> isPathable)
        {
            var goalSet = new HashSet<Tile>(goals);
            var astar = new AStar<Tile>(starts, t => goalSet.Contains(t), (t1, t2) => 1, t => 0, t => t.GetNeighbors().Where(isPathable));
            return astar.Path;
        }

        public static Dictionary<Tile, LinkedList<Tile>> FindPaths(IEnumerable<Tile> starts, IEnumerable<Tile> goals, Func<Tile, bool> isPathable)
        {
            var astar = new AStar<Tile>(starts, t => false, (t1, t2) => 1, t => 0, t => t.GetNeighbors().Where(isPathable));
            return goals.Where(g => astar.GScore.ContainsKey(g)).ToDictionary(g => g, g => astar.CalcPathTo(g));
        }

        public static void MoveAndMine(IEnumerable<Unit> workers, IEnumerable<Tile> mines, int count)
        {
            var workerSet = new HashSet<Unit>(workers);
            var mineSet = new HashSet<Tile>(mines);
            var remaining = count;

            while (workerSet.Count > 0 && mineSet.Count > 0 && remaining > 0)
            {
                var moveTuple = MoveNearest(workerSet, mineSet, AI.WORKER);
                if (moveTuple == null)
                {
                    return;
                }

                var worker = moveTuple.Item1;
                var mine = moveTuple.Item2;

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
            var fishingSet = new HashSet<Tile>(AI.RIVER_NEIGHBORS.Where(t => t.Owner == AI.US));
            var remaining = count;

            while (workerSet.Count > 0 && fishingSet.Count > 0 && remaining > 0)
            {
                var moveTuple = MoveNearest(workerSet, fishingSet, AI.WORKER);
                if (moveTuple == null)
                {
                    return;
                }

                var worker = moveTuple.Item1;
                var fishingSpot = moveTuple.Item2;

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

            score += unit.Job.Health - unit.Health;

            return score;
        }

        public static void attackUnits(this Tower tower, IEnumerable<Unit> units, Func<Unit, float> score)
        {
            if (tower.Cooldown > 0)
            {
                return;
            }

            //can this tower hit the supernatural?
            var canHitSupernatural = tower.Job.Title == "cleansing";

            //find all the units within range of the tower
            var availableUnits = units.Where(u => canAttackJob(tower.Job, u.Job) && ManhattanDistance(tower.Tile.X, u.Tile.X, tower.Tile.Y, u.Tile.Y) <= 2);

            if (availableUnits.Any())
            {
                //find the unit within range that has the highest "score"
                var unit = availableUnits.MaxByValue(score);
                tower.Attack(unit.Tile);
            }
        }

        public static int ManhattanDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        public static int ManhattanDistance(Tile t1, Tile t2)
        {
            return Math.Abs(t1.X - t2.X) + Math.Abs(t1.Y - t2.Y);
        }

        public static bool canAttackJob(TowerJob towerJ, UnitJob unitJ)
        {
            if (unitJ == AI.WORKER)
            {
                return false;
            }
            if (towerJ == AI.CASTLE)
            {
                return true;
            }
            if (towerJ == AI.CLEANSING)
            {
                return unitJ == AI.WRAITH || unitJ == AI.ABOMINATION;
            }
            return unitJ != AI.WRAITH;
        }

        public static int turnsLeft(Unit unit, Tile destination)
        {
            int distance = ManhattanDistance(unit.Tile.X, destination.X, unit.Tile.Y, destination.Y);
            return distance / unit.Job.Moves;
        }
    }

}
