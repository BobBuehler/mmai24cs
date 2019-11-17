// This is where you build your AI for the Necrowar game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add additional using(s) here
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Necrowar
{
    /// <summary>
    /// This is where you build your AI for Necrowar.
    /// </summary>
    public class AI : BaseAI
    {
        #region Properties
        #pragma warning disable 0169 // the never assigned warnings between here are incorrect. We set it for you via reflection. So these will remove it from the Error List.
        #pragma warning disable 0649
        /// <summary>
        /// This is the Game object itself. It contains all the information about the current game.
        /// </summary>
        public readonly Game Game;
        /// <summary>
        /// This is your AI's player. It contains all the information about your player's state.
        /// </summary>
        public readonly Player Player;
#pragma warning restore 0169
#pragma warning restore 0649

        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional properties here for your AI to use
        // <<-- /Creer-Merge: properties -->>
        #endregion

        public static Game GAME;
        public static Player US;
        public static Player THEM;

        public static Tile UNIT_SPAWNER;
        public static Tile WORKER_SPAWNER;

        public static TowerJob CASTLE;
        public static TowerJob CLEANSING;
        public static TowerJob BALLISTA;
        public static TowerJob ARROW;
        public static TowerJob AOE;

        public static Tower CASTLE_TOWER;

        public static UnitJob WORKER;
        public static UnitJob ZOMBIE;
        public static UnitJob GHOUL;
        public static UnitJob HOUND;
        public static UnitJob ABOMINATION;
        public static UnitJob WRAITH;
        public static UnitJob HORSEMAN;

        public static Tower OUR_CASTLE;
        public static Tower THEIR_CASTLE;

        public static HashSet<Tile> GOLD_MINES;
        public static HashSet<Tile> ISLAND_GOLD_MINES;
        public static HashSet<Tile> RIVER_NEIGHBORS;

        public static List<Tile> LEFT_TOWERS;
        public static List<Tile> RIGHT_TOWERS;
        public static List<TowerJob> TOWER_PATTERN;

        static Dictionary<Tile, List<Tower>> towerRanges = new Dictionary<Tile, List<Tower>>();
        #region Methods
        /// <summary>
        /// This returns your AI's name to the game server. Just replace the string.
        /// </summary>
        /// <returns>Your AI's name</returns>
        public override string GetName()
        {
            // <<-- Creer-Merge: get-name -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            return "bugs-as-a-service"; // REPLACE THIS WITH YOUR TEAM NAME!
            // <<-- /Creer-Merge: get-name -->>
        }

        /// <summary>
        /// This is automatically called when the game first starts, once the Game and all GameObjects have been initialized, but before any players do anything.
        /// </summary>
        /// <remarks>
        /// This is a good place to initialize any variables you add to your AI or start tracking game objects.
        /// </remarks>
        public override void Start()
        {
            // <<-- Creer-Merge: start -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Start();

            AI.GAME = this.Game;
            AI.US = this.Player;
            AI.THEM = this.Game.Players.First(p => p != AI.US);

            AI.CASTLE = this.Game.TowerJobs.First(t => t.Title == "castle");
            AI.CLEANSING = this.Game.TowerJobs.First(t => t.Title == "cleansing");
            AI.BALLISTA = this.Game.TowerJobs.First(t => t.Title == "ballista");
            AI.ARROW = this.Game.TowerJobs.First(t => t.Title == "arrow");
            AI.AOE = this.Game.TowerJobs.First(t => t.Title == "aoe");

            AI.WORKER = this.Game.UnitJobs.First(t => t.Title == "worker");
            AI.ZOMBIE = this.Game.UnitJobs.First(t => t.Title == "zombie");
            AI.GHOUL = this.Game.UnitJobs.First(t => t.Title == "ghoul");
            AI.HOUND = this.Game.UnitJobs.First(t => t.Title == "hound");
            AI.ABOMINATION = this.Game.UnitJobs.First(t => t.Title == "abomination");
            AI.WRAITH = this.Game.UnitJobs.First(t => t.Title == "wraith");
            AI.HORSEMAN = this.Game.UnitJobs.First(t => t.Title == "horseman");

            AI.UNIT_SPAWNER = this.Game.Tiles.First(t => t.IsUnitSpawn && t.Owner == AI.US);
            AI.WORKER_SPAWNER = this.Game.Tiles.First(t => t.IsWorkerSpawn && t.Owner == AI.US);

            AI.OUR_CASTLE = AI.US.Towers.First(t => t.Job == AI.CASTLE);
            AI.THEIR_CASTLE = AI.THEM.Towers.First(t => t.Job == AI.CASTLE);

            AI.GOLD_MINES = new HashSet<Tile>(AI.GAME.Tiles.Where(t => t.IsGoldMine && t.Owner == AI.US));
            AI.ISLAND_GOLD_MINES = new HashSet<Tile>(AI.GAME.Tiles.Where(t => t.IsIslandGoldMine));
            AI.RIVER_NEIGHBORS = new HashSet<Tile>(AI.GAME.Tiles.Where(t => t.IsRiver).SelectMany(t => t.GetNeighbors()).Where(t => t.IsGrass));

            AI.CASTLE_TOWER = AI.US.Towers.First(t => t.Job == AI.CASTLE);

            var castleTile = AI.CASTLE_TOWER.Tile;
            if (castleTile.TileNorth.TileNorth.IsPath)
            {
                var leftX = castleTile.X - 1;
                var rightX = castleTile.X + 2;
                var startY = castleTile.Y - 3;
                AI.LEFT_TOWERS = Enumerable.Range(0, 15).Select(o => AI.GAME.GetTileAt(leftX, startY - o)).ToList();
                AI.RIGHT_TOWERS = Enumerable.Range(0, 15).Select(o => AI.GAME.GetTileAt(rightX, startY - o)).ToList();
            }
            else
            {
                var leftX = castleTile.X - 2;
                var rightX = castleTile.X + 1;
                var startY = castleTile.Y + 3;
                AI.LEFT_TOWERS = Enumerable.Range(0, 15).Select(o => AI.GAME.GetTileAt(leftX, startY + o)).ToList();
                AI.RIGHT_TOWERS = Enumerable.Range(0, 15).Select(o => AI.GAME.GetTileAt(rightX, startY + o)).ToList();
            }

            AI.TOWER_PATTERN = new List<TowerJob>() { AI.CLEANSING, AI.AOE, AI.ARROW };

            // <<-- /Creer-Merge: start -->>
        }

        /// <summary>
        /// This is automatically called every time the game (or anything in it) updates.
        /// </summary>
        /// <remarks>
        /// If a function you call triggers an update, this will be called before that function returns.
        /// </remarks>
        public override void GameUpdated()
        {
            // <<-- Creer-Merge: game-updated -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.GameUpdated();
            // <<-- /Creer-Merge: game-updated -->>
        }

        /// <summary>
        /// This is automatically called when the game ends.
        /// </summary>
        /// <remarks>
        /// You can do any cleanup of you AI here, or do custom logging. After this function returns, the application will close.
        /// </remarks>
        /// <param name="won">True if your player won, false otherwise</param>
        /// <param name="reason">A string explaining why you won or lost</param>
        public override void Ended(bool won, string reason)
        {
            // <<-- Creer-Merge: ended -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Ended(won, reason);
            // <<-- /Creer-Merge: ended -->>
        }


        /// <summary>
        /// This is called every time it is this AI.player's turn.
        /// </summary>
        /// <returns>Represents if you want to end your turn. True means end your turn, False means to keep your turn going and re-call this function.</returns>
        public bool RunTurn()
        {
            Console.WriteLine("Turn " + this.Game.CurrentTurn);
            // <<-- Creer-Merge: runTurn -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            // Put your game logic here for runTurn

            //Update list of towers within range of each path tile
            //Solver.updateTowerRanges(AI.THEM, towerRanges);

            //if (AI.GAME.Units.Where(u => u.Job == WORKER).Count() < AI.GOLD_MINES.Count())
            //{
            //    if (Solver.CanAfford(AI.US, AI.WORKER))
            //    {
            //        AI.WORKER_SPAWNER.SpawnWorker();
            //    }
            //}

            // this.workers();


            this.farmers();

            this.builders();

            // this.attackers();

            this.towers();

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        public void farmers()
        {
            if (AI.US.Units.Count(u => u.Job == AI.WORKER) < 13)
            {
                while (Solver.CanAfford(AI.US, AI.WORKER) && AI.US.Units.Count(u => u.Job == AI.WORKER) < 13)
                {
                    AI.WORKER_SPAWNER.SpawnWorker();
                    Solver.MoveAndMine(AI.WORKER_SPAWNER.Unit.ToEnumerable(), AI.GOLD_MINES, 4);
                    if (AI.WORKER_SPAWNER.Unit != null)
                    {
                        Solver.MoveAndFish(AI.WORKER_SPAWNER.Unit.ToEnumerable(), 1);
                    }
                }
            }



            if (AI.GAME.RiverPhase - (AI.GAME.CurrentTurn % AI.GAME.RiverPhase) >= 2)
            {
                Solver.MoveAndMine(AI.US.Units.Where(u => u.Job == AI.WORKER), AI.ISLAND_GOLD_MINES, 3);
                Solver.MoveAndMine(AI.US.Units.Where(u => u.Job == AI.WORKER), AI.GOLD_MINES, 4);
                Solver.MoveAndFish(AI.US.Units.Where(u => u.Job == AI.WORKER), 4);
            }
            else
            {
                Solver.MoveAndMine(AI.US.Units.Where(u => u.Job == AI.WORKER), AI.GOLD_MINES, 4);
                Solver.MoveAndFish(AI.US.Units.Where(u => u.Job == AI.WORKER), 7);
            }
        }

        public void builders()
        {
            var availableWorkers = AI.US.Units.Where(u => u.Job == AI.WORKER && u.Moves > 0 && u.Acted == false);
            if (!availableWorkers.Any())
            {
                return;
            }

            var leftIndex = AI.LEFT_TOWERS.FindIndex(t => t.Tower == null);
            var rightIndex = AI.RIGHT_TOWERS.FindIndex(t => t.Tower == null);
            Tile targetTile1 = null;
            Tile targetTile2 = null;
            if (leftIndex >= 0)
            {
                if (rightIndex >= 0)
                {
                    if (leftIndex <= rightIndex)
                    {
                        targetTile1 = AI.LEFT_TOWERS[leftIndex];
                        targetTile2 = AI.RIGHT_TOWERS[rightIndex];
                    }
                    else
                    {
                        targetTile1 = AI.RIGHT_TOWERS[rightIndex];
                        targetTile2 = AI.LEFT_TOWERS[leftIndex];
                    }
                }
                else
                {
                    targetTile1 = AI.LEFT_TOWERS[leftIndex];
                }
            }
            else if (rightIndex >= 0)
            {
                targetTile1 = AI.RIGHT_TOWERS[rightIndex];
            }

            if (targetTile1 == null)
            {
                return;
            }

            var moved = Solver.MoveNearest(availableWorkers, targetTile1.ToEnumerable(), AI.WORKER);
            if (moved == null)
            {
                return;
            }

            var builder = moved.Item1;
            var tile = moved.Item2;

            if (builder.Tile == tile)
            {
                var towerJob = AI.TOWER_PATTERN[Math.Abs(tile.Y - AI.LEFT_TOWERS[0].Y) % AI.TOWER_PATTERN.Count];
                if (Solver.CanAfford(builder.Owner, towerJob))
                {
                    builder.Build(towerJob.Title);
                }
            }

            if (targetTile2 == null)
            {
                return;
            }
            availableWorkers = availableWorkers.Where(u => u != builder);
            moved = Solver.MoveNearest(availableWorkers, targetTile2.ToEnumerable(), AI.WORKER);
            if (moved == null)
            {
                return;
            }

            builder = moved.Item1;
            tile = moved.Item2;

            if (builder.Tile == tile)
            {
                var tower = AI.TOWER_PATTERN[Math.Abs(tile.Y - AI.LEFT_TOWERS[0].Y) % AI.TOWER_PATTERN.Count];
                if (Solver.CanAfford(builder.Owner, tower))
                {
                    builder.Build(tower.Title);
                }
            }
        }

        public void workers()
        {
            if (AI.US.Units.Count(u => u.Job == AI.WORKER) <= 7 && Solver.CanAfford(AI.US, AI.WORKER))
            {
                AI.WORKER_SPAWNER.SpawnWorker();
            }

            var goldCounts = new int[] { 0, 0, 1, 1, 2, 2, 3, 4, 4, 4 };
            var islandGoldCounts = new int[] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var workers = AI.US.Units.Where(u => u.Job == AI.WORKER);
            var workerCount = workers.Count();

            var availableIslandGoldMines = AI.ISLAND_GOLD_MINES.Where(t => t.Unit == null).Count();
            var availableGoldMines = AI.GOLD_MINES.Where(t => t.Unit == null).Count();

            var remainingWorkers = 0;

            var islandGoldCount = islandGoldCounts[workerCount];
            if(islandGoldCount > availableIslandGoldMines)
            {
                remainingWorkers = islandGoldCount - availableIslandGoldMines;
                islandGoldCount = availableIslandGoldMines;
            }
            var goldCount = goldCounts[workerCount] + remainingWorkers;

            if (goldCount > availableGoldMines)
            {
                remainingWorkers = goldCount - availableGoldMines;
                goldCount = availableGoldMines;
            }
            var fishCount = workerCount - (goldCount + islandGoldCount);

             
            Solver.MoveAndMine(workers, AI.ISLAND_GOLD_MINES.Where(t => t.Unit == null), islandGoldCount);
            Solver.MoveAndMine(workers.Where(w => !w.Acted && w.Moves == AI.WORKER.Moves), AI.GOLD_MINES, goldCount);
            //CHANGE FISH TILES TO EXCLUDE ISLAND TILES
            Solver.MoveAndFish(workers.Where(w => !w.Acted && w.Moves == AI.WORKER.Moves), fishCount);
        }

        public void attackers()
        {

            if (Solver.CanAfford(AI.US, AI.HOUND, 6))
            {
                while (Solver.CanAfford(AI.US, AI.HOUND) && AI.UNIT_SPAWNER.Unit == null)
                {
                    AI.UNIT_SPAWNER.SpawnUnit(AI.HOUND.Title);
                    foreach (var unit in AI.US.Units.Where(u => u.Job != AI.WORKER))
                    {
                        Solver.MoveAndSpreadAndAttack(unit, AI.THEIR_CASTLE.ToEnumerable(), unit.Job);
                        Solver.Attack(unit, AI.THEIR_CASTLE);
                    }
                }
            }

            foreach (var unit in AI.US.Units.Where(u => u.Job != AI.WORKER))
            {
                Solver.MoveAndSpreadAndAttack(unit, AI.THEIR_CASTLE.ToEnumerable(), unit.Job);
            }
        }

        public void towers()
        {
            AI.US.Towers.ForEach(t => t.attackUnits(AI.THEM.Units, Solver.score));
        }


        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
