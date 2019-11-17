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

        public static UnitJob WORKER;
        public static UnitJob ZOMBIE;
        public static UnitJob GHOUL;
        public static UnitJob HOUND;
        public static UnitJob ABOMINATION;
        public static UnitJob WRAITH;
        public static UnitJob HORSEMAN;

        public static Tower OUR_CASTLE;
        public static Tower THEIR_CASTLE;

        public static IEnumerable<Tile> GOLD_MINES;
        public static HashSet<Tile> RIVER_NEIGHBORS;

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

            AI.GOLD_MINES = AI.GAME.Tiles.Where(t => t.IsGoldMine && t.Owner == AI.US);
            AI.RIVER_NEIGHBORS = new HashSet<Tile>(AI.GAME.Tiles.Where(t => t.IsRiver).SelectMany(t => t.GetNeighbors()).Where(t => t.IsGrass));

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

            this.workers();

            while (Solver.CanAfford(AI.US, AI.HOUND))
            {
                AI.UNIT_SPAWNER.SpawnUnit(AI.HOUND.Title);
                foreach (var unit in AI.US.Units.Where(u => u.Job != AI.WORKER))
                {
                    Solver.Move(unit, AI.THEIR_CASTLE.Tile.GetNeighbors());
                    Solver.Attack(unit, AI.THEIR_CASTLE);
                }
            }
            foreach (var unit in AI.US.Units.Where(u => u.Job != AI.WORKER))
            {
                Solver.Move(unit, AI.THEIR_CASTLE.Tile.GetNeighbors());
                Solver.Attack(unit, AI.THEIR_CASTLE);
            }

            AI.OUR_CASTLE.attackUnits(AI.THEM.Units, Solver.score);

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        public void workers()
        {
            if (AI.US.Units.Count(u => u.Job == AI.WORKER) <= 4 && Solver.CanAfford(AI.US, AI.WORKER))
            {
                AI.WORKER_SPAWNER.SpawnWorker();
            }

            var goldCounts = new int[] { 1, 2, 2, 3, 3, 4, 5, 5, 6 };
            var workers = AI.US.Units.Where(u => u.Job == AI.WORKER);
            var workerCount = workers.Count();
            var goldCount = goldCounts[workerCount];
            var fishCount = workerCount - goldCount;

            Solver.MoveAndMine(workers, AI.GOLD_MINES, goldCount);
            Solver.MoveAndFish(workers.Where(w => !w.Acted && w.Moves == AI.WORKER.Moves), fishCount);
        }


        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
